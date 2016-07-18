using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Test Result base class
    /// </summary>
    [DataContract]
    public abstract class ABaseTestResult : ABaseRecordCommsData
    {
        #region Constructor

        /// <summary>
        /// Constructor that takes an array of bytes and parses a TestResult object from it,
        /// </summary>
        /// <param name="dataArray">The array of bytes that contains the data to parse</param>
        /// <param name="offset">provides and offset to the start parsing the dataArray data</param>
        public ABaseTestResult(byte[] dataArray, int offset)
            :this()
        {
            FromArray(dataArray, offset);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseTestResult"/> class.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        public ABaseTestResult(byte[] dataArray)
            : this(dataArray, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseTestResult"/> class.
        /// </summary>
        public ABaseTestResult()
        {
            InitialiseInvalid();
        }

        #endregion Constructor

        #region Public Data

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember]
        public UInt16 Version { get; set; } //2

        /// <summary>
        /// Gets or sets the specific data.
        /// </summary>
        /// <value>
        /// The specific data.
        /// </value>
        [DataMember]
        public ABaseCommsData SpecificData { get; protected set; } //24

        /// <summary>
        /// Gets or sets the test result.
        /// </summary>
        /// <value>
        /// The test result.
        /// </value>
        [DataMember]
        public Result TestResult { get; set; } //48

        /// <summary>
        /// Gets or sets the sample.
        /// </summary>
        /// <value>
        /// The sample.
        /// </value>
        [DataMember]
        public SampleInfo Sample { get; set; } //6

        /// <summary>
        /// Gets or sets the transient.
        /// </summary>
        /// <value>
        /// The transient.
        /// </value>
        [DataMember]
        public TransientDetails Transient { get; set; } //20

        /// <summary>
        /// Gets or sets the vial.
        /// </summary>
        /// <value>
        /// The vial.
        /// </value>
        [DataMember]
        public VialDetails Vial { get; set; } //16

        /// <summary>
        /// Gets or sets the build information.
        /// </summary>
        /// <value>
        /// The build information.
        /// </value>
        [DataMember]
        public ResultBuildInfo BuildInformation { get; set; } //40

        /// <summary>
        /// Gets or sets the fault identifier.
        /// </summary>
        /// <value>
        /// The fault identifier.
        /// </value>
        [DataMember]
        public UInt32 FaultIdentifier { get; set; } //2

        #endregion Public Data

        #region Public Methods

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;
            ValidData = EValidResult.Invalid;
            IsValid = false;
            TestResult = new Result();
            Sample = new SampleInfo();
            Transient = new TransientDetails();
            Vial = new VialDetails();
            BuildInformation = new ResultBuildInfo();
            FaultIdentifier = 0xFFFFFFFF;
        }

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
                var result = 0;

                result += sizeof(UInt16);               //version 
                result += sizeof(UInt16);               //valid data
                result += TestResult.DataSize;          
                result += Sample.DataSize;
                result += Transient.DataSize;
                result += Vial.DataSize;
                result += BuildInformation.DataSize;
                result += sizeof(UInt32);             //fault identifier

                return result;
            }
        }

        /// <summary>
        /// Parse an array of bytes into the Test Result object. Subclasses should override this method
        /// to provide custom parsing for the more specific test result classes.
        /// </summary>
        /// <param name="dataArray">The array that contains the data to be parsed</param>
        /// <param name="offset">The offset.</param>
        /// <returns>
        /// true on success otherwise false.
        /// </returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            var fieldStart = offset;
            
            //Logger.LogMessage(Logger.BDMSwitch,
            //                               TraceLevel.Info,
            //                               dataArray.Length.ToString() + " received data size");

            if (dataArray.Length >= (offset + DataSize))
            {
                Version = BitConverter.ToUInt16(dataArray, fieldStart);
                fieldStart += sizeof(UInt16);

                ValidData = (EValidResult)BitConverter.ToUInt16(dataArray, fieldStart);
                fieldStart += sizeof(UInt16);

                SpecificData.FromArray(dataArray, fieldStart);
                fieldStart += SpecificData.DataSize;

                TestResult.FromArray(dataArray, fieldStart);
                fieldStart += TestResult.DataSize;

                Sample.FromArray(dataArray, fieldStart);
                fieldStart += Sample.DataSize;

                Transient.FromArray(dataArray, fieldStart);
                fieldStart += Transient.DataSize;

                Vial.FromArray(dataArray, fieldStart);
                fieldStart += Vial.DataSize;

                BuildInformation.FromArray(dataArray, fieldStart);
                fieldStart += BuildInformation.DataSize;

                FaultIdentifier = BitConverter.ToUInt32(dataArray, fieldStart);

                IsValid = true;
            }

            return IsValid;
        }

        /// <summary>
        /// Convert the Result into an array of bytes
        /// </summary>
        /// <returns>
        /// the byte array representation of an instance of this class
        /// </returns>
        public override byte[] ToArray()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(Version));
            data.AddRange(BitConverter.GetBytes((UInt16)ValidData));
            data.AddRange(SpecificData.ToArray());
            data.AddRange(TestResult.ToArray());
            data.AddRange(Sample.ToArray());
            data.AddRange(Transient.ToArray());
            data.AddRange(Vial.ToArray());
            data.AddRange(BuildInformation.ToArray());
            data.AddRange(BitConverter.GetBytes(FaultIdentifier));

            //Logger.LogMessage(Logger.BDMSwitch,
            //                                           TraceLevel.Verbose,
            //                                           data.Count.ToString() + " To array size");

            return data.ToArray();
        }

        /// <summary>
        /// Specifics the data from array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected abstract ABaseCommsData SpecificDataFromArray(byte[] dataAsArray, int offset);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var areEqual = false;
            
            if (obj is ABaseTestResult)
            {
                areEqual = this == (obj as ABaseTestResult);
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
        public static bool operator !=(ABaseTestResult x, ABaseTestResult y)
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
        public static bool operator ==(ABaseTestResult x, ABaseTestResult y)
        {
            var areEqual = true;
         
            areEqual &= x.Version == y.Version;
            areEqual &= x.ValidData == y.ValidData;
            areEqual &= x.TestResult == y.TestResult;
            areEqual &= x.Sample == y.Sample;
            areEqual &= x.Transient == y.Transient;
            areEqual &= x.Vial == y.Vial;
            areEqual &= x.BuildInformation == y.BuildInformation;
            areEqual &= x.FaultIdentifier == y.FaultIdentifier;

            areEqual &= x.SpecificData.Equals(y.SpecificData);

            return areEqual;
        }

        #endregion Public Methods
    }
}
