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
using ElementalPastGame.GameObject.EntityManagement;
using ElementalPastGame.GameObject.GameStateHandlers;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.Rendering;
using ElementalPastGame.Rendering.Utility;
using ElementalPastGame.TileManagement.Utility;
using static System.Windows.Forms.Design.AxImporter;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;
using static ElementalPastGame.Items.Item;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
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

        public static string ESCAPE_STRING = "ESCAPE";
        public static string CONSUME_STRING = "CONSUME";
        public static string ATTACK_STRING = "ATTACK";

        internal Color enemySelectorColor;
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }

        internal BattleState state;
        internal InteractableTextComponentTree? textComponents;
        internal Inventory inventory;

        internal List<EntityDataModel> enemies;
        internal List<EntityDataModel> allies;
        internal PriorityQueue<EntityDataModel, int> entityPriorityQueue = new();
        internal List<RenderingModel> allyRenderingModels = new();
        internal List<RenderingModel> enemyRenderingModels = new();
        internal int selectedEnemyIndex;
        internal List<int> deadEnemyIndexes = new();
        internal EntityDataModel activeEntity;
        internal long encounterID;

        internal Bitmap background;
        internal Bitmap backDrop;
        internal Bitmap enemySelector;

        internal DateTime lastEnemySelectionInputTime;
        internal double timeSinceLastEnemySelectionMove;

        public BattleGameStateHandler(Inventory inventory, List<EntityDataModel> allies, long encounterID)
        {
            state = BattleState.Start;
            this.inventory = inventory;

            this.encounterID = encounterID;
            // The battle game state handler needs to be aware of the encounterID so it can pass it back in to the 
            // overworld state handler at the end of the battle.
            this.enemies = ActiveEntityManager.GetInstance().enemiesForEncounterID(encounterID);
            if (enemies.Count == 0)
            {
                throw new ArgumentException("Battle must be created with at least 1 enemy.");
            }
            this.selectedEnemyIndex = -1;
            this.selectedEnemyIndex = this.SeekNextAliveEnemyIndex(true);

            this.allies = allies;

            enemySelectorColor = Color.White;

            lastEnemySelectionInputTime = DateTime.Now;
            timeSinceLastEnemySelectionMove = CommonConstants.KEY_DEBOUNCE_TIME_MS;

            this.InitializeMovePriorities();
            UpdateGameObjectRenderingModels();
        }

        internal void InitializeMovePriorities()
        {
            foreach (EntityDataModel ally in this.allies)
            {
                this.entityPriorityQueue.Enqueue(ally, -ally.agility);
            }

            foreach (EntityDataModel enemy in this.enemies)
            {
                this.entityPriorityQueue.Enqueue(enemy, -enemy.agility);
            }
        }

        internal void UpdateGameObjectRenderingModels()
        {
            allyRenderingModels = new();
            for (int allyIndex = 0; allyIndex < allies.Count; allyIndex++)
            {
                EntityDataModel ally = allies.ElementAt(allyIndex);
                RenderingModel allyRenderingModel = ComputeRenderingModel(ally, allyIndex, false);
                allyRenderingModels.Add(allyRenderingModel);
            }

            enemyRenderingModels = new();
            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
                EntityDataModel enemy = enemies.ElementAt(enemyIndex);
                RenderingModel enemyRenderingModel = ComputeRenderingModel(enemy, enemyIndex, true);
                enemyRenderingModels.Add(enemyRenderingModel);
            }
        }

        internal void UpdateActiveEntity()
        {
            this.activeEntity = this.entityPriorityQueue.Dequeue();
            this.entityPriorityQueue.Enqueue(this.activeEntity, -this.activeEntity.agility);
        }

        internal RenderingModel ComputeRenderingModel(EntityDataModel dataModel, int lineUpIndex, bool isEnemy)
        {
            Point entityLocation = ComputeRenderLocationForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(ComputeSqueezeFactor(entityLocation.Y), 1.1);
            int entityWidth = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            Bitmap dataModelBitmap = new Bitmap(dataModel.Image, entityWidth, entityHeight);
            List<Bitmap> bitmaps = new() { dataModelBitmap };

            return new()
            {
                X = entityLocation.X,
                Y = entityLocation.Y,
                Width = entityWidth,
                Height = entityHeight,
                Bitmaps = bitmaps
            };
        }

        public void HandleKeyPressed(char keyChar)
        {
            switch (state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    GetTextComponents().HandleKeyPressed(keyChar);
                    break;
                case BattleState.EnemySelection:
                    switch (keyChar)
                    {
                        case 's':
                            state = BattleState.MoveResolution;
                            break;
                        case 'd':
                            state = BattleState.MoveSelection;
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
            switch (state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                    GetTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.EnemySelection:
                    UpdateEnemySelection(keyCodes);
                    break;
                case BattleState.MoveResolution:
                    ResolveAttackOnEnemies();
                    break;
                case BattleState.End:
                    if (gameStateHandlerDelegate != null)
                    {
                        Dictionary<String, Object> transitionDictionary = new Dictionary<String, Object>() { { GameStateTransitionConstants.BATTLE_VICTORIOUS_KEY, true}, 
                                                                                                             { GameStateTransitionConstants.ENCOUNTER_ID_KEY, this.encounterID} };
                        gameStateHandlerDelegate.IGameStateHandlerNeedsGameStateUpdate(this, GameState.Overworld, transitionDictionary);
                    }
                    break;
            }

            Redraw();
        }

        internal void UpdateEnemySelection(List<Keys> keyCodes)
        {
            DateTime handleKeysDownTime = DateTime.Now;
            timeSinceLastEnemySelectionMove = (handleKeysDownTime - lastEnemySelectionInputTime).TotalMilliseconds;

            if (timeSinceLastEnemySelectionMove < CommonConstants.KEY_DEBOUNCE_TIME_MS)
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
                    int backAliveIndex = SeekNextAliveEnemyIndex(false);
                    if (backAliveIndex >= 0)
                    {
                        selectedEnemyIndex = backAliveIndex;
                    }
                    break;
                case Keys.Down:
                    int nextAliveIndex = SeekNextAliveEnemyIndex(true);
                    if (nextAliveIndex >= 0)
                    {
                        selectedEnemyIndex = nextAliveIndex;
                    }
                    break;
            }

            lastEnemySelectionInputTime = handleKeysDownTime;
        }

        internal int SeekNextAliveEnemyIndex(bool forward)
        {
            int newSelectedEnemyIndex = forward ? selectedEnemyIndex + 1 : selectedEnemyIndex - 1;
            if (newSelectedEnemyIndex < 0 || newSelectedEnemyIndex >= enemies.Count)
            {
                return -1;
            }

            if (forward)
            {
                while (newSelectedEnemyIndex < enemies.Count && enemies.ElementAt(newSelectedEnemyIndex).isDead)
                {
                    newSelectedEnemyIndex++;
                }
            }
            else
            {
                while (newSelectedEnemyIndex > 0 && enemies.ElementAt(newSelectedEnemyIndex).isDead)
                {
                    newSelectedEnemyIndex--;
                }
            }

            if (newSelectedEnemyIndex < 0 || newSelectedEnemyIndex >= enemies.Count)
            {
                return -1;
            }

            return enemies.ElementAt(newSelectedEnemyIndex).isDead ? -1 : newSelectedEnemyIndex;
        }

        internal void ResolveAttackOnEnemies()
        {
            EntityDataModel selectedEnemyModel = enemies.ElementAt(selectedEnemyIndex);
            selectedEnemyModel.Damage(20);

            if (selectedEnemyModel.isDead)
            {
                int nextAliveIndex = SeekNextAliveEnemyIndex(true);
                selectedEnemyIndex = nextAliveIndex != -1 ? nextAliveIndex : SeekNextAliveEnemyIndex(false);
                UpdateGameObjectRenderingModels();
            }

            if (!BattleVictorious())
            {
                state = BattleState.MoveSelection;
            }
            else
            {
                state = BattleState.End;
            }
        }

        public bool BattleVictorious()
        {
            foreach (EntityDataModel enemyDataModel in enemies)
            {
                if (!enemyDataModel.isDead)
                {
                    return false;
                }
            }

            return true;
        }

        public void MenuDidResolve(TextMenu menu, string key)
        {
            if (key.Contains(ESCAPE_STRING))
            {
                Escape();
            }
            else if (key.Contains(ATTACK_STRING))
            {
                state = BattleState.EnemySelection;
            }
        }

        internal void Escape()
        {
            if (gameStateHandlerDelegate != null)
            {
                Dictionary<String, Object> transitionDictionary = new Dictionary<String, Object>() { { GameStateTransitionConstants.BATTLE_VICTORIOUS_KEY, false } };
                gameStateHandlerDelegate.IGameStateHandlerNeedsGameStateUpdate(this, GameState.Overworld, transitionDictionary);
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

        internal void Redraw()
        {
            UpdateBackground();
            UpdateForeground();

            if (gameStateHandlerDelegate != null)
            {
                gameStateHandlerDelegate.IGameStateHandlerNeedsRedraw(this);
            }
        }

        internal void UpdateBackground()
        {
            if (backDrop == null)
            {
                Bitmap emptyBitmap = new Bitmap(CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
                Graphics graphics = Graphics.FromImage(emptyBitmap);
                graphics.FillRectangle(Brushes.LightBlue, 0, 0, CommonConstants.GAME_DIMENSION, CommonConstants.GAME_DIMENSION);
                backDrop = emptyBitmap;
            }

            RenderingModel backgroundRenderingModel = new()
            {
                X = 0,
                Y = 0,
                Width = CommonConstants.GAME_DIMENSION,
                Height = CommonConstants.GAME_DIMENSION,
                Bitmaps = new List<Bitmap>() { backDrop }
            };

            Bitmap shearedBackground = GetBackground();

            RenderingModel shearedBackgroundRenderingModel = new()
            {
                X = BattleStateConstants.BACKGROUND_X,
                Y = BattleStateConstants.BACKGROUND_Y,
                Width = shearedBackground.Width,
                Height = shearedBackground.Height,
                Bitmaps = new List<Bitmap>() { GetBackground() }
            };

            if (gameStateHandlerDelegate != null)
            {
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, backgroundRenderingModel);
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, shearedBackgroundRenderingModel);
            }

        }

        internal void UpdateForeground()
        {
            UpdateGameObjects();

            switch (state)
            {
                case BattleState.Start:
                case BattleState.MoveSelection:
                case BattleState.MoveResolution:
                case BattleState.End:
                    UpdateTextComponents();
                    break;
                case BattleState.EnemySelection:
                    UpdateSelectedEnemy();
                    break;
            }
        }

        internal void UpdateSelectedEnemy()
        {
            Point selectedEnemyLocation = ComputeRenderLocationForLineUpIndex(selectedEnemyIndex, true);
            double perspectiveFactor = Math.Pow(ComputeSqueezeFactor(selectedEnemyLocation.Y), 1.1);
            int entityDimension = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int selectorDimension = entityDimension + 2 * BattleStateConstants.ENEMY_SELECTOR_PADDING;
            Bitmap selectorBitmap = new Bitmap(selectorDimension, selectorDimension);
            Graphics graphics = Graphics.FromImage(selectorBitmap);
            Pen pen = new Pen(enemySelectorColor);
            pen.Width = 4;
            Rectangle bounds = new Rectangle(0, 0, selectorDimension, selectorDimension);
            graphics.DrawPath(pen, GraphicsPathsFactory.RoundedRect(bounds, 10));

            //if (this.enemySelector != null)
            //{
            //    this.enemySelector.Dispose();
            //}
            //this.enemySelector = selectorBitmap;

            RenderingModel selectorRenderingModel = new()
            {
                X = selectedEnemyLocation.X - BattleStateConstants.ENEMY_SELECTOR_PADDING,
                Y = selectedEnemyLocation.Y - BattleStateConstants.ENEMY_SELECTOR_PADDING,
                Width = selectorDimension,
                Height = selectorDimension,
                Bitmaps = new List<Bitmap>() { selectorBitmap }
            };

            if (gameStateHandlerDelegate != null)
            {
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, selectorRenderingModel);
            }
        }

        internal void UpdateGameObjects()
        {
            foreach (RenderingModel allyRenderingModel in allyRenderingModels)
            {
                if (gameStateHandlerDelegate != null)
                {
                    gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyRenderingModel);
                }
            }

            foreach (RenderingModel enemyRenderingModel in enemyRenderingModels)
            {
                if (gameStateHandlerDelegate != null)
                {
                    gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, enemyRenderingModel);
                }
            }
        }

        internal float GetBackgroundYForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            int lineUpCount = isEnemy ? enemies.Count : allies.Count;
            return ((float)lineUpIndex + 1) * BattleStateConstants.BACKGROUND_HEIGHT / (lineUpCount + 1) + BattleStateConstants.BACKGROUND_Y;
        }

        internal Point ComputeRenderLocationForLineUpIndex(int lineUpIndex, bool isEnemy)
        {
            float gameObjectY = GetBackgroundYForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(ComputeSqueezeFactor(gameObjectY), 1.1);
            int entityWidth = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int gameObjectX = ComputeBackgroundXForPoint(new() { X = BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING, Y = gameObjectY }, isEnemy);

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
                xOnBackground = gameObjectX + BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2;
            }
            else
            {
                xOnBackground = gameObjectX - entityWidth - BattleStateConstants.GAME_OBJECT_HORIZONTAL_PADDING / 2;
            }

            return new Point() { X = xOnBackground, Y = (int)gameObjectY - entityHeight };
        }

        internal int ComputeBackgroundXForPoint(PointF point, bool isEnemy)
        {
            float squeezeFactor = ComputeSqueezeFactor(point.Y);

            float newX;
            int midX = BattleStateConstants.PERSPECTIVE.X;
            if (point.X <= midX)
            {
                newX = midX - (midX - point.X) * squeezeFactor;
            }
            else
            {
                newX = midX + (point.X - midX) * squeezeFactor;
            }


            return isEnemy ? CommonConstants.GAME_DIMENSION - (int)newX : (int)newX;
        }

        internal float ComputeSqueezeFactor(float y)
        {
            float normalizedY = y - BattleStateConstants.BACKGROUND_Y + BattleStateConstants.PERSPECTIVE.Y;
            float normalizedHeight = BattleStateConstants.BACKGROUND_HEIGHT + BattleStateConstants.PERSPECTIVE.Y;
            return normalizedY / normalizedHeight;
        }

        internal void UpdateTextComponents()
        {
            if (gameStateHandlerDelegate != null)
            {
                gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, GetTextComponents().GetRenderingModel());
            }
        }

        public void TransitionFromGameState(GameState state, Dictionary<String, Object> transitionDictionary)
        {
            // TODO: let's figure out if we need to do anything here
        }

        public void TransitionToGameState(GameState state, Dictionary<String, Object> transitionDictionary)
        {
            // TODO: let's figure out if we need to do anything here
        }

        //internal Bitmap GetBackground(Bitmap bitmap, Point perspective)
        internal Bitmap GetBackground()
        {
            if (background != null)
            {
                return background;
            }

            background = TextureFactory.TessalatedTexture(TextureMapping.Mapping[TextureMapping.Dirt], BattleStateConstants.BACKGROUND_WIDTH, BattleStateConstants.BACKGROUND_HEIGHT, BattleStateConstants.PERSPECTIVE);
            return background;
        }
    }
}
