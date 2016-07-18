using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using QBR.ApplicationSettingsManagerModule.Services;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;

namespace QBR.ApplicationSettingsManagerModule.ModuleDefinition
{
    [Module(ModuleName = "ApplicationSettingsManagerModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.ApplicationSettingsManagerModuleInitPriority)]
    public class ApplicationSettingsManagerModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly ILoggerFacade _logger;

        public ApplicationSettingsManagerModule(IUnityContainer container, ILoggerFacade logger)
        {
            _container = container;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Application Settings Manager Module", Category.Debug, Priority.None);

            //Register Services
            _container.RegisterType<IApplicationSettingsService, ApplicationSettingsService>(
                new ContainerControlledLifetimeManager());
        }
    }
}
