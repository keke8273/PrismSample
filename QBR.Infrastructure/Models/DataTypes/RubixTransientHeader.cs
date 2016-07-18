using System;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Rubix Transient Header
    /// </summary>
    public class RubixTransientHeader : ABaseRecordCommsData
    {
        #region Member Variables
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RubixTransientHeader"/> class.
        /// </summary>
        public RubixTransientHeader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RubixTransientHeader"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public RubixTransientHeader(byte[] resultAsArray, int offset)
            : base(resultAsArray, offset)
        {
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return sizeof (UInt16)*2 + sizeof (UInt32)*2 + FileInformation.DataSize + 2; }
        }

        /// <summary>
        /// Gets or sets the protocol version.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public UInt16 ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the accession number.
        /// </summary>
        /// <value>
        /// The accession number.
        /// </value>
        public UInt32 AccessionNumber { get; set; }

        /// <summary>
        /// Gets or sets the file information.
        /// </summary>
        /// <value>
        /// The file information.
        /// </value>
        public FileInformation FileInformation { get; set; }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public UnixDateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public Byte TestType { get; set; }

        /// <summary>
        /// Gets or sets the reserved.
        /// </summary>
        /// <value>
        /// The reserved.
        /// </value>
        public Byte Reserved { get; set; }

        #endregion

        #region Functions
        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            var fieldPosition = offset;

            ProtocolVersion = BitConverter.ToUInt16(dataArray, fieldPosition);
            fieldPosition += sizeof(UInt16);

            ValidData = (EValidResult) BitConverter.ToUInt16(dataArray, fieldPosition);
            fieldPosition += sizeof(UInt16);

            AccessionNumber = BitConverter.ToUInt32(dataArray, fieldPosition);
            fieldPosition += sizeof(UInt32);

            FileInformation.FromArray(dataArray, fieldPosition);
            fieldPosition += FileInformation.DataSize;

            DateTime.FromArray(dataArray, fieldPosition);
            fieldPosition += DateTime.DataSize;

            TestType = dataArray[fieldPosition];
            fieldPosition++;

            Reserved = dataArray[fieldPosition];

            IsValid = true;
            return IsValid;
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
            ProtocolVersion = 0xFFFF;
            AccessionNumber = 0xFFFFFFFF;
            TestType = 0xFF;
            Reserved = 0xFF;
            FileInformation = new FileInformation();
            DateTime = new UnixDateTime();
        }
        #endregion

        #region Enums
        #endregion

    }

    /// <summary>
    /// File Information
    /// </summary>
    public class FileInformation : ABaseCommsData
    {
        /// <summary>
        /// The file name length
        /// </summary>
        public const int FileNameLength = 32;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInformation"/> class.
        /// </summary>
        public FileInformation()
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
            get { return sizeof (UInt16)*4 + sizeof (UInt32)*2 + FileNameLength; }
        }

        /// <summary>
        /// Gets or sets the protocol version.
        /// </summary>
        /// <value>
        /// The protocol version.
        /// </value>
        public UInt16 ProtocolVersion { get; set; }

        /// <summary>
        /// Gets or sets the type of the file.
        /// </summary>
        /// <value>
        /// The type of the file.
        /// </value>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the size of the file.
        /// </summary>
        /// <value>
        /// The size of the file.
        /// </value>
        public UInt32 FileSize { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public byte[] FileName { get; set; }

        /// <summary>
        /// Gets or sets the type of the check.
        /// </summary>
        /// <value>
        /// The type of the check.
        /// </value>
        public CheckType CheckType { get; set; }

        /// <summary>
        /// Gets or sets the check value.
        /// </summary>
        /// <value>
        /// The check value.
        /// </value>
        public UInt32 CheckValue { get; set; }

        /// <summary>
        /// Gets or sets the compression method.
        /// </summary>
        /// <value>
        /// The compression method.
        /// </value>
        public CompressionMethod CompressionMethod { get; set; }

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
                var fieldPosition = offset;

                ProtocolVersion = BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                FileType = (FileType)BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                FileSize = BitConverter.ToUInt32(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                Array.Copy(dataArray, fieldPosition, FileName, 0, FileNameLength);
                fieldPosition += FileNameLength;

                CheckType = (CheckType)BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                CheckValue = BitConverter.ToUInt32(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                CompressionMethod = (CompressionMethod) BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof (UInt16);

                IsValid = true;
            }
            return IsValid;
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
            ProtocolVersion = 0xFFFF;
            FileSize = 0xFFFFFFFF;
            FileType = FileType.InvalidFileType;
            CheckType = CheckType.None;
            CheckValue = 0xFFFFFFFF;
            CompressionMethod = CompressionMethod.None;
            FileName = new byte[FileNameLength];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FileType : ushort
    {
        /// <summary>
        /// The invalid file type
        /// </summary>
        InvalidFileType = 0,
        /// <summary>
        /// The test transient
        /// </summary>
        TestTransient = 1,
        /// <summary>
        /// The security certificate
        /// </summary>
        SecurityCertificate = 2,
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CheckType : ushort
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The 16bit CRC
        /// </summary>
        CRC_16 = 1,
        /// <summary>
        /// The 32bit CRC
        /// </summary>
        CRC_32 = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum CompressionMethod : ushort
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,
        /// <summary>
        /// The gzip
        /// </summary>
        Gzip = 1,
    }
}