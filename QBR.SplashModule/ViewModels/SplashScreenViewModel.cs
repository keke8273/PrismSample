using System.Deployment.Application;
using Microsoft.Practices.Prism.Events;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Models;

namespace QBR.SplashModule.ViewModels
{
    public class SplashScreenViewModel : BindableObject
    {
        #region Private data members
        private string _status;
        #endregion

        #region Constructors

        public SplashScreenViewModel()
        {
        }

        public SplashScreenViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<UpdateSplashEvent>().Subscribe(UpdateStatus);
        }

        #endregion

        #region Properties

        public string ApplicationVersion
        {
            get
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                }

                return "Debug Build";
            }
        }

        public string Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value, "Status"); }
        }
        #endregion

        #region Functions
        private void UpdateStatus(string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return;
            }

            Status = status;
        }
        #endregion
    }
}