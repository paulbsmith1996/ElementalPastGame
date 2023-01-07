using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Audio
{
    public static class AudioPlayer
    {

        public static SoundPlayer soundPlayer = new();
        public static void PlaySoundWithName(String soundName)
        {
            // TODO: Do I need to make a new sound player every time?
            String? soundLocation = AudioMapping.SoundLocationForName(soundName);
            if (soundLocation == null)
            {
                throw new NullReferenceException();
            }
            soundPlayer.SoundLocation = soundLocation;
            soundPlayer.Play();
        }
        public static void Stop()
        {
            soundPlayer.Stop();
        }
        public static void Pause()
        {
            // TODO: provide a real pause function
            soundPlayer.Stop();
        }

        public static void LoopSoundWithName(String soundName)
        {
            // TODO: Do I need to make a new sound player every time?
            String? soundLocation = AudioMapping.SoundLocationForName(soundName);
            if (soundLocation == null)
            {
                throw new NullReferenceException();
            }
            soundPlayer.SoundLocation = soundLocation;
            soundPlayer.PlayLooping();
        }
    }
}
