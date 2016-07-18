using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Interfaces
{
    public interface IAnalyzerConfigurationService
    {
        AnalyzerID GetAnalyzerID(string serialNumber);

        int GetAnalyzerCount();
    }
}