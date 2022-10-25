using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Common
{
    public struct Location
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object? obj)
        {
            if (!(obj is Location))
                return false;

            Location newLocation = (Location)obj;
            // compare elements here
            return newLocation.X == this.X && newLocation.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }
    }
}
