
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO.Protocol;
using DataLinkLayer.Utils;

namespace DataLinkLayer.IO.CommsCntrl
{
    /// <summary>
    /// A State machine class used to manage the Data Link layer level communication.
    /// </summary>
    public class CommsFSM: FSM
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsChannel">the communication channel to use.</param>
        public CommsFSM(IIOPort commsChannel)
        {
            commsChannel.DataReceived += new PortDataReceivedEventHandler(commsChannel_DataReceived);
            commsChannel.PortDeviceError += new EventHandler(commsChannel_PortDeviceError);
            
            EventWaitHandle waitHandle = new AutoResetEvent(false);
            
            Primary = new PrimaryNode(commsChannel);
            Primary.RespWaitHandle = waitHandle;
         
            Secondary = new SecondaryNode(commsChannel);
            Secondary.RespWaitHandle = waitHandle;
        }

        void commsChannel_PortDeviceError(object sender, EventArgs e)
        {
            DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.DeviceError));   
        }

        #endregion Constructor

        #region Public Data

        /// <summary>
        /// The interface that will be used to process incoming responses. If this is null then the 
        /// communication doesn't require any response.
        /// </summary>
        public AResponseHandler ResponseHandler { get; set; }

        /// <summary>
        /// Property containing the 'Primary' from the IdleRQ protocol
        /// </summary>
        public PrimaryNode Primary { get; private set; }

        /// <summary>
        /// Property containing the 'Secondary' from the IdleRQ protocol
        /// </summary>
        public SecondaryNode Secondary { get; private set; }

        /// <summary>
        /// get and set the Command to be transmitted property
        /// </summary>
        public IFrame Command { get; set; }

        /// <summary>
        /// Gets and Sets a flag indicating the comms is being cancelled
        /// </summary>
        public bool Cancelling { get; set; }

        /// <summary>
        /// Flag used to indicate an error occured during communications.
        /// </summary>
        public bool Error { get; set; }

#if DEBUG

        /// <summary>
        /// List to help debug it should contain all the frames received by the comms FSM
        /// </summary>
        public List<IFrame> receivedTypes = new List<IFrame>();

#endif
        #endregion Public Data

        #region Private Methods

        /// <summary>
        /// Event handler for when data is received from the comm channel
        /// </summary>
        /// <param name="sender">The origin of the event</param>
        /// <param name="args"></param>
        private void commsChannel_DataReceived(object sender, PortDataReceivedEventArgs args)
        {
            if (!_stopped)
            {
                switch (args.Error)
                {
                    case PortDataReceivedErrors.DataReceived_OK:
                        Logger.LogMessage(Logger.IOSwitch,
                                          TraceLevel.Verbose,
                                          "Data Received in : " + CurrentState.Name);
                        DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.DataRecvd));
                        break;
                    default:

                        Logger.LogMessage(Logger.IOSwitch,
                                         TraceLevel.Warning,
                                        "Unknown comms channel event in " + CurrentState.Name);
                        break;
                }
            }
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// Build state table that will contain the states to do the processing.
        /// </summary>
        protected override void BuildStates()
        {
            _states.Add((int)ECommsFSMStateID.SendCommand,          new SendCmdState(this));
            _states.Add((int)ECommsFSMStateID.WaitForAck,           new WaitForAckState(this));
            _states.Add((int)ECommsFSMStateID.WaitingForResponse,   new WaitForResponseState(this));
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Start the state machine running.
        /// </summary>
        public void Start()
        {
            Reset();

            //start the state machine proper
            Start((int)ECommsFSMStateID.SendCommand);
        }

        /// <summary>
        /// Continues the FSMs process tasks
        /// </summary>
        public void Continue()
        {
            DispatchEvent(new CommsControllerEvent(ECommsFSMEvent.Continue));
        }

        public void Reset()
        {
            //reset the error flag 
            Error = false;

            //reset the comms nodes
            Primary.Reset();
            Secondary.Reset();

            
            _stopped = false;
        }

        #endregion Public Methods
    }

    /// <summary>
    /// Class that defines a state used in the CommsFSM.
    /// </summary>
    public abstract class CommsFSMState : FSMState
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The enum that defines the id for this state.</param>
        /// <param name="parent">The Parent Comms FSM of this state.</param>
        public CommsFSMState(ECommsFSMStateID id, CommsFSM parent)
            :base(id.ToString(), (int) id)
        {
            Parent = parent;
        }

        /// <summary>
        /// Property for getting containing the parent CommsFSM 
        /// </summary>
        protected CommsFSM Parent { get; private set; }

        /// <summary>
        /// Method used to build an event table. This adds the Exit event handler which is common
        /// to all states in the CommsWorker.
        /// </summary>
        protected override void BuildEventTable()
        {
            _eventTable.Add((int)ECommsFSMEvent.Exit, OnExit);
            _eventTable.Add((int)ECommsFSMEvent.DeviceError, OnDeviceError);
        }

        private int OnDeviceError(FSMEvent ev)
        {
            Logger.LogMessage(Logger.IOSwitch, TraceLevel.Warning, "An error occured in the device stopping comms");

            //flag the error
            Parent.Error = true;

            //stop the state machine
            Parent.Stop();

            return (int)ECommsFSMStateID.None;
        }

        /// <summary>
        /// Exit event handler
        /// </summary>
        /// <param name="ev">The event that triggered the handler</param>
        /// <returns>ECommsFSMStateID.None as this terminates the CommsFSM and there is no need to change state.</returns>
        private int OnExit(FSMEvent ev)
        {
            Parent.Stop();

            return (int)ECommsFSMStateID.None;
        }
    }

    /// <summary>
    /// Event class used for event that occur in the communication controller
    /// </summary>
    public class CommsControllerEvent : FSMEvent
    {
        public CommsControllerEvent(ECommsFSMEvent ev)
            : base(ev.ToString(), (int)ev)
        { }

        public CommsControllerEvent(ECommsFSMEvent ev, object data)
            : base(ev.ToString(), (int)ev, data)
        { }
    }

    /// <summary>
    /// Enum defining the different state IDs used in the CommsFSM
    /// </summary>
    public enum ECommsFSMStateID:int 
    {
        None = FSM.FSM_STATE_ID_NONE,

        SendCommand = FSM.FSM_EVENT_ID_START,
        WaitForAck,
        WaitingForResponse,
    }

    /// <summary>
    /// Enum defining the different events that can occur in a comms fsm
    /// </summary>
    public enum ECommsFSMEvent
    {
        Exit = FSM.FSM_EVENT_ID_STOPFSM,
        CommsError,
        ResponseTimeout,
        Continue,
        DataRecvd,
        DeviceError,

        CmdAckComplete,
        ResponseReceived,
    }
}
