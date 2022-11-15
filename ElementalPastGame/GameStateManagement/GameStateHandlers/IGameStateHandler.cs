using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public interface IGameStateHandler : IKeyEventSubscriber
    {
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }
        public void TransitionFromGameState(GameState state, Dictionary<String, Object> transitionDictionary);
        public void TransitionToGameState(GameState state, Dictionary<String, Object> transitionDictionary);

    }

    public interface IGameStateHandlerDelegate
    {
        public void IGameStateHandlerNeedsRedraw(IGameStateHandler gameStateHandler);
        public void IGameStateHandlerNeedsBitmapUpdateForRenderingModel(IGameStateHandler gameStateHandler, RenderingModel renderingModel);

        public void IGameStateHandlerNeedsGameStateUpdate(IGameStateHandler gameStateHandler, GameState gameState, Dictionary<String, Object> transitionDictionary);
    }
}
