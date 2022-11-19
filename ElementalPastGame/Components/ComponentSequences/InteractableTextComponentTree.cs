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
        internal List<IInteractableTextComponentTreeObserver> observers = new();
        
        public InteractableTextComponentTree(ITextComponentTree tree)
        {
            this.lastInputTime = DateTime.Now;
            this.activeTree = tree;
        }

        public void AddObserver(IInteractableTextComponentTreeObserver observer)
        {
            this.observers.Add(observer);
        }

        public void RemoveObserver(IInteractableTextComponentTreeObserver observer)
        {
            this.observers.Remove(observer);
        }

        // Use this method for selection and generally keys that need debouncing
        public void HandleKeyPressed(char keyChar)
        {
            switch (keyChar)
            {
                case ' ':
                case 's':
                    break;
                case 'd':
                    ITextComponentTree? parent = this.activeTree.GetParentTree();
                    if (parent != null)
                    {
                        this.activeTree = (ITextComponentTree)parent;
                    }
                    return;
                default:
                    return;
            }

            // If there is a child tree, display it
            ITextComponentTree? nextTree = this.activeTree.GetSelectedChild();
            if (nextTree != null)
            {
                this.activeTree = (ITextComponentTree)nextTree;
                return;
            }

            // There are no more child trees, let's dismiss this whole thing
            if (this.activeTree is TextMenu)
            {
                ((TextMenu)this.activeTree).Resolve();
            }

            foreach (IInteractableTextComponentTreeObserver observer in this.observers)
            {
                observer.InteractableTextComponentTreeObserverDidDismiss(this);
            }
        }

        // Use this for option movement, generally things that can happen smoothly, or continually
        // on key down
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
        }

        public RenderingModel GetRenderingModel()
        {
            return this.activeTree.textComponent.getRenderingModel();
        }
    }
}
