using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Interfaces
{
    public interface IRecordTranslateService
    {
        void CreateOutputDirectory();

        string ProteusPatientToMob(Transient transient, string vialCaseId, out string filePath);

        string ProteusLQCToMob(Transient transient, string vialCaseId, out string filePath);

        string AnalyzerErrorToMob(AnalyzerFailure analyzerFailure, string vialCaseId, BuildInfo buildInfo, out string filePath);

        string GetSecurityFilePath();
    }
}