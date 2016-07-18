using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Transient Data
    /// </summary>
    [DataContract]
    public class TransientData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientData"/> class.
        /// </summary>
        public TransientData()
        {
            TransientSize = 0xFFFFFFFF;
            TransientStripCurrents = new float[MaxTransientSize];
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public int DataSize
        {
            get
            {
                var size = sizeof(UInt16) + (sizeof(float) * MaxTransientSize);
                return size;
            }
        }

        /// <summary>
        /// The maximum transient size
        /// </summary>
        public static readonly int MaxTransientSize = 9200;

        /// <summary>
        /// Gets or sets the size of the transient.
        /// </summary>
        /// <value>
        /// The size of the transient.
        /// </value>
        [DataMember]
        public UInt32 TransientSize { get; set; }

        /// <summary>
        /// Gets or sets the transient strip currents.
        /// </summary>
        /// <value>
        /// The transient strip currents.
        /// </value>
        [DataMember]
        public float[] TransientStripCurrents { get; set; }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public bool FromArray(byte[] dataAsArray, int offset)
        {
            var isValid = false;        

            if (dataAsArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                TransientSize = BitConverter.ToUInt32(dataAsArray, fieldPosition);
                fieldPosition += sizeof(UInt32);

                for (var i = 0; i < MaxTransientSize; i++)
                {
                    TransientStripCurrents[i] = BitConverter.ToSingle(dataAsArray, fieldPosition);
                    fieldPosition += sizeof(float);
                }
                isValid = true;
            }
            return isValid;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var data = new List<byte>();

            data.AddRange(BitConverter.GetBytes(TransientSize));
            
            for (var i = 0; i < MaxTransientSize; i++)
            {
                data.AddRange(BitConverter.GetBytes(TransientStripCurrents[i]));
            }
            
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
        public static bool operator !=(TransientData x, TransientData y)
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
        public static bool operator ==(TransientData x, TransientData y)
        {
            var areEqual = true;

            areEqual &= x.TransientSize == y.TransientSize;
            areEqual &= x.TransientStripCurrents == y.TransientStripCurrents;

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

            if (obj.GetType() == typeof(TransientData))
            {
                areEqual = this == (obj as TransientData);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
        }

        public void FromString(string dataAsString)
        {
            var stringList = dataAsString.Split(new[] { '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).ToList();
            float transientCurrent;
            TransientSize = 0;
         
            foreach (var entry in stringList)
            {
                if (entry.Contains(','))
                    continue;

                if (float.TryParse(entry, out transientCurrent) == false)
                {
                    return;
                }

                TransientStripCurrents[TransientSize] = transientCurrent;
                TransientSize++;
            }

        }
    }

    /// <summary>
    /// Transient base class
    /// </summary>
    [DataContract]
    public class Transient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transient"/> class.
        /// </summary>
        public Transient()
        {
            Result = new TransientResult();
            TransientData = new TransientData();
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public int DataSize
        {
            get
            {
                var size = Result.DataSize;
                size += TransientData.DataSize;
                return size;
            }
        }

        /// <summary>
        /// The maximum transient size
        /// </summary>
        public static readonly int MaxTransientSize = 9200;

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        [DataMember(Order = 0)]
        public TransientResult Result { get; set; }

        /// <summary>
        /// Gets or sets the transient data.
        /// </summary>
        /// <value>
        /// The transient data.
        /// </value>
        [DataMember(Order = 1)]
        public TransientData TransientData { get; set; }

        /// <summary>
        /// Froms the array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public bool FromArray(byte[] dataAsArray, int offset)
        {
            var IsValid = false;        

            if (dataAsArray.Length >= (offset + DataSize))
            {
                var fieldPosition = offset;

                Result.FromArray(dataAsArray, fieldPosition);
                fieldPosition += Result.DataSize;

                TransientData.FromArray(dataAsArray, fieldPosition);
                
                IsValid = true;
            }
            return IsValid;
        }

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            var data = new List<byte>();
            
            data.AddRange(Result.ToArray());
            
            data.AddRange(TransientData.ToArray());
            
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
        public static bool operator !=(Transient x, Transient y)
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
        public static bool operator ==(Transient x, Transient y)
        {
            var areEqual = true;

            areEqual &= x.Result == y.Result;
            areEqual &= x.TransientData == y.TransientData;

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

            if (obj.GetType() == typeof(Transient))
            {
                areEqual = this == (obj as Transient);
            }
            else
            {
                areEqual = base.Equals(obj);
            }

            return areEqual;
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

            builder.Append(GetType().Name);
            builder.Append(Environment.NewLine);

            //PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.FlattenHierarchy);
            var props = GetType().GetProperties();
            foreach (var p in props)
            {
                builder.Append(string.Format("\t- {0} : {1}\n", p.Name, p.GetValue(this, null)));
            }

            return builder.ToString();
        }
    }
}
