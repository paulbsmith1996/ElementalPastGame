using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public interface IEntityInteractionModel
    {
        public void BeginInteraction();

        public List<RenderingModel> GetRenderingModels();

        public IEntityInteractionModelDelegate interactionDelegate { get; set;}
    }

    public interface IEntityInteractionModelDelegate
    {
        public void IEntityInteractionModelDidBeginInteraction(IEntityInteractionModel interactionModel);
        public void IEntityInteractionModelDidEndInteraction(IEntityInteractionModel interactionModel);
    }
}
