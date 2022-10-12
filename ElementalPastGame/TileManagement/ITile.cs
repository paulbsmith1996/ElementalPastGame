using ElementalPastGame.GameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public enum TileLoadState
    {
        Central,
        Loaded,
        Unloaded,
    }
    public interface ITile
    {
        public TileLoadState TileLoadState { get; set; }
        public List<Bitmap> Images { get; }
        public String TileModelID {get; set;}
        public void Load();
        public void Unload();

        public Boolean isCollidable { get; set; }
    }
}
