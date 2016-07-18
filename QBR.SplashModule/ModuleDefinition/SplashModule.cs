using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;
using QBR.Infrastructure.Attributes;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Constants;
using QBR.Infrastructure.Interfaces;
using QBR.SplashModule.ViewModels;
using SplashScreen = QBR.SplashModule.Views.SplashScreen;

namespace QBR.SplashModule.ModuleDefinition
{
    [Module(ModuleName = "SplashModule", OnDemand = false)]
    [Priority(ModuleInitializationPriority.SplashModuleInitPriority)]
    public class SplashModule : IModule
    {
        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly IShell _shell;
        private readonly IDispatcherService _dispatcherService;
        private readonly ILoggerFacade _logger;

        private volatile bool _isRestartRequired;
        private AutoResetEvent _waitForSplashScreenCreation;

        public SplashModule(IUnityContainer container, IEventAggregator eventAggregator, IShell shell, IDispatcherService dispatcherService, ILoggerFacade logger, IResourceManager resourceManager)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            _shell = shell;
            _dispatcherService = dispatcherService;
            _logger = logger;
        }

        public void Initialize()
        {
            _logger.Log("Initializing Splash Module", Category.Debug, Priority.None);

            //Register Resources
            _container.Resolve<IResourceManager>().RegisterModuleResourceDictionary(new Uri("pack://application:,,,/QBR.SplashModule;component/Resources/SplashResources.xaml"));

            //Queue the task to show the main screen and close the splash screen onto the current UI dispatcher,
            //The current UI should be busy with the bootstrapping the application so this wont get executed until the bootstrapping is completed. 
            _dispatcherService.Dispatch(
                () =>
                {
                    if (_isRestartRequired)
                    {
                        RestartApplication();
                    }
                    _shell.Show();
                    _eventAggregator.GetEvent<CloseSplashEvent>().Publish(CloseSplashSource.BootStrapper);
                });

            _waitForSplashScreenCreation = new AutoResetEvent(false);

            //Subscribe Events
            _eventAggregator.GetEvent<RestartApplicationEvent>().Subscribe(e => RequestRestartApplication(), ThreadOption.PublisherThread);

            ThreadStart splashThreadMain =
                        () =>
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(
                              (Action)(() =>
                              {
                                  _container.RegisterType<SplashScreenViewModel, SplashScreenViewModel>();
                                  _container.RegisterType<SplashScreen, SplashScreen>();

                                  var splash = _container.Resolve<SplashScreen>();
                                  _eventAggregator.GetEvent<CloseSplashEvent>().Subscribe(
                                    e => splash.Dispatcher.BeginInvoke((Action)splash.Close),
                                    ThreadOption.PublisherThread, true);

                                  splash.Show();

                                  _waitForSplashScreenCreation.Set();
                              }));

                            Dispatcher.Run();
                        };

            var thread = new Thread(splashThreadMain) { Name = "Splash Thread", IsBackground = true };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            _waitForSplashScreenCreation.WaitOne();
        } 

        private void RequestRestartApplication()
        {
            _logger.Log("Request Restart", Category.Debug, Priority.None);
            _isRestartRequired = true;
        }

        private void RestartApplication()
        {
            _logger.Log("Restarting Application", Category.Debug, Priority.None);
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }
    }
}
