using System;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Rubix Transient Chunk
    /// </summary>
    public class RubixTransientChunk : ABaseCommsData
    {
        #region Member Variables
        /// <summary>
        /// The data length
        /// </summary>
        public static int DataLength = 478;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RubixTransientChunk"/> class.
        /// </summary>
        public RubixTransientChunk()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RubixTransientChunk"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public RubixTransientChunk(byte[] resultAsArray, int offset)
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
            get { return sizeof (UInt16)*2 + sizeof (UInt32)*3 + DataLength; }
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
        /// Gets or sets the offset.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public UInt32 Offset { get; set; }

        /// <summary>
        /// Gets or sets the actual size.
        /// </summary>
        /// <value>
        /// The actual size.
        /// </value>
        public UInt16 ActualSize { get; set; }

        /// <summary>
        /// Gets or sets the size of the remaining.
        /// </summary>
        /// <value>
        /// The size of the remaining.
        /// </value>
        public UInt32 RemainingSize { get; set; }

        /// <summary>
        /// Gets or sets the chunk data.
        /// </summary>
        /// <value>
        /// The chunk data.
        /// </value>
        public byte[] ChunkData { get; set; }

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
            if (dataArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                ProtocolVersion = BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                AccessionNumber = BitConverter.ToUInt32(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                Offset = BitConverter.ToUInt32(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                ActualSize = BitConverter.ToUInt16(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                RemainingSize = BitConverter.ToUInt32(dataArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                Array.Copy(dataArray, fieldPosition, ChunkData, 0, DataLength);
                fieldPosition += DataLength;

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
            AccessionNumber = 0xFFFFFFFF;
            Offset = 0xFFFFFFFF;
            ActualSize = 0xFFFF;
            RemainingSize = 0xFFFFFFFF;
            ChunkData = new byte[DataLength];
        }
        #endregion

        #region Enums
        #endregion


    }
}