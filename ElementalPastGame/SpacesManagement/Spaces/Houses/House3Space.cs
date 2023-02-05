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

namespace ElementalPastGame.SpacesManagement.Spaces.Houses
{
    public class House3Space : Space, ISpace
    {

        public House3Space() : base(SpaceConstants.HOUSE_WIDTH + 2, SpaceConstants.HOUSE_HEIGHT + 2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y, true)
        {

        }
        internal override void SetUpSpace()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Blank, true), 0, 0, SpaceConstants.HOUSE_WIDTH + 1, SpaceConstants.HOUSE_HEIGHT + 1);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.HouseFloor), 1, 1, SpaceConstants.HOUSE_WIDTH, SpaceConstants.HOUSE_HEIGHT);

            this.SetTileAtLocation(TileFactory.PortalTileWithBackground(TextureMapping.HouseMat, Spaces.OVERWORLD, 832, 870), SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y);

            EntityInteractionModel villagerInteractionModel = new EntityInteractionModel(new List<String>() { "Quite a good day I imagine for you, apothecary.", "I hear the herbs are blossoming in the forest, and I'm sure your assistant is ready for harvest.", "I know you've only spent a few years in Carroot, but I must say, I'm relieved you ended up here, apothecary." });
            IGameObjectModel villager1 = new GameObjectModel(EntityType.Villager1, this, 5, 6, MovementType.Still, true, false, villagerInteractionModel);
            this.RegisterGameObject(villager1, null);
        }
    }
}
