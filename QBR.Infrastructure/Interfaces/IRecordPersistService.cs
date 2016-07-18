using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Interfaces
{
    public interface IRecordPersistService
    {
        void PersistTransient(Transient transient, string vialCaseID);

        void PersistAnalyzerFailure(ErrorDetectedEventArgs analyzerFailure);
    }
}