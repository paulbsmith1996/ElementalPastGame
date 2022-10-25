using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public class BattleGameStateHandler : IGameStateHandler
    {
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        public void HandleKeyInputs(List<Keys> keyCodes)
        {
            throw new NotImplementedException();
        }
    }
}
