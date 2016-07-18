
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
using QBR.Infrastructure.Models.ResponseHandlers;

namespace DataHandler.ResponseHandlers
{
    /// <summary>
    /// This is a response handler for data that is a record in a list of records on the device
    /// </summary>

    public class TransientRecordResponseHandler : ConfirmResponseHandler
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsManager">The datalink layer responsible for communicating with the device</param>
        /// <param name="numberOfCommands">The total number of command to be sent</param>
        /// <remarks>This will default to having the set iterator command included</remarks>
        public TransientRecordResponseHandler(CommsWorker commsManager, int numberOfCommands)
            : base(commsManager, numberOfCommands)
        {
            _result_received = false;
        }

        #endregion Constructor

        /// <summary>
        /// list of received Transients
        /// </summary>
        /// 
        private List<Transient> _transients;

        private List<byte> _transient_bytes;

        private Transient _transient;

        private TransientResult _result;

        private TransientMsgData _datablock;

        private bool _result_received;

        /// <summary>
        /// Handle a frame that has been received. This will attempt to parse the received frame into the 
        /// data type provided if unable to parse the data then the received data is ignored.
        /// </summary>
        /// <param name="command">The frame that has been received from a meter</param>
        /// <returns>true if the command is handled otherwise false</returns>
        public override bool HandleResponse(IFrame command)
        {
            Complete = false;

            var handled = base.HandleResponse(command);
            if(!handled)
            {
                if (command.FrameType == EFrameType.ReplyTransientResultHeader)
                {
                    _result = new TransientResult(command.Payload.ToArray(), 0);
                   
                    if (_result.IsValid)
                    {

                        if (_result.TestType == (UInt16)TestTypes.ProPTLQC)
                        {
                            _result = new TransientResultLQC(command.Payload.ToArray(), 0);
                        }
                        else if (_result.TestType == (UInt16)TestTypes.ProPTBlood)
                        {
                            _result = new TransientResultPatient(command.Payload.ToArray(), 0);
                        }
                 
                        _transient = new Transient();

                        _transient.Result = _result;

                        _result_received = true;

                        if (_result.ValidData == EValidResult.NoMoreRecords)
                        {
                            Complete = true;
                        }
                        else
                        {
                            _transient_bytes = new List<byte>();
                        }
                    }
                    Logger.LogMessage(Logger.TestSwitch,
                                      TraceLevel.Info,
                                      string.Format("\n**** RECEIVED *****\n{1}\n{0}\n*******************", _result, command));


                    handled = true;
           
                }
                else if ((command.FrameType == EFrameType.ReplyTransientResult) && (_result_received == true))
                {
                    
                    _datablock = new TransientMsgData();
                    _datablock.FromArray(command.Payload.ToArray(), 0);
                    _transient_bytes.AddRange(_datablock.MsgData);

                    Logger.LogMessage(Logger.TestSwitch,
                                      TraceLevel.Info,
                                      string.Format("\n**** RECEIVED *****\n{1}\n{0}\n*******************", _datablock, command));

                    if (_datablock.BlocksRemaining == 1)
                    {
                        Complete = true;
                    }

                    handled = true;
               }

                if (Complete == true) 
                {
                    _result_received = false;
                    if (_result.ValidData != EValidResult.NoMoreRecords)
                    {
                        _transient.TransientData.FromArray(_transient_bytes.ToArray(), 0);
                    }
                        _transients = new List<Transient>();
                        _transients.Add(_transient);

                        Data = _transients;
                }
            }
            
            if (handled == false)
            {
                Logger.LogMessage(Logger.BDMSwitch,
                                                            TraceLevel.Warning,
                                                            "TransientRecordResponseHandler Unhandled");
                Logger.LogMessage(Logger.TestSwitch,
                                                    TraceLevel.Info,
                                                    string.Format("\n**** RECEIVED *****\n{1},{0}\n*******************",command, _result_received));
            }

            return handled;
        }
    }
}
