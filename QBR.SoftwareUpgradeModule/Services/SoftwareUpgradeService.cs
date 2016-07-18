using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;

namespace QBR.SoftwareUpgradeModule.Services
{
    class SoftwareUpgradeService : ISoftwareUpgradeService
    {
        private readonly ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly bool _isNetworkDeployment;
        private AutoResetEvent _waitForUpdate;
        private Version _newVersion;

        #region Constructors
        public SoftwareUpgradeService(IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;
            _isNetworkDeployment = ApplicationDeployment.IsNetworkDeployed;
        } 
        #endregion

        #region ISoftwareUpgradeService Members
        public void CheckForUpdate()
        {
            _eventAggregator.GetEvent<UpdateSplashEvent>().Publish("Check for updates...");

            if (!_isNetworkDeployment) return;

            CurrentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            _logger.Log("Check for updates for " + CurrentVersion, Category.Info, Priority.None);
            ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += OnCheckForUpdateCompleted;
            ApplicationDeployment.CurrentDeployment.CheckForUpdateProgressChanged += OnCheckForUpdateProgressChanged;
            ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();
            _waitForUpdate = new AutoResetEvent(false);
            _waitForUpdate.WaitOne();
            Thread.Sleep(500);
        }

        public Version CurrentVersion { get; private set; } 
        #endregion

        private void OnCheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs eventArgs)
        {
            var message = "Check for updates\n" + String.Format("Downloading: {0}. {1:D}K of {2:D}K downloaded.", GetProgressString(eventArgs.State), eventArgs.BytesCompleted / 1024, eventArgs.BytesTotal / 1024);
            _eventAggregator.GetEvent<UpdateSplashEvent>().Publish(message);
        }

        private void OnCheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs eventArgs)
        {
            string message;

            if (eventArgs.Error != null)
            {
                message = ("Error : " + eventArgs.Error.Message);
            }
            else if (eventArgs.Cancelled)
            {
                message = "Update canceled.";
            }
            else if (eventArgs.UpdateAvailable)
            {
                _newVersion = eventArgs.AvailableVersion;
                message = "Updating " + ApplicationDeployment.CurrentDeployment.CurrentVersion + " to " + _newVersion + "\n";

                //todo:: may need to ask for user confirmation before doing an upgrade. For the time being, mandatory upgrades. 
                ApplicationDeployment.CurrentDeployment.UpdateCompleted += OnUpdateCompleted;
                ApplicationDeployment.CurrentDeployment.UpdateProgressChanged += OnUpdateProgressChanged;
                ApplicationDeployment.CurrentDeployment.UpdateAsync();
            }
            else
            {
                message = "Software is up to date.";
            }

            _eventAggregator.GetEvent<UpdateSplashEvent>().Publish(message);

            if (!eventArgs.UpdateAvailable)
            {
                _waitForUpdate.Set();
            }
        }

        private void OnUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs eventArgs)
        {
            var message = "Updating " + ApplicationDeployment.CurrentDeployment.CurrentVersion + " to " + _newVersion + "\n";
            message += String.Format("Downloading: {0}. {1:D}K of {2:D}K downloaded.", GetProgressString(eventArgs.State), eventArgs.BytesCompleted / 1024, eventArgs.BytesTotal / 1024);

            _eventAggregator.GetEvent<UpdateSplashEvent>().Publish(message);
        }

        private void OnUpdateCompleted(object sender, AsyncCompletedEventArgs eventArgs)
        {
            string message;
            bool isRestartRequired = false;

            if (eventArgs.Error != null)
            {
                message = ("Error : " + eventArgs.Error.Message);
            }
            else if (eventArgs.Cancelled)
            {
                message = "Update canceled.";
            }
            else
            {
                isRestartRequired = true;
                message = "Restarting software.";
            }

            _eventAggregator.GetEvent<UpdateSplashEvent>().Publish(message);


            if (isRestartRequired)
            {
                _eventAggregator.GetEvent<RestartApplicationEvent>().Publish(true);
            }

            _waitForUpdate.Set();
        }

        private string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "application files";
            }

            if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "application manifest";
            }

            return "deployment manifest";
        }
    }
}
