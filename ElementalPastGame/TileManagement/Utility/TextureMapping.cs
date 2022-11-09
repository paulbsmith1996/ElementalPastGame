using ElementalPastGame.Resources.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace ElementalPastGame.TileManagement.Utility
{
    public static class TextureMapping
    {
        internal static String TexturesLocation = TextureDirectoryLocator.Location + "/";

        public static String Dirt = "Dirt";
        public static String Blank = "Blank";
        public static String Grass = "Grass";
        public static String Fence = "Fence";
        public static String Goblin = "Goblin";
        public static String Dead_Goblin = "Dead_Goblin";

        public static Dictionary<String, Bitmap> Mapping = new Dictionary<String, Bitmap>() {
            { Blank, new Bitmap(TexturesLocation + "Blank.png") },
            { Dirt, new Bitmap(TexturesLocation + "Dirt.png") },
            { Fence, new Bitmap(TexturesLocation + "Fence.png") },
            { Goblin, new Bitmap(TexturesLocation + "Goblin.png") },
            { Dead_Goblin, deadGoblin() },
            { Grass, new Bitmap(TexturesLocation + "Grass.png") },
        };

        public static Bitmap deadGoblin()
        {
            Bitmap goblinBitmap = new Bitmap(TexturesLocation + "Goblin.png");
            goblinBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return goblinBitmap;
        }
    }
}
