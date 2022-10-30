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

        /// <summary>
        ///  If true, the user can backtrack to the parent tree if it exists. If false, parent will always be nil.
        /// </summary>
        public bool isReturnable { get; set; }
        public ITextComponentTree? parent { get; set; }

        public ITextComponentTree? GetSelectedChild();
    }
}
