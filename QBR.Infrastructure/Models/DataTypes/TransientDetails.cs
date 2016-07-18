
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

namespace QBR.Infrastructure.Models.DataTypes
{

    /// <summary>
    /// Transient Details
    /// </summary>
    public class TransientDetails: ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientDetails"/> class.
        /// </summary>
        /// <param name="detailsAsArray">The details as array.</param>
        /// <param name="offset">The offset.</param>
        public TransientDetails(byte[] detailsAsArray, int offset)
            : base(detailsAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientDetails"/> class.
        /// </summary>
        public TransientDetails()
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
                return sizeof(float) * 8;
            }
        }

        /// <summary>
        /// Gets or sets the minimum_ time.
        /// </summary>
        /// <value>
        /// The minimum_ time.
        /// </value>
        public float Minimum_Time { get; set; }

        /// <summary>
        /// Gets or sets the minimum_ current.
        /// </summary>
        /// <value>
        /// The minimum_ current.
        /// </value>
        public float Minimum_Current { get; set; }

        /// <summary>
        /// Gets or sets the time1 point2 microamp rise.
        /// </summary>
        /// <value>
        /// The time1 point2 microamp rise.
        /// </value>
        public float Time1Point2MicroampRise { get; set; }

        /// <summary>
        /// Gets or sets the current1 point2 microamp rise.
        /// </summary>
        /// <value>
        /// The current1 point2 microamp rise.
        /// </value>
        public float Current1Point2MicroampRise { get; set; }

        /// <summary>
        /// Gets or sets the time1 point3 microamp rise.
        /// </summary>
        /// <value>
        /// The time1 point3 microamp rise.
        /// </value>
        public float Time1Point3MicroampRise { get; set; }

        /// <summary>
        /// Gets or sets the current1 point3 microamp rise.
        /// </summary>
        /// <value>
        /// The current1 point3 microamp rise.
        /// </value>
        public float Current1Point3MicroampRise { get; set; }

        /// <summary>
        /// Gets or sets the peak current.
        /// </summary>
        /// <value>
        /// The peak current.
        /// </value>
        public float PeakCurrent { get; set; }

        /// <summary>
        /// Gets or sets the obc value.
        /// </summary>
        /// <value>
        /// The obc value.
        /// </value>
        public float OBCValue { get; set; }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Minimum_Time = 0xFFFFFFFF;
            Minimum_Current = 0xFFFFFFFF;
            Time1Point2MicroampRise = 0xFFFFFFFF;
            Current1Point2MicroampRise = 0xFFFFFFFF;
            Time1Point3MicroampRise = 0xFFFFFFFF;
            Current1Point3MicroampRise = 0xFFFFFFFF;
            PeakCurrent = 0xFFFFFFFF;
            OBCValue = 0xFFFFFFFF;
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="detailsAsArray">The details as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] detailsAsArray, int offset)
        {
            if (detailsAsArray.Length >= (offset + DataSize))
            {
                var fieldStart = offset;
                Minimum_Time = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                Minimum_Current = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                Time1Point2MicroampRise = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                Current1Point2MicroampRise = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                Time1Point3MicroampRise = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                Current1Point3MicroampRise = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);
                
                PeakCurrent = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                OBCValue = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

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

            data.AddRange(BitConverter.GetBytes(Minimum_Time));
            data.AddRange(BitConverter.GetBytes(Minimum_Current));
            data.AddRange(BitConverter.GetBytes(Time1Point2MicroampRise));
            data.AddRange(BitConverter.GetBytes(Current1Point2MicroampRise));
            data.AddRange(BitConverter.GetBytes(Time1Point3MicroampRise));
            data.AddRange(BitConverter.GetBytes(Current1Point3MicroampRise));
            data.AddRange(BitConverter.GetBytes(PeakCurrent));
            data.AddRange(BitConverter.GetBytes(OBCValue));

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
        public static bool operator !=(TransientDetails x, TransientDetails y)
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
        public static bool operator ==(TransientDetails x, TransientDetails y)
        {
            var areEqual = true;

            areEqual &= x.Minimum_Time == y.Minimum_Time;
            areEqual &= x.Minimum_Current == y.Minimum_Current;
            areEqual &= x.Time1Point2MicroampRise == y.Time1Point2MicroampRise;
            areEqual &= x.Current1Point2MicroampRise == y.Current1Point2MicroampRise;
            areEqual &= x.Time1Point3MicroampRise == y.Time1Point3MicroampRise;
            areEqual &= x.Current1Point3MicroampRise == y.Current1Point3MicroampRise;
            areEqual &= x.PeakCurrent == y.PeakCurrent;
            areEqual &= x.OBCValue == y.OBCValue;

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

            if (obj.GetType() == typeof(TransientDetails))
            {
                areEqual = this == (obj as TransientDetails);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

    }
}
