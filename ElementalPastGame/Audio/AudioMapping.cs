using ElementalPastGame.Resources.Sounds;
using ElementalPastGame.Resources.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Audio
{
    public static class AudioMapping
    {
        internal static String SoundsLocation = SoundsDirectoryLocator.Location;

        public static String HEAL_SOUND = "Heal_Sound";
        public static String AMBIENT = "Ambient";

        public static Dictionary<String, String> Mapping = new Dictionary<String, String>()
        {
            { AMBIENT, SoundsLocation + "music.ogg" },
            { HEAL_SOUND, SoundsLocation + "healingsound.ogg" },
        };

        public static String? SoundLocationForName(String name)
        {
            return Mapping.GetValueOrDefault(name);
        }
    }
}
