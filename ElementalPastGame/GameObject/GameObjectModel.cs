using ElementalPastGame.Common;
using ElementalPastGame.GameObject.Entities;
using ElementalPastGame.GameObject.EntityManagement;
using ElementalPastGame.GameObject.GameStateHandlers;
using ElementalPastGame.GameObject.Utility;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ElementalPastGame.GameObject.IGameObjectModel;

namespace ElementalPastGame.GameObject
{
    public partial class GameObjectModel : IGameObjectModel
    {
        public Location Location { get; set; }
        public GameObjectSize Size { get; set; }

        public long EntityID { get; set; }
        public Boolean IsCollidable { get; set; }
        public double XAnimationOffset { get; set; }
        public double YAnimationOffset { get; set; }

        internal int framesAnimated { get; set; }
        internal bool isAnimating { get; set; }
        public MovementType movementType { get; set; }
        public List<Direction> Moves { get; set; }
        public bool shouldCycleMoves { get; set; }
        public EntityDataModel dataModel { get; set; }

        internal int currentMoveIndex = 0;
        internal int runloopsSinceLastAggressiveMove = 0;

        public GameObjectModel(EntityType type, int level, int X, int Y, MovementType movementType = MovementType.Wander)
        {
            this.movementType = movementType;
            this.Moves = new();
            this.Location = new Location() { X = X, Y = Y };
            this.dataModel = new EntityDataModel(type, level);

            // Use Szudik's function to uniquely hash every X,Y pair to a unique entity ID
            this.EntityID = X >= Y ? X * X + X + Y : X + Y * Y;

            // TODO: we don't necessarily need to load everything now
            this.LoadIfNeeded();
        }

        public void LoadIfNeeded()
        {
            this.dataModel.Load();
        }

        public void Unload()
        {
            this.dataModel.Image = null;
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

            IActiveEntityManager activeEntityManager = ActiveEntityManager.GetInstance();
            activeEntityManager.RemoveGameObjectFromLocation(this, previousLocation);
            this.Location = newLocation;
            activeEntityManager.AddGameObjectToLocation(this, newLocation);
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
            if (this.runloopsSinceLastAggressiveMove > 0)
            {
                if (this.runloopsSinceLastAggressiveMove < GameObjectConstants.AGGRESSIVE_MOVEMENT_LEEWAY_CYCLES)
                {
                    this.runloopsSinceLastAggressiveMove++;
                    return;
                }
                else
                {
                    this.runloopsSinceLastAggressiveMove = 0;
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
            this.runloopsSinceLastAggressiveMove++;
        }
    }
}
