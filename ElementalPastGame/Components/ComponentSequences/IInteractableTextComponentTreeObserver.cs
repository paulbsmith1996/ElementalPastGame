using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.Components.ComponentSequences
{
    public interface IInteractableTextComponentTreeObserver
    {
        public void InteractableTextComponentTreeObserverDidDismiss(InteractableTextComponentTree tree);
    }
}
