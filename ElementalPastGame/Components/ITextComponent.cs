using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public interface ITextComponent
    {
        public int x { get; }
        public int y { get; }

        public int width { get; }
        public int height { get; }

        public Color backgroundColor { get; }

        public RenderingModel getRenderingModel();
    }
}
