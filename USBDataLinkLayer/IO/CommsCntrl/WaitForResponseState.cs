
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System.Diagnostics;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO.Protocol;
using DataLinkLayer.Utils;

namespace DataLinkLayer.IO.CommsCntrl
{
    /// <summary>
    /// State class for the CommsWorker state machine. This class encapsulates the waiting for 
    /// a response and sending an ack behavior. Once a valid response is received it is passed
    /// to the Response handler for processing. The response handler will determine if the frame 
    /// type is handled and if there are more responses expected.
    /// </summary>
    public class WaitForResponseState: CommsFSMState
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The CommsFSM that is the parent of this state.</param>
        public WaitForResponseState(CommsFSM parent)
            : base(ECommsFSMStateID.WaitingForResponse, parent)
        { }

        #endregion Constructor

        #region Private data
        /// <summary>
        /// Count cache for the number of duplicate frames received
        /// </summary>
        private int DuplicateCount { get; set; }

        /// <summary>
        /// Count cache for the number of invalid frames received
        /// </summary>
        private int InvalidCount { get; set; }

        #endregion Private data

        #region Private Methods

        /// <summary>
        /// Event handler for the event that signals to continue working.
        /// </summary>
        /// <param name="ev">The event that trigger the handler</param>
        /// <returns>ECommsFSMStateID.None once in this state there is no leaving until complete one way or another.</returns>
        private int OnContinue(FSMEvent ev)
        {
            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Verbose,
                              string.Format("Waiting for Response in :{0} timeout {1}",
                                            ((ECommsFSMStateID)ID).ToString(),
                                            Parent.ResponseHandler.ResponseTimeoutInterval) );

            var result = Parent.Secondary.WaitForResponse(Parent.ResponseHandler.ResponseTimeoutInterval);

            if (result.Timeout)
            {
                Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.ResponseTimeout));
            }
            else if (result.CommsError)
            {
                Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.CommsError));
            }
            else if (result.ReceivedFrame != null)
            {
                Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.ResponseReceived,
                                                    result));
            }
            else
            {
                //no errors and no frame indicates we are still receiving the frame
                if (!Parent.Cancelling)
                {
                    if (Parent.ResponseHandler != null && (Parent.ResponseHandler.Complete))
                    {
                        Logger.LogMessage(Logger.IOSwitch,
                                          TraceLevel.Info,
                                          "Response handler complete");
                    }
                    else
                    {
                        Logger.LogMessage(Logger.IOSwitch,
                                            TraceLevel.Info,
                                            "More Response Data required");
                    }
                }
            }

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event Handler for when a valid response is received
        /// </summary>
        /// <param name="ev">The event that triggered the handler</param>
        /// <returns></returns>
        private int OnResponseReceived(FSMEvent ev)
        {
            var result = ev.Data as CommsResult;
            
            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Verbose,
                              "Valid Response Command Received");

            if(result != null)
            {
#if DEBUG
                Parent.receivedTypes.Add(result.ReceivedFrame);
#endif
                if ((result.ReceivedFrame.FrameType != EFrameType.Ack)
                  && (result.ReceivedFrame.FrameType != EFrameType.NaK))
                {
                    //Send the Ack
                    Parent.Secondary.SendAck(result.ReceivedFrame);

                    //check for duplicates
                    if (result.Duplicate)
                    {
                        DuplicateCount++;
                    }
                    else
                    {
                        //a non-duplicate has been received reset the count
                        DuplicateCount = 0;

                        //attempt to handle the frame that has been received
                        var Handled = Parent.ResponseHandler.HandleResponse(result.ReceivedFrame);

                        //if the frame wasn't handled then it was a frame that wasn't expected.
                        if (!Handled)
                        {
                            InvalidCount++;
                        }
                        else
                        {
                            //reset the flag as a valid frame has been received and handled.
                            InvalidCount = 0;
                        }
                    }

                    if ((DuplicateCount == DeviceProtocol.MAX_DUPLICATES)
                       || (InvalidCount == DeviceProtocol.MAX_INVALID))
                    {
                        Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.DeviceError));
                    }
                }
                else
                {
                    InvalidCount++;
                }
            }

            return (int) ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event handler for when a comms error occurs
        /// </summary>
        /// <param name="ev">The event that caused the problem</param>
        /// <returns>ECommsFSMStateID.None </returns>
        private int OnCommError(FSMEvent ev)
        {
            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Error,
                              "Response CommsError detected");

            //invalid frame received so send a nak.
            Parent.Secondary.SendNak();
            InvalidCount++;

            if (InvalidCount == DeviceProtocol.MAX_INVALID)
            {
                //there is an error there are too many invalid frames received in a row
                Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.DeviceError));
            }
        

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event handler for a comms timeout.
        /// </summary>
        /// <param name="ev">The event that trigger the handler</param>
        /// <returns>ECommsFSMStateID.None as this will cause the state machine to stop.</returns>
        /// <remarks>Timeouts don't necessarily mean an error. There are two reasons for the timeout
        /// the first is when there is no response from the device (device has gone mute an error). The second
        /// is when all of the data has been received to make sure the last Ack frame got through there 
        /// is a short wait for retransmission at which point the timeout isn't an error the task is simply
        /// complete.</remarks>
        private int OnResponseTimeout(FSMEvent ev)
        {
            if (!Parent.ResponseHandler.Complete)
            {
                //only report the error if the response handler isn't complete
                Logger.LogMessage(Logger.IOSwitch,
                                  TraceLevel.Warning,
                                  "Response Timeout detected");
                Parent.Error = true;
            }
            //Stop the state machine as the task is complete (see method header comments for more details)
            Parent.Stop();

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event Handler for when data is received from the Comms Channel
        /// </summary>
        /// <param name="ev">The event that triggered the handler</param>
        /// <returns>ECommsFSMStateID.None as no transition is required</returns>
        private int OnDataReceived(FSMEvent ev)
        {
            Parent.Secondary.RespWaitHandle.Set();

            return (int)ECommsFSMStateID.None;
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Build an event table for this state.
        /// </summary>
        protected override void BuildEventTable()
        {
            base.BuildEventTable();

            _eventTable.Add((int)ECommsFSMEvent.ResponseReceived, OnResponseReceived);
            _eventTable.Add((int)ECommsFSMEvent.CommsError, OnCommError);
            _eventTable.Add((int)ECommsFSMEvent.ResponseTimeout, OnResponseTimeout);
            _eventTable.Add((int)ECommsFSMEvent.DataRecvd, OnDataReceived);
            _eventTable.Add((int)ECommsFSMEvent.Continue, OnContinue);
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Exit State Handler.
        /// </summary>
        public override void ExitState()
        {
            base.ExitState();

            if (Parent.Cancelling)
            {
                Parent.Secondary.Stop();
            }
        }

        #endregion Public Methods

    }
}
