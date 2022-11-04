using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementalPastGame.TileManagement.Utility
{
    public static class TextureFactory
    {
        public static Bitmap TessalatedTexture(Bitmap texture, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            Graphics graphics = Graphics.FromImage(bitmap);
            for(int xTileIndex = 0; xTileIndex < width; xTileIndex += texture.Width)
            {
                for (int yTileIndex = 0; yTileIndex < height; yTileIndex += texture.Height)
                {
                    // TODO: actually implement this
                    graphics.DrawImage(texture, xTileIndex, yTileIndex);
                }
            }
            return bitmap;
        }

        /// <summary>
        /// This is a very, very, expensive method to call. Pray to God you only need to call it once.
        /// Cache this result or fear the wrath of the dropped frames.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="perspective"></param>
        /// <returns></returns>
        public static Bitmap TessalatedTexture(Bitmap texture, int bitmapWidth, int bitmapHeight, Point perspective)
        {
            Bitmap originalBitmap = TessalatedTexture(texture, bitmapWidth, bitmapHeight);

            float px = perspective.X;
            float py = perspective.Y;
            float height = bitmapHeight + py;
            float width = bitmapWidth;

            Bitmap transformedBitmap = new Bitmap(originalBitmap.Width, originalBitmap.Height);

            for (float y = 0; y < originalBitmap.Height; y++)
            {

                float leftSqueezeEndpoint = (y + py - height) * px / height;
                float rightSqueezeEndpoint = ((py - y) * (width - px) / height) + px;

                float squeezeFactor = (rightSqueezeEndpoint - leftSqueezeEndpoint) / width;

                for (float x = 0; x < originalBitmap.Width; x++)
                {
                    Color pixelColor = originalBitmap.GetPixel((int)x, (int)y);
                    float newX;
                    if (x <= px)
                    {
                        newX = px - ((px - x) * squeezeFactor);
                    }
                    else
                    {
                        newX = px + ((x - px) * squeezeFactor);
                    }
                    transformedBitmap.SetPixel((int)newX, originalBitmap.Height - (int)y - 1, pixelColor);
                }
            }

            return transformedBitmap;
        }

    }
}
