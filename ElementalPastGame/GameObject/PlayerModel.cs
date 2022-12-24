using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.GameStateManagement;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.KeyInput;
using ElementalPastGame.SpacesManagement.Spaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    public class PlayerModel : GameObjectModel, IPlayerModel
    {
        internal static PlayerModel? _instance { get; set; }

        public Inventory inventory { get; set; }
        public static IPlayerModel GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new PlayerModel();
            return _instance;
        }
        public PlayerModel() : base(EntityType.Aendon, Spaces.Overworld, 10, 0, 0)
        {
            inventory = new Inventory();
        }

    }
}
