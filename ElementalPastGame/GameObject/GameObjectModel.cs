﻿using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject.EntityManagement;
using ElementalPastGame.GameObject.GameStateHandlers;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Rendering;
using ElementalPastGame.SpacesManagement.Spaces;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;

namespace ElementalPastGame.GameObject
{
    public partial class GameObjectModel : IGameObjectModel, IEntityInteractionModelDelegate
    {
        internal ISpace space;
        public Location Location { get; set; }
        public GameObjectSize Size { get; set; }

        public long EntityID { get; set; }
        public Boolean IsCollidable { get; set; }
        internal EntityInteractionModel? _interactionModel;
        public EntityInteractionModel? InteractionModel { get
            {
                return _interactionModel;
            } set {
                if (_interactionModel != value)
                {
                    _interactionModel = value;
                    if (_interactionModel != null)
                    {
                        _interactionModel.interactionDelegate = this;
                    }
                }
            } 
        }
        public IGameObjectModelInteractionDelegate interactionDelegate { get; set; }
        public bool IsHostile { get; set; }
        public double XAnimationOffset { get; set; }
        public double YAnimationOffset { get; set; }

        internal int framesAnimated { get; set; }
        internal bool isAnimating { get; set; }
        public MovementType movementType { get; set; }
        public List<Direction> Moves { get; set; }
        public bool shouldCycleMoves { get; set; }
        public EntityImageData imageData { get; set; }

        internal int currentMoveIndex = 0;
        internal int runloopsSinceLastRandomOrAggressiveMove = 0;

        internal Random rng = new Random();

        public GameObjectModel(EntityType type, ISpace space, int X, int Y) : this(type, space, X, Y, MovementType.Wander, false, false)
        {
        }

        public GameObjectModel(EntityType type, ISpace space, int X, int Y, MovementType movementType) : this(type, space, X, Y, movementType, false, false)
        {
        }

        public GameObjectModel(EntityType type, ISpace space, int X, int Y, bool isCollidable) : this(type, space, X, Y, MovementType.Wander, isCollidable, false)
        {
        }

        public GameObjectModel(EntityType type, ISpace space, int X, int Y, MovementType movementType, bool isCollidable, bool isHostile, EntityInteractionModel? interactionModel=null)
        {
            this.space = space;
            this.interactionDelegate = this.space;
            this.movementType = movementType;
            this.IsCollidable = isCollidable;
            this.InteractionModel = interactionModel;
            this.IsHostile = isHostile;
            this.Moves = new();
            this.Location = new Location() { X = X, Y = Y };
            this.imageData = new EntityImageData(type);

            // Use Szudik's function to uniquely hash every X,Y pair to a unique entity ID
            this.EntityID = X >= Y ? X * X + X + Y : X + Y * Y;

            // TODO: we don't necessarily need to load everything now
            this.LoadIfNeeded();
        }

        public void LoadIfNeeded()
        {
            this.imageData.Load();
        }

        public void Unload()
        {
            this.imageData.Image = null;
        }

        public void MoveTo(int NewX, int NewY, bool isAnimated)
        {
            Location newLocation = new()
            {
                X = NewX,
                Y = NewY,
            };

            Location previousLocation = this.Location;

            if (isAnimated)
            {
                this.isAnimating = true;
            }

            if (!OverworldGameStateHandler.getInstance().ValidateNewGameObjectPosition(this, newLocation))
            {
                return;
            }

            this.Location = newLocation;
            this.space.MoveGameObject(this, previousLocation, newLocation);
        }

        public void Interact()
        {
            if (this.InteractionModel != null)
            {
                this.InteractionModel.BeginInteraction();
            }
        }

        public void IEntityInteractionModelDidBeginInteraction(IEntityInteractionModel interactionModel)
        {
            if (this.interactionDelegate != null) {
                this.interactionDelegate.IGameObjectModelDidBeginInteraction(this);
            }
        }

        public void IEntityInteractionModelDidEndInteraction(IEntityInteractionModel interactionModel)
        {
            if (this.interactionDelegate != null) {
                this.interactionDelegate.IGameObjectModelDidEndInteraction(this);
            }
        }

        public void UpdateModelForNewRunloop()
        {
            if (!this.isAnimating)
            {
                Location previousLocation = this.Location;
                this.PerformNextMove();
                this.XAnimationOffset = previousLocation.X - this.Location.X;
                this.YAnimationOffset = previousLocation.Y - this.Location.Y;
                return;
            }
            else
            {
                this.framesAnimated++;
            }

            if (this.framesAnimated >= GameObjectConstants.MOVEMENT_ANIMATION_LENGTH)
            {
                this.framesAnimated = 0;
                this.isAnimating = false;
                return;
            }

            int framesLeft = GameObjectConstants.MOVEMENT_ANIMATION_LENGTH - this.framesAnimated;
            double fractionOfAnimationOffsetLeft = 1.0 - (1.0 / (framesLeft + 1));
            this.XAnimationOffset = fractionOfAnimationOffsetLeft * this.XAnimationOffset;
            this.YAnimationOffset = fractionOfAnimationOffsetLeft * this.YAnimationOffset;
        }

        internal void PerformNextMove()
        {
            switch (this.movementType)
            {
                case MovementType.SetMoves:
                    this.PerformNextPresetMove();
                    break;
                case MovementType.Aggressive:
                    this.MoveTowardsCenter();
                    break;
                case MovementType.Wander:
                    this.MakeRandomMove();
                    break;
                case MovementType.Still:
                    break;
            }
        }

        internal void PerformNextPresetMove()
        {
            if (this.Moves.Count == 0 || (this.currentMoveIndex >= this.Moves.Count && !this.shouldCycleMoves))
            {
                return;
            }

            if (this.currentMoveIndex >= this.Moves.Count)
            {
                this.currentMoveIndex = 0;
            }

            int newX = this.Location.X;
            int newY = this.Location.Y;

            switch (this.Moves.ElementAt(this.currentMoveIndex))
            {
                case Direction.Up:
                    newY++;
                    break;
                case Direction.Down:
                    newY--;
                    break;
                case Direction.Left:
                    newX--;
                    break;
                case Direction.Right:
                    newX++;
                    break;
            }

            this.MoveTo(newX, newY, true);
            this.currentMoveIndex++;
        }

        internal void MoveTowardsCenter()
        {
            if (this.runloopsSinceLastRandomOrAggressiveMove > 0)
            {
                if (this.runloopsSinceLastRandomOrAggressiveMove < GameObjectConstants.AGGRESSIVE_MOVEMENT_LEEWAY_CYCLES)
                {
                    this.runloopsSinceLastRandomOrAggressiveMove++;
                    return;
                }
                else
                {
                    this.runloopsSinceLastRandomOrAggressiveMove = 0;
                }
            }

            OverworldGameStateHandler ogsh = OverworldGameStateHandler.getInstance();
            int diffX = this.Location.X - ogsh.CenterX;
            int diffY = this.Location.Y - ogsh.CenterY;

            int newX = this.Location.X;
            int newY = this.Location.Y;

            if (Math.Abs(diffX) > Math.Abs(diffY))
            {
                if (diffX < 0)
                {
                    newX++;
                }
                else
                {
                    newX--;
                }
            }
            else
            {
                if (diffY < 0)
                {
                    newY++;
                }
                else
                {
                    newY--;
                }
            }

            this.MoveTo(newX, newY, true);
            this.runloopsSinceLastRandomOrAggressiveMove++;
        }

        internal void MakeRandomMove()
        {
            if (this.runloopsSinceLastRandomOrAggressiveMove > 0)
            {
                if (this.runloopsSinceLastRandomOrAggressiveMove < GameObjectConstants.AGGRESSIVE_MOVEMENT_LEEWAY_CYCLES)
                {
                    this.runloopsSinceLastRandomOrAggressiveMove++;
                    return;
                }
                else
                {
                    this.runloopsSinceLastRandomOrAggressiveMove = 0;
                }
            }

            // Give this guy a good chance of sitting still for a while
            int randomMove = rng.Next(7);
            int newX = this.Location.X;
            int newY = this.Location.Y;
            if (randomMove == 0)
            {
                newX--;
            }
            else if (randomMove == 1)
            {
                newX++;
            }
            else if (randomMove == 2)
            {
                newY--;
            }
            else if (randomMove == 3)
            {
                newY++;
            }

            this.MoveTo(newX, newY, true);
            this.runloopsSinceLastRandomOrAggressiveMove++;
        }
    }
}
