using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Audio
{
    public static class AudioLoader
    {
        public static SoundPlayer soundLoader = new();
        public static void LoadSounds()
        {
            foreach (String soundLocation in AudioMapping.Mapping.Values)
            {
                soundLoader.SoundLocation = soundLocation;
                // TODO: this should really be async, and there should be a loading screen until all the sounds are loaded.
                // For now, because this game is literally tiny, we will just load the sounds synchronously.
                soundLoader.Load();
            }
        }
    }
}
