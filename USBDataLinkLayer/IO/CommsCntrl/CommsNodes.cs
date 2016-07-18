
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
    /// Class used to encapsulate the result of a communication operation
    /// </summary>
    public class CommsResult
    {
        /// <summary>
        /// Property to get and set the error indicator
        /// </summary>
        public bool CommsError { get; set; }
        
        /// <summary>
        /// Property to get and set the timeout indicator
        /// </summary>
        public bool Timeout { get; set; }
        
        /// <summary>
        /// Property to get and set the duplication indicator
        /// </summary>
        public bool Duplicate { get; set; }

        /// <summary>
        /// Property to get and set the frame that the result applies to.
        /// </summary>
        /// <remarks>This should be left as null if there was an error</remarks>
        public IFrame ReceivedFrame;
    }

    /// <summary>
    /// A base class for the communication nodes responsible for collecting the data received from the physical layer into 
    /// a Datalink layer IFrame. This implements simply state machine behavior with respect to agregating the IFrame.
    /// </summary>
    public class ResponseListener
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ResponseListener()
        {
            RespWaitHandle = new AutoResetEvent(false);
            ReceivedData = new List<byte>();
        }

        #endregion Constructor

        #region Public Data

        /// <summary>
        /// Wait handle use to synchronise the receive event or indicate a timeout has occured.
        /// </summary>
        public EventWaitHandle RespWaitHandle { get; set; }

        #endregion Public Data

        #region Protecte Data

        protected UInt16 _payloadLength;

        /// <summary>
        /// int used to keep track of how many bytes have been accumulated. This is used to give a rough
        /// guide as to where in a received frame the new received data would fall.
        /// </summary>
        protected int bytesAccumulated;

        /// <summary>
        /// Definition of the states for the simple comms FSM
        /// </summary>
        public enum EReceiveFrameState
        {
            ReceiveStateInvalid = -1,
            ReceiveStateMin = 0,
            ReceiveStateEmpty,
            ReceiveStateNeedSOF,
            ReceiveStateNeedType,
            ReceiveStateNeedSeq,
            ReceiveStateNeedLength,
            ReceiveStateNeedData,
            ReceiveStateNeedCRC,
            ReceiveStatNeedEOF,
            ReceiveStateFrameComplete,
            ReceiveStateTimedOut,
            ReceiveStateMax
        }    
        
        /// <summary>
        /// A list of bytes used to aggregate a frame
        /// </summary>
        protected List<byte> ReceivedData;

        /// <summary>
        /// Property used to keep track of a received frames state when parsing the incoming data.
        /// </summary>
        protected EReceiveFrameState _frameState { get; set; }

        /// <summary>
        /// flag set when invalid data is received. Invalid data is defined as data that either doesn't
        /// convert into the appropriate type or the frame is corrupt in some way.
        /// </summary>
        protected bool invalidFrameData;

        #endregion Protecte Data

        #region Protect Methods

        /// <summary>
        /// check to see if a Response frame can be accumulated from the data.
        /// </summary>
        /// <param name="frameAsList">list containing the bytes that define the potential frame</param>
        /// <returns>A IFrame frame if there was a complete one accumulated otherwise null. If there was
        /// an error in the frame data then the invalid data flag will be set. The reason can be determined
        /// by examining the frameState</returns>
        protected IFrame checkForIFrame(List<byte> frameAsList)
        {
            IFrame frame = null;
            var framedata = frameAsList.ToArray();
            var byteCount = frameAsList.Count;

            try
            {
                invalidFrameData = false;

                checkSOF(framedata, byteCount);

                checkSequenceNo(framedata, byteCount);

                checkFrameType(framedata, byteCount);

                checkPayloadLength(framedata, byteCount);

                //accumulate payload
                if ((_frameState == EReceiveFrameState.ReceiveStateNeedData)
                   && (framedata.Length >= (bytesAccumulated + (IFrame.PAYLOAD_UNIT_SIZE * _payloadLength))))
                {
                    bytesAccumulated += (IFrame.PAYLOAD_UNIT_SIZE * _payloadLength);
                    
                    Logger.LogMessage(Logger.IOSwitch,
                                      TraceLevel.Verbose,
                                      "Got Payload -> need CRC");

                    _frameState = EReceiveFrameState.ReceiveStateNeedCRC;
                }

                checkCRC(framedata, byteCount);

                checkEOF(framedata, byteCount);

                if (!invalidFrameData)
                {
                    if (_frameState == EReceiveFrameState.ReceiveStateFrameComplete)
                    {
                        frame = new IFrame(frameAsList.ToArray());
                        _frameState = EReceiveFrameState.ReceiveStateNeedSOF;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(Logger.IOSwitch, ex, "");

                invalidFrameData = true;
            }

            if (invalidFrameData)
            {
                Logger.LogMessage(Logger.IOSwitch,
                                  TraceLevel.Error,
                                  "Invalid data received in : " + _frameState.ToString());
                var data = "";
                foreach (var b in framedata)
                {
                    data += b.ToString("X2") + " ";
                }

                Logger.LogMessage(Logger.IOSwitch,
                                  TraceLevel.Error,
                                  data);

                _frameState = EReceiveFrameState.ReceiveStateNeedSOF;
                frame = null;
            }

            return frame;
        }

        /// <summary>
        /// Helper method to check for the SOF in an array of bytes
        /// </summary>
        /// <param name="framedata">the byte array that contains the potential frame</param>
        /// <param name="byteCount">the nubmer of bytes in the array</param>
        protected void checkSOF(byte[] framedata, int byteCount)
        {
            if ((_frameState == EReceiveFrameState.ReceiveStateNeedSOF)
                && (byteCount >= bytesAccumulated))
            {
                var sof = BitConverter.ToUInt16(framedata, IFrame.SOF_INDEX);
                if (sof == IFrame.SOF_PATTERN)
                {
                    _frameState = EReceiveFrameState.ReceiveStateNeedSeq;
                    bytesAccumulated += sizeof(UInt16);

                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Got SOF -> need Seq");
                }
                else
                {
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Error getting SOF");
                    invalidFrameData = true;
                }
            }
        }

        /// <summary>
        /// Helper method to check for the Sequence number in an array of bytes
        /// </summary>
        /// <param name="framedata">the byte array that contains the potential frame</param>
        /// <param name="byteCount">the nubmer of bytes in the array</param>
        protected void checkSequenceNo(byte[] framedata, int byteCount)
        {
            if ((_frameState == EReceiveFrameState.ReceiveStateNeedSeq)
                && (byteCount >= (bytesAccumulated + sizeof(UInt16))))
            {
                var sequenceNo = BitConverter.ToUInt16(framedata, IFrame.SEQ_NUM_INDEX);

                _frameState = EReceiveFrameState.ReceiveStateNeedType;
                bytesAccumulated += sizeof(UInt16);
                Logger.LogMessage(Logger.IOSwitch,
                                 TraceLevel.Verbose,
                                 "Got Seq number -> need Type");
            }
        }

        /// <summary>
        /// Helper method to check for the frame type in an array of bytes
        /// </summary>
        /// <param name="framedata">the byte array that contains the potential frame</param>
        /// <param name="byteCount">the nubmer of bytes in the array</param>
        protected void checkFrameType(byte[] framedata, int byteCount)
        {
            if ((_frameState == EReceiveFrameState.ReceiveStateNeedType)
                && (byteCount >= (bytesAccumulated + sizeof(EFrameType))))
            {
                var ftype = BitConverter.ToUInt16(framedata, IFrame.FRAME_TYPE_INDEX);

                if (Enum.IsDefined(typeof(EFrameType), ftype))
                {
                    _frameState = EReceiveFrameState.ReceiveStateNeedLength;
                    bytesAccumulated += sizeof(EFrameType);
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Got " + ((EFrameType)ftype).ToString() + " Frame type -> need Len");
                }
                else
                {
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Error getting Frame type");
                    invalidFrameData = true;
                }
            }
        }

        /// <summary>
        /// Helper method to check and accumulate the payload length field
        /// </summary>
        /// <param name="framedata">the byte array containing the data to be checked</param>
        /// <param name="byteCount">The length of the byte array</param>
        protected void checkPayloadLength(byte[] framedata, int byteCount)
        {
            //check payload length
            if ((_frameState == EReceiveFrameState.ReceiveStateNeedLength)
                && (byteCount >= (bytesAccumulated + sizeof(UInt16))))
            {
                _frameState = EReceiveFrameState.ReceiveStateNeedData;
                Logger.LogMessage(Logger.IOSwitch,
                                  TraceLevel.Verbose,
                                  "Got Len -> need Payload");

                //get the payload length
                _payloadLength = BitConverter.ToUInt16(framedata, bytesAccumulated);

                bytesAccumulated += sizeof(UInt16);
            }
        }
        
        /// <summary>
        /// Helper method to check for the CRC in an array of bytes
        /// </summary>
        /// <param name="framedata">the byte array that contains the potential frame</param>
        /// <param name="byteCount">the nubmer of bytes in the array</param>
        protected void checkCRC(byte[] framedata, int byteCount)
        {
            if ((_frameState == EReceiveFrameState.ReceiveStateNeedCRC)
                && (byteCount >= (bytesAccumulated + sizeof(UInt16))))
            {
                var crc = BitConverter.ToUInt16(framedata, bytesAccumulated);

                // SOF and sequence number are not included in CRC
                const UInt16 seq_num_sof_size = 4;

                var crcdata = new byte[bytesAccumulated - seq_num_sof_size];
                Array.ConstrainedCopy(framedata, 4, crcdata, 0, bytesAccumulated - seq_num_sof_size);



                var calcCRC = CRC16CCITT.ComputeChecksum(crcdata);
                bytesAccumulated += sizeof(UInt16);

                if (crc == calcCRC)
                {
                    _frameState = EReceiveFrameState.ReceiveStatNeedEOF;

                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Got CRC -> need EOF");
                }
                else
                {
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Error,
                                     string.Format("Error getting CRC 0x{0:X4}(recv) != 0x{1:X4}(calc)", crc, calcCRC));
                    
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Error,
                                     "Accumulated bytes :" + bytesAccumulated.ToString());

                    invalidFrameData = true;
                }
            }
        }

        /// <summary>
        /// Helper method to check for the EOF in an array of bytes
        /// </summary>
        /// <param name="framedata">the byte array that contains the potential frame</param>
        /// <param name="byteCount">the nubmer of bytes in the array</param>
        protected void checkEOF(byte[] framedata, int byteCount)
        {
            if ((_frameState == EReceiveFrameState.ReceiveStatNeedEOF)
                && (byteCount >= (bytesAccumulated + sizeof(UInt16))))
            {
                var eof = BitConverter.ToUInt16(framedata, bytesAccumulated);
                if (eof == IFrame.EOF_PATTERN)
                {
                    _frameState = EReceiveFrameState.ReceiveStateFrameComplete;
                    bytesAccumulated += sizeof(UInt16);

                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "Got EOF -> FRAME COMPLETE");
                }
                else
                {
                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Error,
                                     "Error getting EOF");
                    invalidFrameData = true;
                }
            }
        }

        /// <summary>
        /// Build an Ack frame based on an IFrame
        /// </summary>
        /// <param name="commandSeqNo">the IFrame sequence number to base the Ack on</param>
        /// <returns>An Ack Response frame</returns>
        protected IFrame ComposeAckResponse(UInt16 commandSeqNo)
        {
            var resp = new IFrame(EFrameType.Ack, commandSeqNo);
            return resp;
        }

        /// <summary>
        /// Build an Nak frame based on an IFrame
        /// </summary>
        /// <param name="command">the IFrame sequence number to base the Nak on</param>
        /// <returns>An Nak Response frame</returns>
        protected IFrame ComposeNaKResponse(UInt16 commandSeqNo)
        {
            var resp = new IFrame(EFrameType.NaK, commandSeqNo);
            return resp;
        }

        #endregion Protect Methods
    }

    /// <summary>
    /// Class that performs the role of the Primary in the IdleRQ protocol that is Sending a command and receiving an ack
    /// </summary>
    public class PrimaryNode: ResponseListener
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsChannel">The communication channel to be used.</param>
        public PrimaryNode(IIOPort commsChannel)
        {
            CommsChannel = commsChannel;
        }

        /// <summary>
        /// comms channel used for communication
        /// </summary>
        private IIOPort CommsChannel { get; set; }

        #region Public data

        /// <summary>
        /// Property used to track how many tries to send have been made
        /// </summary>
        public int Retries { get; private set; }

        #endregion Public data

        #region Public Methods

        /// <summary>
        /// Reset the public flags of the Primary node
        /// </summary>
        public void Reset()
        {
            Retries = 0;
            _frameState = EReceiveFrameState.ReceiveStateNeedSOF;
            bytesAccumulated = 0;
            ReceivedData.Clear();

            Logger.LogMessage(Logger.IOSwitch,
                               TraceLevel.Verbose,
                               "Primary Reset");
        }

        /// <summary>
        /// Send a command via the injected communication channel
        /// </summary>
        /// <param name="command">iframe containing the command to be sent</param>
        public void Send(IFrame command)
        {
            if (command == null)
            {
                throw new InvalidOperationException("Command frame cannot be null");
            }
            Retries++;

            //convert the command to a byte stream ready to be sent
            var cmdFrame = command.GetBytes();

            //clear out the buffers ready for a new exchange
            CommsChannel.DiscardOutBuffer();
            CommsChannel.DiscardInBuffer();

            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Verbose,
                              "Sending : " + command.FrameType.ToString());
            //send the command
            CommsChannel.Write(cmdFrame, 0, cmdFrame.Length);

        }

        /// <summary>
        /// Wait for an acknowledgement to arrive
        /// </summary>
        /// <returns>The result which contains indicator for timeout, comms error and the response received.</returns>
        public CommsResult WaitForAck()
        {
            var result = new CommsResult();
            invalidFrameData = false;

            //if there are still bytes left to read or is the mutex has been signalled then process the available bytes.
            if (((CommsChannel.BytesAvailable > 0) 
                    && ((CommsChannel.BytesAvailable % DeviceProtocol.Read_Block_Size) == 0))
                || (RespWaitHandle.WaitOne(DeviceProtocol.ACK_TIMEOUT)))
            {
                Logger.LogMessage(Logger.IOSwitch,
                                 TraceLevel.Verbose,
                                 "Primary Node Bytes Available: " + CommsChannel.BytesAvailable.ToString());

                while (((CommsChannel.BytesAvailable > 0) 
                    && ((CommsChannel.BytesAvailable % DeviceProtocol.Read_Block_Size) == 0))
                    && (!invalidFrameData)
                    && (result.ReceivedFrame == null))
                {
                    var readSize = DeviceProtocol.Read_Block_Size;
                    var data = new byte[readSize];

                    CommsChannel.Read(data, 0, readSize);

                    ReceivedData.AddRange(data);

                    result.ReceivedFrame = checkForIFrame(ReceivedData);
                    
                    if (invalidFrameData)
                    {
                        result.CommsError = true;
                    }

                    if (invalidFrameData || (result.ReceivedFrame != null))
                    {
                        //we either received a frame or the data is corrupt so clear out the data
                        ReceivedData.Clear();
                        bytesAccumulated = 0;
                    }
                }
            }
            else
            {
                result.Timeout = true;
            }

            return result;
        }
       
        #endregion Public Methods
    }

    /// <summary>
    /// Class the defines the behavior of the Secondary node in the IdleRQ protocol that is receiving a command and acking it.
    /// </summary>
    public class SecondaryNode : ResponseListener 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="commsChannel">The communication channel to use for comms</param>
        public SecondaryNode(IIOPort commsChannel)
        {
            CommsChannel = commsChannel;
            _lastValidSeqNo = -1;
        }

        #region Private Data

        private int _lastValidSeqNo;

        /// <summary>
        /// The Communication channel used for all comms
        /// </summary>
        private IIOPort CommsChannel { get; set; }

        private bool stopped;

        #endregion Private Data
        
        #region Public Methods

        /// <summary>
        /// reset the node state.
        /// </summary>
        public void Reset()
        {
            _frameState = EReceiveFrameState.ReceiveStateNeedSOF;
            bytesAccumulated = 0;
            ReceivedData.Clear();
        }

        /// <summary>
        /// wait for a response
        /// </summary>
        /// <returns>a result object containing the result of waiting.</returns>
        public CommsResult WaitForResponse(int timeoutInterval)
        {
            var result = new CommsResult();

            invalidFrameData = false;
            var signalled = false;
            if (((CommsChannel.BytesAvailable > 0) 
                    && ((CommsChannel.BytesAvailable % DeviceProtocol.Read_Block_Size) == 0))
                || ((signalled = RespWaitHandle.WaitOne(timeoutInterval))))
            {
                Logger.LogMessage(Logger.IOSwitch,
                                  TraceLevel.Info,
                                  string.Format("Secondary Node Bytes Available: {0} signalled = {1}",CommsChannel.BytesAvailable.ToString(), signalled));

                while (((CommsChannel.BytesAvailable > 0)
                    && ((CommsChannel.BytesAvailable % DeviceProtocol.Read_Block_Size) == 0))
                    && (!invalidFrameData)
                    && (result.ReceivedFrame == null))
                {
                    var readSize = DeviceProtocol.Read_Block_Size;
                    var data = new byte[readSize];
                    
                    CommsChannel.Read(data, 0, readSize);

                    ReceivedData.AddRange(data);

                    Logger.LogMessage(Logger.IOSwitch,
                                        TraceLevel.Verbose,
                                        "WaitForResponse accumulated " + ReceivedData.Count.ToString() + " bytes");

                    result.ReceivedFrame = checkForIFrame(ReceivedData);

                    if (invalidFrameData)
                    {
                        result.CommsError = true;
                        ReceivedData.Clear();
                        bytesAccumulated = 0;
                    }
                    else if (result.ReceivedFrame != null)
                    {
                        //we either received a frame or the data is corrupt so clear out the data
                        ReceivedData.Clear();
                        bytesAccumulated = 0;

                        if (_lastValidSeqNo == result.ReceivedFrame.SequenceNo)
                        {
                            Logger.LogMessage(Logger.IOSwitch,
                                                TraceLevel.Warning,
                                                "Duplicate frame detected : " + _lastValidSeqNo.ToString() + " -> " + result.ReceivedFrame.FrameType.ToString());
                            result.Duplicate = true;
                        }
                        else
                        {
                            Logger.LogMessage(Logger.IOSwitch,
                                              TraceLevel.Info,
                                              string.Format("Received Frame : {0}  seqNo: {1}", result.ReceivedFrame.FrameType.ToString(), result.ReceivedFrame.SequenceNo));
                        }
                        _lastValidSeqNo = result.ReceivedFrame.SequenceNo;
                    }
                }
            }
            else
            {
                if (!stopped)
                {
                    result.Timeout = true;

                    Logger.LogMessage(Logger.IOSwitch,
                                     TraceLevel.Verbose,
                                     "WaitForResponse timeout");
                }
            }

            return result;
        }

        public void SendAck(IFrame command)
        {
            //send ack
            var response = ComposeAckResponse(command.SequenceNo);

            var responseData = response.GetBytes();

            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Info,
                              string.Format("Sending Ack to {0} command sequnce ID {1}", command.FrameType.ToString(), command.SequenceNo ));

            CommsChannel.Write(responseData, 0, responseData.Length);
        }

        public void SendNak()
        {
            var seqNo = (UInt16)((_lastValidSeqNo + 1) > UInt16.MaxValue ? 0 : _lastValidSeqNo + 1);
            //send NaK
            var response = ComposeNaKResponse(seqNo);

            var responseData = response.GetBytes();

            Logger.LogMessage(Logger.IOSwitch,
                              TraceLevel.Verbose,
                              "Sending Nak: " + response.ToString());
            CommsChannel.Write(responseData, 0, responseData.Length);
        }
        
        public void Stop()
        {
            stopped = true;
        }

        #endregion Public Methods

    }
}

