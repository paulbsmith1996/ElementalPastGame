using ElementalPastGame.Rendering;
using ElementalPastGame.Rendering.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        internal bool hasPointer;
        internal Bitmap pointerBitmap;

        internal DateTime animationStartTime;
        internal int pointerAnimationX;
        internal int pointerAnimationStartY;

        public ITextComponent? childTextComponent { get; set; }

        internal RenderingModel? staticRenderingModel { get; set; }

        public GameTextBox(String text,
                            int x,
                            int y,
                            int width,
                            int height) : this(text, TextComponentConstants.FONT_FAMILY, TextComponentConstants.FONT_SIZE,
                                               x, y, width, height, TextComponentConstants.CORNER_RADIUS,
                                               TextComponentConstants.DEFAULT_BORDER_COLOR,
                                               TextComponentConstants.DEFAULT_BACKGROUND_COLOR, true)
        {
        }

        public GameTextBox (String text, 
                            int x, 
                            int y, 
                            int width, 
                            int height, bool hasPointer) : this(text, TextComponentConstants.FONT_FAMILY, TextComponentConstants.FONT_SIZE, 
                                               x, y, width, height, TextComponentConstants.CORNER_RADIUS, 
                                               TextComponentConstants.DEFAULT_BORDER_COLOR, 
                                               TextComponentConstants.DEFAULT_BACKGROUND_COLOR, hasPointer)
        {
        }

        public GameTextBox(String text, 
                           String fontFamily, 
                           int fontSize, 
                           int x, 
                           int y, 
                           int width, 
                           int height,
                           bool hasPointer) : this(text, fontFamily, fontSize, x, y, width, height, 
                                              TextComponentConstants.CORNER_RADIUS, TextComponentConstants.DEFAULT_BORDER_COLOR, 
                                              TextComponentConstants.DEFAULT_BACKGROUND_COLOR, hasPointer)
        {
        }

        public GameTextBox(String text, 
                           String fontFamily, 
                           int fontSize, 
                           int x, 
                           int y, 
                           int width, 
                           int height, 
                           int cornerRadius, 
                           Color borderColor, 
                           Color backgroundColor,
                           bool hasPointer)
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
            this.hasPointer = hasPointer;

            this.pointerAnimationX = this.x + this.width - TextComponentConstants.TEXT_INSET - TextComponentConstants.TEXTBOX_POINTER_WIDTH;
            int pointerAnimationEndY = this.y + this.height - TextComponentConstants.TEXT_INSET - TextComponentConstants.TEXTBOX_POINTER_HEIGHT;
            this.pointerAnimationStartY = pointerAnimationEndY - TextComponentConstants.TEXTBOX_POINTER_ANIMATION_DISTANCE;
            this.pointerBitmap = this.GetPointerBitmap();
            this.animationStartTime = DateTime.Now;
        }

        public List<RenderingModel> getRenderingModels()
        {
            List<RenderingModel> renderingModels = new List<RenderingModel>() { this.GetStaticRenderingModel() };
            if (this.hasPointer)
            {
                renderingModels.Add(this.GetPointerRenderingModel());
            }
            return renderingModels;
        }

        internal RenderingModel GetStaticRenderingModel()
        {
            if (this.staticRenderingModel != null)
            {
                return (RenderingModel)this.staticRenderingModel;
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

            this.staticRenderingModel = new()
            {
                X = this.x,
                Y = this.y,
                Width = this.width,
                Height = this.height,
                Bitmaps = bitmaps,
            };

            return (RenderingModel)this.staticRenderingModel;
        }

        internal RenderingModel GetPointerRenderingModel()
        {
            DateTime now = DateTime.Now;
            double timeSinceAnimationStart = (now - this.animationStartTime).TotalMilliseconds;
            if (timeSinceAnimationStart > TextComponentConstants.TEXTBOX_POINTER_ANIMATION_DURATION)
            {
                this.animationStartTime = now;
                timeSinceAnimationStart = 0;
            }
            double fractionAnimationCompleted = timeSinceAnimationStart / TextComponentConstants.TEXTBOX_POINTER_ANIMATION_DURATION;
            int pointerAnimationY = (int)(fractionAnimationCompleted * TextComponentConstants.TEXTBOX_POINTER_ANIMATION_DISTANCE);
            int pointerY = pointerAnimationY + this.pointerAnimationStartY;

            return new RenderingModel()
            {
                X = this.pointerAnimationX,
                Y = pointerY,
                Width = this.pointerBitmap.Width,
                Height = this.pointerBitmap.Height,
                Bitmaps = new List<Bitmap>() { this.pointerBitmap }
            };
        }

        internal Bitmap GetPointerBitmap()
        {
            Bitmap pointerBitmap = new Bitmap(TextComponentConstants.TEXTBOX_POINTER_WIDTH, TextComponentConstants.TEXTBOX_POINTER_HEIGHT);
            Graphics pointerGraphics = Graphics.FromImage(pointerBitmap);
            Brush pointerBrush = Brushes.White;
            Point[] points = { new Point(0, 0), 
                               new Point(TextComponentConstants.TEXTBOX_POINTER_WIDTH, 0), 
                               new Point(TextComponentConstants.TEXTBOX_POINTER_WIDTH / 2, TextComponentConstants.TEXTBOX_POINTER_HEIGHT) };
            pointerGraphics.FillPolygon(pointerBrush, points);
            return pointerBitmap;
        }
    }
}
