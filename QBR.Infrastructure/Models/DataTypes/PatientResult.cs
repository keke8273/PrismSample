using System.Runtime.Serialization;

namespace QBR.Infrastructure.Models.DataTypes
{
    /// <summary>
    /// Patient Result
    /// </summary>
    [DataContract]
    public class PatientResult : ABaseTestResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatientResult"/> class.
        /// </summary>
        /// <param name="resultAsArray">The result as array.</param>
        /// <param name="offset">The offset.</param>
        public PatientResult(byte[] resultAsArray, int offset)
            : base(resultAsArray, offset)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatientResult"/> class.
        /// </summary>
        public PatientResult()
        { }

        /// <summary>
        /// Specifics the data from array.
        /// </summary>
        /// <param name="dataAsArray">The data as array.</param>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected override ABaseCommsData SpecificDataFromArray(byte[] dataAsArray, int offset)
        {
            return new SpecificPatientTRData(dataAsArray, offset);
        }

        /// <summary>
        /// Initialises the invalid.
        /// </summary>
        protected override void InitialiseInvalid()
        {
            base.InitialiseInvalid();
            SpecificData = new SpecificPatientTRData();
        }

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
                var result = base.DataSize;

                result += SpecificPatientTRData.PatientDataSize;

                return result;
            }
        }

    }
}
