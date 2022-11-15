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
        public static String Water = "Water";
        public static String Dirt_Grass_1 = "Dirt_Grass_1";
        public static String Dirt_Grass_7 = "Dirt_Grass_7";
        public static String Player = "Player";

        public static String Goblin = "Goblin";
        public static String Dead_Goblin = "Dead_Goblin";

        public static Dictionary<String, Bitmap> Mapping = new Dictionary<String, Bitmap>() {
            { Blank, new Bitmap(TexturesLocation + "Blank.png") },
            { Dirt, new Bitmap(TexturesLocation + "Dirt.jpg") },
            { Dirt_Grass_1, new Bitmap(TexturesLocation + "dirt_grass_v.jpg")},
            { Dirt_Grass_7, dirtGrassDown() },
            { Fence, new Bitmap(TexturesLocation + "Fence.jpg") },
            { Grass, new Bitmap(TexturesLocation + "Grass.jpg") },
            { Water, new Bitmap(TexturesLocation + "Water.jpg") },

            { Goblin, new Bitmap(TexturesLocation + "Goblin.png") },
            { Dead_Goblin, deadGoblin() },
            { Player, new Bitmap(TexturesLocation + "Kitty.jpg") },
        };

        public static Bitmap deadGoblin()
        {
            Bitmap goblinBitmap = new Bitmap(TexturesLocation + "Goblin.png");
            goblinBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
            return goblinBitmap;
        }

        public static Bitmap dirtGrassDown()
        {
            Bitmap dirtGrassUp = new Bitmap(TexturesLocation + "dirt_grass_v.jpg");
            dirtGrassUp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            return dirtGrassUp;
        }
    }
}
