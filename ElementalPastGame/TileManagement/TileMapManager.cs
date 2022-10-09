using ElementalPastGame.Common;
using ElementalPastGame.TileManagement.TileTemplates;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public class TileMapManager : ITileMapManager
    {
        internal static ITileMapManager? _instance;

        internal ITile[,] TileArray = new ITile[CommonConstants.MAX_MAP_TILE_DIMENSION, CommonConstants.MAX_MAP_TILE_DIMENSION];

        public static ITileMapManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new TileMapManager();
            return _instance;
        }

        public TileMapManager()
        {
            this.SetUpStartingRegion();
        }

        public void LoadTileChunk(int leftX, int topY, int rightX, int bottomY)
        {
            for (int YIndex = topY; YIndex <= bottomY; YIndex++)
            {
                for (int XIndex = leftX; XIndex <= rightX; XIndex++)
                {
                    ITile tile = TileArray[XIndex, YIndex];
                    if (tile != null) {
                        tile.Load();
                        // Central is a higher load status than loaded, so we don't downgrade on a load
                        if (tile.TileLoadState != TileLoadState.Central) {
                            tile.TileLoadState = TileLoadState.Loaded;
                        }
                    }
                }
            }   
        }

        public void UnloadTileChunk(int leftX, int topY, int rightX, int bottomY)
        {
            for (int YIndex = topY; YIndex <= bottomY; YIndex++)
            {
                for (int XIndex = leftX; XIndex <= rightX; XIndex++)
                {
                    ITile tile = TileArray[XIndex, YIndex];
                    if (tile != null) {
                        tile.Unload();
                        tile.TileLoadState = TileLoadState.Unloaded;
                    }
                }
            }
        }

        public ITile TileAt(int X, int Y)
        {
            // TODO: remove debug code
            ITile tile = TileArray[X, Y];
            return TileArray[X, Y];
        }

        public void MarkTileChunkCentral(int leftX, int topY, int rightX, int bottomY)
        {
            for (int YIndex = topY; YIndex <= bottomY; YIndex++)
            {
                for (int XIndex = leftX; XIndex <= rightX; XIndex++)
                {
                    ITile tile = TileArray[XIndex, YIndex];
                    if (tile.TileLoadState == TileLoadState.Unloaded)
                    {
                        tile.Load();
                    }
                    tile.TileLoadState = TileLoadState.Central;
                }
            }
        }

        internal void SetTileAtLocation(ITile Tile, int X, int Y)
        {
            this.TileArray[X, Y] = Tile;
            Tile.TileModelID = "Tile " + X + " " + Y;
        }

        internal void SetChunkToTile(ITile tile, int leftX, int topY, int rightX, int bottomY)
        {
            for (int tileXIndex = leftX; tileXIndex <= rightX; tileXIndex++)
            {
                for (int tileYIndex = topY; tileYIndex <= bottomY; tileYIndex++)
                {
                    this.SetTileAtLocation(tile, tileXIndex, tileYIndex);
                }
            }
        }


        // Map setups
        internal void SetUpStartingRegion()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 150, 150, 250, 250);

            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Dirt), 191, 191, 209, 191);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Dirt), 191, 191, 191, 209);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Dirt), 209, 191, 209, 220);

            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Dirt), 180, 220, 209, 220);
        }
    }
}
