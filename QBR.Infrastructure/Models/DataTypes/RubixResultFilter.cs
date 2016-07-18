using System;
using System.Collections.Generic;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Class used to filter the various results received from the meter.
    /// </summary>
    public class RubixResultFilter: ResultFilter
    {

        /// <summary>
        /// The OID length
        /// </summary>
        public const int OIDLength = 16;
        /// <summary>
        /// The PID length
        /// </summary>
        public const int PIDLength = 20;
        /// <summary>
        /// The patient name length
        /// </summary>
        public const int PatientNameLength = 32;
        /// <summary>
        /// The operator name length
        /// </summary>
        public const int OperatorNameLength = 32;

        /// <summary>
        /// The OID
        /// </summary>
        public byte[] OID = new byte[OIDLength];
        /// <summary>
        /// The PID
        /// </summary>
        public byte[] PID = new byte[PIDLength];
        /// <summary>
        /// The patient name
        /// </summary>
        public byte[] PatientName = new byte[PatientNameLength];
        /// <summary>
        /// The operator name
        /// </summary>
        public byte[] OperatorName = new byte[OperatorNameLength];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="version">The frame version number</param>
        /// <param name="enabled">Filter enabled flag</param>
        /// <param name="accessionNumber">the largest accession number received so far</param>
        public RubixResultFilter(UInt16 version, EFilterEnabled enabled, UInt32 accessionNumber):base(version,enabled,accessionNumber)
        {
        }

        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public UInt16 TestType { get; set; }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public UInt32 DateTime { get; set; }


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
        /// Convert an array of bytes to a RubixResultFilter
        /// </summary>
        /// <param name="data">The byte array containing the data</param>
        /// <param name="offset">an offset in the data array to begin the parsing</param>
        /// <returns>
        /// true if the Result filter successfully parsed data otherwise false
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool FromArray(byte[] data, int offset)
        {
            throw new NotImplementedException();
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
            data.AddRange(BitConverter.GetBytes((UInt32)Enabled));
            data.AddRange(BitConverter.GetBytes(AccessionNumber));
            data.AddRange(OID);
            data.AddRange(PID);
            data.AddRange(BitConverter.GetBytes(TestType));
            data.AddRange(BitConverter.GetBytes(DateTime));
            data.AddRange(PatientName);
            data.AddRange(OperatorName);

            return data.ToArray();
        }

        /// <summary>
        /// Method called during construction that will set the properties to invalid values
        /// </summary>
        protected override void InitialiseInvalid()
        {
            base.InitialiseInvalid();
            TestType = 0xFFFF;
            DateTime = 0xFFFFFFFF;
        }
    }
}
