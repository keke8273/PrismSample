using System;
using System.Runtime.Serialization;
using System.Text;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// A base class container for data transmitted or received across the communication channel
    /// </summary>
    [DataContract]
    public abstract class ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseCommsData" /> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        protected ABaseCommsData(byte[] dataAsArray, int offset)
            : this()
        {
            FromArray(dataAsArray, offset);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseCommsData" /> class.
        /// </summary>
        protected ABaseCommsData()
        {
            IsValid = false;
            InitialiseInvalid();
        }

        /// <summary>
        /// Gets the size of the data.
        /// </summary>
        /// <value>
        /// The size of the data.
        /// </value>
        public abstract int DataSize { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid { get; protected set; }

        /// <summary>
        /// Deserialize object from the array.
        /// </summary>
        /// <param name="dataArray">The data array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public abstract bool FromArray(byte[] dataArray, int offset);

        /// <summary>
        /// To the array.
        /// </summary>
        /// <returns></returns>
        public abstract byte[] ToArray();

        /// <summary>
        /// Initializes the invalid.
        /// </summary>
        protected abstract void InitialiseInvalid();

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
