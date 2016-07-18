
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
using DataLinkLayer.Diagnostics;

namespace DataLinkLayer.Utils
{
    /// <summary>
    /// The Finite State Machine class
    /// </summary>
    /// <remarks>
    ///     <para>
    ///     A FSM can be constructed, started, events are dispatched triggering transitions between states
    ///     and possibly transition triggers (work done on a transition).
    ///     </para>
    ///     <para>
    ///     The FSM works with a dictionary of state instances - these are the only states that the FSM can
    ///     work with and any deriving class must override 'BuildStates' and populate the list. The main design
    ///     concept here is that the FSM knows states as an ID/Instance pair. The underlying states know nothing
    ///     about state instances, they just work with state IDs. As such its the FSM itself that dictates the
    ///     actual state object instance(s) to be worked with. If all states are in essence singletons then this
    ///     is of little value, but when identical, parallel state machines are needed then singletons are are a
    ///     pain and each FSM needs an instance of the same class/classes.
    ///     </para>
    /// </remarks>
    public abstract class FSM
    {
        #region Constructors

        public FSM()
        {
            CurrentState = null;
            PreviousState = null;

            _states = new Dictionary<int, FSMState>();

            BuildStates();
        }

        #endregion // Constructors

        //None
        #region Private Data

        #endregion // Private Data

        #region Private Methods

        /// <summary>
        /// Transition from the current state to the next state
        /// </summary>
        /// <param name="NextState"><see cref="HostSimulator.Controller.NextState"/> The state to transition to </param>
        /// <exception cref="ArgumentNullException"> if NextState is <c>null</c></exception>
        private void _transition(FSMState NextState)
        {
            if (NextState != null)
            {
                if (CurrentState != NextState)
                {
                    // We are going somewhere new
                    Logger.LogMessage(Logger.CntrlSwitch,
                                          TraceLevel.Info,
                                          "Transition from : '" + CurrentState.Name + "' to '" + NextState.Name + "'");

                    // Exit the current state
                    CurrentState.ExitState();

                    // Fire-off any transition triggers
                    CurrentState.FireTriggers(NextState.ID);

                    // Transition - a transition to self does not change our notion
                    // of previous state.
                    if (CurrentState != NextState)
                    {
                        PreviousState = CurrentState;
                    }

                    CurrentState = NextState;

                    // Enter the next state
                    CurrentState.EnterState();
                }
                else
                {
                    // We are re-entring the current state - not necessarily an error, but we will flag this
                    Logger.LogMessage(Logger.CntrlSwitch,
                                          TraceLevel.Warning,
                                          "Transition to self in state '" + CurrentState.Name + "'");
                }
            }
            else
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                      TraceLevel.Error,
                                      "State: " + CurrentState.Name + "ArgumentNullException:NextState == null");

                throw (new ArgumentNullException("NextState == null"));
            }
        }

        /// <summary>
        /// Dispatch an event to the current state for processing
        /// </summary>
        /// <param name="ev"><see cref="HostSimulator.Controller.FSMEvent"/> The event to dispatch</param>
        /// <exception cref="ArgumentNullException"> if ev is <c>null</c></exception>
        private void _dispatchEvent(FSMEvent ev)
        {
            if (ev != null)
            {
                if (CurrentState.IsEventHandled(ev) == true)
                {
                    // Invoke the handler
                    var nextState = CurrentState.HandleEvent(ev);

                    if (nextState != FSM_STATE_ID_EXIT)
                    {
                        if (_states.ContainsKey(nextState) == true)
                        {
                            // Transition to the next state
                            var nextStateInstance = _states[nextState];
                            _transition(nextStateInstance);
                        }
                        else
                        {
                            // Just stay in this state
                            if (nextState != FSM_STATE_ID_NONE)
                            {
                                Logger.LogMessage(Logger.CntrlSwitch,
                                                 TraceLevel.Warning,
                                                 "Unable to transition to that state");
                            }
                        }
                    }
                    else
                    {
                        // Force an exit from the current state
                        CurrentState.ExitState();
                    }
                }
                else
                {
                    // This event is not handled in this state
                    Logger.LogMessage(Logger.CntrlSwitch,
                                          TraceLevel.Warning,
                                          string.Format("Event '{0}' not handled in state '{1}'", ev.Name, CurrentState.Name));
                }
            }
            else
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Error,
                                        string.Format("State '{0}' : attempting to dispatch null event", CurrentState.Name));

                throw (new ArgumentNullException("ev == null"));
            }
        }

        #endregion // Private Methods

        #region Protected Data

        /// <summary>
        /// The state instances that this FSM works with stored as a dictionary
        /// and keyed on state ID.
        /// </summary>
        protected Dictionary<int, FSMState> _states;

        /// <summary>
        /// Shows whether the FSM is stopped or not
        /// </summary>
        protected bool _stopped { get; set; }

        #endregion Protected Data

        #region Protected Methods

        /// <summary>
        /// The FSM calls this to build the state instances it is to work with.
        /// The FSM is required to populate the _states dictionary with the instances.
        /// </summary>
        protected abstract void BuildStates();

        #endregion

        #region Public Data

        /// <summary>
        /// The current state of the FSM
        /// </summary>
        public FSMState CurrentState { get; private set; }

        /// <summary>
        /// The previous state of the FSM
        /// </summary>
        public FSMState PreviousState { get; private set; }

        public bool Stopped
        {
            get
            {
                return _stopped;
            }
        }

        /// <summary>
        /// A generic ID for a transition to 'no state'. No FSM state table should contain a state
        /// with an ID of this (and hence the provision of the first 'valid' state ID below).
        /// </summary>
        public const int FSM_STATE_ID_NONE = 0;

        /// <summary>
        /// This state ID is used to force the FSM to exit - this is different from NONE as NONE
        /// will cause the FSM to stay active and remain in the current state. By using EXIT the
        /// FSM will terminate and the stateExit method of the current state is called directly
        /// </summary>
        public const int FSM_STATE_ID_EXIT = -1;

        /// <summary>
        /// The starting ID for all other state IDs
        /// </summary>
        public const int FSM_STATE_ID_START = FSM_STATE_ID_NONE + 1;

        /// <summary>
        /// Constants defining the Stop event and also the starting point for all other event IDs
        /// </summary>
        public const string FSM_STOP_EVENT_NAME = "FSMStop";
        
        /// <summary>
        /// Constant defining the Stop event ID
        /// </summary>
        public const int FSM_EVENT_ID_STOPFSM = 0;
        
        /// <summary>
        /// Constant defining the Start event ID
        /// </summary>
        public const int FSM_EVENT_ID_START = FSM_EVENT_ID_STOPFSM + 1;

        #endregion // Public Data

        #region Public Methods

        /// <summary>
        /// Start the FSM in the specified state
        /// </summary>
        /// <param name="StartState"><see cref="int"/> The ID of the state at which to start</param>
        /// <exception cref="ArgumentException"> if StartState is not a recognized state for this FSM</exception>
        public void Start(int StartState)
        {
            _stopped = false;

            // Lookup the start state
            if (_states.ContainsKey(StartState) == true)
            {
                // Grab the start state instance
                var startStateInstance = _states[StartState];

                CurrentState = startStateInstance;

                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Info,
                                        "Starting FSM in state '" + CurrentState.Name + "'");

                // Enter the start state
                CurrentState.EnterState();
            }
            else
            {
                //Unknown start state
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Error,
                                        "ArgumentException:StartState not recognised");

                throw (new ArgumentException("StartState is unrecognized"));
            }
        }

        /// <summary>
        /// Stops the FSM from processing any further state transitions
        /// </summary>
        public virtual void Stop()
        {
            Logger.LogMessage(Logger.CntrlSwitch,
                                  TraceLevel.Warning,
                                  "Stopping FSM in state: " + CurrentState.Name);
            
            CurrentState.ExitState();

            _stopped = true;
        }

        /// <summary>
        /// Dispatch an event to the current state for processing
        /// </summary>
        /// <param name="ev"><see cref="HostSimulator.Controller.FSMEvent"/> The event to dispatch</param>
        /// <exception cref="ArgumentNullException"> if ev is <c>null</c></exception>
        public void DispatchEvent(FSMEvent ev)
        {
            // If we have been stopped then raise the 'stopped' event to the FSM. Note : as the request to
            // stop the FSM is actioned as part of the event dispatch sequence then the FSM must be executing
            // event processing (dispatch-handle-dispatch-handle-etc-etc) otherwise it will never see the stop event.
            // Stopping the FSM has been implemented in this way to ensure that the thread on which the FSM is executing
            // is the thread that generates and dispatches the stop event.
            if (_stopped == true)
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Warning,
                                        "Forcing FSM to stop");

                // Must clear the stopped flag BEFORE we dispatch the next event!
                _stopped = false;

                // We only try and stop the FSM once - if the stop event goes unhandled then the FSM will not be stopped
                var stopEvent = new FSMEvent(FSM_STOP_EVENT_NAME, FSM_EVENT_ID_STOPFSM);

                _dispatchEvent(stopEvent);
            }
            else
            {
                // dispatch the original event
                _dispatchEvent(ev);
            }
        }

        /// <summary>
        /// Return to the previous state (if we have one)
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This facility is only one-state-deep, i.e. there is not a stack of states
        ///     that can be unwound. Calling this method repeatedly without any other transitions
        ///     occurring will cause an endless flip-flop from one state to the other and back again
        ///     </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException"> if StartState is <c>null</c></exception>
        public void ReturnToPreviousState()
        {
            if (PreviousState != null)
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Info,
                                        "Returning to previous state = " + PreviousState.Name);

                // Back to whence we came
                _transition(PreviousState);
            }
            else
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Error,
                                        "State: " + CurrentState.Name + "InvalidOperationException:_previousStatus == null");

                throw (new InvalidOperationException("_previousStatus == null"));
            }
        }

        /// <summary>
        /// Get the name of the current state
        /// </summary>
        /// <returns> <see cref="System.string"/> containing the state name</returns> 
        public string GetCurrentStateByName()
        {
            var name = "";

            if (CurrentState != null)
            {
                name = CurrentState.Name;
            }
            else
            {
                // We have no state yet, so just return empty string
                name = "";
            }

            return (name);

        }

        #endregion // Public Methods
    }

    /// <summary>
    /// The FSM State Class (abstract)
    /// </summary>
    public abstract class FSMState
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public FSMState(string name, int id)
        {
            _eventTable = new Dictionary<int, EventHandlerDelegate>();
            _transitionTriggers = new Dictionary<int, FSMTriggerDelegate>();

            BuildEventTable();
            BuildTransitionTable();

            Name = name;
            ID = id;
        }

        #endregion // Constructors

        #region Protected Data

        /// <summary>
        /// Dictionary mapping event IDs to event handlers
        /// </summary>
        protected Dictionary<int, EventHandlerDelegate> _eventTable;

        /// <summary>
        /// Dictionary mapping the next state to a transition trigger
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     A trigger is a body of work that is required to happen when transitioning
        ///     from one state to another. The idea is that the triggers will be fired when
        ///     a transition occurs from the current state to the next state.
        ///     </para>
        /// </remarks>
        protected Dictionary<int, FSMTriggerDelegate> _transitionTriggers;

        #endregion

        #region Protected Methods

        /// <summary>
        /// To construct the public event table for this state
        /// </summary>
        protected abstract void BuildEventTable();

        /// <summary>
        /// To construct the public transition trigger table for this state
        /// </summary>
        protected virtual void BuildTransitionTable()
        {
        }

        #endregion

        #region Public Data

        /// <summary>
        /// Public read / write string property for the state Name
        /// </summary>
        public string Name { get; private set; }

        public int ID { get; private set; }

        #endregion // Public Data

        #region Public Methods

        /// <summary>
        /// The method called when entering the state
        /// </summary>
        public virtual void EnterState()
        {
            Logger.LogMessage(Logger.CntrlSwitch,
                                    TraceLevel.Info,
                                    "Entering State '" + Name + "'");
        }

        /// <summary>
        /// The method called when exiting the state
        /// </summary>
        public virtual void ExitState()
        {
            Logger.LogMessage(Logger.CntrlSwitch,
                                    TraceLevel.Info,
                                    "Exiting State '" + Name + "'");
        }

        /// <summary>
        /// The method called execute the transition triggers (if any) when a transition
        /// is occurring from this state to the NextState
        /// </summary>
        /// <param name="nextState"> The next state</param>
        public void FireTriggers(int nextState)
        {
            FSMTriggerDelegate triggers = null;

            // Lookup to see if we have any transition triggers for moving from
            // this state to the specified next state
            if (_transitionTriggers.ContainsKey(nextState) == true)
            {
                triggers = _transitionTriggers[nextState];

                // If we have something to fire then do so
                if (triggers != null)
                {
                    triggers();
                }
            }
            else
            {
                // No triggers for this transition
            }
        }

        /// <summary>
        /// Evaluates whether this state handles the specified event
        /// </summary>
        /// <param name="ev"><see cref="HostSimulator.Controller.FSMEvent"/> The event to test for</param>
        /// <returns> <see cref="System.bool"/> True if handled, otherwise false</returns> 
        public bool IsEventHandled(FSMEvent ev)
        {
            var isHandled = false;

            if (ev != null)
            {
                // Go through the event table and look to see if this event is handled
                isHandled = _eventTable.ContainsKey(ev.ID);
            }
            else
            {
                isHandled = false;
            }

            return (isHandled);
        }

        /// <summary>
        /// Invokes the handler for the specified event
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     The handler can return the state to transition to or null if no transition is required.
        ///     This method simply passes back the result of invoking the handler
        ///     </para>
        /// </remarks>
        /// <param name="ev"><see cref="HostSimulator.Controller.FSMEvent"/> The event to handle</param>
        /// <returns> <see cref="System.int"/> The ID of the state to transition to, or NoState if transition not required</returns> 
        public int HandleEvent(FSMEvent ev)
        {
            var nextState = 0;

            if (ev != null)
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Verbose,
                                        string.Format("Handling event '{0}' in state '{1}'", ev.Name, Name));

                // Access the handler for this event
                var theHandler = _eventTable[ev.ID];
                nextState = theHandler(ev);
            }
            else
            {
                Logger.LogMessage(Logger.CntrlSwitch,
                                        TraceLevel.Error,
                                        "State: " + Name + "ArgumentNullException:ev == null");

                throw (new ArgumentNullException("ev == null"));
            }

            return (nextState);
        }

        #endregion // Public Methods
    }

    /// <summary>
    /// The FSM Event Class (abstract)
    /// </summary>
    public class FSMEvent
    {
        #region Constructors

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Name"><see cref="string"/> The name of the event</param>
        /// <param name="ID"><see cref="int"/> The ID of the event</param>
        public FSMEvent(string Name, int ID)
        {
            this.Name = Name;
            this.ID = ID;
        }

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="Name"><see cref="System.string"/> The name of the event</param>
        /// <param name="ID"><see cref="System.int"/> The ID of the event</param>
        /// <param name="Data"><see cref="System.Object"/> The data associated with the event</param>
        
        public FSMEvent(string Name, int ID, Object Data)
        {
            this.Name = Name;
            this.ID = ID;
            this.Data = Data;
        }

        #endregion // Constructors

        #region Private Methods

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        ///     <para>
        ///     This is hidden to prevent creation of an un-named and unidentified event
        ///     </para>
        /// </remarks>
        private FSMEvent()
        {
            throw (new InvalidOperationException());
        }

        #endregion // Private Methods

        #region Public Data

        /// <summary>
        /// Public read-only string property containing the event name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Public read-only string property containing the event ID
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Placeholder for event-specific data
        /// </summary>
        public object Data { get; set; }

        #endregion // Public Data

    }

    /// <summary>
    /// A delegate to define the event handler signature
    /// </summary>
    public delegate int EventHandlerDelegate(FSMEvent ev);


    /// <summary>
    /// A delegate to define the transition trigger signature
    /// </summary>
    public delegate void FSMTriggerDelegate();

}
