﻿using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Enemies;
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
    public class House1Space : Space, ISpace
    {

        public House1Space() : base(SpaceConstants.HOUSE_WIDTH + 2, SpaceConstants.HOUSE_HEIGHT + 2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y, true)
        {

        }
        internal override void SetUpSpace()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Blank, true), 0, 0, SpaceConstants.HOUSE_WIDTH + 1, SpaceConstants.HOUSE_HEIGHT + 1);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.HouseFloor), 1, 1, SpaceConstants.HOUSE_WIDTH, SpaceConstants.HOUSE_HEIGHT);

            this.SetTileAtLocation(TileFactory.PortalTileWithBackground(TextureMapping.HouseMat, Spaces.OVERWORLD, 815, 873), SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y);

            EntityInteractionModel villagerInteractionModel = new EntityInteractionModel(new List<String>() { "Hello, how are you today?", "I am a villager here in Carroot village.", "I hope you enjoy your stay here." });
            IGameObjectModel villager1 = new GameObjectModel(EntityType.Villager1, this, 5, 6, MovementType.Still, true, false, villagerInteractionModel);
            this.RegisterGameObject(villager1, null);
        }
    }
}
