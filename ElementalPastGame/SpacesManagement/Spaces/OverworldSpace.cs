using ElementalPastGame.Common;
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
using static ElementalPastGame.TileManagement.TileTemplates.TileFactory;
using ElementalPastGame.TileManagement;
using ElementalPastGame.SpacesManagement.TileManagement;

namespace ElementalPastGame.SpacesManagement.Spaces
{
    public class OverworldSpace : Space, ISpace
    {

        public OverworldSpace () : base(CommonConstants.MAX_MAP_TILE_DIMENSION, CommonConstants.MAX_MAP_TILE_DIMENSION, CommonConstants.GAME_START_LOCATION.X, CommonConstants.GAME_START_LOCATION.Y)
        {
        }

        internal override void SetUpSpace()
        {
            this.SetUpStartingRegion();
            this.SetUpFirstTown();
        }

        internal void SetUpStartingRegion()
        {
            this.SetUpStartingRegionEntities();
            this.SetUpStartingRegionMap();
        }

        internal void SetUpStartingRegionEntities()
        {
            IGameObjectModel goblin1 = new Goblin(this, 890, 900, MovementType.Aggressive);
            List<EntityBattleModel> goblin1EncounterList = new() { new EntityBattleModel(EntityType.Goblin, 5), new EntityBattleModel(EntityType.Goblin, 5) };
            this.RegisterGameObject(goblin1, goblin1EncounterList);

            IGameObjectModel goblin2 = new Goblin(this, 860, 915, MovementType.Wander);
            //List<Direction> goblin1Moves = new() { Direction.Up, Direction.None, Direction.None, Direction.Right, Direction.None, Direction.None, Direction.Down, Direction.None, Direction.None, Direction.Left, Direction.None, Direction.None };
            //goblin1.shouldCycleMoves = true;
            //goblin1.Moves = goblin1Moves;
            List<EntityBattleModel> goblin2EncounterList = new() { new EntityBattleModel(EntityType.Goblin, 5) };
            this.RegisterGameObject(goblin2, goblin2EncounterList);
        }

        // Map setups

        // Route 1: (820, 890) -> (910, 920)
        internal void SetUpStartingRegionMap()
        {
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 700, 700, 999, 999);

            // Back fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 910, 890, 910, 920);
            // Top fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 810, 920, 910, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 910, 920);
            // Bottom fence
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 820, 890, 910, 890);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 910, 890);
            // Chunk 1a
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 880, 908, 909, 919);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass8), 880, 907, 909, 907);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 900, 906, 909, 906, true);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 880, 906, 889, 906, true);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 878, 906, 879, 919, true);
            // Lake 1b
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 857, 897, 868, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass8), 857, 896, 868, 896);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass2, true), 857, 903, 868, 903);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass4, true), 869, 897, 869, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.WaterGrass6, true), 856, 897, 856, 902);
            // Chunk 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 906, 869, 910);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 851, 907, 869, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 906, 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTL), 870, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBL), 870, 906);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 851, 907, 869, 909, true);
            // Chunk 1e
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Grass), 850, 890, 870, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 890, 870, 890);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Horizontal), 850, 893, 880, 893);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 870, 891, 870, 892);
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 852, 891, 880, 892, true);
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 882, 891, 908, 894, true);
            // Chunk 1c
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 850, 891, 850, 909);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 853, 894, 853, 905);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 853, 906);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 850, 910);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerBR), 853, 893);
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 850, 891, 853, 906, true);
            // Chunk 1d
            this.SetChunkToImage(TextureMapping.PineTree, TextureMapping.Grass, 1, 2, 840, 900, 843, 919, true);
            // Chunk 1f
            this.SetChunkToImage(TextureMapping.BushyTree, TextureMapping.Grass, 2, 2, 820, 891, 832, 900, true);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Water, true), 822, 901, 831, 912);
            // Dirt path
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 908, 899, 908, 901);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 873, 902, 907, 902);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 873, 899, 907, 901);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 873, 898, 907, 898);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass1), 908, 902);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass7), 908, 898);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass9), 872, 898);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 876, 903, 876, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 873, 901, 875, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 872, 899, 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt9), 876, 902);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass1), 876, 917);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 846, 917, 875, 917);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 846, 914, 873, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 849, 913, 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt1), 872, 913);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass3), 845, 917);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt3), 848, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 848, 895, 848, 912);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 846, 895, 847, 914);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 845, 899, 845, 916);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass7), 848, 894);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt7), 845, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 839, 898, 844, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 835, 895, 845, 897);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass8), 835, 894, 847, 894);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass9), 834, 894);

            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.GrassDirt9), 838, 898);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass4), 838, 899, 838, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 835, 898, 837, 913);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 834, 895, 834, 912);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 834, 914, 836, 914);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 833, 915, 835, 915);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 832, 916, 834, 916);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 819, 915, 832, 917);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 817, 916, 819, 916);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 816, 915, 818, 915);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 815, 914, 817, 914);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 814, 890, 816, 913);
        }

        internal void SetUpFirstTown()
        {
            this.SetUpFirstTownMap();
            this.SetUpFirstTownEntities();
        }

        internal void SetUpFirstTownEntities()
        {

        }

        // First town: (830, 790) -> (730, 890)
        internal void SetUpFirstTownMap()
        {

            // Fence 1a
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 820, 881, 820, 890);
            // Fence 1b
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.Vertical), 810, 881, 810, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.DirtGrass2, TileOrientation.Vertical), 810, 881);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.Dirt, TileOrientation.Vertical), 810, 840, 810, 880);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass6), 809, 840, 809, 880);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 810, 920);
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.Grass, TileOrientation.CornerTR), 820, 890);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.DirtGrass3), 809, 881);

            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.DirtGrass2), 811, 881, 819, 881);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.Dirt), 811, 838, 850, 880);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 812, 839, 849, 879);

            // Roads
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 814, 878, 816, 889);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 817, 878, 828, 879);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 825, 872, 828, 878);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 815, 869, 815, 873);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 815, 869, 822, 869);
            this.SetTileAtLocation(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 820, 870);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 823, 865, 830, 871);
            this.SetChunkToTile(TileFactory.TileWithBackground(TextureMapping.StoneRoad), 825, 853, 828, 864);

            // ??
            this.SetTileAtLocation(TileFactory.FenceWithBackground(TextureMapping.DirtGrass2, TileOrientation.CornerBR), 820, 881);
            this.SetChunkToTile(TileFactory.FenceWithBackground(TextureMapping.DirtGrass2, TileOrientation.Horizontal), 821, 881, 880, 881);

            // House 1
            this.SetEnterableBuildingOnTiles(TextureMapping.LogCabin, TextureMapping.StoneRoad, 814, 874, 3, 4, 1, 0, Spaces.HOUSE_1, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1);

            // House 2
            //this.SetTileAtLocation(TileFactory.PortalTileWithBackground(TextureMapping.HouseDoor, Spaces.HOUSE_2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1), 820, 871);
            this.SetEnterableBuildingOnTiles(TextureMapping.LogCabin, TextureMapping.StoneRoad, 819, 871, 3, 4, 1, 0, Spaces.HOUSE_2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1);

            // Decorative houses
            this.SetChunkToImage(TextureMapping.LogCabin, TextureMapping.StoneRoad, 3, 4, 814, 865, 822, 868, true);
            this.SetChunkToImage(TextureMapping.LogCabin, TextureMapping.StoneRoad, 3, 4, 813, 853, 823, 861, true);
            this.SetChunkToImage(TextureMapping.LogCabin, TextureMapping.StoneRoad, 3, 4, 831, 865, 839, 868, true);
            this.SetChunkToImage(TextureMapping.LogCabin, TextureMapping.StoneRoad, 3, 4, 829, 853, 841, 861, true);
            this.SetEnterableBuildingOnTiles(TextureMapping.WizardTower, TextureMapping.StoneRoad, 834, 871, 3, 6, 1, 0, Spaces.HOUSE_2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1);
            this.SetEnterableBuildingOnTiles(TextureMapping.LogCabin, TextureMapping.StoneRoad, 831, 871, 3, 4, 1, 0, Spaces.HOUSE_2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1);
            this.SetEnterableBuildingOnTiles(TextureMapping.LogCabin, TextureMapping.StoneRoad, 837, 871, 3, 4, 1, 0, Spaces.HOUSE_2, SpaceConstants.HOUSE_START_X, SpaceConstants.HOUSE_START_Y + 1);
        }
    }
}
