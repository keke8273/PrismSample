
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
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Specific Patient Transient Data
    /// </summary>
    public class SpecificPatientTRData: ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificPatientTRData"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public SpecificPatientTRData(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificPatientTRData"/> class.
        /// </summary>
        public SpecificPatientTRData()
        { }

        /// <summary>
        /// The patient data size
        /// </summary>
        public static readonly int PatientDataSize = sizeof(UInt32) + 20;

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>
        /// The accession number.
        /// </value>
        public UInt32 AccessionNumber { get; set; }

        /// <summary>
        /// Gets or sets the PID.
        /// </summary>
        /// <value>
        /// The PID.
        /// </value>
        public string PID { get; set; }

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
                return PatientDataSize;
            }
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
                var fieldStart = offset;
                
                AccessionNumber = BitConverter.ToUInt32(dataArray, fieldStart);
                fieldStart += sizeof(UInt32);

                var fieldsize = 20;
                PID = Encoding.ASCII.GetString(dataArray, fieldStart, fieldsize);
                PID = PID.Trim(new[] { '\0' });
                IsValid = true;
            }
            return IsValid;
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            AccessionNumber = 0xFFFFFFFF;
            PID = "XXXXXXXXXXXXXXXXXXXX";
            IsValid = false;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public override byte[] ToArray()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(AccessionNumber));
            data.AddRange(Encoding.ASCII.GetBytes(PID));

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
        public static bool operator !=(SpecificPatientTRData x, SpecificPatientTRData y)
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
        public static bool operator ==(SpecificPatientTRData x, SpecificPatientTRData y)
        {
            var areEqual = true;

            areEqual &= x.AccessionNumber.CompareTo(y.AccessionNumber) == 0;
            areEqual &= x.PID.CompareTo(y.PID.Trim(new[] { '\0' , ' '})) == 0;

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

            if (obj.GetType() == typeof(SpecificPatientTRData))
            {
                areEqual = this == (obj as SpecificPatientTRData);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

    }
}
