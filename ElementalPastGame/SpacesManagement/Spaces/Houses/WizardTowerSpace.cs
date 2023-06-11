using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject;
using ElementalPastGame.TileManagement.TileTemplates;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;
using ElementalPastGame.GameObject.NPC;
using ElementalPastGame.Items.Inventory;

namespace ElementalPastGame.SpacesManagement.Spaces.Houses
{
    public class WizardTowerSpace : Space, ISpace
    {
        public WizardTowerSpace() : base(SpaceConstants.HOUSE_WIDTH + 2, SpaceConstants.HOUSE_HEIGHT + 2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y, true)
        {

        }
        internal override void SetUpSpace()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Blank, true), 0, 0, SpaceConstants.HOUSE_WIDTH + 1, SpaceConstants.HOUSE_HEIGHT + 1);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.HouseFloor), 1, 1, SpaceConstants.HOUSE_WIDTH, SpaceConstants.HOUSE_HEIGHT);

            this.SetTileAtLocation(TileFactory.PortalTileWithBackground(TextureMapping.HouseMat, Spaces.OVERWORLD, 835, 870), SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y);

            EntityInteractionModel villagerInteractionModel = new EntityInteractionModel(new List<String>() { "Hello, sir, I've ground the Calatian and terterum herbs in the pestle as you asked, and the reaction seems to match what we expected.", 
                                                                                                              "I'll talk to Karam in the morning to gather the remaining ingredients." });
            IGameObjectModel villager1 = new GameObjectModel(EntityType.Villager1, this, 1, 1, MovementType.Still, true, false, villagerInteractionModel);
            this.RegisterGameObject(villager1, null);

            Inventory merchantInventory = Inventory.DebugInventory();
            IGameObjectModel merchant = new Merchant(this, 5, 5, new List<String>() { "Hello, I am the merchant in town, here to visit your tower today."}, merchantInventory);
            this.RegisterGameObject(merchant);
        }
    }
}
