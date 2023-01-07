using ElementalPastGame.Common;
using ElementalPastGame.Components;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.GameObject.Entities
{
    public class EntityInteractionModel : IEntityInteractionModel, IInteractableTextComponentTreeObserver, IKeyEventSubscriber
    {
        internal static int INTERACTION_TEXTBOX_WIDTH = CommonConstants.GAME_DIMENSION;
        internal static int INTERACTION_TEXTBOX_X = 0;

        internal static int INTERACTION_TEXTBOX_HEIGHT = 150;
        internal static int INTERACTION_TEXTBOX_Y = CommonConstants.GAME_DIMENSION - INTERACTION_TEXTBOX_HEIGHT;

        internal List<String> text;
        internal InteractableTextComponentTree interactableTextTree { get; set; }

        public IEntityInteractionModelDelegate interactionDelegate { get; set; }

        public EntityInteractionModel(List<String> text)
        {
            this.text = text;
            this.SetUpTextComponentTree();
        }

        public void BeginInteraction()
        {
            if (this.interactionDelegate != null)
            {
                this.interactionDelegate.IEntityInteractionModelDidBeginInteraction(this);
            }
        }

        internal void EndInteraction()
        {
            if (this.interactionDelegate != null)
            {
                this.interactionDelegate.IEntityInteractionModelDidEndInteraction(this);
            }
        }

        public List<RenderingModel> GetRenderingModels()
        {
            return this.interactableTextTree.GetRenderingModels();
        }

        internal void SetUpTextComponentTree()
        {
            if (this.text.Count == 0)
            {
                return;
            }
            GameTextBox firstBox = new(this.text[0], INTERACTION_TEXTBOX_X, INTERACTION_TEXTBOX_Y, INTERACTION_TEXTBOX_WIDTH, INTERACTION_TEXTBOX_HEIGHT);
            TextComponentTreeTextBoxNode tree = new(firstBox);
            this.interactableTextTree = new(tree);
            if (this.text.Count < 2)
            {
                return;
            }

            TextComponentTreeTextBoxNode curTree = tree;
            for (int textIndex = 1; textIndex < this.text.Count; textIndex++)
            {
                GameTextBox nextBox = new(this.text[textIndex], INTERACTION_TEXTBOX_X, INTERACTION_TEXTBOX_Y, INTERACTION_TEXTBOX_WIDTH, INTERACTION_TEXTBOX_HEIGHT);
                TextComponentTreeTextBoxNode nextTree = new(nextBox);
                curTree.SetChild(nextTree);
                curTree = nextTree;
            }

            this.interactableTextTree.AddObserver(this);
        }

        public void InteractableTextComponentTreeObserverDidDismiss(InteractableTextComponentTree tree)
        {
            this.EndInteraction();
            this.SetUpTextComponentTree();
        }

        public void HandleKeysDown(List<Keys> keyCodes)
        {
            this.interactableTextTree.HandleKeyInputs(keyCodes);
        }

        public void HandleKeyPressed(char keyChar)
        {
            this.interactableTextTree.HandleKeyPressed(keyChar);
        }
    }
}
