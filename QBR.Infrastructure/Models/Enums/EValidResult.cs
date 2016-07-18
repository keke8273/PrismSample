namespace QBR.Infrastructure.Models.Enums
{
    public enum EValidResult : ushort
    {
        /// <summary>
        /// The no more records
        /// </summary>
        NoMoreRecords = 0,
        /// <summary>
        /// The valid record
        /// </summary>
        ValidRecord = 1,
        /// <summary>
        /// The zeroed record
        /// </summary>
        ZeroedRecord = 2,
        /// <summary>
        /// The invalid
        /// </summary>
        Invalid = 0xFFFF
    }
}