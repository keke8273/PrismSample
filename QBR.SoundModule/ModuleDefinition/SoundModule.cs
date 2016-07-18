using System.Media;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.SoundModule.Services;

namespace QBR.SoundModule.ModuleDefinition
{
    [Module(ModuleName = "SoundModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.SoundModulePriority)]
    public class SoundModule : IModule 
    {
        private readonly IUnityContainer _container;
        private readonly ILoggerFacade _logger;

        public SoundModule(IUnityContainer container, ILoggerFacade logger)
        {
            _container = container;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Sound Module", Category.Debug, Priority.None);

            //Register Services
            _container.RegisterType<ISoundPlayingService, SoundPlayingService>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance(typeof(SoundPlayer), new SoundPlayer());
        }
    }
}
