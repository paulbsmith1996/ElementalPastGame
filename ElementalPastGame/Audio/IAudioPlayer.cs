using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Audio
{
    public interface IAudioPlayer
    {
        public void PlaySoundWithName();
        public void Stop();
        public void Pause();

        public void LoopSoundWithName();
    }
}
