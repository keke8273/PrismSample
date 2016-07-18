using System.Runtime.Serialization;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// This is a base class for comms data that is a record in a list of records
    /// </summary>
    [DataContract]
    public abstract class ABaseRecordCommsData : ABaseCommsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseRecordCommsData"/> class.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        protected ABaseRecordCommsData(byte[] dataAsArray, int offset)
            : base(dataAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ABaseRecordCommsData"/> class.
        /// </summary>
        protected ABaseRecordCommsData()
        { }

        /// <summary>
        /// This field is used to indicate if there are more records in the list.
        /// </summary>
        /// <remarks>
        /// Make sure that this is the first field in the XML defining subclasses. order is important
        /// </remarks>
        [DataMember]
        [EnumMember]
        public EValidResult ValidData;
    }
}
