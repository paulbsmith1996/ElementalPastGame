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
using ElementalPastGame.GameObject.Entities;
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
            EnemySelection,
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

        internal List<EntityDataModel> enemies;
        internal List<EntityDataModel> allies;
        internal int selectedEnemyIndex;

        internal Bitmap background;
        internal static int GAME_OBJECT_HORIZONTAL_PADDING = 150;
        internal static int BACKGROUND_WIDTH = CommonConstants.GAME_DIMENSION + 120;
        internal static int BACKGROUND_HEIGHT = 4 * CommonConstants.GAME_DIMENSION / 10;
        internal static int BACKGROUND_X = (CommonConstants.GAME_DIMENSION - BACKGROUND_WIDTH) / 2;
        internal static int BACKGROUND_Y = (CommonConstants.GAME_DIMENSION - BACKGROUND_HEIGHT) - 200;
        internal static int DEPTH = 500;
        internal static Point PERSPECTIVE = new Point(BACKGROUND_WIDTH / 2, DEPTH);

        internal DateTime lastInputTime;

        public BattleGameStateHandler(Inventory inventory, List<EntityDataModel> allies, List<EntityDataModel> enemies)
        {
            this.state = BattleState.Start;
            this.inventory = inventory;

            if (enemies.Count == 0) {
                throw new ArgumentException("Battle must be created with at least 1 enemy.");
            }

            this.enemies = enemies;
            this.selectedEnemyIndex = 0;

            this.allies = allies;

            this.lastInputTime = DateTime.Now;
        }

        public void HandleKeyPressed(char keyChar)
        {
            switch (this.state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    this.GetTextComponents().HandleKeyPressed(keyChar);
                    break;
                case BattleState.EnemySelection:
                    switch (keyChar)
                    {
                        case 's':
                            break;
                        case 'd':
                            break;
                        default:
                            return;
                    }
                    this.state = BattleState.MoveSelection;
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }
        }

        //internal void UpdateEnemySelection(char keyChar)
        //{
        //    switch (keyChar)
        //    {
        //        case 's':
        //            break;
        //        case 'd':
        //            break;
        //        default:
        //            return;
        //    }
        //}

        public void HandleKeysDown(List<Keys> keyCodes)
        {
            DateTime handleKeysDownTime = DateTime.Now;
            double timeSinceLastInput = (handleKeysDownTime - this.lastInputTime).TotalMilliseconds;
            if (keyCodes.Count > 0) {
                this.lastInputTime = handleKeysDownTime;
            }

            switch (this.state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    this.GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.EnemySelection:
                    this.UpdateEnemySelection(keyCodes, timeSinceLastInput);
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }

            this.Redraw();
        }

        internal void UpdateEnemySelection(List<Keys> keyCodes, double timeSinceLastInput)
        {
            if (timeSinceLastInput < CommonConstants.KEY_DEBOUNCE_TIME_MS)
            {
                return;
            }

            if (keyCodes.Count == 0)
            {
                return;
            }

            Keys lastKey = keyCodes.Last();

            switch (lastKey)
            {
                case Keys.Up:
                    if (this.selectedEnemyIndex > 0)
                    {
                        this.selectedEnemyIndex--;
                    }
                    break;
                case Keys.Down:
                    if (this.selectedEnemyIndex < this.enemies.Count - 1)
                    {
                        this.selectedEnemyIndex++;
                    }
                    break;
            }
        }

        public void MenuDidResolve(TextMenu menu, string key)
        {
            if (key.Contains(ESCAPE_STRING))
            {
                this.Escape();
            }
            else if (key.Contains(ATTACK_STRING))
            {
                this.state = BattleState.EnemySelection;
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

            switch (this.state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                case BattleState.MoveResolution:
                case BattleState.End:
                    this.UpdateTextComponents();
                    break;
                case BattleState.EnemySelection:
                    this.UpdateSelectedEnemy();
                    break;
            }
        }

        internal void UpdateSelectedEnemy()
        {

        }

        internal void UpdateGameObjects()
        {
            for (int allyIndex = 0; allyIndex < this.allies.Count; allyIndex++)
            {
                EntityDataModel ally = this.allies.ElementAt(allyIndex);
                if (ally == null)
                {
                    continue;
                }

                this.UpdateGameObject(ally, allyIndex, false);
            }

            for (int enemyIndex = 0; enemyIndex < this.enemies.Count; enemyIndex++)
            {
                EntityDataModel enemy = this.enemies.ElementAt((int)enemyIndex);
                if (enemy == null)
                {
                    continue;
                }

                this.UpdateGameObject(enemy, enemyIndex, true);
            }
        }

        internal void UpdateGameObject(EntityDataModel gameObjectModel, int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? this.enemies.Count : this.allies.Count;
            float gameObjectY = (((float)lineUpIndex + 1) * BACKGROUND_HEIGHT) / (lineUpCount + 1) + BACKGROUND_Y;

            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = this.ComputeBackgroundXForPoint(new() { X = GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

            Bitmap allyBitmap = new Bitmap(gameObjectModel.Image, entityWidth, entityHeight);
            List<Bitmap> bitmaps = new() { allyBitmap };

            int xOnBackground;
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
                X = xOnBackground,
                Y = (int)gameObjectY - entityHeight,
                Width = entityWidth,
                Height = entityHeight,
                Bitmaps = bitmaps
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyModel);
            }
        }

        internal Point ComputeRenderLocationForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? this.enemies.Count : this.allies.Count;
            float gameObjectY = (((float)lineUpIndex + 1) * BACKGROUND_HEIGHT) / (lineUpCount + 1) + BACKGROUND_Y;

            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = this.ComputeBackgroundXForPoint(new() { X = GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

            int xOnBackground;
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

            return new Point() { X = xOnBackground, Y = (int)gameObjectY - entityHeight };
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
