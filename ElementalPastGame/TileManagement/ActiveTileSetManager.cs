using ElementalPastGame.Common;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.TileManagement.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement
{
    public class ActiveTileSetManager : IActiveTileSetManager
    {

        internal static int ActiveTileSetDimension = 2 * CommonConstants.TILE_VIEW_DISTANCE + 1;
        internal static int LoadRadius = ActiveTileSetDimension + CommonConstants.TILE_VIEW_DISTANCE;

        internal ITileMapManager MapManager;
        internal IPictureBoxManager pictureBoxManager;

        internal static IActiveTileSetManager? _instance;
        internal bool isAnimating;

        public int CenterX { get; set; }
        public int CenterY { get; set; }

        internal int PreviousCenterX { get; set; }
        internal int PreviousCenterY { get; set; }
        internal int FramesAnimated { get; set; }
        internal static int FRAMES_PER_ANIMATION = 3;

        public static IActiveTileSetManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new ActiveTileSetManager(PictureBoxManager.GetInstance(), TileMapManager.GetInstance());
            return _instance;
        }
        
        public ActiveTileSetManager(IPictureBoxManager pictureBoxManager, ITileMapManager tileMapManager)
        {
            this.pictureBoxManager = pictureBoxManager;
            this.MapManager = tileMapManager;
            this.isAnimating = false;
            this.CenterX = CommonConstants.GAME_START_LOCATION.X;
            this.CenterY = CommonConstants.GAME_START_LOCATION.Y;
            this.PreviousCenterX = this.CenterX;
            this.PreviousCenterY = this.CenterY;

            // This should load "9 chunks'" worth of tiles: the center chunk plus the 8 surrounding chunks 
            this.MapManager.LoadTileChunk(this.CenterX - LoadRadius,
                                          this.CenterY - LoadRadius,
                                          this.CenterX + LoadRadius,
                                          this.CenterY + LoadRadius);
            this.RelabelCenterChunk();
            this.UpdateRenderingModels();
        }

        public void HandleKeyInput(List<Keys> keyCodes)
        {
            if (this.isAnimating)
            {
                this.UpdateRenderingModels();
                return;
            }

            if (keyCodes.Count == 0)
            {
                return;
            }

            switch (keyCodes.Last())
            {
                case Keys.Left:
                    if (!this.ValidateNewLocation(this.CenterX + 1, this.CenterY))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.ScrollRight();
                    break;
                case Keys.Right:
                    if (!this.ValidateNewLocation(this.CenterX - 1, this.CenterY))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.ScrollLeft();
                    break;
                case Keys.Up:
                    if (!this.ValidateNewLocation(this.CenterX, this.CenterY + 1))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.ScrollDown();
                    break;
                case Keys.Down:
                    if (!this.ValidateNewLocation(this.CenterX, this.CenterY - 1))
                    {
                        return;
                    }
                    this.PreviousCenterX = this.CenterX;
                    this.PreviousCenterY = this.CenterY;
                    this.isAnimating = true;
                    this.ScrollUp();
                    break;
            }

            System.Diagnostics.Debug.WriteLine("Total time spent loading tiles this runloop: " + ITile.DebugTimeSpentLoadingPerRunLoop);
            ITile.DebugTimeSpentLoadingPerRunLoop = 0;
        }

        public void ScrollDown()
        {
            this.CenterY++;
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(this.CenterX, this.CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(this.CenterX - LoadRadius, this.CenterY - LoadRadius - 1, this.CenterX + LoadRadius, this.CenterY - ActiveTileSetDimension);
                    this.MapManager.LoadTileChunk(this.CenterX - LoadRadius, this.CenterY + ActiveTileSetDimension, this.CenterX + LoadRadius, this.CenterY + LoadRadius);
                    this.RelabelCenterChunk();
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels();
        }

        public void ScrollLeft()
        {
            this.CenterX--;
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(this.CenterX, this.CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(this.CenterX + ActiveTileSetDimension, this.CenterY - LoadRadius, this.CenterX + LoadRadius + 1, this.CenterY + LoadRadius);
                    this.MapManager.LoadTileChunk(this.CenterX - LoadRadius, this.CenterY - LoadRadius, this.CenterX - ActiveTileSetDimension, this.CenterY + LoadRadius);
                    this.RelabelCenterChunk();
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels();
        }

        public void ScrollRight()
        {
            this.CenterX++;
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(this.CenterX, this.CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(this.CenterX - LoadRadius - 1, this.CenterY - LoadRadius, this.CenterX - ActiveTileSetDimension, this.CenterY + LoadRadius);
                    this.MapManager.LoadTileChunk(this.CenterX + ActiveTileSetDimension, this.CenterY - LoadRadius, this.CenterX + LoadRadius, this.CenterY + LoadRadius);
                    this.RelabelCenterChunk();
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels();
        }

        public void ScrollUp()
        {
            this.CenterY--;
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(this.CenterX, this.CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(this.CenterX - LoadRadius, this.CenterY + ActiveTileSetDimension, this.CenterX + LoadRadius, this.CenterY + LoadRadius + 1);
                    this.MapManager.LoadTileChunk(this.CenterX - LoadRadius, this.CenterY - LoadRadius, this.CenterX + LoadRadius, this.CenterY - ActiveTileSetDimension);
                    this.RelabelCenterChunk();
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels();
        }

        internal void RelabelCenterChunk()
        {
            this.MapManager.MarkTileChunkCentral(this.CenterX - ActiveTileSetDimension, this.CenterY - ActiveTileSetDimension, this.CenterX + ActiveTileSetDimension, this.CenterY + ActiveTileSetDimension);
        }

        internal void UpdateRenderingModels()
        {
            DateTime startRenderingModels = DateTime.Now;
            double offset = 0;
            if (this.isAnimating) {
                offset = (double)this.FramesAnimated / ActiveTileSetManager.FRAMES_PER_ANIMATION;
                this.FramesAnimated++;
            }

            double DebugMillisecondsSpentGeneratingRenderingModels = 0;
            double DebugMillisecondsSpentUpdatingBitmaps = 0;

            for (int X = this.CenterX - CommonConstants.TILE_VIEW_DISTANCE - 1; X <= this.CenterX + CommonConstants.TILE_VIEW_DISTANCE + 1; X++) {
                for (int Y = this.CenterY - CommonConstants.TILE_VIEW_DISTANCE - 1; Y <= this.CenterY + CommonConstants.TILE_VIEW_DISTANCE + 1; Y++)
                {
                    ITile tileToRender = this.MapManager.TileAt(X,Y);
                    if (tileToRender.TileLoadState != TileLoadState.Loaded)
                    {
                        tileToRender.Load();
                    }
                    DateTime startModelGeneration = DateTime.Now;
                    RenderingModel tileRenderingModel = this.RenderingModelForTile(tileToRender, X, Y, offset);
                    DateTime endModelGeneration = DateTime.Now;
                    this.pictureBoxManager.UpdateBitmapForIRenderingModel(tileRenderingModel);
                    DateTime endBitmapUpdate = DateTime.Now;
                    DebugMillisecondsSpentGeneratingRenderingModels += (endModelGeneration - startModelGeneration).TotalMilliseconds;
                    DebugMillisecondsSpentUpdatingBitmaps += (endBitmapUpdate - endModelGeneration).TotalMilliseconds;
                }
            }

            // TODO: this is suppoed to represent the player so def move it out of here
            Bitmap BlankBitmap = new Bitmap(TextureMapping.Mapping[TextureMapping.Blank], CommonConstants.TILE_DIMENSION, CommonConstants.TILE_DIMENSION);
            List<Bitmap> blankBitmapList = new List<Bitmap>();
            blankBitmapList.Add(BlankBitmap);
            RenderingModel playerModel = new()
            {
                X = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Y = (CommonConstants.TILE_VIEW_DISTANCE) * CommonConstants.TILE_DIMENSION,
                Width = CommonConstants.TILE_DIMENSION,
                Height = CommonConstants.TILE_DIMENSION,
                Bitmaps = blankBitmapList,
                ModelID = "Player Model ID"
            };
            this.pictureBoxManager.UpdateBitmapForIRenderingModel(playerModel);

            if (this.isAnimating && this.FramesAnimated > ActiveTileSetManager.FRAMES_PER_ANIMATION)
            {
                this.FramesAnimated = 0;
                this.isAnimating = false;
            }

            this.pictureBoxManager.Redraw();

            DateTime endRenderingModels = DateTime.Now;
            double millisecondsSpentRenderingModels = (endRenderingModels - startRenderingModels).TotalMilliseconds;
            Debug.WriteLine(millisecondsSpentRenderingModels);
            Debug.WriteLine(DebugMillisecondsSpentGeneratingRenderingModels);
            Debug.WriteLine(DebugMillisecondsSpentUpdatingBitmaps);
        }

        internal RenderingModel RenderingModelForTile(ITile Tile, int X, int Y, double offset)
        {
            int diffY = this.PreviousCenterY - this.CenterY;
            int diffX = this.PreviousCenterX - this.CenterX;

            double animationXOffset = diffX * offset;
            double animationYOffset = diffY * offset;

            int XBasis = this.isAnimating ? this.PreviousCenterX : this.CenterX;
            int YBasis = this.isAnimating ? this.PreviousCenterY : this.CenterY;

            double tileXLocation = XBasis + CommonConstants.TILE_VIEW_DISTANCE - X - animationXOffset;
            double tileYLocation = YBasis + CommonConstants.TILE_VIEW_DISTANCE - Y - animationYOffset;


            if (Tile == null)
            {
                Bitmap BlankBitmap = new Bitmap(TextureMapping.Mapping[TextureMapping.Blank], CommonConstants.TILE_DIMENSION + 1, CommonConstants.TILE_DIMENSION + 1);
                List<Bitmap> blankBitmapList = new List<Bitmap>();
                blankBitmapList.Add(BlankBitmap);
                RenderingModel templateModel = new()
                {
                    X = (int)(tileXLocation * CommonConstants.TILE_DIMENSION),
                    Y = (int)(tileYLocation * CommonConstants.TILE_DIMENSION),
                    Width = CommonConstants.TILE_DIMENSION + 1,
                    Height = CommonConstants.TILE_DIMENSION + 1,
                    Bitmaps = blankBitmapList,
                    ModelID = "Fake Model ID"
                };
                return templateModel;
            }

            RenderingModel renderingModel = new()
            {
                X = (int)(tileXLocation * CommonConstants.TILE_DIMENSION),
                Y = (int)(tileYLocation * CommonConstants.TILE_DIMENSION),
                Width = CommonConstants.TILE_DIMENSION + 1,
                Height = CommonConstants.TILE_DIMENSION + 1,
                Bitmaps = Tile.Images,
                ModelID = Tile.TileModelID
            };
            return renderingModel;
        }

        internal Boolean ValidateNewLocation(int X, int Y)
        {
            ITile newTile = this.MapManager.TileAt(X, Y);
            if (newTile == null)
            {
                return true;
            }

            return !newTile.isCollidable;
        }

        internal TileLoadState TileLoadStateForTileAt(int X, int Y)
        {
            ITile tile = this.MapManager.TileAt(X, Y);
            if (tile == null)
            {
                return TileLoadState.Unloaded;
            }

            return tile.TileLoadState;
        }
    }
}
