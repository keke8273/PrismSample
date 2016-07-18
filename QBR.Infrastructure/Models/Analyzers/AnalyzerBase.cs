using System;
using System.Collections.Generic;
using System.ComponentModel;
using DataLinkLayer.IO;
using DataLinkLayer.IO.CommsCntrl;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Extensions;
using QBR.Infrastructure.Interfaces;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Models.EventArguments;

namespace QBR.Infrastructure.Models.Analyzers
{
    public abstract class AnalyzerBase : IAnalyzer
    {
        protected AnalyzerBase()
        {
        }

        protected AnalyzerBase(IIOPort port, string connectionString, ICommsWorker datalinkLayer, IDispatcherService dispatcherService)
        {
            PhysicalLayerPort = port;
            DataLinkLayer = datalinkLayer;
            BuildInfo = new BuildInfo();
            ConnectionString = connectionString;
            DispatcherService = dispatcherService;
        }

        public abstract void Initialize();
        public abstract void StartTest();
        public abstract void AbortTest();
        public abstract void GetLatestTransientRecord();

        public BuildInfo BuildInfo { get; protected set; }
        public AnalyzerTypes AnalyzerType { get; protected set; }
        public AnalyzerStatus AnalyzerStatus { get; protected set; }
        public string ConnectionString { get; protected set; }
        public AResponseHandler ResponseHandler { get; protected set; }
        public List<IFrame> Frames { get; protected set; }

        protected IIOPort PhysicalLayerPort;
        protected ICommsWorker DataLinkLayer;
        protected IDispatcherService DispatcherService;

        protected void DataLinkLayerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs eventArgs)
        {
            DatalinkEventArgs args;

            if (eventArgs.Cancelled)
            {
                //The comms task was cancelled in this context it is an error
                args = new DatalinkEventArgs(DatalinkLayerEventType.Cancelled);
            }
            else
            {
                //No error work out if there is some data retrieved
                if (ResponseHandler == null)
                {
                    //The command has been sent there is no response expected
                    args = new DatalinkEventArgs(DatalinkLayerEventType.DataSent);
                }
                //data should have been received work out if it is complete 
                else if (ResponseHandler.Complete)
                {
                    //There is a response attach the data 
                    args = new DatalinkEventArgs(DatalinkLayerEventType.DataReceived,
                                                    ResponseHandler.Data);
                }
                else
                {
                    //A Comms Error has Occurred
                    args = new DatalinkEventArgs(DatalinkLayerEventType.CommsError);
                }
            }

            //the task is complete whether it was from error or the task operations are complete clean up 
            ResponseHandler = null;

            //Fire the event to let subscribers know the operation is complete. The invoke call to the dispatcher
            //will marshal it back to the "UI" thread
            DispatcherService.Dispatch(new Action<object>(HandleDatalinkLayerEvent), args);
        }

        /// <summary>
        /// Converts the data link layer events into application layer events
        /// </summary>
        /// <param name="args">event arguments that contain more information on the actual event.</param>
        protected void HandleDatalinkLayerEvent(object args)
        {
            var evArgs = args as DatalinkEventArgs;
            switch (evArgs.EventType)
            {
                case DatalinkLayerEventType.DataReceived:
                    HandleAppLayerEvent(evArgs.ReceivedData);
                    break;

                case DatalinkLayerEventType.Progress:
                    //Progress = (int)evArgs.ReceivedData;
                    break;
                case DatalinkLayerEventType.DataSent: //fall through

                //the data sent should never happen under the Proteus protocol because all command either require a 
                //response in the form of data returned or a Confirmation message with status info. The DataSent
                //Comms event signifies that a command has been sent but that command didn't require a response. So 
                //that makes it an error under the Proteus protocol.

                case DatalinkLayerEventType.CommsError: //fall through
                case DatalinkLayerEventType.Cancelled:  //fall through
                default:
                    AnalyzerStatus = AnalyzerStatus.Failed;
                    AnalyzerStatusChanged.Raise(this, new AnalyzerStatusChangedEventArgs(AnalyzerStatus));
                    break;
            }
        }

        protected abstract void HandleAppLayerEvent(object p);

        public virtual event EventHandler<AnalyzerStatusChangedEventArgs> AnalyzerStatusChanged;
        public virtual event EventHandler<TransientArrivedEventArgs> TransientArrived;
        public virtual event EventHandler<ErrorDetectedEventArgs> ErrorDetected;
    }
}
