using System;
using System.Collections.Generic;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Class used to filter the various results received from the meter.
    /// </summary>
    public class ResultFilter: ABaseCommsData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="version">The frame version number</param>
        /// <param name="enabled">Filter enabled flag</param>
        /// <param name="accessionNumber">the largest accession number received so far</param>
        public ResultFilter(UInt16 version, EFilterEnabled enabled, UInt32 accessionNumber)
        {
            Version = version;
            Enabled = enabled;
            AccessionNumber = accessionNumber;
        }

        /// <summary>
        /// the field backing the Disabled Filter property
        /// </summary>
        private static ResultFilter _disabledFilter = new ResultFilter(0, EFilterEnabled.Disabled, 0);

        /// <summary>
        /// A singleton instance of a disabled filter.
        /// </summary>
        /// <value>
        /// The disabled filter.
        /// </value>
        public static ResultFilter DisabledFilter
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
        public EFilterEnabled Enabled { get; set; }

        /// <summary>
        /// The accession number to filter to
        /// </summary>
        /// <value>
        /// The accession number.
        /// </value>
        public UInt32 AccessionNumber { get; set; }

        /// <summary>
        /// the data size of the payload
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return (sizeof(UInt16) * 2) + sizeof(UInt32); }
        }

        /// <summary>
        /// Convert an array of bytes to a ResultFilter
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

                Enabled = (EFilterEnabled)BitConverter.ToUInt16(data, fieldPosition);
                fieldPosition += sizeof(UInt16);

                AccessionNumber = BitConverter.ToUInt32(data, fieldPosition);
                fieldPosition += sizeof(UInt32);

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
            data.AddRange(BitConverter.GetBytes(AccessionNumber));

            return data.ToArray();
        }

        /// <summary>
        /// Method called during construction that will set the properties to invalid values
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;

            Enabled = EFilterEnabled.Invalid;

            AccessionNumber = 0xFFFFFFFF;
        }
    }

    /// <summary>
    /// Enum defining the Filter enabled states.
    /// </summary>
    public enum EFilterEnabled : ushort
    {
        /// <summary>
        /// Disable Filter
        /// </summary>
        Disabled = 0x0000,
        /// <summary>
        /// Enable Filter
        /// </summary>
        Enable = 0x0001,
        /// <summary>
        /// The Invalid Filter
        /// </summary>
        Invalid = 0xFFFF
    }
}
