using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components
{
    public interface ITextMenuObserver
    {
        public void MenuDidResolve(TextMenu menu, String key);
    }
}
