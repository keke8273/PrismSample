
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
    /// Class used to filter the various results received from the meter.
    /// </summary>
    public class OperatorIDFilter: ABaseCommsData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="version">The frame version number</param>
        /// <param name="enabled">Filter enabled flag</param>
        /// <param name="entryType">Type of the entry.</param>
        public OperatorIDFilter(UInt16 version, EOperatorIDFilterEnabled enabled, EOperatorIDEntryType entryType)
        {
            Version = version;
            Enabled = enabled;
            EntryType = entryType;
        }

        /// <summary>
        /// the field backing the Disabled Filter property
        /// </summary>
        private static readonly OperatorIDFilter _disabledFilter = new OperatorIDFilter(0, EOperatorIDFilterEnabled.Disabled, EOperatorIDEntryType.GUI);

        /// <summary>
        /// A singleton instance of a disabled filter.
        /// </summary>
        /// <value>
        /// The disabled filter.
        /// </value>
        public static OperatorIDFilter DisabledFilter
        {
            get
            {
                return _disabledFilter;
            }
        }

        /// <summary>
        /// The Frame version number
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public UInt16 Version { get; set; }

        /// <summary>
        /// Filtering enabled flag
        /// </summary>
        /// <value>
        /// The enabled.
        /// </value>
        public EOperatorIDFilterEnabled Enabled { get; set; }

        /// <summary>
        /// The accession number to filter to
        /// </summary>
        /// <value>
        /// The type of the entry.
        /// </value>
        public EOperatorIDEntryType EntryType { get; set; }

        /// <summary>
        /// the data size of the payload
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return (sizeof(UInt16) * 3); }
        }

        /// <summary>
        /// Convert an array of bytes to a OperatorIDFilter
        /// </summary>
        /// <param name="data">The byte array containing the data</param>
        /// <param name="offset">an offset in the data array to begin the parsing</param>
        /// <returns>
        /// true if the Result filter successfully parsed data otherwise false
        /// </returns>
        public override bool FromArray(byte[] data, int offset)
        {
            IsValid = false;
            if (data.Length >= DataSize)
            {
                var fieldPosition = offset;

                Version = BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                Enabled = (EOperatorIDFilterEnabled)BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                EntryType = (EOperatorIDEntryType)BitConverter.ToUInt16(data, fieldPosition);

                IsValid = true;
            }
            return IsValid;
        }

        /// <summary>
        /// Convert the Result filter to an array of bytes
        /// </summary>
        /// <returns>
        /// A byte array representation of the Result Filter
        /// </returns>
        public override byte[] ToArray()
        {
            var data = new List<byte>(DataSize);

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes((UInt16)Enabled));
            data.AddRange(BitConverter.GetBytes((UInt16)EntryType));

            return data.ToArray();
        }

        /// <summary>
        /// Method called during construction that will set the properties to invalid values
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;

            Enabled = EOperatorIDFilterEnabled.Invalid;

            EntryType = EOperatorIDEntryType.Invalid;
        }
    }

    /// <summary>
    /// Enum defining the Filter enabled states.
    /// </summary>
    public enum EOperatorIDFilterEnabled : ushort
    {
        /// <summary>
        /// The disabled
        /// </summary>
        Disabled = 0x0000,
        /// <summary>
        /// The enable
        /// </summary>
        Enable = 0x0001,
        /// <summary>
        /// The invalid
        /// </summary>
        Invalid = 0xFFFF
    }

    /// <summary>
    /// 
    /// </summary>
    public enum EOperatorIDEntryType : ushort
    {
        /// <summary>
        /// The GUI
        /// </summary>
        GUI = 0x1,
        /// <summary>
        /// The usb
        /// </summary>
        USB = 0x2,
        /// <summary>
        /// The invalid
        /// </summary>
        Invalid = 0xFFFF
    }
}
