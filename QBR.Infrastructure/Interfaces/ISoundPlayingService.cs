namespace QBR.Infrastructure.Interfaces
{
    public interface ISoundPlayingService
    {
        void PlayAttentionSound();
        void PlayErrorSound();
        void PlaySuccessSound();
    }
}