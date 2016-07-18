namespace QBR.Infrastructure.Models.Enums
{
    public enum AnalyzerFailure
    {
        NoFailure,
        POSTFailure,
        USBCommsFailure,
        HeaterOutOfRange,
        HeaterTimeout,
        EarlySampleApplication,
        CalibrationFailure,
        EarlyStripRemoval,
        DoubleFill,
        PartialFill,
        TestTimeout,
        TransientExFailure,
    }
}