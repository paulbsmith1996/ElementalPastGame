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
            switch (lastKey)
            {
                case Keys.Space:
                case Keys.Enter:
                case Keys.S:
                    break;
                default:
                    return;
            }

            String selectedOption = this.activeTree.GetSelectedOption();
            ITextComponentTree? nextTree = this.activeTree.ChildForKey(selectedOption);
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
