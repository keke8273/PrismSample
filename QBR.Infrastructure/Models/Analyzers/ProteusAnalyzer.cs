using System;
using System.Collections.Generic;
using DataHandler.ResponseHandlers;
using DataLinkLayer.IO;
using DataLinkLayer.IO.CommsCntrl;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;
using QBR.Infrastructure.Models.ResponseHandlers;

namespace QBR.Infrastructure.Models.Analyzers
{
    /// <summary>
    /// The concrete ABaseDeviceController implementation for Proteus analyzer
    /// </summary>
    public class ProteusAnalyzer : AnalyzerBase
    {

        #region Member Variables

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteusAnalyzer"/> class.
        /// </summary>
        public ProteusAnalyzer()
        {
            AnalyzerType = AnalyzerTypes.PRO;
            ConnectionString = string.Empty;             
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteusAnalyzer" /> class.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="datalinkLayer">The datalink layer.</param>
        public ProteusAnalyzer(IIOPort port, string connectionString, ICommsWorker datalinkLayer, IDispatcherService dispatcherService)
            : base(port, connectionString, datalinkLayer, dispatcherService)
        {
            AnalyzerType = AnalyzerTypes.PRO;
            //Register to receive the event notification from the datalink layer
            DataLinkLayer.RunWorkerCompleted += DataLinkLayerRunWorkerCompleted;
        }

        /// <summary>
        /// Connects this instance.
        /// </summary>
        public override void Initialize()
        {
            PhysicalLayerPort.Open();
            AnalyzerStatus = AnalyzerStatus.Initializing;
            AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
            SendGetBuildInfo();
        }

        public override void StartTest()
        {
            throw new NotImplementedException();
        }

        public override void AbortTest()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the latest transient record.
        /// </summary>
        public override void GetLatestTransientRecord()
        {
            AnalyzerStatus = AnalyzerStatus.SendingTransient;
            AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
            SendGetLatestTransient();
        }

        /// <summary>
        /// Handles an application layer event. This acts as a very simply state machine.
        /// </summary>
        /// <param name="p">The is a reference to an object relevant to the current state of the application</param>
        protected override void HandleAppLayerEvent(object p)
        {
            switch (AnalyzerStatus)
            {
                case AnalyzerStatus.Initializing:
                    if (p.GetType() == typeof(ProteusBuildInfo))
                    {
                        BuildInfo = p as BuildInfo;

                        AnalyzerStatus = AnalyzerStatus.Idle;
                        AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                    }
                    else
                    {
                        AnalyzerStatus = AnalyzerStatus.Failed;
                        AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                    }
                    break;

                case AnalyzerStatus.SendingTransient:
                    if (p.GetType() == typeof (List<Transient>))
                    {
                        var transient = (p as List<Transient>)[0];

                        if (transient.Result.ValidData == EValidResult.ValidRecord)
                        {
                            TransientArrived.Raise(this, new TransientArrivedEventArgs(transient));
                        }
                        else if (transient.Result.ValidData == EValidResult.NoMoreRecords)
                        {
                            AnalyzerStatus = AnalyzerStatus.TestCompleted;
                            AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                        }
                        else
                        {
                            AnalyzerStatus = AnalyzerStatus.Failed;
                            AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                        }
                    }
                    else
                    {
                        AnalyzerStatus = AnalyzerStatus.Failed;
                        AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                    }
                    break;
            }   
        }

        /// <summary>
        /// Helper method that constructs the required Iframes and response handler for a get build information request
        /// </summary>
        private void SendGetBuildInfo()
        {
            Frames = new List<IFrame> { FrameBuilder.Instance.Build_SimpleCommand(EFrameType.GetBuildInfo) };
            ResponseHandler = new SimpleResponseHandler<ProteusBuildInfo>();
            DataLinkLayer.Start(Frames, ResponseHandler);
        }

        /// <summary>
        /// Sends the get first transient result.
        /// </summary>
        private void SendGetLatestTransient()
        {
            Frames = new List<IFrame> ();
            Frames.AddRange(FrameBuilder.Instance.Build_GetNthTransientResult(1));
            ResponseHandler = new TransientRecordResponseHandler(DataLinkLayer as CommsWorker, Frames.Count);
            DataLinkLayer.Start(Frames, ResponseHandler);
        }

        #region Events
        public override event EventHandler<AnalyzerStatusChangedEventArgs> AnalyzerStatusChanged;
        public override event EventHandler<TransientArrivedEventArgs> TransientArrived;
        public override event EventHandler<ErrorDetectedEventArgs> ErrorDetected;
        #endregion
    }
}
