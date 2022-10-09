using ElementalPastGame.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ElementalPastGame.Rendering
{
    public class PictureBoxManager : IPictureBoxManager
    {
        internal Dictionary<String, Bitmap> bitmapsByRenderingModelIDs = new Dictionary<String, Bitmap>();
       
        internal Bitmap _activeScene = new((2 * CommonConstants.TILE_VIEW_DISTANCE + 1) * CommonConstants.TILE_DIMENSION, (2 * CommonConstants.TILE_VIEW_DISTANCE + 1) * CommonConstants.TILE_DIMENSION);

        public Bitmap ActiveScene => _activeScene;

        internal static PictureBoxManager? _instance;
        internal List<IPictureBoxManagerObserver> _observers = new();

        public static IPictureBoxManager GetInstance()
        {
            if (_instance != null)
            {
                return _instance;
            }

            _instance = new PictureBoxManager();
            return _instance;
        }

        public void AddIPictureBoxManagerObserver(IPictureBoxManagerObserver observer)
        {
            this._observers.Add(observer);
        }

        public void RemoveIPictureBoxManagerObserver(IPictureBoxManagerObserver observer)
        {
            this._observers.Remove(observer);
        }

        public void Redraw()
        {
            foreach (IPictureBoxManagerObserver observer in this._observers)
            {
                observer.IPictureBoxManagerNeedsRedraw();
            }
        }

        public void UpdateBitmapForIRenderingModel(RenderingModel renderingModel)
        {
            // TODO: need to have an erase type result for passing in a null bitmap
            if (renderingModel.Bitmaps.Count > 0)
            {
                this.RedrawActiveBitmap(renderingModel.Bitmaps, renderingModel.X, renderingModel.Y);
            }
        }

        internal void RedrawActiveBitmap(List<Bitmap> bitmaps, int x, int y)
        {
            foreach (Bitmap bitmap in bitmaps)
            {
                using (Graphics graphics = Graphics.FromImage(this._activeScene))
                {
                    //Rectangle patch = new Rectangle(x, y, bitmap.Width + 1, bitmap.Height + 1);
                    graphics.DrawImage(bitmap, x, y);
                }
            }
        }

        //public void UpdateBitmapForIRenderingModel(RenderingModel renderingModel)
        //{
        //    // TODO: need to have an erase type result for passing in a null bitmap
        //    if (renderingModel.Bitmaps.Count > 0)
        //    {
        //        for (int x = 0; x < renderingModel.Width; x++)
        //        {
        //            for (int y = 0; y < renderingModel.Height; y++)
        //            {
        //                foreach (Bitmap bitmap in renderingModel.Bitmaps)
        //                {
        //                    Color pixelColor = bitmap.GetPixel(x, y);
        //                    if (pixelColor != Color.Empty)
        //                    {
        //                        int renderingX = renderingModel.X + x;
        //                        int renderingY = renderingModel.Y + y;
        //                        if (renderingX >= 0 && renderingX < this.ActiveScene.Width && renderingY >= 0 && renderingY < this.ActiveScene.Height) {
        //                            this.ActiveScene.SetPixel(renderingModel.X + x, renderingModel.Y + y, pixelColor);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
    }

}
