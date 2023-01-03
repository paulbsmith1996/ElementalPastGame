using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject;
using ElementalPastGame.GameObject.EntityManagement;
using ElementalPastGame.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.SpacesManagement.TileManagement.TileMapManagers;
using ElementalPastGame.Rendering;

namespace ElementalPastGame.SpacesManagement.Spaces
{
    public abstract class Space : ISpace
    {

        public int initialX { get; set; }
        public int initialY { get; set; }

        internal bool padded;

        internal IActiveEntityManager activeEntityManager;
        internal ITileMapManager tileMapManager;
        internal IActiveTileSetManager tileSetManager;

        public Space(int spaceWidth, int spaceHeight, int initialX, int initialY, bool padded=false)
        {
            this.initialX = initialX;
            this.initialY = initialY;
            this.activeEntityManager = new ActiveEntityManager();
            this.tileMapManager = new TileMapManager(spaceWidth, spaceHeight);
            this.SetUpSpace();
            this.tileSetManager = new ActiveTileSetManager(PictureBoxManager.GetInstance(), tileMapManager, initialX, initialY, spaceWidth, spaceHeight);
            this.padded = padded;
        }

        public bool isPadded()
        {
            return this.padded;
        }

        // Override to set up custom space
        internal abstract void SetUpSpace();

        public bool LocationIsNavigable(int x, int y)
        {
            ITile newTile = this.tileMapManager.TileAt(x, y);
            if (newTile == null)
            {
                return true;
            }

            bool colliableEntityPresent = this.activeEntityManager.IsCollidableEntityPresent(x, y);

            return !newTile.isCollidable && !colliableEntityPresent;
        }

        public List<IGameObjectModel> GetActiveEntities(int CenterX, int CenterY)
        {
            return this.activeEntityManager.GetActiveEntities(CenterX, CenterY);
        }

        public void RegisterGameObject(IGameObjectModel gameObjectModel, List<EntityBattleModel> encounterEnemies)
        {
            this.activeEntityManager.RegisterGameObject(gameObjectModel, encounterEnemies);
        }
        public void MoveGameObject(IGameObjectModel gameObjectModel, Location fromLocation, Location toLocation)
        {
            this.activeEntityManager.MoveGameObject(gameObjectModel, fromLocation, toLocation);
        }

        public void MarkEntityIDDead(long entityID)
        {
            this.activeEntityManager.MarkEntityIDDead(entityID);
        }

        public List<EntityBattleModel> enemiesForEncounterID(long encounterID)
        {
            return this.activeEntityManager.enemiesForEncounterID(encounterID);
        }

        public ITile GetTileAt(int x, int y)
        {
            return this.tileMapManager.TileAt(x, y);
        }

        public void UpdateActiveTileSet(int PreviousCenterX, int PreviousCenterY, int CenterX, int CenterY, bool isAnimating, double offset)
        {
            this.tileSetManager.Update(PreviousCenterX, PreviousCenterY, CenterX, CenterY, isAnimating, offset);
        }

        public void SetTileAtLocation(ITile Tile, int X, int Y)
        {
            this.tileMapManager.SetTileAtLocation(Tile, X, Y);
        }

        public void SetChunkToTile(ITile tile, int leftX, int topY, int rightX, int bottomY)
        {
            this.tileMapManager.SetChunkToTile(tile, leftX, topY, rightX, bottomY);
        }

        public void SetImageOnTiles(String foregroundImageName, String backgroundImageName, int x, int y, int width, int height, bool isCollidable)
        {
            this.tileMapManager.SetImageOnTiles(foregroundImageName, backgroundImageName, x, y, width, height, isCollidable);
        }

        public void SetChunkToImage(String foregroundImageName, String backgroundImageName,
                                      int imageTileWidth, int imageTileHeight,
                                      int rightX, int bottomY,
                                      int leftX, int topY,
                                      bool isCollidable)
        {
            this.tileMapManager.SetChunkToImage(foregroundImageName, backgroundImageName, imageTileWidth, imageTileHeight, rightX, bottomY, leftX, topY, isCollidable);
        }

        public void UnloadSpace()
        {
            this.activeEntityManager.Unload();
            this.tileMapManager.Unload();
        }
    }
}
