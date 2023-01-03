using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.SpacesManagement.Spaces;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;

namespace ElementalPastGame.GameObject.Enemies
{
    public class Goblin : GameObjectModel, IGameObjectModel
    {
        public Goblin(ISpace space, int X, int Y, MovementType movementType) : base(EntityType.Goblin, space, X, Y, movementType, false, true, false)
        {
        }
    }
}
