using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using QBR.Infrastructure.Attributes;

namespace QBR.Shell.Services
{
    class PrioritizedDirectoryModuleCatalog : DirectoryModuleCatalog
    {
        protected override void InnerLoad()
        {
            if(string.IsNullOrEmpty(ModulePath))
                throw new InvalidOperationException("The ModulePath cannot contain a null value or be empty.");
            if (!Directory.Exists(ModulePath))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Directory {0} was not found.", ModulePath));

            var childDomain = BuildChildDomain(AppDomain.CurrentDomain);

            try
            {
                var loadedAssemblies = new List<string>();

                var assemblies = (
                                     from Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     where !(assembly is System.Reflection.Emit.AssemblyBuilder)
                                        && assembly.GetType().FullName != "System.Reflection.Emit.InternalAssemblyBuilder"
                                        && !String.IsNullOrEmpty(assembly.Location)
                                     select assembly.Location
                                 );

                loadedAssemblies.AddRange(assemblies);

                var loaderType = typeof(ModulePriorityLoader);

                if (loaderType.Assembly != null)
                {
                    var loader =
                        (ModulePriorityLoader)
                        childDomain.CreateInstanceFrom(loaderType.Assembly.Location, loaderType.FullName).Unwrap();
                    loader.LoadAssemblies(loadedAssemblies);
                    Items.AddRange(Sort(loader.GetModuleInfos(ModulePath)));
                }
            }
            finally
            {
                AppDomain.Unload(childDomain);
            }
        }

        protected override IEnumerable<ModuleInfo> Sort(IEnumerable<ModuleInfo> modules)
        {
            Dictionary<string, int> priorities = GetPriorities(modules);

            //call the base sort to resolve the dependencies, then re-sort by priorities
            var result = new List<ModuleInfo>(base.Sort(modules));

            result.Sort((x, y) =>
            {
                string xModuleName = x.ModuleName;
                string yModuleName = y.ModuleName;

                if (x.DependsOn.Contains(yModuleName))
                    return 1;
                
                if (y.DependsOn.Contains(xModuleName))
                    return -1;
                
                return priorities[xModuleName].CompareTo(priorities[yModuleName]);
            });

            return result;
        }

        public Dictionary<string, int> GetPriorities(IEnumerable<ModuleInfo> modules)
        {
            //retrieve the priorities of each module, so that we can use them to override the 
            //sorting - but only so far as we don't mess up the dependencies
            var priorities = new Dictionary<string, int>();
            var assemblies = new Dictionary<string, Assembly>();

            foreach (ModuleInfo module in modules)
            {
                if (!assemblies.ContainsKey(module.Ref))
                {
                    //LoadFrom should generally be avoided apparently due to unexpected side effects,
                    //but since we are doing all this in a separate AppDomain which is discarded
                    //this needn't worry us
                    assemblies.Add(module.Ref, Assembly.LoadFrom(module.Ref));
                }

                Type type = assemblies[module.Ref].GetExportedTypes()
                    .Where(t => t.AssemblyQualifiedName.Equals(module.ModuleType, StringComparison.Ordinal))
                    .First();

                var priorityAttribute =
                    CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                        cad => cad.Constructor.DeclaringType.FullName == typeof(PriorityAttribute).FullName);

                int priority;
                if (priorityAttribute != null)
                {
                    priority = (int)priorityAttribute.ConstructorArguments[0].Value;
                }
                else
                {
                    priority = 0;
                }

                priorities.Add(module.ModuleName, priority);
            }

            return priorities;
        }


        /// <summary>
        /// Local class to load assemblies into different AppDomain which is then discarded
        /// </summary>
        private class ModulePriorityLoader : MarshalByRefObject
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal void LoadAssemblies(IEnumerable<string> assemblies)
            {
                foreach (string assemblyPath in assemblies)
                {
                    try
                    {
                        Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    }
                    catch (FileNotFoundException)
                    {
                        // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain
                    }
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
            internal IEnumerable<ModuleInfo> GetModuleInfos(string path)
            {
                DirectoryInfo directory = new DirectoryInfo(path);

                ResolveEventHandler resolveEventHandler =
                    delegate(object sender, ResolveEventArgs args) { return OnReflectionOnlyResolve(args, directory); };

                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;

                Assembly moduleReflectionOnlyAssembly =
                    AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().First(
                        asm => asm.FullName == typeof(IModule).Assembly.FullName);

                Type IModuleType = moduleReflectionOnlyAssembly.GetType(typeof(IModule).FullName);

                IEnumerable<ModuleInfo> modules = GetNotAlreadyLoadedModuleInfos(directory, IModuleType);

                var array = modules.ToArray();
                AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;

                return array;
            }

            private static IEnumerable<ModuleInfo> GetNotAlreadyLoadedModuleInfos(DirectoryInfo directory, Type IModuleType)
            {
                List<FileInfo> validAssemblies = new List<FileInfo>();
                Assembly[] alreadyLoadedAssemblies = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

                var fileInfos = directory.GetFiles("*.dll")
                    .Where(file => alreadyLoadedAssemblies
                                       .FirstOrDefault(
                                       assembly =>
                                       String.Compare(Path.GetFileName(assembly.Location), file.Name,
                                                      StringComparison.OrdinalIgnoreCase) == 0) == null);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    Assembly assembly = null;
                    try
                    {
                        assembly = Assembly.ReflectionOnlyLoadFrom(fileInfo.FullName);
                        validAssemblies.Add(fileInfo);
                    }
                    catch (BadImageFormatException)
                    {
                        // skip non-.NET Dlls
                    }
                }

                return validAssemblies.SelectMany(file => Assembly.ReflectionOnlyLoadFrom(file.FullName)
                                            .GetExportedTypes()
                                            .Where(IModuleType.IsAssignableFrom)
                                            .Where(t => t != IModuleType)
                                            .Where(t => !t.IsAbstract)
                                            .Select(type => CreateModuleInfo(type)));
            }

            private static Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
            {
                Assembly loadedAssembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(
                    asm => string.Equals(asm.FullName, args.Name, StringComparison.OrdinalIgnoreCase));
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                AssemblyName assemblyName = new AssemblyName(args.Name);
                string dependentAssemblyFilename = Path.Combine(directory.FullName, assemblyName.Name + ".dll");
                if (File.Exists(dependentAssemblyFilename))
                {
                    return Assembly.ReflectionOnlyLoadFrom(dependentAssemblyFilename);
                }
                return Assembly.ReflectionOnlyLoad(args.Name);
            }

            private static ModuleInfo CreateModuleInfo(Type type)
            {
                string moduleName = type.Name;
                List<string> dependsOn = new List<string>();
                bool onDemand = false;
                var moduleAttribute =
                    CustomAttributeData.GetCustomAttributes(type).FirstOrDefault(
                        cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleAttribute).FullName);

                if (moduleAttribute != null)
                {
                    foreach (CustomAttributeNamedArgument argument in moduleAttribute.NamedArguments)
                    {
                        string argumentName = argument.MemberInfo.Name;
                        switch (argumentName)
                        {
                            case "ModuleName":
                                moduleName = (string)argument.TypedValue.Value;
                                break;

                            case "OnDemand":
                                onDemand = (bool)argument.TypedValue.Value;
                                break;

                            case "StartupLoaded":
                                onDemand = !((bool)argument.TypedValue.Value);
                                break;
                        }
                    }
                }

                var moduleDependencyAttributes =
                    CustomAttributeData.GetCustomAttributes(type).Where(
                        cad => cad.Constructor.DeclaringType.FullName == typeof(ModuleDependencyAttribute).FullName);

                foreach (CustomAttributeData cad in moduleDependencyAttributes)
                {
                    dependsOn.Add((string)cad.ConstructorArguments[0].Value);
                }

                ModuleInfo moduleInfo = new ModuleInfo(moduleName, type.AssemblyQualifiedName)
                {
                    InitializationMode =
                        onDemand
                            ? InitializationMode.OnDemand
                            : InitializationMode.WhenAvailable,
                    Ref = type.Assembly.CodeBase,
                };
                moduleInfo.DependsOn.AddRange(dependsOn);

                return moduleInfo;
            }
        }

    }
}
