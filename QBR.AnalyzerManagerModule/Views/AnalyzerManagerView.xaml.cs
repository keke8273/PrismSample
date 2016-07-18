using System.Windows.Controls;
using Microsoft.Practices.Unity;
using QBR.AnalyzerManagerModule.ViewModels;

namespace QBR.AnalyzerManagerModule.Views
{
    /// <summary>
    /// Interaction logic for AnalyzerManagerView.xaml
    /// </summary>
    public partial class AnalyzerManagerView : UserControl
    {
        public AnalyzerManagerView()
        {
            InitializeComponent();
        }

        [Dependency]
        public AnalyzerManagerViewModel ViewModel
        {
            set { DataContext = value; }
        }    
    }
}
