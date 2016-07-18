using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Interfaces;
using QBR.Shell.Services;

namespace QBR.Shell
{
    class QBRBootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            //register singleton Shell
            Container.RegisterType<IShell, Views.Shell>(new ContainerControlledLifetimeManager());
            
            //register singleton Dispatcher
            Container.RegisterInstance(typeof(Dispatcher), Application.Current.Dispatcher);

            //register singleton DispatcherService
            Container.RegisterType<IDispatcherService, DispatcherService>(new ContainerControlledLifetimeManager());

            //register singleton Resource Manager
            Container.RegisterType<IResourceManager, ResourceManager>(new ContainerControlledLifetimeManager());

            //register singleton DialogService
            Container.RegisterType<IDialogService, DialogService>(new ContainerControlledLifetimeManager());

            //register singleton UserNotificationService
            Container.RegisterType<IUserNotificationService, UserNotificationService>(new ContainerControlledLifetimeManager());
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Views.Shell>();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new PrioritizedDirectoryModuleCatalog() { ModulePath = @".\Modules" };
        }

        protected override ILoggerFacade CreateLogger()
        {
            return new Log4NetLogger();
        }
    }
}
