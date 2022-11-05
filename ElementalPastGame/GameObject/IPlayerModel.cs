using ElementalPastGame.Items.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public interface IPlayerModel : IGameObjectModel
    {
        public static IPlayerModel GetInstance() { throw new NotImplementedException(); }
        public Inventory inventory { get; }

    }
}
