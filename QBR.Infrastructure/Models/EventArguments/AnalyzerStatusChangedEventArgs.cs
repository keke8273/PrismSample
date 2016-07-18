using System;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class AnalyzerStatusChangedEventArgs : EventArgs
    {
        private readonly AnalyzerStatus _newStatus;

        public AnalyzerStatusChangedEventArgs(AnalyzerStatus newStatus)
        {
            _newStatus = newStatus;
        }

        public AnalyzerStatus NewStatus { get { return _newStatus; } }
    }
}
