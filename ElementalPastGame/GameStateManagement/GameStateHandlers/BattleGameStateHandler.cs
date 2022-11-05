using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Configuration.Internal;
using System.Drawing;
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
        public static String ATTACK_STRING = "ATTACK";
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        internal BattleState state;
        internal InteractableTextComponentTree? textComponents;
        internal Inventory inventory;

        internal List<IGameObjectModel> enemies;
        internal List<IGameObjectModel> allies;

        internal Bitmap background;
        internal static int GAME_OBJECT_HORIZONTAL_PADDING = 150;
        internal static int BACKGROUND_WIDTH = CommonConstants.GAME_DIMENSION + 120;
        internal static int BACKGROUND_HEIGHT = 4 * CommonConstants.GAME_DIMENSION / 10;
        internal static int BACKGROUND_X = (CommonConstants.GAME_DIMENSION - BACKGROUND_WIDTH) / 2;
        internal static int BACKGROUND_Y = (CommonConstants.GAME_DIMENSION - BACKGROUND_HEIGHT) - 200;
        internal static int DEPTH = 500;
        internal static Point PERSPECTIVE = new Point(BACKGROUND_WIDTH / 2, DEPTH);

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
            else if (key.Contains(ATTACK_STRING))
            {

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

            mainBattleMenu.AddTerminalOption(ATTACK_STRING);
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
            graphics.FillRectangle(Brushes.LightBlue, 0, 0, CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);

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
            this.UpdateGameObjects();
            this.UpdateTextComponents();
        }

        internal void UpdateGameObjects()
        {
            for (int allyIndex = 0; allyIndex < this.allies.Count; allyIndex++)
            {
                IGameObjectModel ally = this.allies.ElementAt(allyIndex);
                if (ally == null)
                {
                    continue;
                }

                this.UpdateGameObject(ally, allyIndex, false);
            }

            for (int enemyIndex = 0; enemyIndex < this.enemies.Count; enemyIndex++)
            {
                IGameObjectModel enemy = this.enemies.ElementAt((int)enemyIndex);
                if (enemy == null)
                {
                    continue;
                }

                this.UpdateGameObject(enemy, enemyIndex, true);
            }
        }

        internal void UpdateGameObject(IGameObjectModel gameObjectModel, int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? this.enemies.Count : this.allies.Count;
            float gameObjectY = (((float)lineUpIndex + 1) * BACKGROUND_HEIGHT) / (lineUpCount + 1) + BACKGROUND_Y;

            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = this.ComputeBackgroundXForPoint(new() { X = GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

            Bitmap allyBitmap = new Bitmap(gameObjectModel.Image, entityWidth, entityHeight);
            List<Bitmap> bitmaps = new() { allyBitmap };

            int xOnBackground;// = gameObjectX < CommonConstants.GAME_DIMENSION / 2 ? gameObjectX + (GAME_OBJECT_HORIZONTAL_PADDING / 2) : gameObjectX - (GAME_OBJECT_HORIZONTAL_PADDING / 2) - entityWidth;
                              //xOnBackground = lineUpIndex % 2 == 0 ? gameObjectX
                              //lineUpIndex % 2 == 0 ? gameObjectX : gameObjectX < CommonConstants.GAME_DIMENSION / 2 ? gameObjectX + (GAME_OBJECT_HORIZONTAL_PADDING / 2) : gameObjectX - (GAME_OBJECT_HORIZONTAL_PADDING / 2)
            bool isEvenRank = lineUpIndex % 2 == 0;
            bool isOnLeftSide = gameObjectX < PERSPECTIVE.X;

            if (isEvenRank && isOnLeftSide)
            {
                xOnBackground = gameObjectX;
            }
            else if (isEvenRank && !isOnLeftSide)
            {
                xOnBackground = gameObjectX - entityWidth;
            }
            else if (!isEvenRank && isOnLeftSide)
            {
                xOnBackground = gameObjectX + (GAME_OBJECT_HORIZONTAL_PADDING / 2);
            }
            else
            {
                xOnBackground = gameObjectX - entityWidth - (GAME_OBJECT_HORIZONTAL_PADDING / 2);
            }

            RenderingModel allyModel = new()
            {
                X = xOnBackground,//lineUpIndex % 2 == 0 ? gameObjectX : gameObjectX < CommonConstants.GAME_DIMENSION / 2 ? gameObjectX + (GAME_OBJECT_HORIZONTAL_PADDING / 2) : gameObjectX - (GAME_OBJECT_HORIZONTAL_PADDING / 2),
                Y = (int)gameObjectY - entityHeight,//(2 * BACKGROUND_Y + BACKGROUND_HEIGHT) - (int)gameObjectY,
                Width = entityWidth,
                Height = entityHeight,
                Bitmaps = bitmaps
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyModel);
            }
        }

        internal int ComputeBackgroundXForPoint(PointF point, bool isEnemy)
        {
            float squeezeFactor = this.ComputeSqueezeFactor(point.Y);

            float newX;
            int midX = PERSPECTIVE.X;
            if (point.X <= midX)
            {
                newX = midX - ((midX - point.X) * squeezeFactor);
            }
            else
            {
                newX = midX + ((point.X - midX) * squeezeFactor);
            }
            

            return isEnemy ? CommonConstants.GAME_DIMENSION - (int)newX : (int)newX;
        }

        internal float ComputeSqueezeFactor(float y)
        {
            //float px = PERSPECTIVE.X;
            //float py = PERSPECTIVE.Y;
            //float height = BACKGROUND_HEIGHT + py;
            //float width = BACKGROUND_WIDTH;

            //float leftSqueezeEndpoint = (y + py - height) * px / height;
            //float rightSqueezeEndpoint = ((py - y) * (width - px) / height) + px;

            // return (rightSqueezeEndpoint - leftSqueezeEndpoint) / width;
            //return y / (CommonConstants.GAME_DIMENSION + PERSPECTIVE.Y - BACKGROUND_Y);
            //return (y - BACKGROUND_Y + PERSPECTIVE.Y) / (BACKGROUND_HEIGHT + PERSPECTIVE.Y);
            float normalizedY = (y - BACKGROUND_Y + PERSPECTIVE.Y);
            float normalizedHeight = (BACKGROUND_HEIGHT + PERSPECTIVE.Y);
            return normalizedY / normalizedHeight;
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
