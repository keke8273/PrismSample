
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Diagnostics;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO.Protocol;
using DataLinkLayer.Utils;

namespace DataLinkLayer.IO.CommsCntrl
{
    /// <summary>
    /// State class for the CommsWorker state machine. This encapsulates the SendCommand behaviour
    /// which will simply send the Iframe command and then transition to the WaitForAck state.
    /// </summary>
    public class SendCmdState: CommsFSMState
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The parent commsFSM of this state</param>
        public SendCmdState(CommsFSM parent)
            : base(ECommsFSMStateID.SendCommand, parent)
        {}

        #endregion Constructor

        #region Private Methods

        /// <summary>
        /// Event handler for a comms error. This will set the error flag of the parent and stop the 
        /// state machine.
        /// </summary>
        /// <param name="ev">The event that triggered the handler</param>
        /// <returns>ECommsFSMStateID.None there is no transition required as the state machine will be stopped.</returns>
        private int OnCommsError(FSMEvent ev)
        {
            //flag Error in parent
            Parent.Error = true;
            //Stop State machine
            Parent.Stop();

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Event handler for the event that signals to continue working.
        /// </summary>
        /// <param name="ev">The event that trigger the handler</param>
        /// <returns>ECommsFSMStateID.None as the event may cause other transitions by the work that is performed.</returns>
        private int OnContinue(FSMEvent ev)
        {
            var state = (int)ECommsFSMStateID.None;
            if (Parent.Primary.Retries < DeviceProtocol.MAX_RETRIES)
            {
                try
                {
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Info,
                                     "Sending Command : " + Parent.Command.FrameType.ToString());

                    Parent.Primary.Send(Parent.Command);

                    state = (int)ECommsFSMStateID.WaitForAck;
                }
                catch (Exception ex)
                {
                    Logger.LogException(Logger.IOSwitch, ex, "Failed to Send command cleanly");
                    Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.CommsError));
                }
            }
            else
            {
                Parent.DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.CommsError));
            }

            return state;
        }

        /// <summary>
        /// Event handler for when data is received from the physical layer. This will set the wait handle of the CommsNode
        /// waiting for a reponse and will not directly cause a state change. A state change could occur as a result of 
        /// setting the wait handle. Really data shouldn't be received in this state and if it does it simply means that 
        /// it has been received before the transition to the WaitForAck state in which case setting the wait handle will
        /// mean that as soon as the WaitForAck state waits on the handle it will return and it will process the received 
        /// data.
        /// </summary>
        /// <param name="ev">State event containing information about the specific event</param>
        /// <returns>this will always return ECommsFSMStateID.None so there is no transition in state directly.</returns>
        private int OnDataReceived(FSMEvent ev)
        {
            Parent.Primary.RespWaitHandle.Set();

            return (int)ECommsFSMStateID.None;
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Build the event table for the state.
        /// </summary>
        protected override void BuildEventTable()
        {
            base.BuildEventTable();

            _eventTable.Add((int)ECommsFSMEvent.CommsError, OnCommsError);
            _eventTable.Add((int)ECommsFSMEvent.Continue, OnContinue);
            _eventTable.Add((int)ECommsFSMEvent.DataRecvd, OnDataReceived);
        }

        #endregion Protected Methods

    }
}
