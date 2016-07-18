using System.Windows.Controls;
using Microsoft.Practices.Unity;
using QBR.TestManagerModule.ViewModels;

namespace QBR.TestManagerModule.Views
{
    /// <summary>
    /// Interaction logic for TestProgressView.xaml
    /// </summary>
    public partial class TestProgressView : UserControl
    {
        public TestProgressView()
        {
            InitializeComponent();
        }

        [Dependency]
        public TestProgressViewModel ViewModel
        {
            set { DataContext = value; }
        }    
    }
}
