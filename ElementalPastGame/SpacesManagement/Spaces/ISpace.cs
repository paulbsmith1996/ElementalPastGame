using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.TileManagement.TileTemplates;
using ElementalPastGame.TileManagement;

namespace ElementalPastGame.SpacesManagement.Spaces
{
    public interface ISpace
    {
        public int initialX { get; set; }
        public int initialY { get; set; }

        public bool isPadded();

        public bool LocationIsNavigable(int x, int y);
        /// <summary>
        /// Returns a list of all the entities that are active in the neighborhood of the player
        /// </summary>
        /// <param name="CenterX">The x coordinate of the player's location</param>
        /// <param name="CenterY">The y corrdinate of the player's location</param>
        /// <returns></returns>
        public List<IGameObjectModel> GetActiveEntities(int CenterX, int CenterY);
        public void RegisterGameObject(IGameObjectModel gameObjectModel, List<EntityBattleModel> encounterEnemies);
        public void MoveGameObject(IGameObjectModel gameObjectModel, Location fromLocation, Location toLocation);

        public void MarkEntityIDDead(long entityID);

        public List<EntityBattleModel> enemiesForEncounterID(long encounterID);

        public ITile GetTileAt(int x, int y);

        public void UpdateActiveTileSet(int PreviousCenterX, int PreviousCenterY, int CenterX, int CenterY, bool isAnimating, double offset);

        public void SetTileAtLocation(ITile Tile, int X, int Y);

        public void SetChunkToTile(ITile tile, int leftX, int topY, int rightX, int bottomY);

        public void SetImageOnTiles(String foregroundImageName, String backgroundImageName, int x, int y, int width, int height, bool isCollidable);

        public void SetChunkToImage(String foregroundImageName, String backgroundImageName,
                                      int imageTileWidth, int imageTileHeight,
                                      int rightX, int bottomY,
                                      int leftX, int topY,
                                      bool isCollidable);
        public void UnloadSpace();
    }
}
