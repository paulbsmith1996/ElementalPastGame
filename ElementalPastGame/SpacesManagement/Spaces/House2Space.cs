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

namespace ElementalPastGame.SpacesManagement.Spaces
{
    public class House2Space : Space, ISpace
    {
        public House2Space() : base(SpaceConstants.HOUSE_WIDTH + 2, SpaceConstants.HOUSE_HEIGHT + 2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y, true)
        {

        }
        internal override void SetUpSpace()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Blank, true), 0, 0, SpaceConstants.HOUSE_WIDTH + 1, SpaceConstants.HOUSE_HEIGHT + 1);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.HouseFloor), 1, 1, SpaceConstants.HOUSE_WIDTH, SpaceConstants.HOUSE_HEIGHT);

            this.SetTileAtLocation(TileFactory.PortalTileWithBackground(TextureMapping.HouseMat, Spaces.OVERWORLD, 820, 870), SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y);

            EntityInteractionModel villagerInteractionModel = new EntityInteractionModel(new List<String>() { "We're actually a very friendly bunch here!", "Thanks for taking care of the goblins in the north.", "They've been bothering our town for ages now." });
            IGameObjectModel villager1 = new GameObjectModel(EntityType.Villager1, this, 3, 7, MovementType.Still, true, false, villagerInteractionModel);
            this.RegisterGameObject(villager1, null);
        }
    }
}
