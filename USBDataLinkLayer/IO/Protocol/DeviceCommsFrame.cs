
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
using DataLinkLayer.Utils;

namespace DataLinkLayer.IO.Protocol
{
    /// <summary>
    /// A frame that is used to transmit command/data between a device and the Application
    /// </summary>
    public  class IFrame
    {
        #region Constructor

        /// <summary>
        /// Default contractor
        /// </summary>
        /// <param name="fType">The frame type to assign the IFrame</param>
        /// <param name="seqNo">The sequence number to assign the IFrame.</param>
        /// <remarks>Note: The sequence number value must be valid for a UInt16 or an ArgumentException will be thrown</remarks>
        public IFrame(EFrameType fType, UInt32 seqNo)
        {
            SOF = SOF_PATTERN;
            FrameType = fType;
            if (seqNo > UInt16.MaxValue)
            {
                throw new ArgumentException("The supplied sequence number argument is to big for the frame");
            }
            SequenceNo = Convert.ToUInt16(seqNo);
            PayloadLength = 0;
            Payload = null;
            EOF = EOF_PATTERN;
            CalculateCRC();
        }

        /// <summary>
        /// Frame with a payload constructor
        /// </summary>
        /// <param name="fType">The frame type to assign the IFrame</param>
        /// <param name="seqNo">The sequence number to assign the IFrame.</param>
        /// <param name="payload">The byte array containing the payload to be sent with this IFrame</param>
        /// <remarks>Note: The sequence number value must be valid for a UInt16 or an ArgumentException will be thrown</remarks>
        public IFrame(EFrameType fType, UInt32 seqNo, byte[] payload)
            :this(fType, seqNo)
        {
            PayloadLength = Convert.ToUInt16(payload.Length / PAYLOAD_UNIT_SIZE);
            Payload = new byte[payload.Length];
            Array.Copy(payload, Payload, payload.Length);

            //re-calculate the CRC now the payload is set
            CalculateCRC();
        }

        /// <summary>
        /// Construct an IFrame from an array of bytes
        /// </summary>
        /// <param name="frameAsArray">The byte array containing the frame in byte format</param>
        public IFrame(byte[] frameAsArray)
        {
            FromBytes(frameAsArray);
        }

        #endregion Constructor

        #region Public const Data

        /// <summary>
        /// Byte Pattern used for the Start of Frame marker 
        /// </summary>
        public static UInt16 SOF_PATTERN = 0xDEAD;
        
        /// <summary>
        /// Byte pattern used for the End of Frame marker
        /// </summary>
        public static UInt16 EOF_PATTERN = 0xCAFE;

        /// <summary>
        /// The expected index of the start of frame marker within a byte
        /// array representation of the frame.
        /// </summary>
        public const int SOF_INDEX = 0;

        /// <summary>
        /// The expected index of the sequence number within a byte array 
        /// representation of the frame.
        /// </summary>
        public const int SEQ_NUM_INDEX = 2;        

        /// <summary>
        /// The expected index of the Frame type within a byte array 
        /// representation of the frame.
        /// </summary>
        public const int FRAME_TYPE_INDEX = 4;

        /// <summary>
        /// The expected index of the payload length in a byte array representation of the 
        /// IFrame
        /// </summary>
        public static int PAYLOAD_LEN_INDEX = 6;

        /// <summary>
        /// The expected index of the start of the payload in a byte array representation of the 
        /// IFrame
        /// </summary>
        public static int PAYLOAD_START_INDEX = 8;

        /// <summary>
        /// Minimum length of a frame
        /// </summary>
        public static int MIN_FRAME_LEN = 12;

        /// <summary>
        /// The size of the units used in the payload
        /// </summary>
        public static int PAYLOAD_UNIT_SIZE = sizeof(UInt16);

        public static UInt16 MaxPayloadSize = 247 * sizeof(UInt16);

        #endregion Public const Data

        #region Public Data

        /// <summary>
        /// Property containing the Start of Frame marker for this frame
        /// </summary>
        public UInt16 SOF { get; private set; }

        /// <summary>
        /// Property containing the Sequence number of the frame
        /// </summary>
        public UInt16 SequenceNo { get; private set; }
        
        /// <summary>
        /// Property containing the Frame type of this frame
        /// </summary>
        public EFrameType FrameType { get; private set; }
        
        /// <summary>
        /// Property Containing the length of the payload 
        /// </summary>
        public UInt16 PayloadLength { get; private set; }
        
        /// <summary>
        /// Property Containing the byte array payload of the IFrame
        /// </summary>
        public byte[] Payload { get; private set; }

        /// <summary>
        /// Property containing the CRC for this frame
        /// </summary>
        public UInt16 CRC { get; private set; }

        /// <summary>
        /// Property Containing the End of frame marker for this frame.
        /// </summary>
        public UInt16 EOF { get; private set; }

        /// <summary>
        /// get the frame length of the frame including SOF and EOF
        /// </summary>
        public int FrameLength
        {
            get 
            {
                var total = 0;

                total += sizeof(UInt16); //SOF
                total += sizeof(EFrameType); //FrameType
                total += sizeof(UInt16); //SequenceNo
                total += sizeof(UInt16); //Payload length
                total += sizeof(byte) * PayloadLength; //Payload
                total += sizeof(UInt16); //CRC
                total += sizeof(UInt16); //EOF

                return total;
            }
        }

        #endregion Public Data

        #region Private Methods

        /// <summary>
        /// Calculate the CRC for the IFrame
        /// </summary>
        private void CalculateCRC()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes((short)FrameType));
            data.AddRange(BitConverter.GetBytes(PayloadLength));
            if (Payload != null)
            {
                data.AddRange(Payload);
            }

            CRC = CRC16CCITT.ComputeChecksum(data.ToArray());
        }

        /// <summary>
        /// Parse an array of bytes into this IFrame
        /// </summary>
        /// <param name="source">The array of bytes that will be parse for this IFrame values</param>
        private void FromBytes(byte[] source)
        {
            if (source.Length < MIN_FRAME_LEN)
            {
                throw new InvalidOperationException("There aren't enough bytes to set the IFrame with");
            }

            SOF = BitConverter.ToUInt16(source, SOF_INDEX);

            SequenceNo = BitConverter.ToUInt16(source, SEQ_NUM_INDEX);

            FrameType = (EFrameType)BitConverter.ToUInt16(source, FRAME_TYPE_INDEX);

            PayloadLength = BitConverter.ToUInt16(source, PAYLOAD_LEN_INDEX);
            var payloadByteLength = PayloadLength * PAYLOAD_UNIT_SIZE;
            if (PayloadLength > 0)
            {
                Payload = new byte[PayloadLength * PAYLOAD_UNIT_SIZE];
                Array.Copy(source, PAYLOAD_START_INDEX, Payload, 0, payloadByteLength);
            }
            var CRC_Index = PAYLOAD_START_INDEX + payloadByteLength;
            CRC = BitConverter.ToUInt16(source, CRC_Index);

            var EOF_Index = CRC_Index + sizeof(UInt16);
            EOF = BitConverter.ToUInt16(source, EOF_Index);
        }

        #endregion Private Methods

        #region Public  Methods

        /// <summary>
        /// Get a string respresentation of the frame.
        /// </summary>
        /// <returns>a string that represents the bytes of this frame</returns>
        public override string ToString()
        {
            var stream = GetBytes();
            var frameStr = "";

            foreach (var b in stream)
            {
                frameStr += b.ToString("X2") + " ";
            }

            return frameStr;
        }

        /// <summary>
        /// Convert the IFrame to a byte array representation
        /// </summary>
        /// <returns>a byte array containing the data described in this IFrame</returns>
        public byte[] GetBytes()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(SOF));
            data.AddRange(BitConverter.GetBytes(SequenceNo));
            data.AddRange(BitConverter.GetBytes((short)FrameType));
            data.AddRange(BitConverter.GetBytes(PayloadLength));
            if (PayloadLength > 0)
            {
                data.AddRange(Payload);
            }
            data.AddRange(BitConverter.GetBytes(CRC));
            data.AddRange(BitConverter.GetBytes(EOF));

            return data.ToArray();            
        }

        public static bool operator !=(IFrame a, IFrame b)
        {
            return !(a == b);
        }

        public static bool operator ==(IFrame a, IFrame b)
        {
            var result = true;

            if (((a as object) == null) && ((b as object) == null))
            {
                result = true;
            }
            else if (((a as object) == null) || ((b as object) == null))
            {
                result = false;
            }
            else
            {
                //check the common fields
                result &= a.SOF == b.SOF;
                result &= a.FrameType == b.FrameType;
                result &= a.SequenceNo == b.SequenceNo;
                result &= a.PayloadLength == b.PayloadLength;
                result &= a.CRC == b.CRC;
                result &= a.EOF == b.EOF;

                //check the payload
                result &= (!((a.Payload != null) ^ (b.Payload != null)));
                if ((a.Payload != null) && (b.Payload != null))
                {
                    result &= a.Payload.Length == b.Payload.Length;
                    for (var i = 0; (i < a.Payload.Length) && (result); i++)
                    {
                        result &= a.Payload[i] == b.Payload[i];
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Test the frame for equality
        /// </summary>
        /// <param name="obj">the object to test against</param>
        /// <returns>if the obj object is an IFrame then True if the underlying properties match otherwise false.
        /// If the obj object is not an iframe equality will be established by object.equals.</returns>
        public override bool Equals(object obj)
        {
            var result = false;
            if (obj.GetType() == typeof(IFrame))
            {
                result = this == (obj as IFrame);
            }
            else
            {
                //the object being compared against isn't an IFrame
                result = base.Equals(obj);
            }

            return result;
        }

        /// <summary>
        /// This is necessary because of overriding on equals it will simply use the base GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion Public  Methods

    }
}
