using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.RecordManagerModule.Services;

namespace QBR.RecordManagerModule.ModuleDefinition
{
    [Module(ModuleName = "RecordManagerModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.RecordManagerModuleInitPriority)]
    public class RecordManagerModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly ILoggerFacade _logger;
        private IRecordPersistService _recordPersistService;

        public RecordManagerModule(IUnityContainer container, ILoggerFacade logger)
        {
            _container = container;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Record Manager Module", Category.Debug, Priority.None);

            //Register Module Settings
            _container.Resolve<IApplicationSettingsService>().RegisterModuleSettings("RecordManagerModule", Properties.Settings.Default);

            //Register Services
            _container.RegisterType<IRecordPersistService, RecordPersistService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<IRecordTranslateService, RecordTranslateService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ITimeStampService, TimeStampService>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ISecurityCodeService, SecurityCodeService>(new ContainerControlledLifetimeManager());

            _recordPersistService = _container.Resolve<IRecordPersistService>();
        }
    }
}
