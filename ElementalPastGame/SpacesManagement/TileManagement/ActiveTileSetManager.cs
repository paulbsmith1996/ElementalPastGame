using ElementalPastGame.Common;
using ElementalPastGame.GameObject;
using ElementalPastGame.KeyInput;
using ElementalPastGame.Rendering;
using ElementalPastGame.SpacesManagement.TileManagement.TileMapManagers;
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
        
        /// <summary>
        /// TODO: Remove the dependency on GameObjectManager from this class entirely. It's causing waaaayyyy to many headaches.
        /// </summary>
        /// <param name="pictureBoxManager"></param>
        /// <param name="tileMapManager"></param>
        /// <param name="CenterX"></param>
        /// <param name="CenterY"></param>
        public ActiveTileSetManager(IPictureBoxManager pictureBoxManager, ITileMapManager tileMapManager, int CenterX, int CenterY)
        {
            this.pictureBoxManager = pictureBoxManager;
            this.MapManager = tileMapManager;

            // This should load "9 chunks'" worth of tiles: the center chunk plus the 8 surrounding chunks 
            //this.MapManager.LoadTileChunk(CenterX - LoadRadius,
            //                              CenterY - LoadRadius,
            //                              CenterX + LoadRadius,
            //                              CenterY + LoadRadius);
            this.MapManager.LoadTileChunk(0, 0, CommonConstants.MAX_MAP_TILE_DIMENSION - 1, CommonConstants.MAX_MAP_TILE_DIMENSION - 1);
            this.RelabelCenterChunk(CenterX, CenterY);
            this.UpdateRenderingModels(CenterX, CenterY, CenterX, CenterY, false, 0);
        }

        public void Update(int PreviousCenterX, int PreviousCenterY, int CenterX, int CenterY, bool isAnimating, double offset)
        {
            if (isAnimating)
            {
                this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, offset);
                return;
            }

            //int diffX = CenterX - PreviousCenterX;
            //int diffY = CenterY - PreviousCenterY;

            //TileLoadState CenterTileLoadState = this.TileLoadStateForTileAt(CenterX, CenterY);
            //switch (CenterTileLoadState)
            //{
            //    case TileLoadState.Central:
            //        break;
            //    case TileLoadState.Loaded:
            //        if (diffX > 0)
            //        {
            //            this.MapManager.UnloadTileChunk(CenterX - LoadRadius - 1, CenterY - LoadRadius, CenterX - ActiveTileSetDimension, CenterY + LoadRadius);
            //            this.MapManager.LoadTileChunk(CenterX + ActiveTileSetDimension, CenterY - LoadRadius, CenterX + LoadRadius, CenterY + LoadRadius);
            //        }
            //        else if (diffX < 0)
            //        {
            //            this.MapManager.UnloadTileChunk(CenterX + ActiveTileSetDimension, CenterY - LoadRadius, CenterX + LoadRadius + 1, CenterY + LoadRadius);
            //            this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius, CenterX - ActiveTileSetDimension, CenterY + LoadRadius);
            //        }
            //        else if (diffY > 0) {
            //            this.MapManager.UnloadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius - 1, CenterX + LoadRadius, CenterY - ActiveTileSetDimension);
            //            this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY + ActiveTileSetDimension, CenterX + LoadRadius, CenterY + LoadRadius);
            //        }
            //        else if (diffY < 0)
            //        {
            //            this.MapManager.UnloadTileChunk(CenterX - LoadRadius, CenterY + ActiveTileSetDimension, CenterX + LoadRadius, CenterY + LoadRadius + 1);
            //            this.MapManager.LoadTileChunk(CenterX - LoadRadius, CenterY - LoadRadius, CenterX + LoadRadius, CenterY - ActiveTileSetDimension);
            //        }

            //        this.RelabelCenterChunk(CenterX, CenterY);

            //        break;
            //    case TileLoadState.Unloaded:
            //        break;
            //}
            this.UpdateRenderingModels(CenterX, CenterY, PreviousCenterX, PreviousCenterY, isAnimating, offset);
        }

        internal void RelabelCenterChunk(int CenterX, int CenterY)
        {
            this.MapManager.MarkTileChunkCentral(CenterX - ActiveTileSetDimension, CenterY - ActiveTileSetDimension, CenterX + ActiveTileSetDimension, CenterY + ActiveTileSetDimension);
        }

        internal void UpdateRenderingModels(int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, bool isAnimating, double offset)
        {

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
        }

        internal RenderingModel RenderingModelForTile(ITile Tile, int X, int Y, int CenterX, int CenterY, int PreviousCenterX, int PreviousCenterY, double offset, bool isAnimating)
        {
            int diffY = CenterY - PreviousCenterY;
            int diffX = CenterX - PreviousCenterX;

            double animationXOffset = diffX * offset;
            double animationYOffset = diffY * offset;

            double tileXLocation = CenterX + CommonConstants.TILE_VIEW_DISTANCE - X - animationXOffset;
            double tileYLocation = CenterY + CommonConstants.TILE_VIEW_DISTANCE - Y - animationYOffset;

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
