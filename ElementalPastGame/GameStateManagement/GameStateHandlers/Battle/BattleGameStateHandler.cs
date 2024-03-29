﻿using System;
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
using ElementalPastGame.Items.Equipment;
using ElementalPastGame.Items.Equipment.Weapons;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.Rendering;
using ElementalPastGame.Rendering.Utility;
using ElementalPastGame.SpacesManagement.Spaces;
using ElementalPastGame.TileManagement.Utility;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.Design.AxImporter;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;
using static ElementalPastGame.Items.Equipment.Weapons.Weapon;
using static ElementalPastGame.Items.IItem;

namespace ElementalPastGame.GameStateManagement.GameStateHandlers.Battle
{
    public class BattleGameStateHandler : IGameStateHandler, ITextMenuObserver, IInteractableTextComponentTreeObserver
    {
        public enum BattleState
        {
            Start,
            MoveSelection,
            EnemySelection,
            MoveResolutionInfoDisplay,
            End,
        }

        public static string ESCAPE_STRING = "ESCAPE";
        public static string CONSUME_STRING = "CONSUME";
        public static string ATTACK_STRING = "ATTACK";

        internal Color enemySelectorColor;
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }
        internal BattleStateUtilities battleStateUtilities { get; set; }
        internal Dictionary<EntityBattleModel, InteractableTextComponentTree> moveSelectionTextComponentsByBattleModel = new Dictionary<EntityBattleModel, InteractableTextComponentTree>();
        internal InteractableTextComponentTree? moveResolutionTextComponents;

        internal BattleState state;
        internal Inventory inventory;

        internal List<EntityBattleModel> enemies;
        internal List<EntityBattleModel> allies;
        internal BattlePrioritizer battlePrioritizer;
        internal List<RenderingModel> allyRenderingModels = new();
        internal List<RenderingModel> enemyRenderingModels = new();
        internal int selectedEnemyIndex;
        internal List<int> deadEnemyIndexes = new();
        internal EntityBattleModel activeEntity;
        internal WeaponAction selectedMove;
        internal long encounterID;

        internal Bitmap background;
        internal Bitmap backDrop;
        internal Bitmap enemySelector;

        internal DateTime lastEnemySelectionInputTime;
        internal double timeSinceLastEnemySelectionMove;

        public BattleGameStateHandler(ISpace space, Inventory inventory, List<EntityBattleModel> allies, long encounterID)
        {
            state = BattleState.Start;
            this.inventory = inventory;

            this.encounterID = encounterID;
            // The battle game state handler needs to be aware of the encounterID so it can pass it back in to the 
            // overworld state handler at the end of the battle.
            this.enemies = space.enemiesForEncounterID(encounterID);
            if (enemies.Count == 0)
            {
                throw new ArgumentException("Battle must be created with at least 1 enemy.");
            }
            this.battleStateUtilities = new BattleStateUtilities(allies, enemies);
            this.selectedEnemyIndex = -1;
            this.selectedEnemyIndex = this.SeekNextAliveEnemyIndex(true);

            this.allies = allies;

            enemySelectorColor = Color.White;

            lastEnemySelectionInputTime = DateTime.Now;
            timeSinceLastEnemySelectionMove = CommonConstants.KEY_DEBOUNCE_TIME_MS;

            List<EntityBattleModel> entities = new();
            entities.AddRange(allies);
            entities.AddRange(enemies);
            this.battlePrioritizer = new BattlePrioritizer(entities);

            this.activeEntity = this.battlePrioritizer.PopNextEntityAndEnqueue();

            UpdateGameObjectRenderingModels();
        }

        internal void UpdateGameObjectRenderingModels()
        {
            allyRenderingModels = new();
            for (int allyIndex = 0; allyIndex < allies.Count; allyIndex++)
            {
                EntityBattleModel ally = allies.ElementAt(allyIndex);
                RenderingModel allyRenderingModel = ComputeRenderingModel(ally, allyIndex, false);
                allyRenderingModels.Add(allyRenderingModel);
            }

            enemyRenderingModels = new();
            for (int enemyIndex = 0; enemyIndex < enemies.Count; enemyIndex++)
            {
                EntityBattleModel enemy = enemies.ElementAt(enemyIndex);
                RenderingModel enemyRenderingModel = ComputeRenderingModel(enemy, enemyIndex, true);
                enemyRenderingModels.Add(enemyRenderingModel);
            }
        }

        internal RenderingModel ComputeRenderingModel(EntityBattleModel dataModel, int lineUpIndex, bool isEnemy)
        {
            Point entityLocation = this.battleStateUtilities.ComputeRenderLocationForLineUpIndex(lineUpIndex, isEnemy);

            double perspectiveFactor = Math.Pow(this.battleStateUtilities.ComputeSqueezeFactor(entityLocation.Y), 1.1);
            int entityWidth = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);
            int entityHeight = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            Bitmap dataModelBitmap = new Bitmap(dataModel.imageData.Image, entityWidth, entityHeight);
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
                    this.GetMoveSelectionTextComponents().HandleKeyPressed(keyChar);
                    break;
                case BattleState.EnemySelection:
                    switch (keyChar)
                    {
                        case 's':
                            this.DamageSelectedEnemies();
                            break;
                        case 'd':
                            state = BattleState.MoveSelection;
                            break;
                        default:
                            return;
                    }
                    break;
                case BattleState.MoveResolutionInfoDisplay:
                    this.moveResolutionTextComponents.HandleKeyPressed(keyChar);
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
                    this.GetMoveSelectionTextComponents().HandleKeyInputs(keyCodes);
                    break;
                case BattleState.EnemySelection:
                    UpdateEnemySelection(keyCodes);
                    break;
                case BattleState.MoveResolutionInfoDisplay:
                    this.moveResolutionTextComponents.HandleKeyInputs(keyCodes);
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

        internal int DamageSelectedEnemies()
        {
            EntityBattleModel selectedEnemyModel = enemies.ElementAt(selectedEnemyIndex);
            int damage = battleStateUtilities.ComputePhysicalDamage(this.activeEntity, selectedEnemyModel, this.selectedMove);
            selectedEnemyModel.Damage(damage);

            if (selectedEnemyModel.isDead)
            {
                int nextAliveIndex = SeekNextAliveEnemyIndex(true);
                selectedEnemyIndex = nextAliveIndex != -1 ? nextAliveIndex : SeekNextAliveEnemyIndex(false);
            }

            this.TransitionToMoveResolutionDisplay(selectedEnemyModel, damage);

            return damage;
        }

        internal void ResolveAttackOnAllies()
        {
            int randomIndex = 0;
            while (randomIndex < this.allies.Count && this.allies.ElementAt(randomIndex).isDead)
            {
                randomIndex++;
            }
            EntityBattleModel selectedAlly = this.allies.ElementAt(randomIndex);
            this.selectedMove = battleStateUtilities.SelectRandomMoveForEntityBattleModel(this.activeEntity);
            int damage = battleStateUtilities.ComputePhysicalDamage(this.activeEntity, selectedAlly, this.selectedMove);
            selectedAlly.Damage(damage);

            this.TransitionToMoveResolutionDisplay(selectedAlly, damage);
        }

        internal void TransitionToMoveResolutionDisplay(EntityBattleModel target, int damage)
        {
            if (target.isDead)
            {
                this.UpdateGameObjectRenderingModels();
            }
            this.moveResolutionTextComponents = this.GetMoveResolutionTextComponents(this.activeEntity, target, damage);
            this.state = BattleState.MoveResolutionInfoDisplay;
        }

        public bool BattleVictorious()
        {
            foreach (EntityBattleModel enemyDataModel in enemies)
            {
                if (!enemyDataModel.isDead)
                {
                    return false;
                }
            }

            return true;
        }

        internal void Escape()
        {
            if (gameStateHandlerDelegate != null)
            {
                Dictionary<String, Object> transitionDictionary = new Dictionary<String, Object>() { { GameStateTransitionConstants.BATTLE_VICTORIOUS_KEY, false } };
                gameStateHandlerDelegate.IGameStateHandlerNeedsGameStateUpdate(this, GameState.Overworld, transitionDictionary);
            }
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
            if (this.backDrop == null)
            {
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
                Bitmaps = new List<Bitmap>() { backDrop }
            };

            Bitmap shearedBackground = this.battleStateUtilities.GetBackground();

            RenderingModel shearedBackgroundRenderingModel = new()
            {
                X = BattleStateConstants.BACKGROUND_X,
                Y = BattleStateConstants.BACKGROUND_Y,
                Width = shearedBackground.Width,
                Height = shearedBackground.Height,
                Bitmaps = new List<Bitmap>() { this.battleStateUtilities.GetBackground() }
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
                    foreach (RenderingModel allyInfoBoxRenderingModels in this.GetAllyInfoBox().getRenderingModels())
                    {
                        this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyInfoBoxRenderingModels);
                    }
                    foreach (RenderingModel moveSelectionRenderingModel in this.GetMoveSelectionTextComponents().GetRenderingModels())
                    {
                        this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, moveSelectionRenderingModel);
                    }
                    break;
                case BattleState.MoveSelection:
                    if (this.gameStateHandlerDelegate != null)
                    {
                        foreach (RenderingModel allyInfoBoxRenderingModels in this.GetAllyInfoBox().getRenderingModels())
                        {
                            this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyInfoBoxRenderingModels);
                        }
                        foreach (RenderingModel moveSelectionRenderingModel in this.GetMoveSelectionTextComponents().GetRenderingModels())
                        {
                            this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, moveSelectionRenderingModel);
                        }
                    }
                    break;
                case BattleState.MoveResolutionInfoDisplay:
                    if (this.gameStateHandlerDelegate != null)
                    {
                        foreach (RenderingModel allyInfoBoxRenderingModels in this.GetAllyInfoBox().getRenderingModels())
                        {
                            this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyInfoBoxRenderingModels);
                        }
                        foreach (RenderingModel moveResolutionRenderingModel in this.moveResolutionTextComponents.GetRenderingModels())
                        {
                            this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, moveResolutionRenderingModel);
                        }
                    }
                    break;
                case BattleState.End:
                    break;
                case BattleState.EnemySelection:
                    foreach (RenderingModel allyInfoBoxRenderingModels in this.GetAllyInfoBox().getRenderingModels())
                    {
                        this.gameStateHandlerDelegate.IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, allyInfoBoxRenderingModels);
                    }
                    UpdateSelectedEnemy();
                    break;
            }
        }

        internal void UpdateSelectedEnemy()
        {
            Point selectedEnemyLocation = this.battleStateUtilities.ComputeRenderLocationForLineUpIndex(selectedEnemyIndex, true);
            double perspectiveFactor = Math.Pow(this.battleStateUtilities.ComputeSqueezeFactor(selectedEnemyLocation.Y), 1.1);
            int entityDimension = (int)(CommonConstants.BATTLE_PARTICIPANT_DIMENSION * perspectiveFactor);

            int selectorDimension = entityDimension + 2 * BattleStateConstants.ENEMY_SELECTOR_PADDING;
            Bitmap selectorBitmap = new Bitmap(selectorDimension, selectorDimension);
            Graphics graphics = Graphics.FromImage(selectorBitmap);
            Pen pen = new Pen(enemySelectorColor);
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

        internal bool IsEntityAlly(EntityBattleModel candidate)
        {
            foreach (EntityBattleModel dataModel in this.allies)
            {
                if (dataModel.Equals(candidate))
                {
                    return true;
                }
            }

            return false;
        }

        internal List<WeaponAction> GetWeaponActionsForEntityBattleModel(EntityBattleModel battleModel)
        {
            ActiveEquipment? activeEquipment = battleModel.activeEquipment;
            if (activeEquipment == null || activeEquipment.weapon == null)
            {
                return new List<WeaponAction>() { WeaponAction.Punch };
            }

            return activeEquipment.weapon.multipliersForWeaponActions.Keys.ToList();
        }

        internal InteractableTextComponentTree GetMoveSelectionTextComponents()
        {
            InteractableTextComponentTree? existingTextComponents = this.moveSelectionTextComponentsByBattleModel.GetValueOrDefault(this.activeEntity);
            if (existingTextComponents != null)
            {
                return existingTextComponents;
            }

            //GameTextBox firstBox = new("An enemy unit has spotted you.", 0, CommonConstants.GAME_DIMENSION - CommonConstants.STANDARD_TEXTBOX_HEIGHT - 4, CommonConstants.GAME_DIMENSION, CommonConstants.STANDARD_TEXTBOX_HEIGHT);
            //GameTextBox secondBox = new("Gather up and prepare to defend yourselves.", 0, CommonConstants.GAME_DIMENSION - CommonConstants.STANDARD_TEXTBOX_HEIGHT - 4, CommonConstants.GAME_DIMENSION, CommonConstants.STANDARD_TEXTBOX_HEIGHT);
            //TextComponentTreeTextBoxNode firstBoxNode = new TextComponentTreeTextBoxNode(firstBox);
            //TextComponentTreeTextBoxNode secondBoxNode = new TextComponentTreeTextBoxNode(secondBox);

            TextMenu mainBattleMenu = new(false, 500, 700);
            mainBattleMenu.AddMenuObserver(this);

            // Attack moves submenu
            List<String> weaponActions = this.GetWeaponActionsForEntityBattleModel(this.activeEntity).Select(weaponAction => weaponAction.ToString()).ToList();
            TextMenu weaponActionsSubmenu = new TextMenu(weaponActions, true, 500, 700);
            mainBattleMenu.AddSubOptionWithKey(weaponActionsSubmenu, ATTACK_STRING);

            // Inventory submenu
            List<string> inventoryContents = new();
            foreach (InventoryItemEntry entry in inventory.GetItemEntriesForType(ItemType.Consumable))
            {
                string itemOptionString = entry.item.displayName + "  (" + entry.count + ")";
                inventoryContents.Add(itemOptionString);
            }
            TextMenu inventorySubmenu = new TextMenu(inventoryContents, true, 500, 700);
            mainBattleMenu.AddSubOptionWithKey(inventorySubmenu, CONSUME_STRING);

            mainBattleMenu.AddTerminalOption(ESCAPE_STRING);

            //firstBoxNode.SetChild(secondBoxNode);
            //secondBoxNode.SetChild(mainBattleMenu);
            InteractableTextComponentTree activeComponentTree = new InteractableTextComponentTree(mainBattleMenu);
            this.moveSelectionTextComponentsByBattleModel[this.activeEntity] = activeComponentTree;
            return activeComponentTree;
        }

        internal InteractableTextComponentTree GetMoveResolutionTextComponents(EntityBattleModel actor, EntityBattleModel recipient, int damage)
        {
            String damageString = actor.characterData.name + " used " + this.selectedMove.ToString() + " and dealt " + damage + " damage to " + recipient.characterData.name + ".";
            GameTextBox enemyDamageInformationBox = new(damageString, 0, CommonConstants.GAME_DIMENSION - CommonConstants.STANDARD_TEXTBOX_HEIGHT - 4, CommonConstants.GAME_DIMENSION, CommonConstants.STANDARD_TEXTBOX_HEIGHT);
            TextComponentTreeTextBoxNode firstBoxNode = new TextComponentTreeTextBoxNode(enemyDamageInformationBox);
            this.moveResolutionTextComponents = new InteractableTextComponentTree(firstBoxNode);
            this.moveResolutionTextComponents.AddObserver(this);
            return this.moveResolutionTextComponents;
        }

        internal GameTextBox GetAllyInfoBox()
        {
            String allyInfoString = "";
            for (int allyIndex = 0; allyIndex < this.allies.Count; allyIndex++)
            {
                EntityBattleModel allyDataModel = this.allies.ElementAt(allyIndex);
                allyInfoString += allyDataModel.characterData.name + "  :  " + allyDataModel.health + "/" + allyDataModel.maxHealth;
                if (allyIndex != this.allies.Count - 1)
                {
                    allyInfoString += "\n";
                }
            }

            return new GameTextBox(allyInfoString, "Luminari", 15, 0, 0, CommonConstants.GAME_DIMENSION, 300, false);
        }

        public void MenuDidResolve(TextMenu menu, string key)
        {
            if (key.Contains(ESCAPE_STRING))
            {
                this.Escape();
            }
            else if (key.Contains(ATTACK_STRING))
            {
                String selectedWeaponAction = key.Split(TextMenu.KEY_PATH_DELIMITER).Last();
                this.selectedMove = Weapon.WeaponActionForString(selectedWeaponAction);
                this.state = BattleState.EnemySelection;
                this.moveSelectionTextComponentsByBattleModel.Remove(this.activeEntity);
            }
        }

        public void InteractableTextComponentTreeObserverDidDismiss(InteractableTextComponentTree tree)
        {
            switch(state)
            {
                case BattleState.MoveResolutionInfoDisplay:
                    this.UpdateBattleStateAfterMoveResolution();
                    break;
            }
        }

        internal void UpdateBattleStateAfterMoveResolution()
        {
            if (!BattleVictorious())
            {
                this.activeEntity = this.battlePrioritizer.PopNextEntityAndEnqueue();
                if (this.IsEntityAlly(activeEntity))
                {
                    state = BattleState.MoveSelection;
                }
                else
                {
                    this.ResolveAttackOnAllies();
                }
            }
            else
            {
                state = BattleState.End;
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
    }
}
