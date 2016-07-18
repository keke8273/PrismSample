
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
using System.Linq;
using System.Runtime.Serialization;
using QBR.Infrastructure.Models.Enums;
using QBR.Infrastructure.Utilities;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Transient Results
    /// </summary>
    [DataContract]
    public class TransientResult : ABaseRecordCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResult"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public TransientResult(byte[] resultAsArray, int offset)
            : base(resultAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResult"/> class.
        /// </summary>
        public TransientResult()
        {
        }

        /// <summary>
        /// Gets the size of the transient result data.
        /// </summary>
        /// <value>
        /// The size of the transient result data.
        /// </value>
        public int TransientResultDataSize
        {
            get
            {
                var size = (sizeof(UInt16) * 4) + (sizeof(UInt32) * 2);

                return size;
            }
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
                return TransientResultDataSize;
            }
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        [DataMember(Order = 0)]          
        public UInt16 Version { get; set; }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>
        /// The accession number.
        /// </value>
        [DataMember(Order = 1)]
        public UInt32 AccessionNumber { get; set; }

        /// <summary>
        /// Gets or sets the CRC.
        /// </summary>
        /// <value>
        /// The CRC.
        /// </value>
        [DataMember(Order = 2)]
        public UInt32 CRC { get; set; }

        /// <summary>
        /// Gets or sets the blocks remaining.
        /// </summary>
        /// <value>
        /// The blocks remaining.
        /// </value>
        [DataMember(Order = 3)]
        public UInt16 BlocksRemaining { get; set; }

        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        [DataMember(Order = 4)]
        public UInt16 TestType { get; set; }

        /// <summary>
        /// Gets or sets the k value.
        /// </summary>
        /// <value>
        /// The k value.
        /// </value>
        //[DataMember(Order = 5)]
        public string VaidCaseID { get; set; }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            Version = 0xFFFF;
            AccessionNumber = 0xFFFFFFFF;
            CRC = 0xFFFFFFFF;
            BlocksRemaining = 0xFFFF;
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataAsArray, int offset)
        {
            if (dataAsArray.Length >= (offset + TransientResultDataSize))
            {
                var fieldPosition = offset;

                Version = BitConverter.ToUInt16(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                ValidData = (EValidResult)BitConverter.ToUInt16(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                AccessionNumber = BitConverter.ToUInt32(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                CRC = BitConverter.ToUInt32(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                BlocksRemaining = BitConverter.ToUInt16(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                TestType = BitConverter.ToUInt16(dataAsArray, fieldPosition);

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
            data.AddRange(BitConverter.GetBytes(Version));

            data.AddRange(BitConverter.GetBytes((UInt16)ValidData));
            
            data.AddRange(BitConverter.GetBytes(AccessionNumber));
            
            data.AddRange(BitConverter.GetBytes(CRC));

            data.AddRange(BitConverter.GetBytes(BlocksRemaining));

            data.AddRange(BitConverter.GetBytes(TestType));

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
        public static bool operator !=(TransientResult x, TransientResult y)
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
        public static bool operator ==(TransientResult x, TransientResult y)
        {
            var areEqual = true;
            areEqual &= x.Version == y.Version;
            areEqual &= x.AccessionNumber == y.AccessionNumber;
            areEqual &= x.ValidData == y.ValidData;
            areEqual &= x.CRC == y.CRC;
            areEqual &= x.BlocksRemaining == y.BlocksRemaining;
            areEqual &= x.TestType == y.TestType;

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

            if (obj.GetType() == typeof(TransientResult))
            {
                areEqual = this == (obj as TransientResult);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        /// <summary>
        /// Froms the string.
        /// </summary>
        /// <param name="dataAsString">The data as string.</param>
        /// <returns></returns>
        public virtual bool FromString(string dataAsString)
        {
            IsValid = false;

            var stringList = dataAsString.Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var testTypeIndex = stringList.IndexOf("TestType");
            var solutionIndex = stringList.IndexOf("Solution");

            if (testTypeIndex == -1 || solutionIndex == -1)
            {
                testTypeIndex = stringList.IndexOf("Assay Type");
                solutionIndex = stringList.IndexOf("Sample Type Flag");
            }

            TestType = ParseTestType(stringList[testTypeIndex + 1], stringList[solutionIndex + 1]);
            return IsValid;
        }

        /// <summary>
        /// Parses the type of the test.
        /// </summary>
        /// <param name="assayName">Name of the assay.</param>
        /// <param name="sampleName">Name of the sample.</param>
        /// <returns></returns>
        private UInt16 ParseTestType(string assayName, string sampleName)
        {
            if (assayName == "ACT" && sampleName == "Blood")
            {
                return (UInt16)TestTypes.RubACTBlood;
            }
            if (assayName == "ACT" && sampleName == "LQC")
            {
                return (UInt16)TestTypes.RubACTLQC; 
            }
            if (assayName == "APTT" && sampleName == "Blood")
            {
                return (UInt16)TestTypes.RubAPTTBlood;
            }
            if (assayName == "APTT" && sampleName == "LQC")
            {
                return (UInt16)TestTypes.RubAPTTLQC;
            }
            if (assayName == "PT/INR" && sampleName == "Blood")
            {
                return (UInt16)TestTypes.RubPTBlood;
            }
            if (assayName == "PT/INR" && sampleName == "LQC")
            {
                return (UInt16)TestTypes.RubPTLQC;
            }
            if (assayName == "PT/INR" && sampleName == "1")
            {
                return (UInt16)TestTypes.ProPTBlood;
            }
            if (assayName == "PT/INR" && sampleName == "2")
            {
                return (UInt16)TestTypes.ProPTLQC;
            }
            return (UInt16) TestTypes.Unknown;
        }
    }

    /// <summary>
    /// LQC Transient Result
    /// </summary>
    [DataContract]
    public class TransientResultLQC : TransientResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResultLQC"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public TransientResultLQC(byte[] resultAsArray, int offset)
        : base(resultAsArray, offset)
       { }


        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResultLQC"/> class.
        /// </summary>
        public TransientResultLQC()
        {
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
                var size = base.DataSize;
                size += sizeof(UInt16);               //LQC version 
                size += sizeof(UInt16);               //LQC valid data
                size += LQC.SpecificData.DataSize;
                size += LQC.TestResult.DataSize;
                size += LQC.Sample.DataSize;
                size += LQC.Transient.DataSize;
                size += LQC.Vial.DataSize;
                size += LQC.BuildInformation.DataSize;
                size += sizeof(UInt32);              //fault identifier

                return size;
            }
        }

        /// <summary>
        /// Gets or sets the LQC.
        /// </summary>
        /// <value>
        /// The LQC.
        /// </value>
        [DataMember]
        public LQCResult LQC { get; set; }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            base.InitialiseInvalid();
            LQC = new LQCResult();
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataAsArray, int offset)
        {
            // expected length is >= DataSize - sizeof(LQC.Version) - sizeof(LQC.ValidData) 
            if (dataAsArray.Length >= (offset + DataSize - sizeof(UInt16) - sizeof(UInt16)))
            {
                var fieldPosition = offset;

                base.FromArray(dataAsArray, fieldPosition);

                fieldPosition += base.DataSize;

                LQC.Version = 0;

                LQC.ValidData = EValidResult.ValidRecord;

                LQC.SpecificData.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.SpecificData.DataSize;

                LQC.TestResult.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.TestResult.DataSize;

                LQC.Sample.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.Sample.DataSize;

                LQC.Transient.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.Transient.DataSize;

                LQC.Vial.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.Vial.DataSize;

                LQC.BuildInformation.FromArray(dataAsArray, fieldPosition);
                fieldPosition += LQC.BuildInformation.DataSize;

                LQC.FaultIdentifier = BitConverter.ToUInt32(dataAsArray, fieldPosition);
                     
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
            data.AddRange(base.ToArray());

            data.AddRange(LQC.ToArray());

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
        public static bool operator !=(TransientResultLQC x, TransientResultLQC y)
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
        public static bool operator ==(TransientResultLQC x, TransientResultLQC y)
        {
            var areEqual = true;

            areEqual &= x.LQC.BuildInformation == y.LQC.BuildInformation;
            areEqual &= x.LQC.FaultIdentifier == y.LQC.FaultIdentifier;
            areEqual &= x.LQC.Sample == y.LQC.Sample;
            areEqual &= x.LQC.SpecificData == y.LQC.SpecificData;
            areEqual &= x.LQC.TestResult == y.LQC.TestResult;
            areEqual &= x.LQC.Transient == y.LQC.Transient;
            areEqual &= x.LQC.Vial == y.LQC.Vial;
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

            if (obj.GetType() == typeof(TransientResultLQC))
            {
                areEqual &= this == (obj as TransientResultLQC);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        public override bool FromString(string dataAsString)
        {
            IsValid = false;

            var stringList = dataAsString.Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var index = stringList.IndexOf("Accession Number");
            if (index >= 0)
                base.AccessionNumber = base.AccessionNumber.ParseOrDefault(stringList[index + 1]);

            base.TestType = (UInt16)TestTypes.ProPTLQC;
            LQC.Sample.TypeFlag = LQC.Sample.TypeFlag.ParseOrDefault(stringList[index + 1]);

            //Build Information
            //index = stringList.IndexOf("PartNumber");
            //if (index >= 0)
            //    Patient.BuildInformation.PartNumber = stringList[index + 1];

            index = stringList.IndexOf("Serial Number");
            if (index >= 0)
                LQC.BuildInformation.SerialNumber = stringList[index + 1];

            //index = stringList.IndexOf("SoftwareBuild");
            //if (index >= 0)
            //    PatientResult.BuildInformation.SWVersion = stringList[index + 1];

            //index = stringList.IndexOf("HardwareBuild");
            //if (index >= 0)
            //    PatientResult.BuildInformation.HWRelease = stringList[index + 1];

            //index = stringList.IndexOf("SoftwareVersion");
            //if (index >= 0)
            //    PatientResult.BuildInformation.SWVersion = stringList[index + 1];

            //index = stringList.IndexOf("HardwareRelease");
            //if (index >= 0)
            //    PatientResult.BuildInformation.HWRelease = stringList[index + 1];

            //Sample Information
            index = stringList.IndexOf("Sample Detect Time");
            if (index >= 0)
            {
                var detectTime = new DateTime();
                detectTime = detectTime.ParseOrDefault(stringList[index + 1]);
                LQC.Sample.DetectTime = new UnixDateTime((uint)(detectTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            }


            //Test Results
            index = stringList.IndexOf("OID");
            if (index >= 0)
                LQC.TestResult.OperatorID = stringList[index + 1];

            //index = stringList.IndexOf("StripIndex");
            //if (index >= 0) Patient.TestResult.TestStripIndex = Patient.TestResult.TestStripIndex.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Strip Lot Number");
            if (index >= 0) LQC.TestResult.TestStripLotNumber = LQC.TestResult.TestStripLotNumber.ParseOrDefault(stringList[index + 1]);

            //index = stringList.IndexOf("StripExpiry");
            //if (index >= 0)
            //{
            //    var stripExpiryDate = new DateTime();
            //    stripExpiryDate = stripExpiryDate.ParseOrDefault(stringList[index + 1]);
            //    Patient.TestResult.TestStripExpiryDate = new UnixDateTime((uint)(stripExpiryDate - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            //}

            index = stringList.IndexOf("Clot Time");
            if (index >= 0)
                LQC.TestResult.CT = LQC.TestResult.CT.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Calibrated Clot Time");
            if (index >= 0)
                LQC.TestResult.CCT = LQC.TestResult.CCT.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("INR");
            if (index >= 0)
                LQC.TestResult.INR = LQC.TestResult.INR.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Fault Identifier");
            if (index >= 0)
                LQC.FaultIdentifier = LQC.FaultIdentifier.ParseOrDefault(stringList[index + 1]);
            //Vial Information

            //Transient Information
            var transientDetials = LQC.Transient;
            index = stringList.IndexOf("Peak Current");
            if (index >= 0)
                transientDetials.PeakCurrent = transientDetials.PeakCurrent.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("C12");
            if (index >= 0)
                transientDetials.Current1Point2MicroampRise = transientDetials.Current1Point2MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("C13");
            if (index >= 0)
                transientDetials.Current1Point3MicroampRise = transientDetials.Current1Point3MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("T12");
            if (index >= 0)
                transientDetials.Time1Point2MicroampRise = transientDetials.Time1Point2MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("T13");
            if (index >= 0)
                transientDetials.Time1Point3MicroampRise = transientDetials.Time1Point3MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("TBase");
            if (index >= 0)
                transientDetials.Minimum_Time = transientDetials.Minimum_Time.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("IBase");
            if (index >= 0)
                transientDetials.Minimum_Current = transientDetials.Minimum_Current.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("OBC");
            if (index >= 0)
                transientDetials.OBCValue = transientDetials.OBCValue.ParseOrDefault(stringList[index + 1]);

            //Specific Data
            var specificData = LQC.SpecificData as SpecificLQCTRData;
            index = stringList.IndexOf("Lot Number");
            if (index >= 0)
                specificData.LotNumber = stringList[index + 1];

            index = stringList.IndexOf("Result Accession");
            if (index >= 0)
                specificData.AccessionNumber = specificData.AccessionNumber.ParseOrDefault(stringList[index + 1]);

            LQC.ValidData = EValidResult.ValidRecord;
            ValidData = EValidResult.ValidRecord;
            Version = 0;
            IsValid = true;
            return IsValid;

        }

    }

    /// <summary>
    /// Patient Transient Result
    /// </summary>
    [DataContract]
    public class TransientResultPatient : TransientResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResultPatient"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public TransientResultPatient(byte[] resultAsArray, int offset)
         : base(resultAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientResultPatient"/> class.
        /// </summary>
        public TransientResultPatient()
        {
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
                var size = base.DataSize;
                size += sizeof(UInt16);               //PatientVersion
                size += sizeof(UInt16);               //Patient valid data
                size += Patient.SpecificData.DataSize;
                size += Patient.TestResult.DataSize;
                size += Patient.Sample.DataSize;
                size += Patient.Transient.DataSize;
                size += Patient.Vial.DataSize;
                size += Patient.BuildInformation.DataSize;
                size += sizeof(UInt32);             //fault identifier

                return size;
            }
        }

        /// <summary>
        /// Gets or sets the patient.
        /// </summary>
        /// <value>
        /// The patient.
        /// </value>
        [DataMember]
        public PatientResult Patient { get; set; }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            base.InitialiseInvalid();
            Patient = new PatientResult();
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataAsArray, int offset)
        {
            // expected length is >= DataSize - sizeof(Patient.Version) - sizeof(Patient.ValidData) 
            if (dataAsArray.Length >= (offset + DataSize - sizeof(UInt16) - sizeof(UInt16)))
            {
                var fieldPosition = offset;

                base.FromArray(dataAsArray, fieldPosition);

                fieldPosition += base.DataSize;

                Patient.Version = 0;
                
                Patient.ValidData = EValidResult.ValidRecord;

                Patient.SpecificData.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.SpecificData.DataSize;

                Patient.TestResult.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.TestResult.DataSize;

                Patient.Sample.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.Sample.DataSize;

                Patient.Transient.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.Transient.DataSize;

                Patient.Vial.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.Vial.DataSize;

                Patient.BuildInformation.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Patient.BuildInformation.DataSize;

                Patient.FaultIdentifier = BitConverter.ToUInt32(dataAsArray, fieldPosition);
                
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
            data.AddRange(base.ToArray());

            data.AddRange(Patient.ToArray());

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
        public static bool operator !=(TransientResultPatient x, TransientResultPatient y)
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
        public static bool operator ==(TransientResultPatient x, TransientResultPatient y)
        {
            var areEqual = true;

            areEqual &= x.Patient == y.Patient;

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

            if (obj.GetType() == typeof(TransientResultPatient))
            {
                areEqual = this == (obj as TransientResultPatient);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        public override bool FromString(string dataAsString)
        {
            IsValid = false;

            var stringList = dataAsString.Split(new[] { '\r', '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var index = stringList.IndexOf("Accession Number");
            if (index >= 0)
                base.AccessionNumber = base.AccessionNumber.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Sample Type Flag");
            if (index >= 0)
            {
                base.TestType = stringList[index + 1] == "1" ? (UInt16)TestTypes.ProPTBlood : (UInt16)TestTypes.ProPTLQC;
                Patient.Sample.TypeFlag = Patient.Sample.TypeFlag.ParseOrDefault(stringList[index + 1]);
            }

            //Build Information
            //index = stringList.IndexOf("PartNumber");
            //if (index >= 0)
            //    Patient.BuildInformation.PartNumber = stringList[index + 1];

            index = stringList.IndexOf("Serial Number");
            if (index >= 0)
                Patient.BuildInformation.SerialNumber = stringList[index + 1];

            //index = stringList.IndexOf("SoftwareBuild");
            //if (index >= 0)
            //    PatientResult.BuildInformation.SWVersion = stringList[index + 1];

            //index = stringList.IndexOf("HardwareBuild");
            //if (index >= 0)
            //    PatientResult.BuildInformation.HWRelease = stringList[index + 1];

            //index = stringList.IndexOf("SoftwareVersion");
            //if (index >= 0)
            //    PatientResult.BuildInformation.SWVersion = stringList[index + 1];

            //index = stringList.IndexOf("HardwareRelease");
            //if (index >= 0)
            //    PatientResult.BuildInformation.HWRelease = stringList[index + 1];

            //Sample Information
            index = stringList.IndexOf("Sample Detect Time");
            if (index >= 0)
            {
                var detectTime = new DateTime();
                detectTime = detectTime.ParseOrDefault(stringList[index + 1]);
                Patient.Sample.DetectTime = new UnixDateTime((uint)(detectTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            }


            //Test Results
            index = stringList.IndexOf("OID");
            if (index >= 0)
                Patient.TestResult.OperatorID = stringList[index + 1];

            //index = stringList.IndexOf("StripIndex");
            //if (index >= 0) Patient.TestResult.TestStripIndex = Patient.TestResult.TestStripIndex.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Strip Lot Number");
            if (index >= 0) Patient.TestResult.TestStripLotNumber = Patient.TestResult.TestStripLotNumber.ParseOrDefault(stringList[index + 1]);

            //index = stringList.IndexOf("StripExpiry");
            //if (index >= 0)
            //{
            //    var stripExpiryDate = new DateTime();
            //    stripExpiryDate = stripExpiryDate.ParseOrDefault(stringList[index + 1]);
            //    Patient.TestResult.TestStripExpiryDate = new UnixDateTime((uint)(stripExpiryDate - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds);
            //}

            index = stringList.IndexOf("Clot Time");
            if (index >= 0)
                Patient.TestResult.CT = Patient.TestResult.CT.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Calibrated Clot Time");
            if (index >= 0)
                Patient.TestResult.CCT = Patient.TestResult.CCT.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("INR");
            if (index >= 0)
                Patient.TestResult.INR = Patient.TestResult.INR.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("Fault Identifier");
            if (index >= 0)
                Patient.FaultIdentifier = Patient.FaultIdentifier.ParseOrDefault(stringList[index + 1]);
            //Vial Information

            //Transient Information
            var transientDetials = Patient.Transient;
            index = stringList.IndexOf("Peak Current");
            if (index >= 0)
                transientDetials.PeakCurrent = transientDetials.PeakCurrent.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("C12");
            if (index >= 0)
                transientDetials.Current1Point2MicroampRise = transientDetials.Current1Point2MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("C13");
            if (index >= 0)
                transientDetials.Current1Point3MicroampRise = transientDetials.Current1Point3MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("T12");
            if (index >= 0)
                transientDetials.Time1Point2MicroampRise = transientDetials.Time1Point2MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("T13");
            if (index >= 0)
                transientDetials.Time1Point3MicroampRise = transientDetials.Time1Point3MicroampRise.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("TBase");
            if (index >= 0)
                transientDetials.Minimum_Time = transientDetials.Minimum_Time.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("IBase");
            if (index >= 0)
                transientDetials.Minimum_Current = transientDetials.Minimum_Current.ParseOrDefault(stringList[index + 1]);

            index = stringList.IndexOf("OBC");
            if (index >= 0)
                transientDetials.OBCValue = transientDetials.OBCValue.ParseOrDefault(stringList[index + 1]);

            //Specific Data
            var specificData = Patient.SpecificData as SpecificPatientTRData;
            index = stringList.IndexOf("PID");
            if (index >= 0)
                specificData.PID = stringList[index + 1];

            index = stringList.IndexOf("Result Accession");
            if (index >= 0)
                specificData.AccessionNumber = specificData.AccessionNumber.ParseOrDefault(stringList[index + 1]);

            Patient.ValidData = EValidResult.ValidRecord;
            ValidData = EValidResult.ValidRecord;
            Version = 0;
            IsValid = true;
            return IsValid;

        }
    }
}
