
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.IO;
using System.Reflection;

namespace DataLinkLayer.IO
{
    /// <summary>
    /// A Singleton class used to manage the comms plugin.
    /// </summary>
    public class CommsInterfacePluginManager
    {
        #region Constructors

        /// <summary>
        /// Private constructor used to discover the plugin
        /// </summary>
        private CommsInterfacePluginManager()
        {
            FindPlugin();
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static CommsInterfacePluginManager()
        { }

        #endregion Constructor

        #region Private data

        /// <summary>
        /// backing field for the singleton Instance property.
        /// </summary>
        private static CommsInterfacePluginManager _instance = new CommsInterfacePluginManager();

        #endregion Private data

        #region Public Data

        /// <summary>
        /// the fully qualified path to the comms plugin
        /// </summary>
        public readonly string TargetCommsPlugin = "./CommsPlugin.dll";

        /// <summary>
        /// The full name of the interface type we will look for
        /// </summary>
        public readonly string TargetInterface = (typeof(ICommsInterface).FullName);

        /// <summary>
        /// Property used to get the comms interface that was loaded from the plugin
        /// </summary>
        public ICommsInterface CommsInterface { get; private set; }
            
        /// <summary>
        /// Single Instance property of this class
        /// </summary>
        public static CommsInterfacePluginManager Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion Public Data

        #region Private methods

        /// <summary>
        /// Find and load the comms plugin from the current directory.
        /// </summary>
        private void FindPlugin()
        {
            CommsInterface = null;
            if (File.Exists(TargetCommsPlugin))
            {
                //Create a new assembly from the plugin file we're adding..
                var pluginAssembly = Assembly.LoadFrom(TargetCommsPlugin);

                //Next we'll loop through all the Types found in the assembly
                foreach (var pluginType in pluginAssembly.GetTypes())
                {
                    if (pluginType.IsPublic) //Only look at public types
                    {
                        if (!pluginType.IsAbstract)  //Only look at non-abstract types
                        {
                            //Gets a type object of the interface we need the plugins to match
                            var typeInterface = pluginType.GetInterface(TargetInterface, true);

                            //Make sure the interface we want to use actually exists
                            if (typeInterface != null)
                            {
                                //create the comms interface from the loaded plugin
                                CommsInterface= (ICommsInterface)Activator.CreateInstance(pluginAssembly.GetType((pluginType.ToString())));
                                
                                break;
                            }
                            typeInterface = null; //Mr. Clean			
                        }
                    }
                }
            }

            if (CommsInterface == null)
            {
                //if the comms interface is null at this point the manager was unable to load a dll containing a valid Comms Interface class
                throw new FileLoadException("Unable to load a comms plugin.");
            }
        }

        #endregion Private methods
    }
}
