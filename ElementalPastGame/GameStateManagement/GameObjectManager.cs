using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Enemies;
using ElementalPastGame.GameObject.GameStateHandlers;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Items.Inventory;
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
        internal IGameStateHandler currentGameStateHandler;

        internal GameState previousGameState;
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
            this.currentGameStateHandler = this.overworldGameStateHandler;
            this.currentGameStateHandler.gameStateHandlerDelegate = this;
            this.previousGameState = this.gameState = GameState.Overworld;

            this.pictureBoxManager = pictureBoxManager;

            IKeyEventPublisher keyEventPublisher = KeyEventPublisher.GetInstance();
            keyEventPublisher.AddIKeyEventSubscriber(this);

            // TODO: probably remove this line later
            this.pictureBoxManager.Redraw();
        }

        public void HandleKeyPressed(char keyChar)
        {
            this.currentGameStateHandler.HandleKeyPressed(keyChar);
        }

        // IKeyEventSubscriber
        public void HandleKeysDown(List<Keys> keyCodes)
        {
            this.currentGameStateHandler.HandleKeysDown(keyCodes);
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
            this.previousGameState = this.gameState;
            this.gameState = gameState;

            this.currentGameStateHandler.TransitionToGameState(this.gameState);

            switch (gameState)
            {
                case GameState.Overworld:
                    this.currentGameStateHandler = this.overworldGameStateHandler;
                    break;
                case GameState.Battle:
                    this.currentGameStateHandler = new BattleGameStateHandler(Inventory.DebugInventory(), new() { new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel }, 
                                                                                                          new() { new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel, new Goblin(0, 0, 5).dataModel });
                    break;
            }

            this.currentGameStateHandler.gameStateHandlerDelegate = this;

            this.currentGameStateHandler.TransitionFromGameState(this.gameState);

        }
    }
}
