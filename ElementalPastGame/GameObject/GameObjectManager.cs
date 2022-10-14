using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public class GameObjectManager : IGameObjectManager, IKeyEventSubscriber
    {
        internal static GameObjectManager? _instance;
        internal List<IGameObjectModel> gameObjects = new();
        internal IPictureBoxManager pictureBoxManager;
        internal IActiveTileSetManager activeTileSetManager;
        internal IActiveEntityManager activeEntityManager;

        internal bool _isAnimating;
        public bool isAnimating { get { return _isAnimating; } set { _isAnimating = value; } }
        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public int PreviousCenterX { get; set; }
        public int PreviousCenterY { get; set; }

        public int FramesAnimated { get; set; }

        internal static ITileMapManager TextureMapManager = TileMapManager.GetInstance();

        public static IGameObjectManager getInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new GameObjectManager(PictureBoxManager.GetInstance());
            return _instance;
        }

        internal GameObjectManager(IPictureBoxManager pictureBoxManager)
        {
            this.CenterX = CommonConstants.GAME_START_LOCATION.X;
            this.CenterY = CommonConstants.GAME_START_LOCATION.Y;
            this.PreviousCenterX = this.CenterX;
            this.PreviousCenterY = this.CenterY;

            this.isAnimating = false;
            this.activeTileSetManager = ActiveTileSetManager.GetInstance();

            this.pictureBoxManager = pictureBoxManager;
            this.activeEntityManager = ActiveEntityManager.GetInstance();
            
            IKeyEventPublisher keyEventPublisher = KeyEventPublisher.GetInstance();
            keyEventPublisher.AddIKeyEventSubscriber(this);

            // TODO: probably remove this line later
            this.pictureBoxManager.Redraw();
        }

        public Boolean ValidateNewGameObjectPosition(IGameObjectModel gameObject, Location newLocation)
        {
            foreach (IGameObjectModel activeGameObject in this.gameObjects)
            {
                if (!activeGameObject.IsCollidable)
                {
                    continue;
                }
                if (activeGameObject.Location.Equals(newLocation))
                {
                    return false;
                }
            }

            return true;
        }

        // IKeyEventSubscriber
        public void HandlePressedKeys(List<Keys> keyCodes)
        {
            double offset = 0;
            if (isAnimating)
            {
                offset = 1.0 - ((double)this.FramesAnimated / GameObjectConstants.MOVEMENT_ANIMATION_LENGTH);

                int diffY = CenterY - PreviousCenterY;
                int diffX = CenterX - PreviousCenterX;

                double animationXOffset = diffX * offset;
                double animationYOffset = diffY * offset;
                this.FramesAnimated++;

                if (this.FramesAnimated > GameObjectConstants.MOVEMENT_ANIMATION_LENGTH)
                {
                    this.FramesAnimated = 1;
                    this.isAnimating = false;
                }
                this.UpdateBackgroundWithOffset(offset);
                this.UpdateForegroundWithOffset(animationXOffset, animationYOffset);
                this.pictureBoxManager.Redraw();
                return;
            }

            if (keyCodes.Count > 0)
            {
                Keys lastKey = keyCodes.Last();
                if (this.ValidateProposedNewLocationForKey(lastKey))
                {
                    this.HandleInputKey(keyCodes.Last());
                    this.UpdateBackgroundWithOffset(1.0);
                    this.UpdateForegroundWithOffset(this.CenterX - this.PreviousCenterX, this.CenterY - this.PreviousCenterY);
                }
                else
                {
                    this.UpdateBackgroundWithOffset(0.0);
                    this.UpdateForegroundWithOffset(0.0, 0.0);
                }
            }
            else
            {
                this.UpdateBackgroundWithOffset(0.0);
                this.UpdateForegroundWithOffset(0.0, 0.0);
            }
            this.pictureBoxManager.Redraw();
        }

        internal void HandleInputKey(Keys? key)
        {
            switch (key)
            {
                case Keys.Left:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX++;
                    break;
                case Keys.Right:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX--;
                    break;
                case Keys.Up:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY++;
                    break;
                case Keys.Down:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY--;
                    break;
            }
        }

        internal bool ValidateProposedNewLocationForKey(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    return this.ValidateNewLocation(this.CenterX + 1, this.CenterY);
                case Keys.Right:
                    return this.ValidateNewLocation(this.CenterX - 1, this.CenterY);
                case Keys.Up:
                    return this.ValidateNewLocation(this.CenterX, this.CenterY + 1);
                case Keys.Down:
                    return this.ValidateNewLocation(this.CenterX, this.CenterY - 1);                    
            }

            return true;
        }

        internal void UpdateBackgroundWithOffset(double offset)
        {
            this.activeTileSetManager.Update(this.PreviousCenterX, this.PreviousCenterY, this.CenterX, this.CenterY, this.isAnimating, offset);

            // TODO: this is suppoed to represent the player so def move it out of here
            Bitmap BlankBitmap = new Bitmap(TextureMapping.Mapping[TextureMapping.Blank], CommonConstants.TILE_DIMENSION, CommonConstants.TILE_DIMENSION);
            List<Bitmap> blankBitmapList = new List<Bitmap>();
            blankBitmapList.Add(BlankBitmap);
            RenderingModel playerModel = new()
            {
                X = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Y = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Width = CommonConstants.TILE_DIMENSION,
                Height = CommonConstants.TILE_DIMENSION,
                Bitmaps = blankBitmapList,
            };

            this.pictureBoxManager.UpdateBitmapForIRenderingModel(playerModel);
        }

        internal void UpdateForegroundWithOffset(double animationXOffset, double animationYOffset)
        {
            foreach (IGameObjectModel gameObjectModel in this.activeEntityManager.GetActiveEntities(this.CenterX, this.CenterY))
            {
                gameObjectModel.UpdateModelForNewRunloop();
                RenderingModel renderingModel = this.CreateRenderingModelForGameObject(gameObjectModel, animationXOffset, animationYOffset);
                this.pictureBoxManager.UpdateBitmapForIRenderingModel(renderingModel);
            }
        }

        internal Boolean ValidateNewLocation(int X, int Y)
        {
            ITile newTile = TextureMapManager.TileAt(X, Y);
            if (newTile == null)
            {
                return true;
            }

            return !newTile.isCollidable;
        }

        internal RenderingModel CreateRenderingModelForGameObject(IGameObjectModel gameObjectModel, double animationXOffset, double animationYOffset)
        {

            double gameObjectTileX = (double)(this.CenterX + CommonConstants.TILE_VIEW_DISTANCE - gameObjectModel.Location.X - gameObjectModel.XAnimationOffset - animationXOffset) * CommonConstants.TILE_DIMENSION;
            double gameObjectTileY = (double)(this.CenterY + CommonConstants.TILE_VIEW_DISTANCE - gameObjectModel.Location.Y - gameObjectModel.YAnimationOffset - animationYOffset) * CommonConstants.TILE_DIMENSION;

            List<Bitmap> bitmaps = new();
            if (gameObjectModel.Image != null) {
                Bitmap bitmap = new Bitmap((Bitmap)gameObjectModel.Image, CommonConstants.TILE_DIMENSION, CommonConstants.TILE_DIMENSION);
                bitmaps.Add(bitmap);
            }

            RenderingModel renderingModel = new()
            {
                X = (int)gameObjectTileX,
                Y = (int)gameObjectTileY,
                Width = CommonConstants.TILE_DIMENSION,
                Height = CommonConstants.TILE_DIMENSION,
                Bitmaps = bitmaps
            };

            return renderingModel;
        }
    }
}
