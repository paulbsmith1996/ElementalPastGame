using ElementalPastGame.Common;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public class TextMenu : ITextComponent, ITextComponentTree
    {
        public int x { get; set; }

        public int y { get; set; }

        public int width { get; set; }

        public int height { get; set; }

        public Color backgroundColor { get; set; }

        public List<String> options { get; set; }
        public bool isReturnable { get; set; }
        internal int selectedX { get; set; }
        internal int selectedY { get; set; }
        public String selectedOption { get; set; }
        public int cornerRadius { get; set; }
        public Color borderColor { get; set; }

        public int optionsWidth { get; set; }
        internal RenderingModel? renderingModel;
        internal Dictionary<String, ITextComponentTree> textComponentTreesByKey = new();
        internal List<ITextMenuObserver> observers = new();
        internal bool needsRenderingModelUpdate;

        public ITextComponent textComponent { 
            get {
                return this;
            } 
            set { 
                throw new NotImplementedException(); 
            } 
        }
        public ITextComponentTree? parent { get; set; }

        public TextMenu(bool isReturnable, int x, int y) : this(1, isReturnable, x, y)
        {
        }

        public TextMenu(int optionsWidth, bool isReturnable, int x, int y) : this(new List<String>(), optionsWidth, isReturnable, x, y)
        {
        }

        public TextMenu(List<String> options, bool isReturnable, int x, int y) : this(options, 1, isReturnable, x, y)
        {
        }

        public TextMenu(List<String> options, int optionsWidth, bool isReturnable, int x, int y) : this(options, optionsWidth, isReturnable, x, y, TextComponentConstants.CORNER_RADIUS, TextComponentConstants.DEFAULT_BORDER_COLOR, TextComponentConstants.DEFAULT_BACKGROUND_COLOR)
        {
        }

        public TextMenu(List<String> options, int optionsWidth, bool isReturnable, int x, int y, int cornerRadius, Color borderColor, Color backgroundColor)
        {
            this.x = x;
            this.y = y;
            this.isReturnable = isReturnable;
            this.cornerRadius = cornerRadius;
            this.borderColor = borderColor;
            this.backgroundColor = backgroundColor;
            this.options = options;
            this.optionsWidth = optionsWidth;
            this.selectedX = 0;
            this.selectedY = 0;
            this.selectedOption = options.Count > 0 ? options[0] : "";
        }

        public void SetSelected(Location location)
        {
            this.selectedX = location.X;
            this.selectedY = location.Y;
            int optionsIndex = this.selectedX + this.selectedY * this.optionsWidth;
            if (optionsIndex >= this.options.Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.selectedOption = this.options.ElementAt(optionsIndex);
            this.needsRenderingModelUpdate = true;
        }

        public Location GetSelected()
        {
            return new Location() { X = this.selectedX, Y = this.selectedY };
        }

        public ITextComponentTree? ChildForKey(string key = "")
        {
            return this.textComponentTreesByKey.GetValueOrDefault(key);
        }

        public void AddSubOptionWithKey(ITextComponentTree subTree, String key)
        {
            if (this.options.Count == 0)
            {
                this.selectedOption = key;
            }
            this.options.Add(key);
            this.SetChildForKey(key, subTree);
            this.needsRenderingModelUpdate = true;
        }

        public void AddTerminalOption(String key)
        {
            if (this.options.Count == 0)
            {
                this.selectedOption = key;
            }
            this.options.Add(key);
        }

        public void SetChildForKey(string key, ITextComponentTree child)
        {
            this.textComponentTreesByKey[key] = child;
            child.parent = this;
        }

        public void RemoveChildForKey(string key)
        {
            ITextComponentTree? child = this.textComponentTreesByKey[key];
            if (child != null)
            {
                child.parent = null;
            }
            this.textComponentTreesByKey.Remove(key);
        }

        public ITextComponentTree? GetParentTree()
        {
            return this.isReturnable ? this.parent : null;
        }

        public ITextComponentTree? GetSelectedChild()
        {
            String selectedOption = this.selectedOption;
            return this.ChildForKey(selectedOption);
        }
        public void AddMenuObserver(ITextMenuObserver menuObserver)
        {
            this.observers.Add(menuObserver);
        }

        public void RemoveMenuObserver(ITextMenuObserver menuObserver)
        {
            this.observers.Remove(menuObserver);
        }

        public void Resolve()
        {
            ITextComponentTree? currentTextComponentTree = this;
            while (currentTextComponentTree != null)
            {
                if (currentTextComponentTree is TextMenu) {
                    TextMenu currentMenu = (TextMenu)currentTextComponentTree;
                    foreach (ITextMenuObserver observer in currentMenu.observers)
                    {
                        observer.MenuDidResolve(currentMenu, this.GetAncestorPath() + this.selectedOption);
                    }
                }
                currentTextComponentTree = currentTextComponentTree.GetParentTree();
            }
        }

        public RenderingModel getRenderingModel()
        {
            if (this.renderingModel != null && !this.needsRenderingModelUpdate)
            {
                return (RenderingModel)this.renderingModel;
            }
            this.needsRenderingModelUpdate = false;

            // First create a maximally-sized bitmap to do calculations
            Bitmap bitmap = new Bitmap(CommonConstants.TILE_DIMENSION * (2 * CommonConstants.TILE_VIEW_DISTANCE + 1), CommonConstants.TILE_DIMENSION * (2 * CommonConstants.TILE_VIEW_DISTANCE + 1));
            Graphics g = Graphics.FromImage(bitmap);

            this.width = (int)this.ComputeLongestRowWidth(g);
            this.height = (int)this.ComputeHeight(g);

            // Actually get a correctly-sized bitmap after calculations
            bitmap = new Bitmap(this.width, this.height);
            g = Graphics.FromImage(bitmap);

            Rectangle bounds = new Rectangle(0, 0, this.width, this.height);
            Brush brush = new SolidBrush(this.backgroundColor);
            g.FillPath(brush, GameTextBox.RoundedRect(bounds, this.cornerRadius));

            Pen borderPen = new Pen(this.borderColor);
            borderPen.Width = 4;
            int halfBorderPenWidth = (int)borderPen.Width / 2;
            Rectangle borderBounds = new Rectangle(halfBorderPenWidth, halfBorderPenWidth, this.width - 2 * halfBorderPenWidth, this.height - 2 * halfBorderPenWidth);
            g.DrawPath(borderPen, GameTextBox.RoundedRect(borderBounds, this.cornerRadius - halfBorderPenWidth));

            Rectangle innerBorderBounds = new Rectangle(TextComponentConstants.INNER_RECT_OFFSET, TextComponentConstants.INNER_RECT_OFFSET, this.width - 2 * TextComponentConstants.INNER_RECT_OFFSET, this.height - 2 * TextComponentConstants.INNER_RECT_OFFSET);
            g.DrawPath(borderPen, GameTextBox.RoundedRect(innerBorderBounds, this.cornerRadius));

            int currentX = TextComponentConstants.MENU_ITEM_HORIZONTAL_SPACING;
            int maximalRowHeight = (int)g.MeasureString("L", TextComponentConstants.FONT).Height;
            int selectedOptionIndex = this.selectedY * this.optionsWidth + this.selectedX;
            for (int optionsIndex = 0; optionsIndex < this.options.Count; optionsIndex++) {
                int rowNum = optionsIndex / this.optionsWidth;
                int optionY = (rowNum * maximalRowHeight) + ((rowNum + 1) * TextComponentConstants.MENU_ITEM_VERTICAL_SPACING);
                String option = this.options[optionsIndex];

                g.DrawString(option, TextComponentConstants.FONT, Brushes.White, currentX, optionY);

                SizeF stringSize = g.MeasureString(option, TextComponentConstants.FONT);
                int stringWidth = (int)stringSize.Width;

                if (optionsIndex == selectedOptionIndex) {
                    int selectIconY = optionY + (maximalRowHeight / 2) - (TextComponentConstants.MENU_SELECT_ICON_HEIGHT / 2);

                    int frontMenuSelectIconX = currentX - TextComponentConstants.MENU_SELECT_ICON_WIDTH - TextComponentConstants.MENU_SELECT_ICON_TEXT_PADDING;
                    this.FillTriangleInRectangleWithOrientation(frontMenuSelectIconX, selectIconY, TextComponentConstants.MENU_SELECT_ICON_WIDTH, TextComponentConstants.MENU_SELECT_ICON_HEIGHT, true, g, new SolidBrush(TextComponentConstants.MENU_SELECT_ICON_COLOR));

                    int trailingMenuSelectIconX = currentX + stringWidth + TextComponentConstants.MENU_SELECT_ICON_TEXT_PADDING;
                    this.FillTriangleInRectangleWithOrientation(trailingMenuSelectIconX, selectIconY, TextComponentConstants.MENU_SELECT_ICON_WIDTH, TextComponentConstants.MENU_SELECT_ICON_HEIGHT, false, g, new SolidBrush(TextComponentConstants.MENU_SELECT_ICON_COLOR));
                }

                if (this.optionsWidth == 1 || (optionsIndex > 0 && optionsIndex % this.optionsWidth == 0)) {
                    currentX = TextComponentConstants.MENU_ITEM_HORIZONTAL_SPACING;
                    continue;
                }

                currentX += stringWidth;
            }

            List<Bitmap> bitmaps = new()
            {
                bitmap
            };

            this.renderingModel = new()
            {
                X = this.x,
                Y = this.y,
                Width = this.width,
                Height = this.height,
                Bitmaps = bitmaps,
            };
            return (RenderingModel)this.renderingModel;
        }

        internal String GetAncestorPath()
        {
            ITextComponentTree? parent = this.GetParentTree();
            if (!(parent is TextMenu))
            {
                return "";
            }

            TextMenu parentMenu = (TextMenu)parent;
            return parentMenu.GetAncestorPath() + ">" + parentMenu.selectedOption;
        }

        internal float ComputeLongestRowWidth(Graphics graphics)
        {

            float maxWidth = 0;
            float currentRowWidth = 0;
            // Since the horizontal spacing should be the same for every row (same number of options
            // in each row), we can just add it at the end of the calculation and omit it in the calculation
            // of the maximum row width.
            for (int optionsIndex = 0; optionsIndex < options.Count; optionsIndex++)
            {
                currentRowWidth += graphics.MeasureString(options[optionsIndex], TextComponentConstants.FONT).Width;
                if (optionsIndex % optionsWidth == optionsWidth - 1 || optionsIndex == options.Count - 1) {
                    if (currentRowWidth > maxWidth)
                    {
                        maxWidth = currentRowWidth;
                    }
                    currentRowWidth = 0;
                }
            }

            return maxWidth + (TextComponentConstants.MENU_ITEM_HORIZONTAL_SPACING * (this.optionsWidth + 1));
        }

        internal float ComputeHeight(Graphics graphics)
        {
            float textHeight = graphics.MeasureString("L", TextComponentConstants.FONT).Height;
            int numRows = this.options.Count % this.optionsWidth == 0 ? this.options.Count / this.optionsWidth : (this.options.Count / this.optionsWidth) + 1;
            return (textHeight * numRows) + (TextComponentConstants.MENU_ITEM_VERTICAL_SPACING * (numRows + 1));
        }

        internal void FillTriangleInRectangleWithOrientation(int x, int y, int width, int height, bool isPointingRight, Graphics graphics, Brush brush)
        {
            PointF topPoint = new PointF(isPointingRight? x : x + width, y);
            PointF bottomPoint = new PointF(isPointingRight ? x : x + width, y + height);
            PointF pointyPoint = new PointF(isPointingRight ? x + width : x, y + (height / 2));
            PointF[] points = { topPoint, bottomPoint, pointyPoint };

            graphics.FillPolygon(brush, points);
        }
    }
}
