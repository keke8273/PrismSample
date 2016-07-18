using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.UserDataModule.Services;
using QBR.UserDataModule.ViewModels;

namespace QBR.UserDataModule.ModuleDefinition
{
    [Module(ModuleName = "UserDataModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.UserDataModuleInitPriority)]
    public class UserDataModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILoggerFacade _logger;

        public UserDataModule(IUnityContainer container, IRegionManager regionManager, IEventAggregator eventAggregator, ILoggerFacade logger, IResourceManager resourceManager)
        {
            _container = container;
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing User Data Module", Category.Debug, Priority.None);

            //Register Module Settings
            _container.Resolve<IApplicationSettingsService>().RegisterModuleSettings("UserDataModule", Properties.Settings.Default);

            //Register Resources
            _container.Resolve<IResourceManager>().RegisterModuleResourceDictionary(new Uri("pack://application:,,,/QBR.UserDataModule;component/Resources/UserDataResources.xaml"));

            //Register Services
            _container.RegisterType<IUserEntryService, UserEntryService>(new ContainerControlledLifetimeManager());
            _container.RegisterTypeForNavigation<DataEntryViewModel>();
            _container.RegisterTypeForNavigation<DataReferenceViewModel>();

            //Register Views
            //_regionManager.Regions[RegionNames.UserDataRegion].Add(_container.Resolve<DataEntryViewModel>());
            _regionManager.RequestNavigate(RegionNames.UserDataRegion, new Uri(typeof(DataEntryViewModel).FullName, UriKind.RelativeOrAbsolute));

            //Subscribe Events
        }
    }
}
