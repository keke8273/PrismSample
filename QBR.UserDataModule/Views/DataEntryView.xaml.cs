using System.Windows.Controls;
using Microsoft.Practices.Unity;
using QBR.UserDataModule.ViewModels;

namespace QBR.UserDataModule.Views
{
    /// <summary>
    /// Interaction logic for DataEntryView.xaml
    /// </summary>
    public partial class DataEntryView : UserControl
    {
        public DataEntryView()
        {
            InitializeComponent();
        }

        [Dependency]
        public DataEntryViewModel ViewModel
        {
            set { DataContext = value; }
        }
    }
}
