
// Software Copyright (c) 2013 by Hydrix Pty. Ltd.
//
// This material is protected by copyright law. It is unlawful
// to copy it.
//
// This document contains confidential information. It is not to be
// disclosed or used except in accordance with applicable contracts
// or agreements.

using System;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Helper class providing static methods for converting from a .NET DateTime object to
    /// the ISO c time_t 32bit int value and back.
    /// </summary>
    public class UnixDateTime: ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnixDateTime"/> class.
        /// </summary>
        public UnixDateTime()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixDateTime"/> class.
        /// </summary>
        /// <param name="sec">The sec.</param>
        public UnixDateTime(UInt32 sec)
        {
            _unixTime = sec;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixDateTime"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        public UnixDateTime(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// The _unix time
        /// </summary>
        private UInt32 _unixTime;

        /// <summary>
        /// The unix epoch
        /// </summary>
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// The unix time minimum
        /// </summary>
        public static DateTime UnixTimeMin = UnixEpoch;

        /// <summary>
        /// The unix time maximum
        /// </summary>
        public static DateTime UnixTimeMax = UnixEpoch.AddSeconds(Convert.ToDouble(Int32.MaxValue - 1));

        /// <summary>
        /// Gets or sets the second since epoch.
        /// </summary>
        /// <value>
        /// The second since epoch.
        /// </value>
        public UInt32 SecondSinceEpoch
        {
            get
            {
                return _unixTime;
            }
            set
            {
                _unixTime = value;
            }
        }

        /// <summary>
        /// Method to convert from Unix time (seconds since 1/1/1970 12.00.00am)
        /// </summary>
        /// <param name="sec">Seconds since Unix time</param>
        /// <returns>
        /// A Datetime object that represents the utc date time define in the sec param
        /// </returns>
        public static DateTime FromUnixTime(UInt32 sec)
        {
            return UnixEpoch.AddSeconds(sec);
        }

        /// <summary>
        /// Method to cover from a .NET DateTime to Unix time (seconds since 1/1/1970 12.00.00am)
        /// </summary>
        /// <param name="dt">the date time to be converted</param>
        /// <returns>
        /// The number of seconds since Unix time represented be dt
        /// </returns>
        public static UInt32 ToUnixTime(DateTime dt)
        {
            TimeSpan diff;
            
            if(dt < UnixTimeMin)
            {
                diff = UnixTimeMin - UnixEpoch; //should have total 0 seconds
            }
            else if (dt > UnixTimeMax)
            {
                diff = UnixTimeMax - UnixTimeMin;
            }
            else
            {
               diff = dt - UnixEpoch;
            }

            return Convert.ToUInt32(Math.Floor(diff.TotalSeconds));
        }

        #region Implicit Conversions

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt32"/> to <see cref="UnixDateTime"/>.
        /// </summary>
        /// <param name="secs">The secs.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UnixDateTime(UInt32 secs)
        {
            return new UnixDateTime(secs);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnixDateTime"/> to <see cref="UInt32"/>.
        /// </summary>
        /// <param name="udt">The udt.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator UInt32(UnixDateTime udt)
        {
            return udt._unixTime;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UnixDateTime"/> to <see cref="DateTime"/>.
        /// </summary>
        /// <param name="udt">The udt.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator DateTime(UnixDateTime udt)
        {
            return FromUnixTime(udt._unixTime);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(UnixDateTime x, UnixDateTime y)
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
        public static bool operator == (UnixDateTime x, UnixDateTime y)
        {
            return x._unixTime == y._unixTime;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append(((DateTime)this).ToString("yyyy-MMM-dd HH:mm:ss"));

            return builder.ToString();
        }

        /// <summary>
        /// To the name of the file.
        /// </summary>
        /// <returns></returns>
        public string ToFileName()
        {
            return ((DateTime) this).ToString("yyyy_MMM_dd_HH_mm_ss");
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var areEqual = true;
            if (obj.GetType() == typeof(UnixDateTime))
            {
                areEqual = this == (obj as UnixDateTime);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        protected bool Equals(UnixDateTime other)
        {
            return _unixTime == other._unixTime;
        }

        public override int GetHashCode()
        {
            var hash = 3;
            hash = hash * 7 + SecondSinceEpoch.GetHashCode();
            return hash;
        }

        #endregion Implicit Conversions

        #region ABaseCommsData Implementation

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public override int DataSize
        {
            get { return sizeof(UInt32); }
        }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public override bool FromArray(byte[] dataArray, int offset)
        {
            IsValid = false;

            if (dataArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                _unixTime = BitConverter.ToUInt32(dataArray, fieldPosition);

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
            return BitConverter.GetBytes(_unixTime);
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            _unixTime = 0x00000000;
        }

        #endregion ABaseCommsData Implementation
    }
}
