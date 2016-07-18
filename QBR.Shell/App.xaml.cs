using System.Windows;
using log4net.Config;

namespace QBR.Shell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure Log4Net
            XmlConfigurator.Configure();

            // Start Bootstrapper
            var bootstrapper = new QBRBootstrapper();
            bootstrapper.Run();
        }
    }
}
