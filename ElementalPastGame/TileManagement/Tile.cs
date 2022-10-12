using ElementalPastGame.Common;
using ElementalPastGame.GameObject;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public class Tile : ITile
    {
        public TileLoadState TileLoadState { get; set; }
        public String TileModelID { get; set; }

        internal List<String> ImageNames;
        internal Boolean _isCollidable;
        public Boolean isCollidable { get { return _isCollidable; } set { _isCollidable = value; } }

        internal List<Bitmap> _images = new List<Bitmap>();
        public List<Bitmap> Images { get { return _images; } set { _images = value; } }

        internal static double _debugTimeSpentLoadingPerRunLoop = 0;
        public static double DebugTimeSpentLoadingPerRunLoop { get { return _debugTimeSpentLoadingPerRunLoop; } set { _debugTimeSpentLoadingPerRunLoop = value; } }

        public Tile (List<String> ImageNames, Boolean isCollidable)
        {
            this.ImageNames = ImageNames;
            this.isCollidable = isCollidable;
            this.TileLoadState = TileLoadState.Unloaded;
            this.TileModelID = "TilePlaceholderModelID";
        }

        public void Load()
        {
            if (this.Images.Count > 0)
            {
                return;
            }

            foreach (String ImageName in this.ImageNames) {
                Bitmap potentialImage = TextureMapping.Mapping[ImageName];
                if (potentialImage != null)
                {
                    this.Images.Add(new Bitmap(potentialImage, new Size(CommonConstants.TILE_DIMENSION + 1, CommonConstants.TILE_DIMENSION + 1)));
                }
                else {
                    this.Images.Add(new Bitmap(TextureMapping.Mapping[TextureMapping.Blank], CommonConstants.TILE_DIMENSION + 1, CommonConstants.TILE_DIMENSION + 1));
                }

                this.TileLoadState = TileLoadState.Loaded;
            }
        }

        public void Unload()
        {
            this.Images = new List<Bitmap>();
            this.TileLoadState = TileLoadState.Unloaded;
        }
    }
}
