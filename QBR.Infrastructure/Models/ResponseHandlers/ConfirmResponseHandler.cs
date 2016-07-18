
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
using DataLinkLayer.IO;
using DataLinkLayer.IO.CommsCntrl;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models.Responses;

namespace QBR.Infrastructure.Models.ResponseHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfirmResponseHandler : AResponseHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfirmResponseHandler"/> class.
        /// </summary>
        /// <param name="commsManager">The comms manager.</param>
        /// <param name="numberOfCommands">The number of commands.</param>
        public ConfirmResponseHandler(CommsWorker commsManager, int numberOfCommands)
        {
            CommsManager = commsManager;
            _CustomTimeout = 1000;
            ResponseTimeoutInterval = _CustomTimeout;
            NoCmdsToBeSent = numberOfCommands;
        }

        /// <summary>
        /// The number of command to be sent as this handler is reused for multiple commands.
        /// This will be decremented each time a status ready is received and when it reaches
        /// 0 the response timeout will be set to the default to make sure there is a wait
        /// for duplicates frames at the end of the operation.
        /// </summary>
        /// <value>
        /// The no CMDS to be sent.
        /// </value>
        protected int NoCmdsToBeSent { get; set; }

        /// <summary>
        /// The commsWorker managing the communication for the the response handler. This will be used
        /// to cancel the operation in the event of an error.
        /// </summary>
        /// <value>
        /// The comms manager.
        /// </value>
        private CommsWorker CommsManager { get; set; }

        /// <summary>
        /// A cache for the user supplied response timeout interval (in ms)
        /// </summary>
        private int _CustomTimeout;

        /// <summary>
        /// Handle and IFrame command in the appropriate fashion
        /// </summary>
        /// <param name="command">The IFrame containing the data to be processed</param>
        /// <returns>
        /// True if the command frame was handled otherwise false
        /// </returns>
        public override bool HandleResponse(IFrame command)
        {
            var handled = false;

            if (command.FrameType == EFrameType.DoConfirm)
            {
                var confirmation = new Confirm(command.Payload, 0);
                Logger.LogMessage(Logger.TestSwitch,
                                  TraceLevel.Info,
                                  string.Format("\n**** RECEIVED *****\n{1}\n{0}\n*******************", confirmation, command));

                if (confirmation.IsValid)
                {
                    Data = confirmation;

                    Logger.LogMessage(Logger.BDMSwitch,
                                      TraceLevel.Info,
                                      "Status = " + confirmation.Status.ToString());

                    switch (confirmation.Status)
                    {
                        case EProteusStatus.Ready:

                            handled = true;

                            Complete = true;

                            NoCmdsToBeSent--;

                            SetNextTimeoutInterval();

                            break;
                        case EProteusStatus.Busy:

                            handled = true;

                            break;
                        case EProteusStatus.Error:

                            handled = true;

                            Logger.LogMessage(Logger.BDMSwitch,
                                              TraceLevel.Error,
                                              "Error status received from device");

                            //stop the comms worker as there has been an error on the device
                            CommsManager.Stop();
                            break;
                        default:
                            Logger.LogMessage(Logger.BDMSwitch,
                                              TraceLevel.Error,
                                              "Invalid status received from device");
                            break;
                    }
                }

            }

            return handled;
        }

        /// <summary>
        /// Sets the next timeout interval.
        /// </summary>
        protected void SetNextTimeoutInterval()
        {
            if (NoCmdsToBeSent <= 0)
            {
                //reached the end of the commands being sent now we just have to wait
                //incase there are duplicates sent due to errors.
                ResponseTimeoutInterval = DeviceProtocol.RESPONSE_TIMEOUT;
            }
            else
            {
                //the handler is complete but there are more commands being sent so set 
                //the timeout to 0 to allow the next command to be sent.
                ResponseTimeoutInterval = 0;
            }
        }

        /// <summary>
        /// Reset the public state
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            ResponseTimeoutInterval = _CustomTimeout;
        }
    }
}
