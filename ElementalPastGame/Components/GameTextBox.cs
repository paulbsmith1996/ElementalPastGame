using ElementalPastGame.Rendering;
using ElementalPastGame.Rendering.Utility;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public class GameTextBox : ITextComponent
    {
        public int x { get; set; }

        public int y { get; set; }

        public int width { get; set; }

        public int height { get; set; }
        public int cornerRadius { get; set; }
        public Color borderColor { get; set; }
        public Color backgroundColor { get; set; }
        public Font font { get; set; }

        public String text;

        public ITextComponent? childTextComponent { get; set; }

        internal RenderingModel? renderingModel { get; set; }

        public GameTextBox (String text, int x, int y, int width, int height) : this(text, TextComponentConstants.FONT_FAMILY, TextComponentConstants.FONT_SIZE, x, y, width, height, TextComponentConstants.CORNER_RADIUS, TextComponentConstants.DEFAULT_BORDER_COLOR, TextComponentConstants.DEFAULT_BACKGROUND_COLOR)
        {
        }

        public GameTextBox(String text, String fontFamily, int fontSize, int x, int y, int width, int height) : this(text, fontFamily, fontSize, x, y, width, height, TextComponentConstants.CORNER_RADIUS, TextComponentConstants.DEFAULT_BORDER_COLOR, TextComponentConstants.DEFAULT_BACKGROUND_COLOR)
        {
        }

        public GameTextBox(String text, String fontFamily, int fontSize, int x, int y, int width, int height, int cornerRadius, Color borderColor, Color backgroundColor)
        {
            this.text = text;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.cornerRadius = cornerRadius;
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.font = new Font(fontFamily, fontSize);
        }

        public RenderingModel getRenderingModel()
        {
            if (this.renderingModel != null)
            {
                return (RenderingModel)this.renderingModel;
            }

            Bitmap bitmap = new Bitmap(this.width, this.height);

            Graphics g = Graphics.FromImage(bitmap);

            Rectangle bounds = new Rectangle(0, 0, this.width, this.height);
            Brush brush = new SolidBrush(this.backgroundColor);
            g.FillPath(brush, GraphicsPathsFactory.RoundedRect(bounds, this.cornerRadius));

            Pen borderPen = new Pen(this.borderColor);
            borderPen.Width = 4;
            int halfBorderPenWidth = (int)borderPen.Width / 2;
            Rectangle borderBounds = new Rectangle(halfBorderPenWidth, halfBorderPenWidth, this.width - 2 * halfBorderPenWidth, this.height - 2 * halfBorderPenWidth);
            g.DrawPath(borderPen, GraphicsPathsFactory.RoundedRect(borderBounds, this.cornerRadius - halfBorderPenWidth));

            Rectangle innerBorderBounds = new Rectangle(TextComponentConstants.INNER_RECT_OFFSET, TextComponentConstants.INNER_RECT_OFFSET, this.width - 2 * TextComponentConstants.INNER_RECT_OFFSET, this.height - 2 * TextComponentConstants.INNER_RECT_OFFSET);
            g.DrawPath(borderPen, GraphicsPathsFactory.RoundedRect(innerBorderBounds, this.cornerRadius));

            g.DrawString(this.text, this.font, Brushes.White, TextComponentConstants.INNER_RECT_OFFSET + TextComponentConstants.TEXT_INSET, TextComponentConstants.INNER_RECT_OFFSET + TextComponentConstants.TEXT_INSET);

            List<Bitmap> bitmaps = new()
            {
                bitmap
            };

            this.renderingModel = new ()
            {
                X = this.x,
                Y = this.y,
                Width = this.width,
                Height = this.height,
                Bitmaps = bitmaps,
            };

            return (RenderingModel)this.renderingModel;
        }
    }
}
