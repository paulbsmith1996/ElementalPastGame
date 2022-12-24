using ElementalPastGame.TileManagement.TileTemplates;
using ElementalPastGame.SpacesManagement.TileManagement.TileMapManagers;
using ElementalPastGame.TileManagement;

namespace ElementalPastGame.SpacesManagement.TileManagement.TileMapManagers
{
    public class TileMapManager : ITileMapManager
    {
        internal static ITileMapManager? _instance;

        internal int mapWidth;
        internal int mapHeight;

        internal ITile[,] TileArray;

        public TileMapManager(int mapWidth, int mapHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;
            this.TileArray = new ITile[mapWidth, mapHeight];
        }

        public void LoadTileChunk(int leftX, int topY, int rightX, int bottomY)
        {
            for (int YIndex = topY; YIndex <= bottomY; YIndex++)
            {
                for (int XIndex = leftX; XIndex <= rightX; XIndex++)
                {
                    ITile tile = TileArray[XIndex, YIndex];
                    if (tile != null)
                    {
                        tile.Load();
                        // Central is a higher load status than loaded, so we don't downgrade on a load
                        if (tile.TileLoadState != TileLoadState.Central)
                        {
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
                    if (tile != null)
                    {
                        tile.Unload();
                        tile.TileLoadState = TileLoadState.Unloaded;
                    }
                }
            }
        }

        public void Unload()
        {
            this.UnloadTileChunk(0, 0, this.mapWidth - 1, this.mapHeight - 1);
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

        public void SetTileAtLocation(ITile Tile, int X, int Y)
        {
            this.TileArray[X, Y] = Tile;
            Tile.TileModelID = "Tile " + X + " " + Y;
        }

        public void SetChunkToTile(ITile tile, int leftX, int topY, int rightX, int bottomY)
        {
            for (int tileXIndex = leftX; tileXIndex <= rightX; tileXIndex++)
            {
                for (int tileYIndex = topY; tileYIndex <= bottomY; tileYIndex++)
                {
                    this.SetTileAtLocation(tile, tileXIndex, tileYIndex);
                }
            }
        }

        public void SetImageOnTiles(String foregroundImageName, String backgroundImageName, int x, int y, int width, int height, bool isCollidable)
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

        public void SetChunkToImage(String foregroundImageName, String backgroundImageName,
                                      int imageTileWidth, int imageTileHeight,
                                      int rightX, int bottomY,
                                      int leftX, int topY,
                                      bool isCollidable)
        {
            for (int xIndex = rightX; xIndex <= leftX; xIndex += imageTileWidth)
            {
                for (int yIndex = bottomY; yIndex <= topY; yIndex += imageTileHeight)
                {
                    SetImageOnTiles(foregroundImageName, backgroundImageName, xIndex, yIndex, imageTileWidth, imageTileHeight, isCollidable);
                }
            }
        }
    }
}
