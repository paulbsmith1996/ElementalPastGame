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

    }
}
