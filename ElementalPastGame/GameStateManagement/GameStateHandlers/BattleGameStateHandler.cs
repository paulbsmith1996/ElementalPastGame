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
using ElementalPastGame.GameStateManagement.GameStateHandlers;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.Rendering;
using ElementalPastGame.Rendering.Utility;
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

        internal Color enemySelectorColor;
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        internal BattleState state;
        internal InteractableTextComponentTree? textComponents;
        internal Inventory inventory;

        internal List<EntityDataModel> enemies;
        internal List<EntityDataModel> allies;
        internal int selectedEnemyIndex;

        internal Bitmap background;
        internal Bitmap backDrop;

        internal DateTime lastEnemySelectionInputTime;
        internal double timeSinceLastEnemySelectionMove;

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

            this.enemySelectorColor = Color.White;

            this.lastEnemySelectionInputTime = DateTime.Now;
            this.timeSinceLastEnemySelectionMove = CommonConstants.KEY_DEBOUNCE_TIME_MS;
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
                            this.state = BattleState.MoveResolution;
                            break;
                        case 'd':
                            this.state = BattleState.MoveSelection;
                            break;
                        default:
                            return;
                    }
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }
        }

        public void HandleKeysDown(List<Keys> keyCodes)
        {
            switch (this.state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    this.GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.EnemySelection:
                    this.UpdateEnemySelection(keyCodes);
                    break;
                case BattleState.MoveResolution:
                    break;
                case BattleState.End:
                    break;
            }

            this.Redraw();
        }

        internal void UpdateEnemySelection(List<Keys> keyCodes)
        {
            DateTime handleKeysDownTime = DateTime.Now;
            this.timeSinceLastEnemySelectionMove = (handleKeysDownTime - this.lastEnemySelectionInputTime).TotalMilliseconds;

            if (this.timeSinceLastEnemySelectionMove < CommonConstants.KEY_DEBOUNCE_TIME_MS)
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

            this.lastEnemySelectionInputTime = handleKeysDownTime;
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
            if (this.backDrop == null) {
                Bitmap emptyBitmap = new Bitmap(CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
                Graphics graphics = Graphics.FromImage(emptyBitmap);
                graphics.FillRectangle(Brushes.LightBlue, 0, 0, CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
                this.backDrop = emptyBitmap;
            }

            RenderingModel backgroundRenderingModel = new()
            {
                X = 0,
                Y = 0,
                Width = CommonConstants.GAME_DIMENSION,
                Height = CommonConstants.GAME_DIMENSION,
                Bitmaps = new List<Bitmap>() { this.backDrop }
            };

            Bitmap shearedBackground = this.GetBackground();

            RenderingModel shearedBackgroundRenderingModel = new()
            {
                X = BattleStateConstants.BACKGROUND_X,
                Y = BattleStateConstants.BACKGROUND_Y,
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
            Point selectedEnemyLocation = this.ComputeRenderLocationForLineUpIndex(this.selectedEnemyIndex, true);
            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(selectedEnemyLocation.Y), 1.1);
            int entityDimension = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int selectorDimension = entityDimension + 2 * BattleStateConstants.ENEMY_SELECTOR_PADDING;
            Bitmap selectorBitmap = new Bitmap(selectorDimension, selectorDimension);
            Graphics graphics = Graphics.FromImage(selectorBitmap);
            Pen pen = new Pen(this.enemySelectorColor);
            pen.Width = 4;
            Rectangle bounds = new Rectangle(0, 0, selectorDimension, selectorDimension);
            graphics.DrawPath(pen, GraphicsPathsFactory.RoundedRect(bounds, 10));

            RenderingModel selectorRenderingModel = new()
            {
                X = selectedEnemyLocation.X - BattleStateConstants.ENEMY_SELECTOR_PADDING,
                Y = selectedEnemyLocation.Y - BattleStateConstants.ENEMY_SELECTOR_PADDING,
                Width = selectorDimension,
                Height = selectorDimension,
                Bitmaps = new List<Bitmap>() { selectorBitmap }
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, selectorRenderingModel);
            }
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
            Point entityLocation = this.ComputeRenderLocationForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(entityLocation.Y), 1.1);
            int entityWidth = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            Bitmap allyBitmap = new Bitmap(gameObjectModel.Image, entityWidth, entityHeight);
            List<Bitmap> bitmaps = new() { allyBitmap };

            RenderingModel allyModel = new()
            {
                X = entityLocation.X,
                Y = entityLocation.Y,
                Width = entityWidth,
                Height = entityHeight,
                Bitmaps = bitmaps
            };

            if (this.gameStateHandlerDelegate != null)
            {
                this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyModel);
            }
        }

        internal float GetBackgroundYForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? this.enemies.Count : this.allies.Count;
            return (((float)lineUpIndex + 1) * BattleStateConstants.BACKGROUND_HEIGHT) / (lineUpCount + 1) + BattleStateConstants.BACKGROUND_Y;
        }

        internal Point ComputeRenderLocationForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            float gameObjectY = this.GetBackgroundYForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(this.ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)((float)CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = this.ComputeBackgroundXForPoint(new() { X = BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

            int xOnBackground;
            bool isEvenRank = lineUpIndex % 2 == 0;
            bool isOnLeftSide = gameObjectX < BattleStateConstants.PERSPECTIVE.X;

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
                xOnBackground = gameObjectX + (BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2);
            }
            else
            {
                xOnBackground = gameObjectX - entityWidth - (BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2);
            }

            return new Point() { X = xOnBackground, Y = (int)gameObjectY - entityHeight };
        }

        internal int ComputeBackgroundXForPoint(PointF point, bool isEnemy)
        {
            float squeezeFactor = this.ComputeSqueezeFactor(point.Y);

            float newX;
            int midX = BattleStateConstants.PERSPECTIVE.X;
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
            float normalizedY = (y - BattleStateConstants.BACKGROUND_Y + BattleStateConstants.PERSPECTIVE.Y);
            float normalizedHeight = (BattleStateConstants.BACKGROUND_HEIGHT + BattleStateConstants.PERSPECTIVE.Y);
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

            this.background = TextureFactory.TessalatedTexture(TextureMapping.Mapping[TextureMapping.Dirt], BattleStateConstants.BACKGROUND_WIDTH, BattleStateConstants.BACKGROUND_HEIGHT, BattleStateConstants.PERSPECTIVE);
            return this.background;
        }
    }
}
