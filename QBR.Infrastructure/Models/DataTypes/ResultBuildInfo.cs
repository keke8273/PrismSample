
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
using System.Runtime.Serialization;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Result Build Information 
    /// </summary>
    [DataContract]
    public class ResultBuildInfo : ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBuildInfo"/> class.
        /// </summary>
        public ResultBuildInfo()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultBuildInfo"/> class.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        public ResultBuildInfo(byte[] dataArray, int offset)
            : base(dataArray, offset)
        { }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        [DataMember(Order = 0)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the part number.
        /// </summary>
        /// <value>
        /// The part number.
        /// </value>
        [DataMember(Order = 1)]
        public string PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the hw release.
        /// </summary>
        /// <value>
        /// The hw release.
        /// </value>
        [DataMember(Order = 2)]
        public string HWRelease { get; set; }

        /// <summary>
        /// Gets or sets the sw version.
        /// </summary>
        /// <value>
        /// The sw version.
        /// </value>
        [DataMember(Order = 3)]
        public string SWVersion { get; set; }

        /// <summary>
        /// Gets the size of the specific data.
        /// </summary>
        /// <value>
        /// The size of the specific data.
        /// </value>
        public static int SpecificDataSize 
        {
            get { return 16 + 8 + 8 + 8; }
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return SpecificDataSize; }
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            if (dataArray.Length >= (offset + SpecificDataSize))
            {
                var fieldPosition = offset;

                var fieldSize = 16;
                SerialNumber = Encoding.ASCII.GetString(dataArray, fieldPosition, fieldSize);
                SerialNumber = SerialNumber.Trim(new[] { '\0' });
                fieldPosition += fieldSize;

                fieldSize = 8;
                PartNumber = Encoding.ASCII.GetString(dataArray, fieldPosition, fieldSize);
                PartNumber = PartNumber.Trim(new[] { '\0' });
                fieldPosition += fieldSize;
                
                fieldSize = 8;
                HWRelease = Encoding.ASCII.GetString(dataArray, fieldPosition, fieldSize);
                HWRelease = HWRelease.Trim(new[] { '\0' });
                fieldPosition += fieldSize;

                fieldSize = 8;
                SWVersion = Encoding.ASCII.GetString(dataArray, fieldPosition, fieldSize);
                SWVersion = SWVersion.Trim(new[] { '\0' });
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
            
            var strData = new byte[16];
            Array.Copy(Encoding.ASCII.GetBytes(SerialNumber), strData, SerialNumber.Length);
            data.AddRange(strData);//16

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(PartNumber), strData, PartNumber.Length);
            data.AddRange(strData);//8

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(HWRelease), strData, HWRelease.Length);
            data.AddRange(strData);//8

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(SWVersion), strData, SWVersion.Length);
            data.AddRange(strData);//8

            return data.ToArray();
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            SerialNumber = "FFFFFFFFFFFFFFFF";
            PartNumber = "FFFFFFFF";
            HWRelease = "FFFFFFFF";
            SWVersion = "FFFFFFFF";            
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
            if (obj.GetType() == typeof(ResultBuildInfo))
            {
                areEqual = this == (obj as ResultBuildInfo);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ResultBuildInfo x, ResultBuildInfo y)
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
        public static bool operator ==(ResultBuildInfo x, ResultBuildInfo y)
        {
            var areEqual = true;

            areEqual &= x.PartNumber.CompareTo(y.PartNumber.Trim()) == 0; 
            areEqual &= x.SerialNumber.CompareTo(y.SerialNumber.Trim()) == 0; 
            areEqual &= x.HWRelease.CompareTo(y.HWRelease.Trim()) == 0;
            areEqual &= x.SWVersion.CompareTo(y.SWVersion.Trim()) == 0;

            return areEqual;
        }
    }
}
