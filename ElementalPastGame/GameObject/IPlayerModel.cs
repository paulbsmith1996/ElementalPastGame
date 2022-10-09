using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public interface IPlayerModel : IGameObjectModel
    {
        public void HandlePressedKeys(List<Keys> keyCodes);

    }
}
