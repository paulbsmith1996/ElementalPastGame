using ElementalPastGame.Components;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.SpacesManagement.Spaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;

namespace ElementalPastGame.GameObject.NPC
{
    public class Merchant : GameObjectModel, IGameObjectModel
    {

        internal Inventory inventory;
        public Merchant(ISpace space, int X, int Y, List<String> introText, Inventory inventory) : base(EntityType.BasicMerchant, space, X, Y, MovementType.Still, true, false)
        {
            this.inventory = inventory;
            this.InteractionModel = this.InteractionModelForTextAndInventory(introText, inventory);
        }

        internal EntityInteractionModel InteractionModelForTextAndInventory(List<String> introText, Inventory inventory)
        {
            ITextComponentTree? textComponentTree = null;
            TextComponentTreeTextBoxNode? previousNode = null;
            for (int introIndex = 0; introIndex < introText.Count; introIndex++)
            {
                GameTextBox textBox = new GameTextBox(introText.ElementAt(introIndex), EntityInteractionModel.INTERACTION_TEXTBOX_X, EntityInteractionModel.INTERACTION_TEXTBOX_Y, EntityInteractionModel.INTERACTION_TEXTBOX_WIDTH, EntityInteractionModel.INTERACTION_TEXTBOX_HEIGHT);
                TextComponentTreeTextBoxNode treeNode = new TextComponentTreeTextBoxNode(textBox);
                if (introIndex == 0)
                {
                    textComponentTree = treeNode;
                }
                else
                {
                    // Note that previous node should never be nil here anyways, since it should have gotten set on the first iteration of the for loop.
                    if (previousNode != null) {
                        previousNode.SetChild(treeNode);
                    }
                }
                previousNode = treeNode;
            }

            TextMenu shopMenu = this.MenuForInventory(inventory);
            if (textComponentTree == null)
            {
                return new EntityInteractionModel(shopMenu);
            }
            else if (previousNode != null)
            {
                previousNode.SetChild(shopMenu);
                return new EntityInteractionModel(textComponentTree);
            }

            List<String> comicalFailure = new List<String>() { "I don't have anything to sell, I have no nice platitudes to introduce myself with, and I'm quite sorry about that. Why don't", "you ask the developer to double-check his work before he shows off his crappily-designed world..." };
            return new EntityInteractionModel(comicalFailure);
        }

        internal TextMenu MenuForInventory(Inventory inventory)
        {
            List<InventoryItemEntry> allItemEntries = inventory.AllItemEntries();
            List<String> options = allItemEntries.Select((entry, index) => entry.item.displayName + " " + entry.count + " " + entry.item.basePrice).ToList();
            return new TextMenu(options, true, EntityInteractionModel.INTERACTION_TEXTBOX_X, EntityInteractionModel.INTERACTION_TEXTBOX_Y - 50);
        }
    }
}
