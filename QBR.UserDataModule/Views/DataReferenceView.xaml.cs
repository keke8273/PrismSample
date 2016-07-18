using System.Windows.Controls;
using Microsoft.Practices.Unity;
using QBR.UserDataModule.ViewModels;

namespace QBR.UserDataModule.Views
{
    /// <summary>
    /// Interaction logic for DataReferenceView.xaml
    /// </summary>
    public partial class DataReferenceView : UserControl
    {
        public DataReferenceView()
        {
            InitializeComponent();
        }

        [Dependency]
        public DataReferenceViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}
