using ElementalPastGame.Common;
using ElementalPastGame.TileManagement.TileTemplates;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.TileManagement.TileTemplates.TileFactory;

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
            this.SetUpFirstTown();
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

        //internal void SetBushyTree(String backgroundName, int x, int y)
        //{
        //    Tile[] bushyTreeTiles = TileFactory.BushyTreeWithBackground(backgroundName);
        //    this.SetTileAtLocation(bushyTreeTiles[0], x + 1, y + 1);
        //    this.SetTileAtLocation(bushyTreeTiles[1], x, y + 1);
        //    this.SetTileAtLocation(bushyTreeTiles[2], x + 1, y);
        //    this.SetTileAtLocation(bushyTreeTiles[3], x , y);
        //}

        internal void SetImageOnTiles(String foregroundImageName, String backgroundImageName, int x, int y, int width, int height, bool isCollidable)
        {
            Tile[] tiles = TileFactory.TilesForImage(foregroundImageName, backgroundImageName, width, height, isCollidable);
            for (int xIndex = 0; xIndex < width; xIndex++)
            {
                for (int yIndex = 0; yIndex < height; yIndex++)
                {
                    this.SetTileAtLocation(tiles[yIndex * width + xIndex], x + (width - xIndex - 1), y + (height - yIndex - 1));
                }
            }
        }

        // Map setups

        // Route 1: (820, 890) -> (910, 920)
        internal void SetUpStartingRegion()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 700, 700, 999, 999);

            // Back fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 910, 890, 910, 920);
            // Top fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 810, 920, 910, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 910, 920);
            // Bottom fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 820, 890, 910, 890);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 910, 890);
            // Chunk 1a
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 880, 908, 909, 919);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt_Grass_7), 880, 907, 909, 907);
            // Chunk 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 906, 869, 910);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 851, 907, 869, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 906, 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 870, 906);
            // Chunk 1e
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 850, 890, 870, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 890, 870, 890);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 893, 870, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 891, 870, 892);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 870, 890);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 870, 893);
            this.SetImageOnTiles(TextureMapping.BushyTree, TextureMapping.Grass, 860, 894, 2, 2, true);

            // Chunk 1c
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 850, 891, 853, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 850, 891, 850, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 853, 894, 853, 905);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 853, 906);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 850, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBR), 853, 893);
            // Chunk 1d
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 840, 900, 843, 920);
        }

        // First town: (830, 790) -> (730, 890)
        internal void SetUpFirstTown()
        {

            // Fence 1a
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 820, 870, 820, 890);
            // Fence 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 810, 870, 810, 920);
        }
    }
}
