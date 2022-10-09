using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Obstacles;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement;
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
        internal IActiveTileSetManager activeTileSetManager;
        internal PlayerModel PlayerModel;

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
            //this.pictureBoxManager.UpdatePictureBoxForIRenderingModel(gameObjectRenderingModel);
        }

        public void RemoveActiveGameObject(IGameObjectModel gameObject)
        {
            this.gameObjects.Remove(gameObject);
            //this.pictureBoxManager.RemovePictureBoxForIRenderingModel(CreateRenderingModelForGameObject(gameObject));
        }

        public void GameObjectDidUpdate(IGameObjectModel gameObject)
        {
            //this.pictureBoxManager.UpdatePictureBoxForIRenderingModel(CreateRenderingModelForGameObject(gameObject));
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

            // TODO: update this model ID to be something unique for every active object
            renderingModel.ModelID = gameObjectModel.EntityID;

            // TODO: remove this background color setting and set the image instead
            renderingModel.BackgroundColor = Color.Blue;

            return renderingModel;
        }

        // IKeyEventSubscriber
        public void HandlePressedKeys(List<Keys> keyCodes)
        {
            this.UpdateBackgroundWithInput(keyCodes);
            this.UpdateForegroundWithInput(keyCodes);
        }

        internal void UpdateBackgroundWithInput(List<Keys> keyCodes)
        {
            this.activeTileSetManager.HandleKeyInput(keyCodes);
        }

        internal void UpdateForegroundWithInput(List<Keys> keyCodes)
        {
               
        }
    }
}
