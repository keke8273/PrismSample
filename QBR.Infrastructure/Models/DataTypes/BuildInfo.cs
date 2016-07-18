
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Runtime.Serialization;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Build Information message
    /// </summary>
    [DataContract]
    public class BuildInfo : ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildInfo"/> class.
        /// </summary>
        public BuildInfo()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildInfo"/> class.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        public BuildInfo(byte[] dataArray, int offset)
            : base(dataArray, offset)
        { }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember(Order = 0)]
        public UInt16 Version { get; set; }

        /// <summary>
        /// Gets or sets the manufacturing date.
        /// </summary>
        /// <value>
        /// The manufacturing date.
        /// </value>
        [DataMember(Order = 1)]
        public UnixDateTime ManufacturingDate { get; set; }

        /// <summary>
        /// Gets or sets the part number.
        /// </summary>
        /// <value>
        /// The part number.
        /// </value>
        [DataMember(Order = 2)]
        public string PartNumber { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        [DataMember(Order = 3)]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the hw release.
        /// </summary>
        /// <value>
        /// The hw release.
        /// </value>
        [DataMember(Order = 4)]
        public string HWRelease { get; set; }

        /// <summary>
        /// Gets or sets the reserved.
        /// </summary>
        /// <value>
        /// The reserved.
        /// </value>
        [DataMember(Order = 5)]
        public string Reserved { get; set; }

        /// <summary>
        /// Gets or sets the sw version.
        /// </summary>
        /// <value>
        /// The sw version.
        /// </value>
        [DataMember(Order = 6)]
        public string SWVersion { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool areEqual;
            if (obj.GetType() == typeof(BuildInfo))
            {
                areEqual = this == (obj as BuildInfo);
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
        public static bool operator !=(BuildInfo x, BuildInfo y)
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
        public static bool operator ==(BuildInfo x, BuildInfo y)
        {
            var areEqual = true;
            areEqual &= x.Version == y.Version; 
            areEqual &= x.ManufacturingDate == y.ManufacturingDate;
            areEqual &= x.PartNumber.CompareTo(y.PartNumber.Trim()) == 0; 
            areEqual &= x.SerialNumber.CompareTo(y.SerialNumber.Trim()) == 0; 
            areEqual &= x.HWRelease.CompareTo(y.HWRelease.Trim()) == 0;
            areEqual &= x.Reserved.CompareTo(y.Reserved.Trim()) == 0;
            areEqual &= x.SWVersion.CompareTo(y.SWVersion.Trim()) == 0;

            return areEqual;
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int DataSize
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override byte[] ToArray()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;
            ManufacturingDate = 0;
            SerialNumber = "";
            PartNumber = "FFFFFFFFFFFFFFFF";
            HWRelease = "FFFFFFFF";
            SWVersion = "FFFFFFFF";
        }
    }
}
