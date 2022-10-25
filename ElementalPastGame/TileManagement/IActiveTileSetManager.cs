using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public interface IActiveTileSetManager
    {
        public static IActiveTileSetManager GetInstance() => throw new NotImplementedException();
        public void Update(int PreviousCenterX, int PreviousCenterY, int CenterX, int CenterY, bool isAnimating, double offset);

        public bool isTileCollidable(int x, int y);
    }
}
