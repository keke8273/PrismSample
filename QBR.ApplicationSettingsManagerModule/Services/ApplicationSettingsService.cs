using System.Collections.Generic;
using System.Configuration;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using QBR.Infrastructure.CompositePresentationEvents;
using QBR.Infrastructure.Interfaces;

namespace QBR.ApplicationSettingsManagerModule.Services
{
    public class ApplicationSettingsService : IApplicationSettingsService
    {
        private Dictionary<string, ApplicationSettingsBase> _applicationSettingsSections;
        private readonly ILoggerFacade _logger;

        public ApplicationSettingsService(IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            _logger = logger;
            eventAggregator.GetEvent<CloseApplicationEvent>().Subscribe(o => OnCloseApplication());
        }

        public void RegisterModuleSettings(string moduleName, ApplicationSettingsBase settings)
        {
            if(_applicationSettingsSections == null)
                _applicationSettingsSections = new Dictionary<string, ApplicationSettingsBase>();

            _applicationSettingsSections.Add(moduleName, settings);
        }

        public ApplicationSettingsBase GetModuleSettings(string moduleName)
        {
            return _applicationSettingsSections[moduleName];
        }

        private void OnCloseApplication()
        {
            foreach (var applicationSettingsSection in _applicationSettingsSections)
            {
                _logger.Log("Saving " + applicationSettingsSection.Key + " settings", Category.Info, Priority.None);
                applicationSettingsSection.Value.Save();
            }
        }
    }
}
