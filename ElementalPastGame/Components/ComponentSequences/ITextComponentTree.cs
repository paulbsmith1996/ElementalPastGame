using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components.ComponentSequences
{
    public interface ITextComponentTree
    {
        public ITextComponentTree? ChildForKey(string key = "");
        public void SetChildForKey(string key, ITextComponentTree child);
        public void RemoveChildForKey(string key);
        public ITextComponentTree? GetParentTree();

        public ITextComponent textComponent { get; set; }
        public ITextComponentTree? parent { get; set; }

        public String GetSelectedOption();
    }
}
