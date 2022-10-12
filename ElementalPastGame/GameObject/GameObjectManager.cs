using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Obstacles;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public class GameObjectManager : IGameObjectManager, IKeyEventSubscriber
    {
        internal static GameObjectManager? _instance;
        internal List<IGameObjectModel> gameObjects = new();
        internal IPictureBoxManager pictureBoxManager;
        internal PlayerModel PlayerModel;
        internal IActiveTileSetManager activeTileSetManager;

        internal bool _isAnimating;
        public bool isAnimating { get { return _isAnimating; } set { _isAnimating = value; } }
        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public int PreviousCenterX { get; set; }
        public int PreviousCenterY { get; set; }

        public int FramesAnimated { get; set; }
        internal static int FRAMES_PER_ANIMATION = 4;

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
            this.PlayerModel = new PlayerModel(this);
            this.AddActiveGameObject(this.PlayerModel);

            Location fenceLocation = new()
            {
                X = 15,
                Y = 10
            };
            WoodenFenceHorizontal testFence = new(fenceLocation, "WoodenFence1_1", this);
            this.AddActiveGameObject(testFence);
            
            IKeyEventPublisher keyEventPublisher = KeyEventPublisher.GetInstance();
            keyEventPublisher.AddIKeyEventSubscriber(this);
        }

        public void AddActiveGameObject(IGameObjectModel gameObject)
        {
            this.gameObjects.Add(gameObject);
            RenderingModel gameObjectRenderingModel = CreateRenderingModelForGameObject(gameObject);
        }

        public void RemoveActiveGameObject(IGameObjectModel gameObject)
        {
            this.gameObjects.Remove(gameObject);
        }

        public void GameObjectDidUpdate(IGameObjectModel gameObject)
        {
            
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

        internal static RenderingModel CreateRenderingModelForGameObject(IGameObjectModel gameObjectModel)
        {
            // TODO: cache these rendering models so that we don't recreate a new one on every update
            RenderingModel renderingModel = new();
            renderingModel.X = gameObjectModel.Location.X * CommonConstants.TILE_DIMENSION;
            renderingModel.Y = gameObjectModel.Location.Y * CommonConstants.TILE_DIMENSION;
            renderingModel.Width = gameObjectModel.Size.Width * CommonConstants.TILE_DIMENSION;
            renderingModel.Height = gameObjectModel.Size.Height * CommonConstants.TILE_DIMENSION;

            // TODO: remove this background color setting and set the image instead
            renderingModel.BackgroundColor = Color.Blue;

            return renderingModel;
        }

        // IKeyEventSubscriber
        public void HandlePressedKeys(List<Keys> keyCodes)
        {
            if (this.isAnimating)
            {
                this.FramesAnimated++;
                if (this.FramesAnimated > GameObjectManager.FRAMES_PER_ANIMATION)
                {
                    this.FramesAnimated = 1;
                    this.isAnimating = false;
                }
                this.UpdateBackgroundWithInput(null);
                this.UpdateForegroundWithInput(keyCodes);
                return;
            }

            if (keyCodes.Count == 0)
            {
                return;
            }

            switch (keyCodes.Last())
            {
                case Keys.Left:
                    if (!this.ValidateNewLocation(this.CenterX + 1, this.CenterY))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX++;
                    break;
                case Keys.Right:
                    if (!this.ValidateNewLocation(this.CenterX - 1, this.CenterY))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX--;
                    break;
                case Keys.Up:
                    if (!this.ValidateNewLocation(this.CenterX, this.CenterY + 1))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY++;
                    break;
                case Keys.Down:
                    if (!this.ValidateNewLocation(this.CenterX, this.CenterY - 1))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY--;
                    break;
            }

            DateTime DebugBeginUpdatingBackgroundTime = DateTime.Now;
            this.UpdateBackgroundWithInput(keyCodes.Last());
            DateTime DebugEndUpdatingBackground = DateTime.Now;
            this.UpdateForegroundWithInput(keyCodes);
            double DebugTimeToUpdateBackground = (DebugEndUpdatingBackground - DebugBeginUpdatingBackgroundTime).TotalMilliseconds;
            Debug.WriteLine("Updating background took: " + DebugTimeToUpdateBackground + "ms");
        }

        internal void UpdateBackgroundWithInput(Keys? key)
        {
            this.activeTileSetManager.HandleKeyInput(key);

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

        internal void UpdateForegroundWithInput(List<Keys> keyCodes)
        {

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
    }
}
