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

        internal void SetChunkToImage(String foregroundImageName, String backgroundImageName, 
                                      int imageTileWidth, int imageTileHeight, 
                                      int rightX, int bottomY, 
                                      int leftX, int topY, 
                                      bool isCollidable)
        {
            for(int xIndex = rightX; xIndex <= leftX; xIndex += imageTileWidth)
            {
                for (int yIndex = bottomY; yIndex <= topY; yIndex += imageTileHeight)
                {
                    SetImageOnTiles(foregroundImageName, backgroundImageName, xIndex, yIndex, imageTileWidth, imageTileHeight, isCollidable);
                }
            }
        }

        // Map setups

        // Route 1: (820, 890) -> (910, 920)
        internal void SetUpStartingRegion()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 700, 700, 999, 999);

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
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass8), 880, 907, 909, 907);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 900, 906, 909, 906, true);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 880, 906, 889, 906, true);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 878, 906, 879, 919, true);
            // Lake 1b
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 857, 897, 868, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass8), 857, 896, 868, 896);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass2, true), 857, 903, 868, 903);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass4, true), 869, 897, 869, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass6, true), 856, 897, 856, 902);
            // Chunk 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 906, 869, 910);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 851, 907, 869, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 906, 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 870, 906);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 851, 907, 869, 909, true);
            // Chunk 1e
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 850, 890, 870, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 890, 870, 890);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 893, 880, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 891, 870, 892);
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 852, 891, 880, 892, true);
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 882, 891, 908, 894, true);
            // Chunk 1c
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 850, 891, 850, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 853, 894, 853, 905);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 853, 906);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 850, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBR), 853, 893);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 850, 891, 853, 906, true);
            // Chunk 1d
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 840, 900, 843, 919, true);
            // Chunk 1f
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 820, 891, 832, 900, true);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 822, 901, 831, 912);
            // Dirt path
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 908, 899, 908, 901);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 873, 902, 907, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 873, 899, 907, 901);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 873, 898, 907, 898);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass1), 908, 902);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass7), 908, 898);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass9), 872, 898);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 876, 903, 876, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 873, 901, 875, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 872, 899, 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt9), 876, 902);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass1), 876, 917);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 846, 917, 875, 917);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 846, 914, 873, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 849, 913, 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt1), 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass3), 845, 917);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt3), 848, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 848, 895, 848, 912);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 846, 895, 847, 914);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 845, 899, 845, 916);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass7), 848, 894);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt7), 845, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 839, 898, 844, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 835, 895, 845, 897);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 835, 894, 847, 894);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass9), 834, 894);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt9), 838, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 838, 899, 838, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 835, 898, 837, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 834, 895, 834, 912);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 834, 914, 836, 914);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 833, 915, 835, 915);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 832, 916, 834, 916);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 819, 915, 832, 917);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 817, 916, 819, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 816, 915, 818, 915);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 815, 914, 817, 914);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 814, 890, 816, 913);
        }

        // First town: (830, 790) -> (730, 890)
        internal void SetUpFirstTown()
        {

            // Fence 1a
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 820, 870, 820, 890);
            // Fence 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 810, 870, 810, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 810, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 820, 890);
        }
    }
}
