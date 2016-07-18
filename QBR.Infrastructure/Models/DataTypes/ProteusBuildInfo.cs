using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Proteus Build Information message
    /// </summary>
    [DataContract]
    public class ProteusBuildInfo : BuildInfo
    {
        #region Member Variables
        /// <summary>
        /// The size of information array
        /// </summary>
        public const int SizeOfInfoArray = 54;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteusBuildInfo"/> class.
        /// </summary>
        public ProteusBuildInfo()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteusBuildInfo"/> class.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        public ProteusBuildInfo(byte[] dataArray, int offset)
            : base(dataArray, offset)
        { } 
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
            get { return SizeOfInfoArray; }
        }
        #endregion

        #region Functions


        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="infoAsArray">The information as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] infoAsArray, int offset)
        {
            if (infoAsArray.Length >= (SizeOfInfoArray + offset))
            {
                var fieldPosition = offset;

                Version = BitConverter.ToUInt16(infoAsArray, fieldPosition);
                fieldPosition += sizeof(UInt16);

                ManufacturingDate = BitConverter.ToUInt32(infoAsArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                var fieldSize = 8;
                PartNumber = Encoding.ASCII.GetString(infoAsArray, fieldPosition, fieldSize);
                PartNumber = PartNumber.Trim(new[] { '\0' });
                fieldPosition += fieldSize;

                fieldSize = 16;
                SerialNumber = Encoding.ASCII.GetString(infoAsArray, fieldPosition, fieldSize);
                SerialNumber = SerialNumber.Trim();
                fieldPosition += fieldSize;

                fieldSize = 8;
                HWRelease = Encoding.ASCII.GetString(infoAsArray, fieldPosition, fieldSize);
                fieldPosition += fieldSize;

                fieldSize = 8;
                Reserved = Encoding.ASCII.GetString(infoAsArray, fieldPosition, fieldSize);
                fieldPosition += fieldSize;

                fieldSize = 8;
                SWVersion = Encoding.ASCII.GetString(infoAsArray, fieldPosition, fieldSize);
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

            data.AddRange(BitConverter.GetBytes(ManufacturingDate));

            var strData = new byte[16];
            Array.Copy(Encoding.ASCII.GetBytes(SerialNumber), strData, SerialNumber.Length);
            data.AddRange(strData);//16

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(PartNumber), strData, PartNumber.Length);
            data.AddRange(strData);//8

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(HWRelease), strData, HWRelease.Length);
            data.AddRange(strData);//8

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(Reserved), strData, Reserved.Length);
            data.AddRange(strData);//8

            strData = new byte[8];
            Array.Copy(Encoding.ASCII.GetBytes(SWVersion), strData, SWVersion.Length);
            data.AddRange(strData);//8

            return data.ToArray();
        }
        #endregion
    }
}
