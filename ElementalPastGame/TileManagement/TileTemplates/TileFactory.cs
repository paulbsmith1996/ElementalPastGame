using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement.TileTemplates
{
    public static class TileFactory
    {

        public static Tile FenceWithBackground(String BackgroundName)
        {
            List<String> imageNames = new List<string>();
            imageNames.Add(BackgroundName);
            imageNames.Add(TextureMapping.Fence);
            return new Tile(imageNames, true);
        }

        public static Tile TileWithBackground(String BackgroundName)
        {
            List<String> imageNames = new List<string>();
            imageNames.Add(BackgroundName);
            return new Tile(imageNames, false);
        }
    }
}
