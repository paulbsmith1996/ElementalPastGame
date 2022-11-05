using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public interface IGameStateHandler
    {
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }
        public void HandleKeysDown(List<Keys> keyCodes);
        public void HandleKeyPressed(char keyChar);

        public void TransitionFromGameState(GameState state);
        public void TransitionToGameState(GameState state);

    }

    public interface IGameStateHandlerDelegate
    {
        public void IGameStateHandlerNeedsRedraw(IGameStateHandler gameStateHandler);
        public void IGameStateHandlerNeedsBitmapUpdateForRenderingModel(IGameStateHandler gameStateHandler, RenderingModel renderingModel);

        public void IGameStateHandlerNeedsGameStateUpdate(IGameStateHandler gameStateHandler, GameState gameState);
    }
}
