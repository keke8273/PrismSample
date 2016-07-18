namespace QBR.Infrastructure.Interfaces
{
    //todo:: remove this after the demo is approved.
    public interface IDemoAnalyzer : IAnalyzer
    {
        void InsertStrip();

        void ApplySample();

        void EjectStrip();
    }
}