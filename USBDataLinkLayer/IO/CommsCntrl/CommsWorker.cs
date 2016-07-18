
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
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO.Protocol;

namespace DataLinkLayer.IO.CommsCntrl
{
    /// <summary>
    /// The comms worker class sits at the data link layer in the communication stack. It uses a Pool thread to perform the work of
    /// sending and receiving the IFrames to and from the physical layer. Registering event handlers for the Progress changed event will
    /// allow a user to be notified of the progress of data transmission (eg a firmware file transfer). The RunWorker complete
    /// event is fired when the transaction is complete passing some error information in the event args. Any data received will 
    /// be aggregated and stored in the supplied application layer Response Handler and can be retrieved when the Response Handlers
    /// Complete flag is true. It is the callers responsibility to synchronise the events with their UI thread.
    /// </summary>
    public class CommsWorker:IDisposable, ICommsWorker
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsChannel">The comms channel to be used in the communication.</param>
        public CommsWorker(IIOPort commsChannel)
        {
            CommsController = new CommsFSM(commsChannel);
        }

        #endregion Constructor

        #region Private Data

        /// <summary>
        /// Reference to the tread performing any work.
        /// </summary>
        private Thread _thread;

        /// <summary>
        /// The main state machine that controls the flow of the communication.
        /// </summary>
        private CommsFSM CommsController;

        /// <summary>
        /// List of command frames to be sent to the device
        /// </summary>
        private List<IFrame> Commands;

        #endregion Private Data

        #region Public Data

        /// <summary>
        /// Event used for reporting progress
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Event used to report the completion of the work
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Flag indicating if the work is to be cancelled
        /// </summary>
        public bool CancellationPending { get; private set; }

        /// <summary>
        /// Will be true when there is some work being performed otherwise it will be false
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return _thread != null ? _thread.IsAlive : false;
            }
        }

        /// <summary>
        /// flag indicating if the worker is stopped.
        /// </summary>
        public bool Stopped 
        { 
            get
            {
                if (CommsController != null)
                {
                    return CommsController.Stopped && !IsBusy;
                }
                return true;
            }
        }

        #endregion Public Data

        #region Private methods

        /// <summary>
        /// helper method to make sure that the event has registered listeners and to aggregate the
        /// provided data into an EventArgument object before sending.
        /// </summary>
        /// <param name="percentage">The percentage being report by the event</param>
        private void ReportProgress(int percentage)
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(percentage, this));
            }
        }

        ///<summary>
        /// helper method to make sure that the event has registered listeners and to aggregate the
        /// provided data into an EventArgument object before sending.
        /// </summary>
        /// <param name="result">True if the work was completed with an error otherwise false</param>
        /// <param name="ex">any exception that occurred during the work</param>
        /// <param name="canceled">Flag indicating if the work was cancelled</param>
        private void ReportComplete(bool result, Exception ex, bool cancelled)
        {
            if (RunWorkerCompleted != null)
            {
                RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(result, ex, cancelled));
            }
        }

        /// <summary>
        /// Main method that performs the work of the background worker
        /// </summary>
        /// <param name="e">Event args that are used to communicate the result of the work back to the calling thread.</param>
        private void OnDoWork(object args)
        {
            //Cache the pool thread that we are using incase we need to check its state later
            _thread = Thread.CurrentThread;

            Logger.LogMessage(Logger.IOSwitch,
                             TraceLevel.Verbose,
                             "Starting Comms Worker");
            if (Commands.Count == 0)
            {
                throw new ArgumentException("There are no command supplied to send");
            }

            var commandCount = Convert.ToDouble(Commands.Count);
            
            var progress = 0;
            do //outer loop drives sending multiple commands
            {
                //set the command to send
                CommsController.Command = Commands[0];
                Commands.RemoveAt(0);

                Logger.LogMessage(Logger.IOSwitch, TraceLevel.Info,
                    string.Format("Sending command {0} of {1}", commandCount - Commands.Count, commandCount));

                //kick off the controller
                CommsController.Start();

                //Continue processing until complete (commController is stopped)  or the thread is canceled
                while ((!CommsController.Stopped) && (!CancellationPending))
                {
                    CommsController.Continue();
                }

                //this could happen if a cancellation occurred during the handling of the continue
                if (!CommsController.Stopped)
                {
                    CommsController.Stop();
                }
                else if ((Commands.Count > 0) && (!CommsController.Error))
                {
                    //reset ready for the next command
                    CommsController.Reset();
                    if (CommsController.ResponseHandler != null)
                    {
                        CommsController.ResponseHandler.Reset();
                    }
                    else
                    {
                        //rate limiting sleep - this is only here for backward compatibility
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    //out of commands and now finished.
                }
                var currentProgress = Convert.ToInt32(100 - ((Commands.Count / commandCount) * 100));
                if (currentProgress > progress)
                {
                    progress = currentProgress;
                    ReportProgress(progress);
                }

            } while ((!CancellationPending)
                  &&(!CommsController.Stopped));

            //Compile the results
            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Verbose,
                              "Comms Worker Complete");

            //finished with the thread so clear the cache
            _thread = null;

            //report that he work is complete.
            ReportComplete(CommsController.Error, null, CancellationPending);
        }

        #endregion Private methods

        #region Public Methods

        /// <summary>
        /// Send a list of IFrame commands and wait for a response to each command. 
        /// </summary>
        /// <param name="commands">List of IFrames to be send</param>
        /// <param name="handler">The application layer handler that will process and agregate the reponses</param>
        /// <remarks>It is valid for the handler to be null in which case no response is expected.</remarks>
        public void Start(List<IFrame> commands, AResponseHandler handler)
        {
            try
            {
                Logger.LogMessage(Logger.IOSwitch, TraceLevel.Info, "Starting command : " + commands[0].FrameType.ToString());
                if (IsBusy)
                {
                    throw new InvalidOperationException("Can't start the comms task as it is already busy");
                }
                Commands = commands;
                CommsController.ResponseHandler = handler;
                //start 
                ThreadPool.QueueUserWorkItem(OnDoWork);

            }
            catch(Exception ex)
            {
                //fire the worker complete event with the error flags in the result
                ReportComplete(true, ex, false);
            }
        }
        
        /// <summary>
        /// Send the a single IFrame command and wait for responses to be received.
        /// </summary>
        /// <remarks>The worker doesn't not guarantee the completeness of the response beyond the completeness
        /// of the frames received. A check should be made on the handler when the worker is complete to make
        /// sure the entire expected response was received.</remarks>
        /// <param name="command">The single command to be sent</param>
        /// <param name="handler">The application layer handler that will process and agregate the reponses</param>
        public void Start(IFrame command, AResponseHandler handler)
        {
            //setup the communication controller
            Start(new List<IFrame>() { command }, handler);
        }

        /// <summary>
        /// Start the worker with a list of commands to send with no responses expected.
        /// </summary>
        /// <param name="frames">List of IFrame to be sent</param>
        public void Start(IEnumerable<IFrame> frames)
        {
            Start(new List<IFrame>(frames),null);
        }
        
        /// <summary>
        /// Sends the specified command and does not wait for a response.
        /// </summary>
        /// <param name="command">the command to be communicated</param>
        public void Start(IFrame command)
        {
            Start(new List<IFrame>() { command }, null);
        }
        
        /// <summary>
        /// Stop the worker imdediately. This will cause any current work to be cancelled
        /// </summary>
        public void Stop()
        {
            if (!Stopped)
            {
                CancellationPending = true;

                //Stop state machine
                CommsController.Cancelling = true;
            }
        }

        #endregion Public Methods

        #region IDisposable

        /// <summary>
        /// Flag indicating if the worker is already disposed of
        /// </summary>
        private bool _disposedOf;

        /// <summary>
        /// Disposer
        /// </summary>
        /// <param name="disposing">
        /// Flag indicating if the method is called as part of a managed clean up or if it is called from the destructor
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposedOf)
            {
                if (disposing)
                {
                    Stop();
                    if (IsBusy)
                    {
                        _thread.Join();
                    }
                }

                _disposedOf = true;
            }
        }

        /// <summary>
        /// Implementation of the IDisposable interface
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~CommsWorker()
        {
            Dispose(false);
        }

        #endregion IDisposable
    }
}

