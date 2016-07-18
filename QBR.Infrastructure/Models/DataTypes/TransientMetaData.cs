using System;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Model that captures the Transient Meta Data used for grouping and searching the transient database
    /// </summary>
    public class TransientMetaData
    {
        #region Member Variables
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial.
        /// </value>
        public string Serial { get; set; }

        /// <summary>
        /// Gets or sets the accession.
        /// </summary>
        /// <value>
        /// The accession.
        /// </value>
        public UInt32 Accession { get; set; }

        /// <summary>
        /// Gets or sets the PID.
        /// </summary>
        /// <value>
        /// The PID.
        /// </value>
        public string PID { get; set; }

        /// <summary>
        /// Gets or sets the OID.
        /// </summary>
        /// <value>
        /// The OID.
        /// </value>
        public string OID { get; set; }

        /// <summary>
        /// Gets or sets the type of the test.
        /// </summary>
        /// <value>
        /// The type of the test.
        /// </value>
        public TestTypes TestType { get; set; }

        /// <summary>
        /// Gets or sets the md5 security code.
        /// </summary>
        /// <value>
        /// The md5 value.
        /// </value>
        public string Md5 { get; set; }

        /// <summary>
        /// Gets or sets the sample detection time.
        /// </summary>
        /// <value>
        /// The sample detection time.
        /// </value>
        public DateTime SampleDetectionTime { get; set; }

        #endregion

        #region Functions
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            bool areEqual;
            if (obj.GetType() == typeof (TransientMetaData))
            {
                areEqual = this == (obj as TransientMetaData);
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
        public static bool operator !=(TransientMetaData x, TransientMetaData y)
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
        public static bool operator ==(TransientMetaData x, TransientMetaData y)
        {
            var areEqual = true;
            areEqual &= x.Serial == y.Serial;
            areEqual &= x.Accession == y.Accession;
            areEqual &= x.OID == y.OID;
            areEqual &= x.PID == y.PID;
            areEqual &= x.Md5 == y.Md5;
            areEqual &= x.TestType == y.TestType;

            return areEqual;
        }

        #endregion
    }
}