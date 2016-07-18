
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
    /// State class for the CommsWorker. This encapsulates Waiting for an Ack behaviour that occurs
    /// once a command IFrame has been sent. Once a valid ack is received if there is a response 
    /// expected (usualy determined by the presence of a ResponseHandler) then it will transition 
    /// to the WaitForResponse state. If there is an error that requires a resend then it will
    /// transition back to the SendCommand State for retransmission.
    /// </summary>
    public class WaitForAckState: CommsFSMState
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">the parent state machine of this state</param>
        public WaitForAckState(CommsFSM parent)
            : base(ECommsFSMStateID.WaitForAck, parent)
        { }

        /// <summary>
        /// event handler for when the command is send successfully
        /// </summary>
        /// <param name="ev">event specific data</param>
        /// <returns>ECommsFSMStateID.None if there are no further response expected otherwise 
        /// ECommsFSMStateID.WaitingForResponse</returns>
        private int OnCmdAckComplete(FSMEvent ev)
        {
            var state = (int)ECommsFSMStateID.None;

            if (Parent.ResponseHandler != null)
            {
                //there is a handler that indicates there is a response expected so goto the appropriate state.
                state = (int)ECommsFSMStateID.WaitingForResponse;
            }
            else
            {
                //There is no handler so the operation is done
                Parent.Stop();
                state = (int)ECommsFSMStateID.None;
            }

            return state;
        }

        /// <summary>
        /// Event handler for when comms error is detected.
        /// </summary>
        /// <param name="ev">Event specific data</param>
        /// <returns>ECommsFSMStateID.SendCommand to cause the transition back to the send state
        /// so a retry can be made.</returns>
        private int OnCommError(FSMEvent ev)
        {
            return (int)ECommsFSMStateID.SendCommand;
        }

        /// <summary>
        /// event handler for the data received event
        /// </summary>
        /// <param name="ev">Event specific data</param>
        /// <returns>ECommsFSMStateID.None as no transitionis required</returns>
        private int OnDataReceived(FSMEvent ev)
        {
            Parent.Primary.RespWaitHandle.Set();

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event handler for the continue event. This is event is sent in a loop and allows the state 
        /// to perform some of its work and then unwind the call stack. This is where all the work of 
        /// aggregating data should be performed 
        /// </summary>
        /// <param name="ev">Event specific data</param>
        /// <returns>ECommsFSMStateID.None as the handler itself doesn't cause a transition but could fire
        /// other events that cause a transition.</returns>
        private int OnContinue(FSMEvent ev)
        {
            var result = Parent.Primary.WaitForAck();
            var logMsg = "";
            FSMEvent commsEvent = null; ;

            if (result.Timeout)
            {
                logMsg = "Command Timeout detected";
                commsEvent = new CommsControllerEvent(ECommsFSMEvent.CommsError);
            }
            else if (result.CommsError)
            {
                logMsg = "Command CommsError detected";
                commsEvent = new CommsControllerEvent(ECommsFSMEvent.CommsError);
            }
            else if (result.ReceivedFrame != null)
            {
#if DEBUG                
                Parent.receivedTypes.Add(result.ReceivedFrame);
#endif
                if (result.ReceivedFrame.FrameType == EFrameType.NaK)
                {
                    logMsg = "Command Nak detected";

                    commsEvent = new CommsControllerEvent(ECommsFSMEvent.CommsError);
                }
                else if (result.ReceivedFrame.FrameType != EFrameType.Ack)
                {
                    //If we receive a frame that isn't an ack in this state then it is likely due to a frame  
                    //being corrupt in some way which will have been detected and retried already so just ack
                    logMsg = "Expected an Ack and received : " + result.ReceivedFrame.FrameType.ToString();
                    Parent.Secondary.SendAck(result.ReceivedFrame);
                }
                else if (Parent.Command.SequenceNo != result.ReceivedFrame.SequenceNo)
                {
                    logMsg = "Sequence number mismatch detected ";
                    logMsg += Parent.Command.SequenceNo.ToString() + " != " + result.ReceivedFrame.SequenceNo.ToString();
                    commsEvent = new CommsControllerEvent(ECommsFSMEvent.CommsError);
                }
                else
                {
                    //received a correct Ack
                    commsEvent = new CommsControllerEvent(ECommsFSMEvent.CmdAckComplete);
                    Logger.LogMessage(Logger.IOSwitch, TraceLevel.Info, "Valid Command Ack Received");
                    logMsg = null;
                }
            }
            else
            {
                //no errors and no frame indicates we are still receiving the frame
                logMsg = "Command Ack more data required";
            }

            if (logMsg != null)
            {
                Logger.LogMessage(Logger.IOSwitch, TraceLevel.Verbose, logMsg);
            }
            
            if (commsEvent != null)
            {
                Parent.DispatchEvent(commsEvent);
            }

            return (int)ECommsFSMStateID.None;
        }

        protected override void BuildEventTable()
        {
            base.BuildEventTable();
            _eventTable.Add((int)ECommsFSMEvent.CmdAckComplete, OnCmdAckComplete);
            _eventTable.Add((int)ECommsFSMEvent.CommsError, OnCommError);
            _eventTable.Add((int)ECommsFSMEvent.DataRecvd, OnDataReceived);
            _eventTable.Add((int)ECommsFSMEvent.Continue, OnContinue);
        }

        public override void ExitState()
        {
            base.ExitState();
            if (Parent.Cancelling)
            {
                Parent.Primary.RespWaitHandle.Set();
            }
        }

    }
}
