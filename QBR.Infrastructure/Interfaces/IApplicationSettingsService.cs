using System.Configuration;

namespace QBR.Infrastructure.Interfaces
{
    public interface IApplicationSettingsService
    {
        void RegisterModuleSettings(string moduleName, ApplicationSettingsBase settings);

        ApplicationSettingsBase GetModuleSettings(string moduleName);
    }
}
