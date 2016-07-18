using Microsoft.Practices.Unity;
using QBR.Infrastructure.Interfaces;
using QBR.Shell.ViewModels;

namespace QBR.Shell.Views
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : IShell
    {
        public Shell()
        {
            InitializeComponent();
        }

        [Dependency]
        public ShellViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}
