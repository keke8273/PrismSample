namespace QBR.Infrastructure.Models.Enums
{
    public enum AnalyzerStatus
    {
        Unknown,
        Disconnected,
        Initializing,
        Idle,
        AwaitStripInsertion,
        Heating,
        AwaitSampleApplication,
        TestInProgress,
        SendingTransient,
        AwaitStripEjection,
        TestCompleted,
        Failed
    }
}