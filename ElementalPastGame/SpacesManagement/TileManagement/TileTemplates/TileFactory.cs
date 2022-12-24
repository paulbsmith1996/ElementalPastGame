using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement.TileTemplates
{
    public static class TileFactory
    {
        public enum TileOrientation
        {
            Horizontal = 0,
            Vertical = 1,
            CornerTL = 2,
            CornerTR = 3,
            CornerBL = 4,
            CornerBR = 5,
        }
        public static Tile FenceWithBackground(String BackgroundName, TileOrientation orientation)
        {
            List<String> imageNames = new List<string>();
            imageNames.Add(BackgroundName);
            String fenceTextureName = "";
            switch(orientation)
            {
                case TileOrientation.Horizontal:
                    fenceTextureName = TextureMapping.FenceHorizontal;
                    break;
                case TileOrientation.Vertical:
                    fenceTextureName = TextureMapping.FenceVertical;
                    break;
                case TileOrientation.CornerTL:
                    fenceTextureName = TextureMapping.FenceCornerTL;
                    break;
                case TileOrientation.CornerTR:
                    fenceTextureName = TextureMapping.FenceCornerTR;
                    break;
                case TileOrientation.CornerBL:
                    fenceTextureName = TextureMapping.FenceCornerBL;
                    break;
                case TileOrientation.CornerBR:
                    fenceTextureName = TextureMapping.FenceCornerBR;
                    break;
            }
            imageNames.Add(fenceTextureName);
            return new Tile(imageNames, true);
        }

        public static Tile TileWithBackground(String BackgroundName, bool isCollidable=false)
        {
            List<String> imageNames = new List<string>();
            imageNames.Add(BackgroundName);
            return new Tile(imageNames, isCollidable);
        }

        public static Tile[] TilesForImage(String foregroundImageName, String backgroundImageName, int width, int height, bool isCollidable)
        {
            WriteSubImagesIfNeeded(foregroundImageName, width, height);

            Tile[] tiles = new Tile[width * height];
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                for (int yIndex = 0; yIndex < height; yIndex++)
                {
                    List<String> imageNames = new();
                    imageNames.Add(backgroundImageName);
                    imageNames.Add(foregroundImageName + "_" + xIndex + "_" + yIndex);
                    Tile currentTile = new Tile(imageNames, isCollidable);
                    tiles[yIndex * width + xIndex] = currentTile;
                }
            }

            return tiles;
        }

        internal static void WriteSubImagesIfNeeded(String imageName, int widthInTiles, int heightInTiles)
        {
            Bitmap imageBitmap = TextureMapping.Mapping[imageName];
            if (TextureMapping.Mapping.GetValueOrDefault(imageName + "_0_0") != null)
            {
                return;
            }

            int subimageBitmapWidth = imageBitmap.Width / widthInTiles;
            int subimageBitmapHeight = imageBitmap.Height / heightInTiles;

            for (int xIndex = 0; xIndex < widthInTiles; xIndex++)
            {
                for (int yIndex = 0; yIndex < heightInTiles; yIndex++)
                {
                    Rectangle cloneRectangle = new Rectangle(xIndex * subimageBitmapWidth, yIndex * subimageBitmapHeight, subimageBitmapWidth, subimageBitmapHeight);
                    Bitmap cloneBitmap = imageBitmap.Clone(cloneRectangle, imageBitmap.PixelFormat);
                    TextureMapping.Mapping.Add(imageName + "_" + xIndex + "_" + yIndex, cloneBitmap);
                }
            }
        }

    }
}
