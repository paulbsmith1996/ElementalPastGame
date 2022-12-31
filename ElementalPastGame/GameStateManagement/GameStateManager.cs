using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject.GameStateHandlers;
using ElementalPastGame.GameStateManagement.GameStateHandlers.Battle;
using ElementalPastGame.Items.Equipment;
using ElementalPastGame.Items.Equipment.Weapons;
using ElementalPastGame.Items.Inventory;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.SpacesManagement.Spaces;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;

namespace ElementalPastGame.GameStateManagement
{
    public class GameStateManager : IGameObjectManager, IKeyEventSubscriber, IGameStateHandlerDelegate
    {
        internal static GameStateManager? _instance;
        internal IPictureBoxManager pictureBoxManager;

        internal IGameStateHandler overworldGameStateHandler = OverworldGameStateHandler.getInstance();
        internal IGameStateHandler currentGameStateHandler;

        internal GameState previousGameState;
        internal GameState gameState;

        public static IGameObjectManager getInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new GameStateManager(PictureBoxManager.GetInstance());
            return _instance;
        }

        internal GameStateManager(IPictureBoxManager pictureBoxManager)
        {
            this.currentGameStateHandler = this.overworldGameStateHandler;
            this.currentGameStateHandler.gameStateHandlerDelegate = this;
            this.previousGameState = this.gameState = GameState.Overworld;

            this.pictureBoxManager = pictureBoxManager;

            IKeyEventPublisher keyEventPublisher = KeyEventPublisher.GetInstance();
            keyEventPublisher.AddIKeyEventSubscriber(this);

            // TODO: probably remove this line later
            this.pictureBoxManager.Redraw();
        }

        public void HandleKeyPressed(char keyChar)
        {
            this.currentGameStateHandler.HandleKeyPressed(keyChar);
        }

        // IKeyEventSubscriber
        public void HandleKeysDown(List<Keys> keyCodes)
        {
            this.currentGameStateHandler.HandleKeysDown(keyCodes);
        }

        public void IGameStateHandlerNeedsRedraw(IGameStateHandler gameStateHandler)
        {
            pictureBoxManager.Redraw();
        }

        public void IGameStateHandlerNeedsBitmapUpdateForRenderingModel(IGameStateHandler gameStateHandler, RenderingModel renderingModel)
        {
            pictureBoxManager.UpdateBitmapForIRenderingModel(renderingModel);
        }

        public void IGameStateHandlerNeedsGameStateUpdate(IGameStateHandler gameStateHandler, GameState gameState, Dictionary<String, Object> transitionDictionary)
        {
            gameStateHandler.gameStateHandlerDelegate = null;
            this.previousGameState = this.gameState;
            this.gameState = gameState;

            this.currentGameStateHandler.TransitionToGameState(this.gameState, transitionDictionary);

            switch (gameState)
            {
                case GameState.Overworld:
                    this.currentGameStateHandler = this.overworldGameStateHandler;
                    break;
                case GameState.Battle:
                    Object? encounterIDObject = transitionDictionary.GetValueOrDefault(GameStateTransitionConstants.ENCOUNTER_ID_KEY);
                    long encounterID = -1;
                    if (encounterIDObject != null)
                    {
                        encounterID = (long)encounterIDObject;
                    }

                    Object? spaceObject = transitionDictionary.GetValueOrDefault(GameStateTransitionConstants.SPACE_KEY);
                    ISpace space = Spaces.SpaceForIdentity(Spaces.OVERWORLD);
                    if (spaceObject != null && spaceObject != Spaces.SpaceForIdentity(Spaces.OVERWORLD))
                    {
                        space = (ISpace)spaceObject;
                    }

                    EntityBattleModel swordBattleModel = new EntityBattleModel(EntityType.Aendon, 4);
                    ActiveEquipment swordActiveEquipment = new ActiveEquipment();
                    swordActiveEquipment.weapon = new WoodSword();
                    swordBattleModel.activeEquipment = swordActiveEquipment;

                    EntityBattleModel daggerBattleModel = new EntityBattleModel(EntityType.Aendon, 4);
                    ActiveEquipment daggerActiveEquipment = new ActiveEquipment();
                    daggerActiveEquipment.weapon = new BronzeDagger();
                    daggerBattleModel.activeEquipment = daggerActiveEquipment;

                    List<EntityBattleModel> allies = new() { swordBattleModel, daggerBattleModel };
                    this.currentGameStateHandler = new BattleGameStateHandler(space, Inventory.DebugInventory(), allies, encounterID);
                    break;
            }

            this.currentGameStateHandler.gameStateHandlerDelegate = this;

            this.currentGameStateHandler.TransitionFromGameState(this.previousGameState, transitionDictionary);

        }
    }
}
