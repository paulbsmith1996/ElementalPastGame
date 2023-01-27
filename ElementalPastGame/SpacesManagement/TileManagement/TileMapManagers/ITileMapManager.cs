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

        /// <summary>
        /// The entrance coordinates for this function are in the coordinate space of the building image, not the larger map. e.g. to set a door
        /// in the front middle of a 3x3 building, entranceX = 1, entranceY = 0.
        /// </summary>
        /// <param name="foregroundImageName"></param>
        /// <param name="backgroundImageName"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="entranceX"></param>
        /// <param name="entranceY"></param>
        /// <param name="portalSpaceID"></param>
        /// <param name="portalX"></param>
        /// <param name="portalY"></param>
        public void SetEnterableBuildingOnTiles(String foregroundImageName, String backgroundImageName,
                                                int x, int y,
                                                int width, int height,
                                                int entranceX, int entranceY,
                                                String portalSpaceID, int portalX, int portalY);

        public void SetChunkToImage(String foregroundImageName, String backgroundImageName,
                                      int imageTileWidth, int imageTileHeight,
                                      int rightX, int bottomY,
                                      int leftX, int topY,
                                      bool isCollidable);
        public void Unload();
    }
}
