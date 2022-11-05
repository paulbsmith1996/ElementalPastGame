using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Enemies
{
    public class Goblin : GameObjectModel, IGameObjectModel
    {
        public Goblin(int X, int Y, int level) : base(EntityType.Goblin, level, X, Y)
        {
        }
    }
}
