
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Extra Result 
    /// </summary>
    public class ExtResult : Result
    {
        /// <summary>
        /// Gets or sets the pass fail.
        /// </summary>
        /// <value>
        /// The pass fail.
        /// </value>
        public UInt16 PassFail { get; set; }

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
                return base.DataSize + sizeof(UInt16);
            }
        }

        /// <summary>
        /// From the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            var results = base.FromArray(dataArray, offset);

            if (results)
            {
                PassFail = BitConverter.ToUInt16(dataArray, offset + base.DataSize);
            }

            return results;
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
            bool areEqual;
            if (obj.GetType() == typeof(ExtResult))
            {
                areEqual = this == (obj as ExtResult);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }
    }
}
