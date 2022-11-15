using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameStateManagement.GameStateHandlers.Battle.BattleGameStateHandler;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public interface IBattleTextManagerDelegate
    {

        public void BattleTextManagerWantsEscape(BattleTextManager textManager);

        public void BattleTextManagerWantsSwitchToState(BattleTextManager textManager, BattleState state);
    }
}
