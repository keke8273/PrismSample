using System;
using QBR.Infrastructure.Models.DataTypes;
using QBR.Infrastructure.Models.Enums;

namespace QBR.Infrastructure.Extensions
{
    public static class ProteusTransientExtensions
    {

        public static DateTime GetSampleDetectionTime(this Transient transient)
        {
            var testType = (TestTypes)transient.Result.TestType;
            switch (testType)
            {
                case TestTypes.ProPTLQC:
                    {
                        return (transient.Result as TransientResultLQC).LQC.Sample.DetectTime;
                    }
                case TestTypes.ProPTBlood:
                    {
                        return (transient.Result as TransientResultPatient).Patient.Sample.DetectTime;
                    }
                default:
                    return default(DateTime);
            }
        }

        /// <summary>
        /// Gets the serial number from transient.
        /// </summary>
        /// <param name="transient">The transient.</param>
        /// <returns></returns>
        public static string GetSerialNumber(this Transient transient)
        {
            var testType = (TestTypes)transient.Result.TestType;
            switch (testType)
            {
                case TestTypes.ProPTLQC:
                    {
                        return (transient.Result as TransientResultLQC).LQC.BuildInformation.SerialNumber;
                    }
                case TestTypes.ProPTBlood:
                    {
                        return (transient.Result as TransientResultPatient).Patient.BuildInformation.SerialNumber;
                    }
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the status meta of a transient file.
        /// </summary>
        /// <param name="transient">The transient.</param>
        /// <returns></returns>
        public static TransientMetaData GetMetaData(this Transient transient)
        {
            return new TransientMetaData()
            {
                Serial = "PRO-" + transient.GetSerialNumber().Trim(),
                Accession = transient.Result.AccessionNumber,
                TestType = (TestTypes) transient.Result.TestType,
                SampleDetectionTime = transient.GetSampleDetectionTime(),
                OID = string.Empty,
                PID = string.Empty,
                Md5 = string.Empty
            };
        }
    }
}
