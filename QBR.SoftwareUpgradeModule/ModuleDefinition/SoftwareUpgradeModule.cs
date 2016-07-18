using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.SoftwareUpgradeModule.Services;

namespace QBR.SoftwareUpgradeModule.ModuleDefinition
{
    [Module(ModuleName = "SoftwareUpgradeModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.SoftwareUpgradeModuleInitPriority)]
    public class SoftwareUpgradeModule :IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;
 
        public SoftwareUpgradeModule(IUnityContainer container, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Software Upgrade Module", Category.Debug, Priority.None);

            //Loads Module Configurations
            bool autoSoftwareUpdate = Properties.Settings.Default.AutoSoftwareUpdate;

            //Register Module Settings
            _container.Resolve<IApplicationSettingsService>().RegisterModuleSettings("SoftwareUpgradeModule", Properties.Settings.Default);

            //Register Services
            _container.RegisterType<ISoftwareUpgradeService, SoftwareUpgradeService>(
                new ContainerControlledLifetimeManager());

            //Register Views

            //Subscribe to application level events or services such as error handling, logging, etc.

            if (autoSoftwareUpdate)
            {
                var softwareUpgradeService = _container.Resolve<ISoftwareUpgradeService>();
                softwareUpgradeService.CheckForUpdate();   
            }
        }
    }
}
