using System;
using DataLinkLayer.IO;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using QBR.AnalyzerManagerModule.Services;
using QBR.AnalyzerManagerModule.ViewModels;
using QBR.AnalyzerManagerModule.Views;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;

namespace QBR.AnalyzerManagerModule.ModuleDefinitions
{
    [Module(ModuleName = "AnalyzerManagerModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.AnalyzerManagerModuleInitPriority)]
    public class AnalyzerManagerModule : IModule 
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;

        public AnalyzerManagerModule(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Analyzer Manager Module", Category.Debug, Priority.None);

            //Register Module Settings
            _container.Resolve<IApplicationSettingsService>().RegisterModuleSettings("AnalyzerManagerModule", Properties.Settings.Default);

            //Register Resources
            _container.Resolve<IResourceManager>().RegisterModuleResourceDictionary(new Uri("pack://application:,,,/QBR.AnalyzerManagerModule;component/Resources/AnalyzerManagerResources.xaml"));

            //Register Services
             _container.RegisterInstance(CommsInterfacePluginManager.Instance.CommsInterface);

            _container.RegisterTypeForNavigation<AnalyzerManagerViewModel>();
            _container.RegisterType<IAnalyzerConfigurationService, AnalyzerConfigurationService>(
    new ContainerControlledLifetimeManager());
            //Register Views
            _regionManager.RegisterViewWithRegion(RegionNames.AnalyzerRegion, typeof(AnalyzerManagerView));
        }
    }
}
