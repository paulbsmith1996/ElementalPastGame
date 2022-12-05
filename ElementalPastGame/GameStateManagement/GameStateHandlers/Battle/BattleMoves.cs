using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public static class BattleMoves
    {

        public enum PhysicalAttackMove
        {
            Slash,
            Stab,
            Chop,
            Bash,
            Shoot
        };

        // TODO: actually take a weapon type as a parameter here
        public static List<PhysicalAttackMove> GetMovesForWeapon()
        {
            return new List<PhysicalAttackMove>() { PhysicalAttackMove.Slash, PhysicalAttackMove.Stab, PhysicalAttackMove.Chop, PhysicalAttackMove.Bash };
        }
    }
}
