using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.EntityManagement
{
    public interface IActiveEntityManager
    {
        public static IActiveEntityManager GetInstance() => throw new NotImplementedException();
        /// <summary>
        /// Returns a list of all the entities that are active in the neighborhood of the player
        /// </summary>
        /// <param name="CenterX">The x coordinate of the player's location</param>
        /// <param name="CenterY">The y corrdinate of the player's location</param>
        /// <returns></returns>
        public List<IGameObjectModel> GetActiveEntities(int CenterX, int CenterY);

        public IGameObjectModel? ActiveEntityAt(int x, int y);

        public bool IsCollidableEntityPresent(int x, int y);

        public void RegisterGameObject(IGameObjectModel gameObjectModel, List<EntityBattleModel> encounterEnemies);

        public void MoveGameObject(IGameObjectModel gameObjectModel, Location fromLocation, Location toLocation);

        public void MarkEntityIDDead(long entityID);

        public List<EntityBattleModel> enemiesForEncounterID(long encounterID);
        public void Unload();
    }
}
