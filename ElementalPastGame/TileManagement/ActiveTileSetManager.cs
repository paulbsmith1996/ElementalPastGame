using ElementalPastGame.Common;
using ElementalPastGame.GameObject;
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

        public static IActiveTileSetManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new ActiveTileSetManager(PictureBoxManager.GetInstance(), TileMapManager.GetInstance(), CommonConstants.GAME_START_LOCATION.X, CommonConstants.GAME_START_LOCATION.Y);
            return _instance;
        }
        
        public ActiveTileSetManager(IPictureBoxManager pictureBoxManager, ITileMapManager tileMapManager, int CenterX, int CenterY)
        {
            this.pictureBoxManager = pictureBoxManager;
            this.MapManager = tileMapManager;

            // This should load "9 chunks'" worth of tiles: the center chunk plus the 8 surrounding chunks 
            this.MapManager.LoadTileChunk(CenterX - LoadRadius,
                                          CenterY - LoadRadius,
                                          CenterX + LoadRadius,
                                          CenterY + LoadRadius);
            this.RelabelCenterChunk(CenterX, CenterY);
            this.UpdateRenderingModels(CenterX, CenterY, CenterX, CenterY, false, 0);
        }

        public void HandleKeyInput(Keys? key)
        {
            IGameObjectManager gameObjectManager = GameObjectManager.getInstance();
            if (gameObjectManager.isAnimating)
            {
                this.UpdateRenderingModels(gameObjectManager.CenterX, gameObjectManager.CenterY, gameObjectManager.PreviousCenterX, gameObjectManager.PreviousCenterY, gameObjectManager.isAnimating, gameObjectManager.FramesAnimated);
                return;
            }

            if (key == null)
            {
                return;
            }

            switch (key)
            {
                case Keys.Left:
                    this.ScrollRight(gameObjectManager.CenterX, gameObjectManager.CenterY, gameObjectManager.PreviousCenterX, gameObjectManager.PreviousCenterY, gameObjectManager.isAnimating, gameObjectManager.FramesAnimated);
                    break;
                case Keys.Right:
                    this.ScrollLeft(gameObjectManager.CenterX, gameObjectManager.CenterY, gameObjectManager.PreviousCenterX, gameObjectManager.PreviousCenterY, gameObjectManager.isAnimating, gameObjectManager.FramesAnimated);
                    break;
                case Keys.Up:
                    this.ScrollDown(gameObjectManager.CenterX, gameObjectManager.CenterY, gameObjectManager.PreviousCenterX, gameObjectManager.PreviousCenterY, gameObjectManager.isAnimating, gameObjectManager.FramesAnimated);
                    break;
                case Keys.Down:
                    this.ScrollUp(gameObjectManager.CenterX, gameObjectManager.CenterY, gameObjectManager.PreviousCenterX, gameObjectManager.PreviousCenterY, gameObjectManager.isAnimating, gameObjectManager.FramesAnimated);
                    break;
            }
        }

        public void ScrollDown(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, int framesAnimated)
        {
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(CenterX, CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius - 1, CenterX + LoadRadius, CenterY - ActiveTileSetDimension);
                    this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY + ActiveTileSetDimension, CenterX + LoadRadius, CenterY + LoadRadius);
                    this.RelabelCenterChunk(CenterX, CenterY);
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, framesAnimated);
        }

        public void ScrollLeft(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, int framesAnimated)
        {
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(CenterX, CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(CenterX + ActiveTileSetDimension, CenterY - LoadRadius, CenterX + LoadRadius + 1, CenterY + LoadRadius);
                    this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius, CenterX - ActiveTileSetDimension, CenterY + LoadRadius);
                    this.RelabelCenterChunk(CenterX, CenterY);
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, framesAnimated);
        }

        public void ScrollRight(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, int framesAnimated)
        {
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(CenterX, CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(CenterX - LoadRadius - 1, CenterY - LoadRadius, CenterX - ActiveTileSetDimension, CenterY + LoadRadius);
                    this.MapManager.LoadTileChunk(CenterX + ActiveTileSetDimension, CenterY - LoadRadius, CenterX + LoadRadius, CenterY + LoadRadius);
                    this.RelabelCenterChunk(CenterX, CenterY);
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, framesAnimated);
        }

        public void ScrollUp(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, int framesAnimated)
        {
            TileLoadState CenterTileLoadState = this.MapManager.TileAt(CenterX, CenterY).TileLoadState;
            switch (CenterTileLoadState)
            {
                case TileLoadState.Central:
                    break;
                case TileLoadState.Loaded:
                    this.MapManager.UnloadTileChunk(CenterX - LoadRadius, CenterY + ActiveTileSetDimension, CenterX + LoadRadius, CenterY + LoadRadius + 1);
                    this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius, CenterX + LoadRadius, CenterY - ActiveTileSetDimension);
                    this.RelabelCenterChunk(CenterX, CenterY);
                    break;
                case TileLoadState.Unloaded:
                    break;
            }
            this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, framesAnimated);
        }

        internal void RelabelCenterChunk(int CenterX, int CenterY)
        {
            this.MapManager.MarkTileChunkCentral(CenterX - ActiveTileSetDimension, CenterY - ActiveTileSetDimension, CenterX + ActiveTileSetDimension, CenterY + ActiveTileSetDimension);
        }

        internal void UpdateRenderingModels(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, int framesAnimated)
        {
            double offset = 0;
            if (isAnimating) {
                offset = (double)framesAnimated / GameObjectManager.FRAMES_PER_ANIMATION;
            }

            for (int X = CenterX - CommonConstants.TILE_VIEW_DISTANCE - 1; X <= CenterX + CommonConstants.TILE_VIEW_DISTANCE + 1; X++) {
                for (int Y = CenterY - CommonConstants.TILE_VIEW_DISTANCE - 1; Y <= CenterY + CommonConstants.TILE_VIEW_DISTANCE + 1; Y++)
                {
                    ITile tileToRender = this.MapManager.TileAt(X, Y);
                    if (tileToRender.TileLoadState != TileLoadState.Loaded)
                    {
                        tileToRender.Load();
                    }
                    RenderingModel tileRenderingModel = this.RenderingModelForTile(tileToRender, X, Y, CenterX, CenterY, PreviousCenterX, PreviousCenterY, offset, isAnimating);
                    this.pictureBoxManager.UpdateBitmapForIRenderingModel(tileRenderingModel);
                }
            }

            this.pictureBoxManager.Redraw();
        }

        internal RenderingModel RenderingModelForTile(ITile Tile, int X, int Y, int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, double offset, bool isAnimating)
        {
            int diffY = PreviousCenterY - CenterY;
            int diffX = PreviousCenterX - CenterX;

            double animationXOffset = diffX * offset;
            double animationYOffset = diffY * offset;

            int XBasis = isAnimating ? PreviousCenterX : CenterX;
            int YBasis = isAnimating ? PreviousCenterY : CenterY;

            double tileXLocation = XBasis + CommonConstants.TILE_VIEW_DISTANCE - X - animationXOffset;
            double tileYLocation = YBasis + CommonConstants.TILE_VIEW_DISTANCE - Y - animationYOffset;

            if (Tile == null)
            {
                Bitmap BlankBitmap = new Bitmap(TextureMapping.Mapping[TextureMapping.Blank], CommonConstants.TILE_DIMENSION + 1, CommonConstants.TILE_DIMENSION + 1);
                List<Bitmap> blankBitmapList = new();
                blankBitmapList.Add(BlankBitmap);
                RenderingModel templateModel = new()
                {
                    X = (int)(tileXLocation * CommonConstants.TILE_DIMENSION),
                    Y = (int)(tileYLocation * CommonConstants.TILE_DIMENSION),
                    Width = CommonConstants.TILE_DIMENSION + 1,
                    Height = CommonConstants.TILE_DIMENSION + 1,
                    Bitmaps = blankBitmapList,
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
