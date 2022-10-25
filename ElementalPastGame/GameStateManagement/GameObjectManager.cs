using ElementalPastGame.Common;
using ElementalPastGame.GameObject.GameStateHandlers;
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
using static ElementalPastGame.GameStateManagement.IGameObjectManager;

namespace ElementalPastGame.GameStateManagement
{
    public class GameObjectManager : IGameObjectManager, IKeyEventSubscriber, IGameStateHandlerDelegate
    {
        internal static GameObjectManager? _instance;
        internal IPictureBoxManager pictureBoxManager;

        internal IGameStateHandler overworldGameStateHandler = OverworldGameStateHandler.getInstance();

        internal GameState gameState;

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
            overworldGameStateHandler.gameStateHandlerDelegate = this;
            gameState = GameState.Overworld;

            this.pictureBoxManager = pictureBoxManager;

            IKeyEventPublisher keyEventPublisher = KeyEventPublisher.GetInstance();
            keyEventPublisher.AddIKeyEventSubscriber(this);

            // TODO: probably remove this line later
            this.pictureBoxManager.Redraw();
        }

        // IKeyEventSubscriber
        public void HandlePressedKeys(List<Keys> keyCodes)
        {
            switch (gameState)
            {
                case GameState.Overworld:
                    overworldGameStateHandler.HandleKeyInputs(keyCodes);
                    break;
                case GameState.Battle:
                    break;
            }
        }

        public void IGameStateHandlerNeedsRedraw(IGameStateHandler gameStateHandler)
        {
            pictureBoxManager.Redraw();
        }

        public void IGameStateHandlerNeedsBitmapUpdateForRenderingModel(IGameStateHandler gameStateHandler, RenderingModel renderingModel)
        {
            pictureBoxManager.UpdateBitmapForIRenderingModel(renderingModel);
        }

        public void IGameStateHandlerNeedsGameStateUpdate(IGameStateHandler gameStateHandler, GameState gameState)
        {
            gameStateHandler.gameStateHandlerDelegate = null;
            this.gameState = gameState;
            switch (gameState)
            {
                case GameState.Overworld:
                    overworldGameStateHandler.gameStateHandlerDelegate = this;
                    break;
                case GameState.Battle:
                    // TODO: add this as delegate after creating the battle state handler
                    break;
            }
        }
    }
}
