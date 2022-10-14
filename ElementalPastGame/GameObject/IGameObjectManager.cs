using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject
{
    /// <summary>
    /// The GameObjectManager is supposed to update the world on every run loop. It also
    /// performs validation logic and any test that may involve multiple GameObjects (e.g.
    /// collision detection).
    /// </summary>
    public interface IGameObjectManager
    {
        /// <summary>
        /// Never call GameObjectManager new(). All new instances of this interface should
        /// be acquire via this static method.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IGameObjectManager getInstance() => throw new NotImplementedException();

        public Boolean ValidateNewGameObjectPosition(IGameObjectModel gameObject, Location newLocation);

        public bool isAnimating { get; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public int PreviousCenterX { get; set; }
        public int PreviousCenterY { get; set; }

        public int FramesAnimated { get; }
    }
}
