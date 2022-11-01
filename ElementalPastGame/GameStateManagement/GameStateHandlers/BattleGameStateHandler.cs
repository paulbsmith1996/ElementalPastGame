using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.Common;
using ElementalPastGame.Components;
using ElementalPastGame.Components.ComponentSequences;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.Rendering;
using static System.Windows.Forms.Design.AxImporter;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public class BattleGameStateHandler : IGameStateHandler, ITextMenuObserver
    {
        internal enum BattleState
        {
            Start,
            MoveSelection,
            MoveResolution,
            End,
        }

        public static String ESCAPE_STRING = "ESCAPE";
        public static String CONSUME_STRING = "CONSUME";
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        internal BattleState state;
        internal InteractableTextComponentTree? textComponents;
        internal Inventory inventory;

        internal List<IGameObjectModel> enemies;
        internal List<IGameObjectModel> allies;

        public BattleGameStateHandler(Inventory inventory, List<IGameObjectModel> allies, List<IGameObjectModel> enemies)
        {
            this.state = BattleState.Start;
            this.inventory = inventory;

            this.enemies = enemies;
            this.allies = allies;
        }

        public void HandleKeyInputs(List<Keys> keyCodes)
        {
            switch (this.state)
            {
                case BattleState.Start:
                    this.GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.MoveSelection:
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }
            this.Redraw();
        }

        public void MenuDidResolve(TextMenu menu, string key)
        {
            if (key.Contains(ESCAPE_STRING))
            {
                this.Escape();
            }
        }

        internal void Escape()
        {
            if (this.gameStateHandlerDelegate != null)
            {
                ((IGameStateHandlerDelegate)this.gameStateHandlerDelegate).IGameStateHandlerNeedsGameStateUpdate(this, GameState.Overworld);
            }
        }

        internal InteractableTextComponentTree GetTextComponents()
        {
            if (this.textComponents != null)
            {
                return this.textComponents;
            }

            int textBoxHeight = 125;
            GameTextBox firstBox = new("An enemy unit has spotted you.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            GameTextBox secondBox = new("Gather up and prepare to defend yourselves.", 0, CommonConstants.GAME_DIMENSION - textBoxHeight - 4, CommonConstants.GAME_DIMENSION, textBoxHeight);
            TextComponentTreeTextBoxNode firstBoxNode = new TextComponentTreeTextBoxNode(firstBox);
            TextComponentTreeTextBoxNode secondBoxNode = new TextComponentTreeTextBoxNode(secondBox);

            TextMenu mainBattleMenu = new(false, 500, 700);
            mainBattleMenu.AddMenuObserver(this);

            List<String> inventoryContents = new();
            foreach (InventoryItemEntry entry in this.inventory.GetItemEntriesForType(ItemType.Consumable))
            {
                String itemOptionString = entry.item.displayName + "  (" + entry.count + ")";
                inventoryContents.Add(itemOptionString);
            }
            TextMenu inventorySubmenu = new TextMenu(inventoryContents, true, 500, 700);

            mainBattleMenu.AddSubOptionWithKey(inventorySubmenu, CONSUME_STRING);
            mainBattleMenu.AddTerminalOption(ESCAPE_STRING);

            firstBoxNode.SetChild(secondBoxNode);
            secondBoxNode.SetChild(mainBattleMenu);
            this.textComponents = new InteractableTextComponentTree(firstBoxNode);
            return this.textComponents;
        }

        internal void Redraw()
        {
            this.UpdateBackground();
            this.UpdateForeground();

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsRedraw(this);
            }
        }

        internal void UpdateBackground()
        {
            Bitmap emptyBitmap = new Bitmap(CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
            Graphics graphics = Graphics.FromImage(emptyBitmap);
            graphics.FillRectangle(Brushes.Black, 0, 0, CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);

            RenderingModel backgroundRenderingModel = new()
            {
                X = 0,
                Y = 0,
                Width = CommonConstants.GAME_DIMENSION,
                Height = CommonConstants.GAME_DIMENSION,
                Bitmaps = new List<Bitmap>() { emptyBitmap }
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, backgroundRenderingModel);
            }

        }

        internal void UpdateForeground()
        {
            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, this.GetTextComponents().GetRenderingModel());
            }
        }

        public void TransitionFromGameState(GameState state)
        {
            // TODO: let's figure out if we need to do anything here
        }

        public void TransitionToGameState(GameState state)
        {
            // TODO: let's figure out if we need to do anything here
        }
    }
}
