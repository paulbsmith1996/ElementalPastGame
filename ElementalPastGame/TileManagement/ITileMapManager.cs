using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public interface ITileMapManager
    {
        public static ITileMapManager GetInstance() => throw new NotImplementedException();

        public ITile TileAt(int X, int Y);

        public void LoadTileChunk(int leftX, int topY, int rightX, int bottomY);
        public void UnloadTileChunk(int leftX, int topY, int rightX, int bottomY);

        public void MarkTileChunkCentral(int leftX, int topY, int rightX, int bottomY);
    }
}
