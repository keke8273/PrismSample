using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.TestManagerModule.Services;
using QBR.TestManagerModule.Views;

namespace QBR.TestManagerModule.ModuleDefinition
{
    [Module(ModuleName = "TestManagerModule", OnDemand = false)]
    [ModuleDependency("SoundModule")]
    [Priority(ModuleInitializationPriority.TestManagerModuleInitPriority)]
    public class TestManagerModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;

        public TestManagerModule(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }


        public void Initialize()
        {
            _logger.Log("Initializing Test Manager Module", Category.Debug, Priority.None);

            //Register Services
            _container.RegisterType<ITestProgressService, TestProgressService>(new ContainerControlledLifetimeManager());

            //Register Views
            _regionManager.RegisterViewWithRegion(RegionNames.TestProgressRegion, typeof(TestProgressView));
        }
    }
}
