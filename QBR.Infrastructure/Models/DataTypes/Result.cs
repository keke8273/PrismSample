using System;
using System.Collections.Generic;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Test Result base class
    /// </summary>
    public class Result : ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public Result(byte[] resultAsArray, int offset)
            : base(resultAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result"/> class.
        /// </summary>
        public Result()
        { }

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
                return 26 + sizeof(UInt16) + (sizeof(UInt32) * 2) + (sizeof(float) * 3) ;
            }
        }

        /// <summary>
        /// Gets or sets the operator identifier.
        /// </summary>
        /// <value>
        /// The operator identifier.
        /// </value>
        public string OperatorID { get; set; }

        /// <summary>
        /// Gets or sets the index of the test strip.
        /// </summary>
        /// <value>
        /// The index of the test strip.
        /// </value>
        public UInt16 TestStripIndex { get; set; }

        /// <summary>
        /// Gets or sets the test strip lot number.
        /// </summary>
        /// <value>
        /// The test strip lot number.
        /// </value>
        public UInt32 TestStripLotNumber { get; set; }

        /// <summary>
        /// Gets or sets the test strip expiry date.
        /// </summary>
        /// <value>
        /// The test strip expiry date.
        /// </value>
        public UnixDateTime TestStripExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the CCT.
        /// </summary>
        /// <value>
        /// The CCT.
        /// </value>
        public float CCT { get; set; }

        /// <summary>
        /// Gets or sets the displayed CCT.
        /// </summary>
        /// <value>
        /// The displayed CCT.
        /// </value>
        public string DisplayedCCT { get; set; }

        /// <summary>
        /// Gets or sets the ct.
        /// </summary>
        /// <value>
        /// The ct.
        /// </value>
        public float CT { get; set; }

        /// <summary>
        /// Gets or sets the INR.
        /// </summary>
        /// <value>
        /// The INR.
        /// </value>
        public float INR { get; set; }

        /// <summary>
        /// Gets or sets the displayed INR.
        /// </summary>
        /// <value>
        /// The displayed INR.
        /// </value>
        public string DisplayedINR { get; set; }

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
                var fieldStart = offset;

                var fieldSize = 16;
                OperatorID = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                OperatorID = OperatorID.Trim(new[] { '\0' });
                fieldStart += fieldSize;

                TestStripIndex = BitConverter.ToUInt16(dataArray, fieldStart);
                fieldStart += sizeof(UInt16);

                TestStripLotNumber = BitConverter.ToUInt32(dataArray, fieldStart);
                fieldStart += sizeof(UInt32);

                TestStripExpiryDate = BitConverter.ToUInt32(dataArray, fieldStart);
                fieldStart += sizeof(UInt32);

                CCT = BitConverter.ToSingle(dataArray, fieldStart);
                fieldStart += sizeof(float);

                fieldSize = 6;
                DisplayedCCT = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                DisplayedCCT = DisplayedCCT.Trim(new[] { '\0' });
                fieldStart += fieldSize;

                CT = BitConverter.ToSingle(dataArray, fieldStart);
                fieldStart += sizeof(float);

                INR = BitConverter.ToSingle(dataArray, fieldStart);
                fieldStart += sizeof(float);

                fieldSize = 4;
                DisplayedINR = Encoding.ASCII.GetString(dataArray, fieldStart, fieldSize);
                DisplayedINR = DisplayedINR.Trim(new[] { '\0' });

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

            data.AddRange(Encoding.ASCII.GetBytes(OperatorID));
            data.AddRange(BitConverter.GetBytes(TestStripIndex));
            data.AddRange(BitConverter.GetBytes(TestStripLotNumber));
            data.AddRange(BitConverter.GetBytes(TestStripExpiryDate));
            data.AddRange(BitConverter.GetBytes(CCT));
            data.AddRange(Encoding.ASCII.GetBytes(DisplayedCCT));
            data.AddRange(BitConverter.GetBytes(CT));
            data.AddRange(BitConverter.GetBytes(INR));
            data.AddRange(Encoding.ASCII.GetBytes(DisplayedINR));

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
        public static bool operator !=(Result x, Result y)
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
        public static bool operator ==(Result x, Result y)
        {
            var areEqual = true;

            areEqual &= x.OperatorID.CompareTo(y.OperatorID.Trim()) == 0;
            areEqual &= x.TestStripIndex == y.TestStripIndex; 
            areEqual &= x.TestStripLotNumber == y.TestStripLotNumber; 
            areEqual &= x.TestStripExpiryDate == y.TestStripExpiryDate;
            areEqual &= x.CCT == y.CCT; 
            areEqual &= x.DisplayedCCT.CompareTo(y.DisplayedCCT.Trim()) == 0; 
            areEqual &= x.CT == y.CT; 
            areEqual &= x.INR == y.INR; 
            areEqual &= x.DisplayedINR.CompareTo(y.DisplayedINR.Trim()) == 0; 

            return areEqual;
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            OperatorID = "FFFFFFFFFFFFFFFF";

            TestStripIndex = 0xFFFF;
            TestStripLotNumber = 0xFFFFFFFF;
            TestStripExpiryDate = 0x00000000;
            CCT = 0xFFFFFFFF;
            DisplayedCCT = "FFFFFF";
            CT = 0xFFFFFFFF;
            INR = 0xFFFFFFFF;
            DisplayedINR = "FFFF";
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
            var areEqual = false;
            if (obj.GetType() == typeof(Result))
            {
                areEqual = this == (obj as Result);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }
    }
}
