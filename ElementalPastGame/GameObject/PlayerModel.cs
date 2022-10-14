using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.KeyInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public class PlayerModel : GameObjectModel, IPlayerModel
    {

        internal int cyclesSinceLastMove = 0;

        public PlayerModel(IGameObjectManager gameObjectManager) : base("", 0, 0)
        {
            GameObjectSize size = new()
            {
                Height = 1,
                Width = 1
            };
            this.Size = size;
            Location initialLocation = new()
            {
                X = 10,
                Y = 10
            };
            this.Location = initialLocation;
            this.IsCollidable = true;

            //gameObjectManager.GameObjectDidUpdate(this);
        }

        public void HandlePressedKeys(List<Keys> keyCodes)
        {
            if (keyCodes.Count == 0)
            {
                return;
            }

            Keys lastKeyDown = keyCodes.Last();

            Location newLocation = new();

            switch (lastKeyDown)
            {
                case Keys.Left:
                    newLocation.X = this.Location.X - 1;
                    newLocation.Y = this.Location.Y;
                    break;
                case Keys.Right:
                    newLocation.X = this.Location.X + 1;
                    newLocation.Y = this.Location.Y;
                    break;
                case Keys.Up:
                    newLocation.X = this.Location.X;
                    newLocation.Y = this.Location.Y - 1;
                    break;
                case Keys.Down:
                    newLocation.X = this.Location.X;
                    newLocation.Y = this.Location.Y + 1;
                    break;
            }

            cyclesSinceLastMove++;

            //if (cyclesSinceLastMove < GameObjectConstants.RUN_CYCLES_PER_MOVEMENT)
            //{
            //    return;
            //}

            cyclesSinceLastMove = 0;

        //    if (!this.gameObjectManager.ValidateNewGameObjectPosition(this, newLocation))
        //    {
        //        return;
        //    }

        //    this.Location = newLocation;
        //    this.gameObjectManager.GameObjectDidUpdate(this);
        }
    }
}
