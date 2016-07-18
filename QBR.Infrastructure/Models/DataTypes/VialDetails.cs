using System;
using System.Collections.Generic;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Vial Details
    /// </summary>
    public class VialDetails : ABaseCommsData
    {
        /// <summary>
        /// The _is k value included
        /// </summary>
        private bool _isKValueIncluded;

        /// <summary>
        /// Initializes a new instance of the <see cref="VialDetails"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public VialDetails(byte[] dataAsArray, int offset)
            :base(dataAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="VialDetails"/> class.
        /// </summary>
        public VialDetails()
        { }

        /// <summary>
        /// Gets or sets the ISI.
        /// </summary>
        /// <value>
        /// The ISI.
        /// </value>
        public float ISI { get; set; }

        /// <summary>
        /// Gets or sets the MNPT.
        /// </summary>
        /// <value>
        /// The MNPT.
        /// </value>
        public float MNPT { get; set; }

        /// <summary>
        /// Gets or sets the OBC limit.
        /// </summary>
        /// <value>
        /// The OBC limit.
        /// </value>
        public float OBCLimit { get; set; }

        /// <summary>
        /// Gets or sets the k value.
        /// </summary>
        /// <value>
        /// The k value.
        /// </value>
        public float KValue { get; set; }

        /// <summary>
        /// Gets or sets the pf limit.
        /// </summary>
        /// <value>
        /// The pf limit.
        /// </value>
        public float PFLimit { get; set; }

        /// <summary>
        /// Gets or sets the time.
        /// </summary>
        /// <value>
        /// The time.
        /// </value>
        public float Time { get; set; }

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
                if (_isKValueIncluded)
                {
                    return 5 * sizeof(float);
                }
                return 4 * sizeof(float);
            }
        }

        /// <summary>
        /// Initializes the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            ISI = 0xFFFFFFFF;
            MNPT = 0xFFFFFFFF;
            OBCLimit = 0xFFFFFFFF;
            PFLimit = 0xFFFFFFFF;
            KValue = 0xFFFFFFFF;
            Time = 0xFFFFFFFF;
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="detailsAsArray">The details as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] detailsAsArray, int offset)
        {
            var fieldStart = offset;

            if (detailsAsArray.Length == 0xCA)
            {
                ISI = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                MNPT = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                OBCLimit = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                KValue = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                PFLimit = BitConverter.ToSingle(detailsAsArray, fieldStart);


                _isKValueIncluded = true;

                IsValid = true;
            }
            //make it compatible with pre-v0.6.3.x transients and post v.0.7.0.0 transients
            else if (detailsAsArray.Length >= (offset + DataSize))
            {
                ISI = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                MNPT = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                OBCLimit = BitConverter.ToSingle(detailsAsArray, fieldStart);
                fieldStart += sizeof(float);

                PFLimit = BitConverter.ToSingle(detailsAsArray, fieldStart);

                _isKValueIncluded = false;

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

            data.AddRange(BitConverter.GetBytes(ISI));
            data.AddRange(BitConverter.GetBytes(MNPT));
            data.AddRange(BitConverter.GetBytes(OBCLimit));
            data.AddRange(BitConverter.GetBytes(KValue));
            data.AddRange(BitConverter.GetBytes(PFLimit));

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
        public static bool operator !=(VialDetails x, VialDetails y)
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
        public static bool operator ==(VialDetails x, VialDetails y)
        {
            var areEqual = true;
            
            areEqual &= x.ISI == y.ISI;
            areEqual &= x.MNPT == y.MNPT;
            areEqual &= x.OBCLimit == y.OBCLimit;
            areEqual &= x.KValue == y.KValue;
            areEqual &= x.PFLimit == y.PFLimit;

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
            var areEqual = false;
            if (obj.GetType() == typeof(VialDetails))
            {
                areEqual = this == (obj as VialDetails);
            }
            else
            {
                areEqual = base.Equals(obj);
            }
            return areEqual;
        }
    }
}
