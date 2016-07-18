using System;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Interfaces
{
    public interface IAnalyzerConnectionService
    {
        void InitializeConnectedAnanlyzers();

        event EventHandler<AnalyzerConnectionEventArgs> AnalyzerConnection;
    }
}
