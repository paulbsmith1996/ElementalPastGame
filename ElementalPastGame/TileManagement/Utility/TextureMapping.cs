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

        public static String FenceHorizontal = "Fence_Horizontal";
        public static String FenceVertical = "Fence_Vertical";
        public static String FenceCornerTL = "Fence_Corner_TL";
        public static String FenceCornerTR = "Fence_Corner_TR";
        public static String FenceCornerBL = "Fence_Corner_BL";
        public static String FenceCornerBR = "Fence_Corner_BR";

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

            { FenceHorizontal, new Bitmap(TexturesLocation + "fence_horizontal.png") },
            { FenceVertical, new Bitmap(TexturesLocation + "fence_vertical.png") },
            { FenceCornerTL, new Bitmap(TexturesLocation + "fence_corner_tl.png") },
            { FenceCornerTR, fenceCornerTR() },
            { FenceCornerBL, new Bitmap(TexturesLocation + "fence_corner_bl.png") },
            { FenceCornerBR, fenceCornerBR() },

            { Grass, new Bitmap(TexturesLocation + "Grass.png") },
            { Water, new Bitmap(TexturesLocation + "Water.jpg") },

            { Goblin, new Bitmap(TexturesLocation + "Goblin.png") },
            { Dead_Goblin, deadGoblin() },
            { Player, new Bitmap(TexturesLocation + "Kitty.png") },
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

        public static Bitmap fenceCornerTR()
        {
            Bitmap fenceCornerTL = new Bitmap(TexturesLocation + "fence_corner_tl.png");
            fenceCornerTL.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return fenceCornerTL;
        }

        public static Bitmap fenceCornerBR()
        {
            Bitmap fenceCornerBL = new Bitmap(TexturesLocation + "fence_corner_bl.png");
            fenceCornerBL.RotateFlip(RotateFlipType.RotateNoneFlipX);
            return fenceCornerBL;
        }
    }
}
