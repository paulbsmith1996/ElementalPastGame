using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public static class TextComponentConstants
    {
        public static int TEXT_INSET = 10;

        public static int FONT_SIZE = 12;

        public static int CORNER_RADIUS = 10;

        public static int INNER_RECT_OFFSET = 5;

        public static Font FONT = new Font("Arial Narrow", FONT_SIZE);
        public static Color DEFAULT_BORDER_COLOR = Color.FromArgb(255, 70, 120, 150);
        public static Color DEFAULT_BACKGROUND_COLOR = Color.FromArgb(15, 30, 80, 150);

        public static double TEXT_COMPONENT_WAIT_TIME_MS = 1000;
    }
}
