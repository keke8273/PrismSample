using System;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.EventArguments
{
    public class ErrorDetectedEventArgs : EventArgs
    {
        private readonly BuildInfo _buildInfo;
        private readonly AnalyzerFailure _analyzerFailure;

        public ErrorDetectedEventArgs(BuildInfo buildInfo, AnalyzerFailure analyzerFailure)
        {
            _buildInfo = buildInfo;
            _analyzerFailure = analyzerFailure;
        }

        public BuildInfo BuildInfo
        {
            get { return _buildInfo; }
        }

        public AnalyzerFailure AnalyzerFailure
        {
            get { return _analyzerFailure; }
        }

        public string VialCaseId { get; set; }
    }
}
