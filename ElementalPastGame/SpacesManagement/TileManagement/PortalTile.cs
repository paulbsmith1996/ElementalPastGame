using ElementalPastGame.SpacesManagement.Spaces;
using ElementalPastGame.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.SpacesManagement.TileManagement
{
    public class PortalTile : Tile, ITile
    {

        internal int portalX;
        internal int portalY;
        internal String portalSpaceIdentity;

        public PortalTile(List<String> imageNames, String portalSpaceIdentity, int portalX, int portalY) : base(imageNames, false)
        {
            this.portalX = portalX;
            this.portalY = portalY;
            this.portalSpaceIdentity = portalSpaceIdentity;
        }
    }
}
