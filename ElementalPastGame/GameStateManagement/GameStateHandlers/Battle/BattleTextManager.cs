using ElementalPastGame.Common;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.Components;
using ElementalPastGame.Items.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.Items.Item;
using static ElementalPastGame.GameStateManagement.GameStateHandlers.Battle.BattleGameStateHandler;
using static System.Windows.Forms.AxHost;
using ElementalPastGame.GameObject.GameStateHandlers;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public class BattleTextManager : ITextMenuObserver
    {
        public static string ESCAPE_STRING = "ESCAPE";
        public static string CONSUME_STRING = "CONSUME";
        public static string ATTACK_STRING = "ATTACK";

        public IBattleTextManagerDelegate? textManagerDelegate;

        internal InteractableTextComponentTree? textComponents;
        internal Inventory inventory;

        public BattleTextManager(Inventory inventory)
        {
            this.inventory = inventory;
        }

        public RenderingModel GetRenderingModel()
        {
            return this.GetTextComponents().GetRenderingModel();
        }

        public void HandleKeyPressed(char keyChar, BattleState state)
        {
            switch (state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    GetTextComponents().HandleKeyPressed(keyChar);
                    break;
                case BattleState.EnemySelection:
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.EnemyTurn:
                    break;
                case BattleState.End:
                    break;
            }
        }

        public void HandleKeysDown(List<Keys> keyCodes, BattleState state)
        {
            switch (state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.EnemySelection:
                case BattleState.MoveResolution:
                    break;
                case BattleState.EnemyTurn:
                    break;
                case BattleState.End:
                    break;
            }
        }

        internal InteractableTextComponentTree GetTextComponents()
        {
            if (textComponents != null)
            {
                return textComponents;
            }

            int textBoxHeight = 125;
            GameTextBox firstBox = new("An enemy unit has spotted you.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            GameTextBox secondBox = new("Gather up and prepare to defend yourselves.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            TextComponentTreeTextBoxNode firstBoxNode = new TextComponentTreeTextBoxNode(firstBox);
            TextComponentTreeTextBoxNode secondBoxNode = new TextComponentTreeTextBoxNode(secondBox);

            TextMenu mainBattleMenu = new(false, 500, 700);
            mainBattleMenu.AddMenuObserver(this);

            List<string> inventoryContents = new();
            foreach (InventoryItemEntry entry in inventory.GetItemEntriesForType(ItemType.Consumable))
            {
                string itemOptionString = entry.item.displayName + "  (" + entry.count + ")";
                inventoryContents.Add(itemOptionString);
            }
            TextMenu inventorySubmenu = new TextMenu(inventoryContents, true, 500, 700);

            mainBattleMenu.AddTerminalOption(ATTACK_STRING);
            mainBattleMenu.AddSubOptionWithKey(inventorySubmenu, CONSUME_STRING);
            mainBattleMenu.AddTerminalOption(ESCAPE_STRING);

            firstBoxNode.SetChild(secondBoxNode);
            secondBoxNode.SetChild(mainBattleMenu);
            textComponents = new InteractableTextComponentTree(firstBoxNode);
            return textComponents;
        }

        public void MenuDidResolve(TextMenu menu, string key)
        {
            if (key.Contains(ESCAPE_STRING))
            {
                if (this.textManagerDelegate != null) {
                    this.textManagerDelegate.BattleTextManagerWantsEscape(this);
                }
            }
            else if (key.Contains(ATTACK_STRING))
            {
                if (this.textManagerDelegate != null)
                {
                    this.textManagerDelegate.BattleTextManagerWantsSwitchToState(this, BattleState.EnemySelection);
                }
            }
        }
    }
}
