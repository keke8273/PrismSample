using System;
using System.Collections.Generic;
using DataLinkLayer.IO;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Interfaces
{
    public interface IAnalyzer
    {
        /// <summary>
        /// Initialize analyzer info and status this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Start QC Batch Release Test.
        /// </summary>
        void StartTest();

        /// <summary>
        /// Restart QC Batch Release Test.
        /// </summary>
        void AbortTest();

        /// <summary>
        /// Gets the latest transient record.
        /// </summary>
        void GetLatestTransientRecord();

        BuildInfo BuildInfo { get; }
        AnalyzerTypes AnalyzerType { get; }
        AnalyzerStatus AnalyzerStatus { get; }
        string ConnectionString { get; }
        AResponseHandler ResponseHandler { get;}
        List<IFrame> Frames { get;}

        event EventHandler<AnalyzerStatusChangedEventArgs> AnalyzerStatusChanged;

        event EventHandler<TransientArrivedEventArgs> TransientArrived;

        event EventHandler<ErrorDetectedEventArgs> ErrorDetected;
    }
}
