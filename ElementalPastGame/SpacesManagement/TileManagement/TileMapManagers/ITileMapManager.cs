using ElementalPastGame.TileManagement;

namespace ElementalPastGame.SpacesManagement.TileManagement.TileMapManagers
{
    public interface ITileMapManager
    {
        public static ITileMapManager GetInstance() => throw new NotImplementedException();

        public ITile TileAt(int X, int Y);

        public void LoadTileChunk(int leftX, int topY, int rightX, int bottomY);
        public void UnloadTileChunk(int leftX, int topY, int rightX, int bottomY);

        public void MarkTileChunkCentral(int leftX, int topY, int rightX, int bottomY);

        public void SetTileAtLocation(ITile Tile, int X, int Y);

        public void SetChunkToTile(ITile tile, int leftX, int topY, int rightX, int bottomY);

        public void SetImageOnTiles(String foregroundImageName, String backgroundImageName, int x, int y, int width, int height, bool isCollidable);

        public void SetChunkToImage(String foregroundImageName, String backgroundImageName,
                                      int imageTileWidth, int imageTileHeight,
                                      int rightX, int bottomY,
                                      int leftX, int topY,
                                      bool isCollidable);
        public void Unload();
    }
}
