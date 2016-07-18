
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
using System.Linq;
using DataLinkLayer.Diagnostics;
using DataLinkLayer.IO.CommsCntrl;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.ResponseHandlers
{
    /// <summary>
    /// This is a response handler for data that is a record in a list of records on the device
    /// </summary>
    /// <typeparam name="T">The record data type</typeparam>
    public class RecordResponseHandler<T> : ConfirmResponseHandler where T : ABaseRecordCommsData
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsManager">The datalink layer responsable for communicating with the device</param>
        /// <param name="numberOfCommands">The total number of command to be sent</param>
        /// <remarks>This will default to having the set iterator command included</remarks>
        public RecordResponseHandler(CommsWorker commsManager, int numberOfCommands)
            : this(commsManager, numberOfCommands, true)
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsManager">The datalink layer responsable for communicating with the device</param>
        /// <param name="numberOfCommands">The total number of command to be sent</param>
        /// <param name="setIncluded">Flag indicating if the set iterator command is included in the numberOfCommands total</param>
        public RecordResponseHandler(CommsWorker commsManager, int numberOfCommands, bool setIncluded)
            : base(commsManager, numberOfCommands)
        {
            _results = new List<T>();

            ExpectNumberOfResults = numberOfCommands - (setIncluded ? 1 : 0);        
        }

        #endregion Constructor

        /// <summary>
        /// list of recieved operator IDs
        /// </summary>
        private List<T> _results;

        /// <summary>
        /// the number of actual patient result records that are expected
        /// </summary>
        private int ExpectNumberOfResults { get; set; }

        /// <summary>
        /// Handle a frame that has been received. This will attempt to parse the received frame into the 
        /// data type provided if unable to parse the data then the recieved data is ignored.
        /// </summary>
        /// <param name="command">The frame that has been received from a meter</param>
        /// <returns>true if the command is handled otherwise false</returns>
        public override bool HandleResponse(IFrame command)
        {
            Complete = false;

            var handled = base.HandleResponse(command);
            if(!handled)
            {

                var result = Activator.CreateInstance<T>();
                result.FromArray(command.Payload.ToArray(), 0);
                
                Logger.LogMessage(Logger.TestSwitch,
                                  TraceLevel.Info,
                                  string.Format("\n**** RECEIVED *****\n{1}\n{0}\n*******************", result, command));
                
                if(result.IsValid)
                {
                    NoCmdsToBeSent--;
                    
                    _results.Add(result);

                    if ( (result.ValidData == EValidResult.NoMoreRecords)
                        || (_results.Count == ExpectNumberOfResults))
                    {
                        Data = _results;
                    }

                    SetNextTimeoutInterval();
                    Complete = true;
                    handled = true;
                }
                else
                {
                    Logger.LogMessage(Logger.BDMSwitch,
                                                            TraceLevel.Warning,
                                                            "RecordResultHandler Unhandled command");
                }
            }

            return handled;
        }
    }
}
