using System;
using System.Collections.Generic;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Rubix Transient Chunk Header
    /// </summary>
    public class RubixTransientChunkHeader : ABaseCommsData
    {
        #region Member Variables
        #endregion

        #region Constructors
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
            get { return sizeof (UInt16)*2 + sizeof (UInt32)*2; }
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
        /// Gets or sets the size of the requested.
        /// </summary>
        /// <value>
        /// The size of the requested.
        /// </value>
        public UInt16 RequestedSize { get; set; }

        #endregion

        #region Functions


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
        public override byte[] ToArray()
        {
            var data = new List<byte>(DataSize);

            data.AddRange(BitConverter.GetBytes(ProtocolVersion));
            data.AddRange(BitConverter.GetBytes(AccessionNumber));
            data.AddRange(BitConverter.GetBytes(Offset));
            data.AddRange(BitConverter.GetBytes(RequestedSize));

            return data.ToArray();
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            ProtocolVersion = 0xFFFF;
            AccessionNumber = 0xFFFFFFFF;
            Offset = 0xFFFFFFFF;
            RequestedSize = 0xFFFF;
        }
        #endregion

        #region Enums
        #endregion


    }
}