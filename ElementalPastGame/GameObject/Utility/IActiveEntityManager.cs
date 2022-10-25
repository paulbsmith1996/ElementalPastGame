using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Utility
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

        public void RemoveGameObjectFromLocation(IGameObjectModel gameObjectModel, Location location);
        public void AddGameObjectToLocation(IGameObjectModel gameObjectModel, Location location);
    }
}
