using System;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.Analyzers;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class AnalyzerConnectionEventArgs : EventArgs
    {
        private readonly bool _isConnected;
        private readonly IAnalyzer _analyzer;

        public AnalyzerConnectionEventArgs(bool isConnected, IAnalyzer analyzer)
        {
            _isConnected = isConnected;
            _analyzer = analyzer;
        }

        public bool IsConnected { get { return _isConnected; } }
        public IAnalyzer Analyzer { get { return _analyzer; } }
    }
}
