using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Enemies;
using ElementalPastGame.GameObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;

namespace ElementalPastGame.GameObject.EntityManagement
{
    public class ActiveEntityManager : IActiveEntityManager
    {

        internal static ActiveEntityManager? _instance;
        internal Dictionary<Location, IGameObjectModel> EntitiesByLocation = new();
        internal List<IGameObjectModel> previousActiveEntityModels = new();
        internal HashSet<long> deadEntityIDs = new();
        internal Dictionary<long, List<EntityBattleData>> enemyListsByEncounterID = new();
        public static ActiveEntityManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new ActiveEntityManager();
            return _instance;
        }
        public List<IGameObjectModel> GetActiveEntities(int CenterX, int CenterY)
        {
            List<IGameObjectModel> activeEntities = new();
            // Because we have amortized O(1) lookup in the dictionary, it's better to do
            // 441 + 88 = 539 gets (one for each tile in the active scene + one for each
            // tile around the active scene) than to loop through every entity in the
            // mapping to see if it's within the active scene range.
            for (int XCoordinate = CenterX - CommonConstants.TILE_VIEW_DISTANCE - 1; XCoordinate <= CenterX + CommonConstants.TILE_VIEW_DISTANCE + 1; XCoordinate++)
            {
                for (int YCoordinate = CenterY - CommonConstants.TILE_VIEW_DISTANCE - 1; YCoordinate <= CenterY + CommonConstants.TILE_VIEW_DISTANCE + 1; YCoordinate++)
                {
                    Location location = new() { X = XCoordinate, Y = YCoordinate };
                    IGameObjectModel? gameObjectModel = EntitiesByLocation.GetValueOrDefault(location);
                    if (gameObjectModel != null && !deadEntityIDs.Contains(gameObjectModel.EntityID))
                    {
                        // TODO: needed for loading
                        gameObjectModel.LoadIfNeeded();
                        activeEntities.Add(gameObjectModel);
                        previousActiveEntityModels.Remove(gameObjectModel);
                    }
                }
            }
            foreach (IGameObjectModel inactiveGameObjectModel in previousActiveEntityModels)
            {
                inactiveGameObjectModel.Unload();
            }
            previousActiveEntityModels = activeEntities;
            return activeEntities;
        }

        public void RemoveGameObjectFromLocation(IGameObjectModel gameObjectModel, Location location)
        {
            // TODO: This needs to be updated once several game objects can be in the same location
            EntitiesByLocation.Remove(location);
        }

        public void AddGameObjectToLocation(IGameObjectModel gameObjectModel, Location location)
        {
            AddEntityToMapping(gameObjectModel);
        }

        public void MarkEntityIDDead(long entityID)
        {
            deadEntityIDs.Add(entityID);
        }

        public List<EntityBattleData> enemiesForEncounterID(long encounterID)
        {
            return this.enemyListsByEncounterID[encounterID];
        }

        internal ActiveEntityManager()
        {
            LoadEntitiesByLocation();
        }
        internal void LoadEntitiesByLocation()
        {
            IGameObjectModel goblin1 = new Goblin(CommonConstants.GAME_START_LOCATION.X - 10, CommonConstants.GAME_START_LOCATION.Y);
            goblin1.movementType = MovementType.Aggressive;
            this.AddEntityToMapping(goblin1);
            List<EntityBattleData> goblin1EncounterList = new() { new EntityBattleData(EntityType.Goblin, 5), new EntityBattleData(EntityType.Goblin, 5) };
            this.RegisterEnemyListForEncounterID(goblin1EncounterList, goblin1.EntityID);

            IGameObjectModel goblin2 = new Goblin(860, 915);
            //List<Direction> goblin1Moves = new() { Direction.Up, Direction.None, Direction.None, Direction.Right, Direction.None, Direction.None, Direction.Down, Direction.None, Direction.None, Direction.Left, Direction.None, Direction.None };
            //goblin1.shouldCycleMoves = true;
            //goblin1.Moves = goblin1Moves;
            goblin2.movementType = MovementType.Wander;
            this.AddEntityToMapping(goblin2);
            List<EntityBattleData> goblin2EncounterList = new() { new EntityBattleData(EntityType.Goblin, 5) };
            this.RegisterEnemyListForEncounterID(goblin2EncounterList, goblin2.EntityID);
        }

        internal void AddEntityToMapping(IGameObjectModel gameObjectModel)
        {
            EntitiesByLocation.Add(gameObjectModel.Location, gameObjectModel);
        }

        internal void RegisterEnemyListForEncounterID(List<EntityBattleData> enemies, long encounterID)
        {
            this.enemyListsByEncounterID[encounterID] = enemies;
        }
    }
}
