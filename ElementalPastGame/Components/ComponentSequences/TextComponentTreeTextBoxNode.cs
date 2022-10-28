using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components.ComponentSequences
{
    public class TextComponentTreeTextBoxNode : ITextComponentTree
    {
        internal ITextComponentTree? child;
        public ITextComponent textComponent { get; set; }
        public ITextComponentTree? parent { get; set; }

        public TextComponentTreeTextBoxNode(ITextComponent textComponent)
        {
            this.textComponent = textComponent;
        }

        public ITextComponentTree? ChildForKey(string key = "")
        {
            if (!key.Equals("")) {
                throw new Exception();
            }
            return this.child;
        }

        public void RemoveChildForKey(string key)
        {
            if (!key.Equals(""))
            {
                throw new Exception();
            }
            this.child = null;
        }

        public void SetChildForKey(string key, ITextComponentTree tree)
        {
            if (!key.Equals(""))
            {
                throw new Exception();
            }
            this.child = tree;
            tree.parent = this;
        }

        public void SetChild(ITextComponentTree tree)
        {
            this.SetChildForKey("", tree);
        }

        public ITextComponentTree? GetParentTree()
        {
            return this.parent;
        }

        public String GetSelectedOption()
        {
            return "";
        }
    }
}
