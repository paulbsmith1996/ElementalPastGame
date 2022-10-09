﻿using ElementalPastGame.GameObject.Obstacles;
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

        public static Dictionary<String, Bitmap> Mapping = new Dictionary<String, Bitmap>() {
            { Dirt, new Bitmap(TexturesLocation + "Dirt.png") },
            { Grass, new Bitmap(TexturesLocation + "Grass.png") },
            { Blank, new Bitmap(TexturesLocation + "Blank.png") },
            { Fence, new Bitmap(TexturesLocation + "Fence.png")}
        };
    }
}
