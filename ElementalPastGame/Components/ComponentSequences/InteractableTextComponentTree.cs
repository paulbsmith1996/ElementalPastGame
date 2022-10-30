using ElementalPastGame.Common;
using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components.ComponentSequences
{
    public class InteractableTextComponentTree
    {
        internal ITextComponentTree activeTree;
        internal DateTime lastInputTime;
        public InteractableTextComponentTree(ITextComponentTree tree)
        {
            this.lastInputTime = DateTime.Now;
            this.activeTree = tree;
        }

        public void HandleKeyInputs(List<Keys> keyCodes)
        {
            DateTime handleInputTime = DateTime.Now;
            double timeSinceLastInput = (handleInputTime - this.lastInputTime).TotalMilliseconds;
            if (timeSinceLastInput < TextComponentConstants.TEXT_COMPONENT_WAIT_TIME_MS)
            {
                return;
            }
            
            if (keyCodes.Count == 0)
            {
                return;
            }

            lastInputTime = handleInputTime;

            Keys lastKey = keyCodes.Last();

            if (this.activeTree.textComponent is TextMenu)
            {
                TextMenu menu = (TextMenu)this.activeTree.textComponent;
                Location currentSelectedLocation = menu.GetSelected();
                switch (lastKey)
                {
                    case Keys.Up:
                        if (currentSelectedLocation.Y != 0)
                        {
                            menu.SetSelected(new Location() { X = currentSelectedLocation.X, Y = currentSelectedLocation.Y - 1 });
                        }
                        return;
                    case Keys.Down:
                        if (currentSelectedLocation.Y < menu.options.Count - 1)
                        {
                            menu.SetSelected(new Location() { X = currentSelectedLocation.X, Y = currentSelectedLocation.Y + 1 });
                        }
                        return;
                    case Keys.Left:
                        if (currentSelectedLocation.X != 0)
                        {
                            menu.SetSelected(new Location() { X = currentSelectedLocation.X - 1, Y = currentSelectedLocation.Y});
                        }
                        return;
                    case Keys.Right:
                        if (currentSelectedLocation.X < menu.optionsWidth - 1)
                        {
                            menu.SetSelected(new Location() { X = currentSelectedLocation.X + 1, Y = currentSelectedLocation.Y});
                        }
                        return;
                    default:
                        break;
                }
            }

            switch (lastKey)
            {
                case Keys.Space:
                case Keys.S:
                    break;
                case Keys.D:
                case Keys.Escape:
                case Keys.Back:
                    ITextComponentTree? parent = this.activeTree.GetParentTree();
                    if (parent != null)
                    {
                        this.activeTree = (ITextComponentTree)parent;
                    }
                    return;
                default:
                    return;
            }

            ITextComponentTree? nextTree = this.activeTree.GetSelectedChild();
            if (nextTree != null)
            {
                this.activeTree = (ITextComponentTree)nextTree;
            }
        }

        public RenderingModel GetRenderingModel()
        {
            return this.activeTree.textComponent.getRenderingModel();
        }
    }
}
