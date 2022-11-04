using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
using ElementalPastGame.TileManagement.Utility;
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

        internal Bitmap background;
        internal static int BACKGROUND_WIDTH = 9 * CommonConstants.GAME_DIMENSION / 10;
        internal static int BACKGROUND_HEIGHT = 4 * CommonConstants.GAME_DIMENSION / 10;
        internal static int BACKGROUND_X = (CommonConstants.GAME_DIMENSION - BACKGROUND_WIDTH) / 2;
        internal static int BACKGROUND_Y = (CommonConstants.GAME_DIMENSION - BACKGROUND_HEIGHT) / 2;
        internal static Point PERSPECTIVE = new Point(BACKGROUND_WIDTH / 2, 2000);

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

            //Point perspective = new Point(CommonConstants.GAME_DIMENSION / 2, 700);
            ////Bitmap shearedEmptyBitmap = this.GetBackground(emptyBitmap, perspective);
            //Bitmap shearedBackground = TextureFactory.TessalatedTexture(TextureMapping.Mapping[TextureMapping.Dirt], CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION / 2, perspective);

            RenderingModel backgroundRenderingModel = new()
            {
                X = 0,
                Y = 0,
                Width = CommonConstants.GAME_DIMENSION,
                Height = CommonConstants.GAME_DIMENSION,
                Bitmaps = new List<Bitmap>() { emptyBitmap }
            };

            Bitmap shearedBackground = this.GetBackground();

            RenderingModel shearedBackgroundRenderingModel = new()
            {
                X = BACKGROUND_X,
                Y = BACKGROUND_Y,
                Width = shearedBackground.Width,
                Height = shearedBackground.Height,
                Bitmaps = new List<Bitmap>() { this.GetBackground() }
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, backgroundRenderingModel);
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, shearedBackgroundRenderingModel);
            }

        }

        internal void UpdateForeground()
        {
            this.UpdateAllies();
            this.UpdateTextComponents();
        }

        internal void UpdateAllies()
        {
            for (float allyIndex = 0; allyIndex < this.allies.Count; allyIndex++)
            {
                IGameObjectModel ally = this.allies.ElementAt((int)allyIndex);
                if (ally == null)
                {
                    continue;
                }

                int allyXPadding = 50;
                float allyY = ((allyIndex + 1) * BACKGROUND_HEIGHT) / (this.allies.Count) + BACKGROUND_Y;

                float px = PERSPECTIVE.X;
                float py = PERSPECTIVE.Y;
                float height = BACKGROUND_HEIGHT + py;
                float width = BACKGROUND_WIDTH;

                float leftSqueezeEndpoint = (allyY + py - height) * px / height;
                float rightSqueezeEndpoint = ((py - allyY) * (width - px) / height) + px;

                float squeezeFactor = (rightSqueezeEndpoint - leftSqueezeEndpoint) / width;

                float newX;
                if (allyXPadding <= px)
                {
                    newX = px - ((px - allyXPadding) * squeezeFactor);
                }
                else
                {
                    newX = px + ((allyXPadding - px) * squeezeFactor);
                }

                double perspectiveFactor = Math.Pow(squeezeFactor, 1.3);
                int allyWidth = 2 * (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
                int allyHeight = 2 * (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

                Bitmap allyBitmap = new Bitmap(ally.Image, allyWidth, allyHeight);
                List<Bitmap> bitmaps = new() { allyBitmap };

                RenderingModel allyModel = new()
                {
                    X = allyIndex % 2 == 0 ? (int)newX : (int)newX + (allyXPadding / 2),
                    Y = (2 * BACKGROUND_Y + BACKGROUND_HEIGHT) - (int)allyY,
                    Width = allyWidth,
                    Height = allyHeight,
                    Bitmaps = bitmaps
                };

                if (this.gameStateHandlerDelegate != null)
                {
                    this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyModel);
                }
            }
        }

        internal void UpdateTextComponents()
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

        //internal Bitmap GetBackground(Bitmap bitmap, Point perspective)
        internal Bitmap GetBackground()
        {
            if (this.background != null)
            {
                return this.background;
            }

            this.background =  TextureFactory.TessalatedTexture(TextureMapping.Mapping[TextureMapping.Dirt], BACKGROUND_WIDTH, BACKGROUND_HEIGHT, PERSPECTIVE);
            return this.background;
        }
    }
}
