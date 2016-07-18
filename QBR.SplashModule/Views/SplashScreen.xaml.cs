using Microsoft.Practices.Unity;
using System.Windows;
using QBR.SplashModule.ViewModels;

namespace QBR.SplashModule.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        [Dependency]
        public SplashScreenViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}
