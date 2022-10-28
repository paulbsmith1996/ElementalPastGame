using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components.ComponentSequences
{
    public class TextComponentTree : ITextComponentTree
    {
        Dictionary<String, ITextComponentTree> childrenByKeys = new();
        public ITextComponent textComponent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ITextComponentTree? parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TextComponentTree(ITextComponent textComponent)
        {
            this.textComponent = textComponent;
        }

        public ITextComponentTree? ChildForKey(string key = "")
        {
            return this.childrenByKeys[key];
        }

        public ITextComponentTree? GetParentTree()
        {
            return this.parent;
        }

        public void RemoveChildForKey(string key)
        {
            this.childrenByKeys.Remove(key);
        }

        public void SetChildForKey(string key, ITextComponentTree child)
        {
            this.childrenByKeys[key] = child;
        }

        public String GetSelectedOption()
        {
            throw new NotImplementedException();
        }
    }
}
