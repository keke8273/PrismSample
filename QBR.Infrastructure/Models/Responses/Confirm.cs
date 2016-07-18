
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using DataLinkLayer.IO.Protocol;
using QBR.Infrastructure.Models.DataTypes;

namespace QBR.Infrastructure.Models.Responses
{
    /// <summary>
    /// Confirm message
    /// </summary>
    public class Confirm : ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Confirm"/> class.
        /// </summary>
        public Confirm()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Confirm"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public Confirm(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public EProteusStatus Status { get; set; }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return sizeof(EProteusStatus); }
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            if (dataArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                Status = (EProteusStatus)BitConverter.ToUInt16(dataArray, fieldPosition);

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
            return BitConverter.GetBytes((UInt16)Status);
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Status = EProteusStatus.Invalid;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="EProteusStatus"/> to <see cref="Confirm"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Confirm(EProteusStatus status)
        {
            var confirmation = new Confirm();
            confirmation.Status = status;
            return confirmation;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Confirm"/> to <see cref="EProteusStatus"/>.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator EProteusStatus(Confirm status)
        {
            return status.Status;
        }

    }
}
