namespace QBR.Infrastructure.Models.Enums
{
    /// <summary>
    /// Enum defines test types available in all analyzer
    /// </summary>
    public enum TestTypes
    {
        /// <summary>
        /// The unknown test type
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// The Proteus PT/INR LQC test
        /// </summary>
        ProPTLQC = 1,
        /// <summary>
        /// The Proteus PT/INR Blood test
        /// </summary>
        ProPTBlood = 2,
        /// <summary>
        /// The Rubix PT/INR LQC test
        /// </summary>
        RubPTLQC = 3,
        /// <summary>
        /// The Rubix PT/INR Blood test
        /// </summary>
        RubPTBlood = 4,
        /// <summary>
        /// The Rubix ACT LQC test
        /// </summary>
        RubACTLQC = 5,
        /// <summary>
        /// The Rubix ACT Blood test
        /// </summary>
        RubACTBlood = 6,
        /// <summary>
        /// The Rubix APTT LQC test
        /// </summary>
        RubAPTTLQC = 7,
        /// <summary>
        /// The Rubix APTT Blood test
        /// </summary>
        RubAPTTBlood = 8
    }
}
