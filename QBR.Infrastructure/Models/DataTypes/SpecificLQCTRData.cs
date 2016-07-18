
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
    /// Specific LQC Transient Data
    /// </summary>
    public class SpecificLQCTRData: ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificLQCTRData"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public SpecificLQCTRData(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificLQCTRData"/> class.
        /// </summary>
        public SpecificLQCTRData()
        { }

        /// <summary>
        /// The LQC result data size
        /// </summary>
        public static readonly int LQCResultDataSize = (2 * sizeof(UInt32)) + (2 * sizeof(float)) + sizeof(UInt16) + ((4 + 4 + 8));

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
                return LQCResultDataSize;
            }
                
        }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>
        /// The accession number.
        /// </value>
        public UInt32 AccessionNumber { get; set; } //4

        /// <summary>
        /// Gets or sets the lot number.
        /// </summary>
        /// <value>
        /// The lot number.
        /// </value>
        public string LotNumber { get; set; } //8

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>
        /// The expiry date.
        /// </value>
        public UnixDateTime ExpiryDate { get; set; } //4

        /// <summary>
        /// Gets or sets the level number.
        /// </summary>
        /// <value>
        /// The level number.
        /// </value>
        public UInt16 LevelNumber { get; set; } // 2

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value.
        /// </value>
        public float MinimumValue { get; set; } //4

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value.
        /// </value>
        public float MaximumValue { get; set; } //4

        /// <summary>
        /// Gets or sets the minimum value display.
        /// </summary>
        /// <value>
        /// The minimum value display.
        /// </value>
        public string MinimumValueDisplay { get; set; } //4

        /// <summary>
        /// Gets or sets the maximum value display.
        /// </summary>
        /// <value>
        /// The maximum value display.
        /// </value>
        public string MaximumValueDisplay { get; set; } //4

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
                var fieldSize = 0;

                fieldSize = sizeof(UInt32);
                AccessionNumber = BitConverter.ToUInt32(dataArray, fieldStart);
                fieldStart += fieldSize;

                fieldSize = 8;
                LotNumber = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                LotNumber = LotNumber.Trim();
                fieldStart += fieldSize;

                fieldSize = sizeof(UInt32);
                ExpiryDate = BitConverter.ToUInt32(dataArray, fieldStart);
                fieldStart += fieldSize;

                fieldSize = sizeof(UInt16);
                LevelNumber = BitConverter.ToUInt16(dataArray, fieldStart);
                fieldStart += fieldSize;

                fieldSize = sizeof(float);
                MinimumValue = BitConverter.ToSingle(dataArray, fieldStart);
                fieldStart += fieldSize;

                fieldSize = 4;
                MinimumValueDisplay = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                MinimumValueDisplay = MinimumValueDisplay.Trim(new[] { '\0', ' ' });
                fieldStart += fieldSize;

                fieldSize = sizeof(float);
                MaximumValue = BitConverter.ToSingle(dataArray, fieldStart);
                fieldStart += fieldSize;

                fieldSize = 4;
                MaximumValueDisplay = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                MaximumValueDisplay = MaximumValueDisplay.Trim(new[] { '\0', ' ' });
                fieldStart += fieldSize;

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

            data.AddRange(BitConverter.GetBytes(AccessionNumber));
            data.AddRange(Encoding.ASCII.GetBytes(LotNumber));
            data.AddRange(BitConverter.GetBytes(ExpiryDate));
            data.AddRange(BitConverter.GetBytes(LevelNumber));
            data.AddRange(BitConverter.GetBytes(MinimumValue));
            data.AddRange(Encoding.ASCII.GetBytes(MinimumValueDisplay));
            data.AddRange(BitConverter.GetBytes(MaximumValue));
            data.AddRange(Encoding.ASCII.GetBytes(MaximumValueDisplay));

            return data.ToArray();
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            AccessionNumber = 0xFFFFFFFF;
            LotNumber = "XXXXXXXX";
            ExpiryDate = 0x00000000;
            LevelNumber = 0xFFFF;
            MinimumValue = 0xFFFFFFFF;
            MinimumValueDisplay = "XXXX";
            MaximumValue = 0x00000000;
            MaximumValueDisplay = "XXXX";
            IsValid = false;
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(SpecificLQCTRData x, SpecificLQCTRData y)
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
        public static bool operator ==(SpecificLQCTRData x, SpecificLQCTRData y)
        {
            var areEqual = true;

            areEqual &= x.AccessionNumber == y.AccessionNumber;
            areEqual &= x.LotNumber.CompareTo(y.LotNumber) == 0;
            areEqual &= x.ExpiryDate == y.ExpiryDate;
            areEqual &= x.LevelNumber == y.LevelNumber;
            areEqual &= x.MinimumValue == y.MinimumValue;
            areEqual &= x.MinimumValueDisplay.CompareTo(y.MinimumValueDisplay) == 0;
            areEqual &= x.MaximumValue == y.MaximumValue;
            areEqual &= x.MaximumValueDisplay.CompareTo(y.MaximumValueDisplay) == 0;

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

            if (obj.GetType() == typeof(SpecificLQCTRData))
            {
                areEqual = this == (obj as SpecificLQCTRData);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }
    }
}
