using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement.Utility;
using ElementalPastGame.TileManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElementalPastGame.KeyInput;
using static ElementalPastGame.GameStateManagement.IGameObjectManager;
using ElementalPastGame.GameStateManagement;
using ElementalPastGame.GameObject.EntityManagement;
using ElementalPastGame.SpacesManagement.Spaces;
using ElementalPastGame.SpacesManagement.TileManagement;
using System.DirectoryServices.ActiveDirectory;

namespace ElementalPastGame.GameObject.GameStateHandlers
{
    public class OverworldGameStateHandler : IGameStateHandler, ISpaceInteractionDelegate
    {
        public IGameStateHandlerDelegate? gameStateHandlerDelegate { get; set; }
        internal static OverworldGameStateHandler? _instance;

        internal bool _isAnimating;
        internal bool isInteracting { get; set; }
        public bool isAnimating { get { return _isAnimating; } set { _isAnimating = value; } }
        public int CenterX { get; set; }
        public int CenterY { get; set; }

        public int PreviousCenterX { get; set; }
        public int PreviousCenterY { get; set; }

        public int FramesAnimated { get; set; }
        internal DateTime lastInteractionTime = DateTime.Now;

        internal ISpace space { get; set; }

        internal RenderingModel playerRenderingModel;
        internal IGameObjectModel? activeInteractionGameObjectModel { get; set; }

        public static OverworldGameStateHandler getInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new OverworldGameStateHandler();
            return _instance;
        }

        internal OverworldGameStateHandler()
        {
            this.CenterX = CommonConstants.GAME_START_LOCATION.X;
            this.CenterY = CommonConstants.GAME_START_LOCATION.Y;
            this.PreviousCenterX = this.CenterX;
            this.PreviousCenterY = this.CenterY;

            this.space = Spaces.SpaceForIdentity(Spaces.OVERWORLD);
            this.space.interactionDelegate = this;

            this.isAnimating = false;

            // TODO: this is suppoed to represent the player so def move it out of here
            Bitmap playerBitmap = new Bitmap(TextureMapping.Mapping[TextureMapping.Player], CommonConstants.TILE_DIMENSION, CommonConstants.TILE_DIMENSION);
            List<Bitmap> blankBitmapList = new List<Bitmap>();
            blankBitmapList.Add(playerBitmap);
            this.playerRenderingModel = new()
            {
                X = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Y = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Width = CommonConstants.TILE_DIMENSION,
                Height = CommonConstants.TILE_DIMENSION,
                Bitmaps = blankBitmapList,
            };

            // TODO: probably remove this line later
            this.RedrawForNonnullDelegate();
        }

        public Boolean ValidateNewGameObjectPosition(IGameObjectModel gameObject, Location newLocation)
        {
            return this.space.LocationIsNavigable(newLocation.X, newLocation.Y);
        }

        public void HandleKeyPressed(char keyChar)
        {
            if (this.isInteracting && this.activeInteractionGameObjectModel != null)
            {
                this.activeInteractionGameObjectModel.InteractionModel.HandleKeyPressed(keyChar);
            }
        }

        public void HandleKeysDown(List<Keys> keyCodes)
        {
            DateTime startHandleKeysDown = DateTime.Now;
            this.UpdateGameState();
            double offset = 0;
            if (isAnimating)
            {
                offset = 1.0 - ((double)this.FramesAnimated / GameObjectConstants.MOVEMENT_ANIMATION_LENGTH);

                int diffY = CenterY - PreviousCenterY;
                int diffX = CenterX - PreviousCenterX;

                double animationXOffset = diffX * offset;
                double animationYOffset = diffY * offset;
                this.FramesAnimated++;

                if (this.FramesAnimated > GameObjectConstants.MOVEMENT_ANIMATION_LENGTH)
                {
                    this.FramesAnimated = 1;
                    this.isAnimating = false;
                }
                this.UpdateBackgroundWithOffset(offset);
                this.UpdateForegroundWithOffset(animationXOffset, animationYOffset);
                this.RedrawForNonnullDelegate();
                return;
            }

            if (this.space.GetTileAt(this.CenterX, this.CenterY) is PortalTile portalTile)
            {
                this.space = Spaces.SpaceForIdentity(portalTile.portalSpaceIdentity);
                this.space.interactionDelegate = this;
                this.CenterX = portalTile.portalX;
                this.CenterY = portalTile.portalY;
                this.PreviousCenterX = this.CenterX;
                this.PreviousCenterY = this.PreviousCenterY;
            }

            if (keyCodes.Count > 0 && !this.isInteracting)
            {
                Keys lastKey = keyCodes.Last();
                if (this.ValidateProposedNewLocationForKey(lastKey))
                {
                    // TODO: Figure out the last directional key pressed. As it stands, if you press another
                    // key, then you can't move.
                    this.HandleOverworldInputKey(keyCodes.Last());
                    this.UpdateBackgroundWithOffset(1.0);
                    this.UpdateForegroundWithOffset(this.CenterX - this.PreviousCenterX, this.CenterY - this.PreviousCenterY);
                }
                else
                {
                    this.UpdateBackgroundWithOffset(0.0);
                    this.UpdateForegroundWithOffset(0.0, 0.0);
                }

                if (lastKey.Equals(Keys.S))
                {
                    this.InteractIfPossible();
                }
            }
            else
            {
                if (this.isInteracting && this.activeInteractionGameObjectModel != null)
                {
                    if (this.activeInteractionGameObjectModel.InteractionModel != null) {
                        this.activeInteractionGameObjectModel.InteractionModel.HandleKeysDown(keyCodes);
                    }
                }
                this.UpdateBackgroundWithOffset(0.0);
                this.UpdateForegroundWithOffset(0.0, 0.0);
            }

            this.RedrawForNonnullDelegate();
            DateTime endHandleKeysDown = DateTime.Now;
            double timeToHandleKeysDown = (endHandleKeysDown - startHandleKeysDown).TotalMilliseconds;
            Console.WriteLine(timeToHandleKeysDown);
        }

        internal void UpdateGameState ()
        {
            foreach (IGameObjectModel gameObjectModel in this.space.GetActiveEntities(this.CenterX, this.CenterY))
            {
                if (gameObjectModel.Location.X == this.CenterX && gameObjectModel.Location.Y == this.CenterY && gameObjectModel.IsHostile)
                {
                    if (this.gameStateHandlerDelegate != null)
                    {
                        this.MoveObjectAwayFromCenter(gameObjectModel);
                        Dictionary<String, Object> transitionDictionary = new Dictionary<String, Object>() { { GameStateTransitionConstants.ENCOUNTER_ID_KEY, gameObjectModel.EntityID },
                                                                                                             { GameStateTransitionConstants.SPACE_KEY, this.space } };
                        ((IGameStateHandlerDelegate)this.gameStateHandlerDelegate).IGameStateHandlerNeedsGameStateUpdate(this, GameState.Battle, transitionDictionary);
                    }
                    return;
                }
            }
        }

        internal void InteractIfPossible()
        {
            double timeSinceLastInteraction = (DateTime.Now - this.lastInteractionTime).TotalMilliseconds;
            if (timeSinceLastInteraction < CommonConstants.INTERACTION_BUFFER_TIME)
            {
                return;
            }
            // Some sneaky math here. Really, it's broken down into centerX + (centerX - previousCenterX). The value in parentheses will eitehr be -1, 0, or 1, and should
            // equate to the coordinate "in front of" the player
            int locationX = 2 * this.CenterX - this.PreviousCenterX;
            int locationY = 2 * this.CenterY - this.PreviousCenterY;

            IGameObjectModel? gameObjectModel = this.space.ActiveEntityAt(locationX, locationY);
            if (gameObjectModel != null && gameObjectModel.InteractionModel != null)
            {
                gameObjectModel.Interact();
            }
        }

        internal void MoveObjectAwayFromCenter(IGameObjectModel gameObjectModel)
        {
            for (int xPositionModifier = -2; xPositionModifier <= 2; xPositionModifier++)
            {
                for (int yPositionModifier = -2; yPositionModifier <= 2; yPositionModifier++)
                {
                    if (xPositionModifier == 1 && yPositionModifier == 1)
                    {
                        continue;
                    }
                    Location newLocation = new()
                    {
                        X = gameObjectModel.Location.X + xPositionModifier,
                        Y = gameObjectModel.Location.Y + yPositionModifier
                    };

                    if (this.ValidateNewGameObjectPosition(gameObjectModel, newLocation))
                    {
                        gameObjectModel.MoveTo(newLocation.X, newLocation.Y, false);
                        return;
                    }
                }
            }
        }

        internal void HandleOverworldInputKey(Keys? key)
        {
            switch (key)
            {
                case Keys.Left:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX++;
                    break;
                case Keys.Right:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterX--;
                    break;
                case Keys.Up:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY++;
                    break;
                case Keys.Down:
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.CenterY--;
                    break;
            }
        }

        internal bool ValidateProposedNewLocationForKey(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    return this.space.LocationIsNavigable(this.CenterX + 1, this.CenterY);
                case Keys.Right:
                    return this.space.LocationIsNavigable(this.CenterX - 1, this.CenterY);
                case Keys.Up:
                    return this.space.LocationIsNavigable(this.CenterX, this.CenterY + 1);
                case Keys.Down:
                    return this.space.LocationIsNavigable(this.CenterX, this.CenterY - 1);
            }

            return false;
        }

        internal void UpdateBackgroundWithOffset(double offset)
        {
            DateTime startUpdateBackground = DateTime.Now;
            this.space.UpdateActiveTileSet(this.PreviousCenterX, this.PreviousCenterY, this.CenterX, this.CenterY, this.isAnimating, offset);
            DateTime endTileSetUpdate = DateTime.Now;

            this.UpdateBitmapForRenderingModelForNonnullDelegate(this.playerRenderingModel);
            DateTime endUpdateBackground = DateTime.Now;
            double timeToUpdateBackground = (endUpdateBackground - startUpdateBackground).TotalMilliseconds;
            Console.WriteLine(timeToUpdateBackground);

            double timeToUpdateTileSet = (endTileSetUpdate - startUpdateBackground).TotalMilliseconds;
            Console.WriteLine(timeToUpdateTileSet);
        }

        internal void UpdateForegroundWithOffset(double animationXOffset, double animationYOffset)
        {
            DateTime startUpdateForeground = DateTime.Now;
            foreach (IGameObjectModel gameObjectModel in this.space.GetActiveEntities(this.CenterX, this.CenterY))
            {
                gameObjectModel.UpdateModelForNewRunloop();
                RenderingModel renderingModel = this.CreateRenderingModelForGameObject(gameObjectModel, animationXOffset, animationYOffset);
                this.UpdateBitmapForRenderingModelForNonnullDelegate(renderingModel);
            }

            if (this.isInteracting && this.activeInteractionGameObjectModel != null && this.activeInteractionGameObjectModel.InteractionModel != null)
            {
                foreach (RenderingModel renderingModel in this.activeInteractionGameObjectModel.InteractionModel.GetRenderingModels())
                {
                    this.UpdateBitmapForRenderingModelForNonnullDelegate(renderingModel);
                }
            }
            DateTime endUpdateForeground = DateTime.Now;
            double timeToUpdateForeground = (endUpdateForeground - startUpdateForeground).TotalMilliseconds;
            Console.WriteLine(timeToUpdateForeground);
        }

        internal RenderingModel CreateRenderingModelForGameObject(IGameObjectModel gameObjectModel, double animationXOffset, double animationYOffset)
        {

            double gameObjectTileX = (double)(this.CenterX + CommonConstants.TILE_VIEW_DISTANCE - gameObjectModel.Location.X - gameObjectModel.XAnimationOffset - animationXOffset) * CommonConstants.TILE_DIMENSION;
            double gameObjectTileY = (double)(this.CenterY + CommonConstants.TILE_VIEW_DISTANCE - gameObjectModel.Location.Y - gameObjectModel.YAnimationOffset - animationYOffset) * CommonConstants.TILE_DIMENSION;

            List<Bitmap> bitmaps = new();
            if (gameObjectModel.imageData.Image != null)
            {
                Bitmap bitmap = new Bitmap((Bitmap)gameObjectModel.imageData.Image, CommonConstants.TILE_DIMENSION, CommonConstants.TILE_DIMENSION);
                bitmaps.Add(bitmap);
            }

            RenderingModel renderingModel = new()
            {
                X = (int)gameObjectTileX,
                Y = (int)gameObjectTileY,
                Width = CommonConstants.TILE_DIMENSION,
                Height = CommonConstants.TILE_DIMENSION,
                Bitmaps = bitmaps
            };

            return renderingModel;
        }

        internal void RedrawForNonnullDelegate()
        {
            if (this.gameStateHandlerDelegate != null)
            {
                ((IGameStateHandlerDelegate)this.gameStateHandlerDelegate).IGameStateHandlerNeedsRedraw(this);
            }
        }

        internal void UpdateBitmapForRenderingModelForNonnullDelegate(RenderingModel renderingModel)
        {
            if (this.gameStateHandlerDelegate != null)
            {
                ((IGameStateHandlerDelegate)this.gameStateHandlerDelegate).IGameStateHandlerNeedsBitmapUpdateForRenderingModel(this, renderingModel);
            }
        }

        public void TransitionFromGameState(GameState state, Dictionary<String, Object> transitionDictionary)
        {
            switch(state)
            {
                case GameState.Battle:
                    Object battleVictoriousObject = transitionDictionary.GetValueOrDefault(GameStateTransitionConstants.BATTLE_VICTORIOUS_KEY);
                    if (battleVictoriousObject != null && (bool)battleVictoriousObject)
                    {
                        Object entityIDObject = transitionDictionary.GetValueOrDefault(GameStateTransitionConstants.ENCOUNTER_ID_KEY);
                        if (entityIDObject != null)
                        {
                            this.space.MarkEntityIDDead((long)entityIDObject);
                        }
                    }
                    break;
            }
        }

        public void TransitionToGameState(GameState state, Dictionary<String, Object> transitionDictionary)
        {
            // So far we just no-op here
        }

        // ISpaceInteractionDelegate

        public void ISpaceDidBeginInteractionWithGameObjectModel(ISpace space, IGameObjectModel gameObjectModel)
        {
            this.isInteracting = true;
            this.activeInteractionGameObjectModel = gameObjectModel;
        }

        public void ISpaceDidEndInteractionWithGameObjectModel(ISpace space, IGameObjectModel gameObjectModel)
        {
            this.isInteracting = false;
            this.activeInteractionGameObjectModel = null;
            this.lastInteractionTime = DateTime.Now;
        }
    }
}
