using ElementalPastGame.Common;
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
        public String ImageID { get; set; }

        internal static long CurrentEntityID = 1;
        public long EntityID { get; set; }
        public Boolean IsCollidable { get; set; }
        public Image? Image { get; set; }

        public double XAnimationOffset { get; set; }
        public double YAnimationOffset { get; set; }

        internal int framesAnimated { get; set; }
        internal bool isAnimating { get; set; }
        public List<Direction> Moves { get; set; }
        public bool shouldCycleMoves { get; set; }
        internal int currentMoveIndex = 0;
        
        public GameObjectModel(String ImageID, int X, int Y)
        {
            this.Moves = new();
            this.Location = new Location() { X = X, Y = Y };
            this.ImageID = ImageID;
            this.EntityID = GameObjectModel.CurrentEntityID;
            GameObjectModel.CurrentEntityID++;
        }

        public void LoadIfNeeded()
        {
            if (this.Image != null)
            {
                return;
            }

            this.Image = TextureMapping.Mapping[this.ImageID];
        }

        public void Unload()
        {
            this.Image = null;
        }

        public void MoveTo(int NewX, int NewY, bool isAnimated)
        {
            Location newLocation = new()
            {
                X = NewX,
                Y = NewY,
            };

            if (isAnimated)
            {
                Location previousLocation = this.Location;
                this.isAnimating = true;
                this.XAnimationOffset = previousLocation.X - newLocation.X;
                this.YAnimationOffset = previousLocation.Y - newLocation.Y;
            }
            this.Location = newLocation;
        }

        public void UpdateModelForNewRunloop()
        {
            if (!this.isAnimating)
            {
                this.PerformNextMove();
                return;
            }
            this.framesAnimated++;
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
            if (this.Moves.Count == 0 || (this.currentMoveIndex >= this.Moves.Count && !this.shouldCycleMoves))
            {
                return;
            }

            if (this.currentMoveIndex >= this.Moves.Count)
            {
                this.currentMoveIndex = 0;
            }

            int previousX = this.Location.X;
            int previousY = this.Location.Y;
            switch(this.Moves.ElementAt(this.currentMoveIndex))
            {
                case Direction.Up:
                    this.MoveTo(previousX, previousY + 1, true);
                    break;
                case Direction.Down:
                    this.MoveTo(previousX, previousY - 1, true);
                    break;
                case Direction.Left:
                    this.MoveTo(previousX - 1, previousY, true);
                    break;
                case Direction.Right:
                    this.MoveTo(previousX + 1, previousY, true);
                    break;
            }

            this.currentMoveIndex++;
        }
    }
}
