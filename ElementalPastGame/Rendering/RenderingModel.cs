using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Rendering
{
    public struct RenderingModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public List<Bitmap> Bitmaps { get; set; }

        // More for test than anything else
        public Color? BackgroundColor { get; set; }

    }
}
