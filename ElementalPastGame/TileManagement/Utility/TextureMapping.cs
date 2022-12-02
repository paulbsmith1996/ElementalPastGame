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

        public static String BushyTree = "Bushy_Tree";
        public static String PineTree = "Pine_Tree";

        public static String Water = "Water";
        public static String WaterGrass2 = "Water_Grass_2";
        public static String WaterGrass4 = "Water_Grass_4";
        public static String WaterGrass6 = "Water_Grass_6";
        public static String WaterGrass8 = "Water_Grass_8";

        public static String DirtGrass1 = "Dirt_Grass_1";
        public static String DirtGrass2 = "Dirt_Grass_2";
        public static String DirtGrass3 = "Dirt_Grass_3";
        public static String DirtGrass4 = "Dirt_Grass_4";
        public static String DirtGrass6 = "Dirt_Grass_6";
        public static String DirtGrass7 = "Dirt_Grass_7";
        public static String DirtGrass8 = "Dirt_Grass_8";
        public static String DirtGrass9 = "Dirt_Grass_9";

        public static String GrassDirt1 = "Grass_Dirt_1";
        public static String GrassDirt3 = "Grass_Dirt_3";
        public static String GrassDirt7 = "Grass_Dirt_7";
        public static String GrassDirt9 = "Grass_Dirt_9";

        public static String Player = "Player";

        public static String Goblin = "Goblin";
        public static String Dead_Goblin = "Dead_Goblin";

        public static Dictionary<String, Bitmap> Mapping = new Dictionary<String, Bitmap>() {
            { Blank, new Bitmap(TexturesLocation + "Blank.png") },
            { Dirt, new Bitmap(TexturesLocation + "Dirt.jpg") },

            { DirtGrass1, new Bitmap(TexturesLocation + "dirt_grass_1.png") },
            { DirtGrass2, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "dirt_grass_8.png"), RotateFlipType.RotateNoneFlipY) },
            { DirtGrass3, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "dirt_grass_1.png"), RotateFlipType.RotateNoneFlipX) },
            { DirtGrass4, new Bitmap(TexturesLocation + "dirt_grass_4.png") },
            { DirtGrass6, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "dirt_grass_4.png"), RotateFlipType.RotateNoneFlipX) },
            { DirtGrass7, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "dirt_grass_1.png"), RotateFlipType.RotateNoneFlipY) },
            { DirtGrass8, new Bitmap(TexturesLocation + "dirt_grass_8.png") },
            { DirtGrass9, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "dirt_grass_1.png"), RotateFlipType.RotateNoneFlipXY) },

            { GrassDirt1, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "grass_dirt_9.png"), RotateFlipType.RotateNoneFlipXY) },
            { GrassDirt3, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "grass_dirt_9.png"), RotateFlipType.RotateNoneFlipY) },
            { GrassDirt7, BitmapByApplyingRotateFlip(new Bitmap(TexturesLocation + "grass_dirt_9.png"), RotateFlipType.RotateNoneFlipX) },
            { GrassDirt9, new Bitmap(TexturesLocation + "grass_dirt_9.png") },

            { FenceHorizontal, new Bitmap(TexturesLocation + "fence_horizontal.png") },
            { FenceVertical, new Bitmap(TexturesLocation + "fence_vertical.png") },
            { FenceCornerTL, new Bitmap(TexturesLocation + "fence_corner_tl.png") },
            { FenceCornerTR, fenceCornerTR() },
            { FenceCornerBL, new Bitmap(TexturesLocation + "fence_corner_bl.png") },
            { FenceCornerBR, fenceCornerBR() },

            { BushyTree, new Bitmap(TexturesLocation + "bushytree.png") },
            { PineTree, new Bitmap(TexturesLocation + "pinetree.png") },

            { Grass, new Bitmap(TexturesLocation + "Grass.png") },

            { Water, new Bitmap(TexturesLocation + "Water.jpg") },
            { WaterGrass2, waterGrassUp() },
            { WaterGrass4, new Bitmap(TexturesLocation + "water_grass_4.png") },
            { WaterGrass6, new Bitmap(TexturesLocation + "water_grass_6.png") },
            { WaterGrass8, new Bitmap(TexturesLocation + "water_grass_8.png") },

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

        internal static Bitmap BitmapByApplyingRotateFlip(Bitmap bitmap, RotateFlipType rotateFlipType)
        {
            bitmap.RotateFlip(rotateFlipType);
            return bitmap;
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

        public static Bitmap waterGrassUp()
        {
            Bitmap waterGrassDown = new Bitmap(TexturesLocation + "water_grass_8.png");
            waterGrassDown.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return waterGrassDown;
        }
    }
}
