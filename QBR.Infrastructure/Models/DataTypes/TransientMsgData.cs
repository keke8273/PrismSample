
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
using DataLinkLayer.IO.Protocol;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Transient Data message
    /// </summary>
    public class TransientMsgData : ABaseRecordCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientMsgData"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public TransientMsgData(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientMsgData"/> class.
        /// </summary>
        public TransientMsgData()
        { }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get
            {
                return IFrame.MaxPayloadSize;
            }
        }

        /// <summary>
        /// The maximum MSG data size
        /// </summary>
        public static int MaxMsgDataSize = IFrame.MaxPayloadSize - (sizeof(UInt16) * 2);


        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public UInt16 Version { get; set; }

        /// <summary>
        /// Gets or sets the blocks remaining.
        /// </summary>
        /// <value>
        /// The blocks remaining.
        /// </value>
        public UInt16 BlocksRemaining { get; set; }

        /// <summary>
        /// The MSG data
        /// </summary>
        public byte[] MsgData;

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;
            BlocksRemaining = 0xFFFF;
            MsgData = new byte[MaxMsgDataSize];
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataAsArray, int offset)
        {
            IsValid = false;

            if (dataAsArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                Version = BitConverter.ToUInt16(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);
                            
                BlocksRemaining = BitConverter.ToUInt16(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);
                
                Array.ConstrainedCopy(dataAsArray, fieldPosition, MsgData, 0, MaxMsgDataSize);

                IsValid = true;
            }
            return IsValid;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public override byte[] ToArray()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(Version));

            data.AddRange(BitConverter.GetBytes(BlocksRemaining));
            
            data.AddRange(MsgData);
           
            return data.ToArray();
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TransientMsgData x, TransientMsgData y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TransientMsgData x, TransientMsgData y)
        {
            var areEqual = true;
            areEqual &= x.Version == y.Version;
            areEqual &= x.BlocksRemaining == y.BlocksRemaining;
            areEqual &= x.MsgData == y.MsgData;
            return areEqual;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var areEqual = true;

            if (obj.GetType() == typeof(TransientMsgData))
            {
                areEqual = this == (obj as TransientMsgData);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

    }
}
