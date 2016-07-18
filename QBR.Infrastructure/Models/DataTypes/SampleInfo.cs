using System;
using System.Collections.Generic;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Sample Information
    /// </summary>
    public class SampleInfo: ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SampleInfo"/> class.
        /// </summary>
        /// <param name="sampleInfoAsArray">The sample information as array.</param>
        /// <param name="offset">The offset.</param>
        public SampleInfo(byte[] sampleInfoAsArray,int offset)
            :base(sampleInfoAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleInfo"/> class.
        /// </summary>
        public SampleInfo()
        { }

        /// <summary>
        /// Gets or sets the detect time.
        /// </summary>
        /// <value>
        /// The detect time.
        /// </value>
        public UnixDateTime DetectTime { get; set; }

        /// <summary>
        /// Gets or sets the type flag.
        /// </summary>
        /// <value>
        /// The type flag.
        /// </value>
        public UInt16 TypeFlag { get; set; }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return sizeof(UInt16) + sizeof(UInt32); }
        }

        /// <summary>
        /// Initializes the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            DetectTime = 0xFFFFFFFF;
            TypeFlag = 0xFFFF;
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="sampleInfoAsArray">The sample information as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] sampleInfoAsArray, int offset)
        {
            if ((sampleInfoAsArray.Length - offset) >= (sizeof(UInt32) + sizeof(UInt16)))
            {
                DetectTime = BitConverter.ToUInt32(sampleInfoAsArray, offset);

                offset += sizeof(UInt32);
                TypeFlag = BitConverter.ToUInt16(sampleInfoAsArray, offset);

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

            data.AddRange(BitConverter.GetBytes(DetectTime));
            data.AddRange(BitConverter.GetBytes(TypeFlag));

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
        public static bool operator !=(SampleInfo x, SampleInfo y)
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
        public static bool operator ==(SampleInfo x, SampleInfo y)
        {
            var areEqual = true;

            areEqual &= x.DetectTime == y.DetectTime;
            areEqual &= x.TypeFlag == y.TypeFlag;

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

            if (obj.GetType() == typeof(SampleInfo))
            {
                areEqual = this == (obj as SampleInfo);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }
    }
}
