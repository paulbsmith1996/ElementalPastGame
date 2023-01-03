using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityInteractionModel
    {
        internal String text;

        public EntityInteractionModel(String text)
        {
            this.text = text;
            this.SetUpTextComponentTree();
        }

        public void BeginInteraction()
        {

        }

        internal void SetUpTextComponentTree()
        {

        }
    }
}
