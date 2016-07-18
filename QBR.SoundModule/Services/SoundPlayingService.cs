using System;
using System.Media;
using System.Windows;
using System.Windows.Resources;
using QBR.Infrastructure.Interfaces;

namespace QBR.SoundModule.Services
{
    public class SoundPlayingService : ISoundPlayingService
    {
        private readonly SoundPlayer _soundPlayer;

        public SoundPlayingService(SoundPlayer soundPlayer)
        {
            _soundPlayer = soundPlayer;
        }

        public void PlayAttentionSound()
        {
            Uri uri = new Uri(@"pack://application:,,,/QBR.SoundModule;component/Resources/Sounds/sound_attention.wav");
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            _soundPlayer.Stream = sri.Stream;
            _soundPlayer.Play();
        }

        public void PlayErrorSound()
        {
            Uri uri = new Uri(@"pack://application:,,,/QBR.SoundModule;component/Resources/Sounds/sound_error.wav");
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            _soundPlayer.Stream = sri.Stream;
            _soundPlayer.Play();
        }

        public void PlaySuccessSound()
        {
            Uri uri = new Uri(@"pack://application:,,,/QBR.SoundModule;component/Resources/Sounds/sound_success.wav");
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            _soundPlayer.Stream = sri.Stream;
            _soundPlayer.Play();
        }
    }
}
