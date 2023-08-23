using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data.BitmapModification
{
    internal class BmpModColorsPixels : IBitmapModifyStep
    {
        private readonly Dictionary<int, Dictionary<int, int>> _Points = new Dictionary<int, Dictionary<int, int>>();

        public RotateFlipType RotateFlipStateWhenCreated { get; }

        public BmpModColorsPixels(RotateFlipType rotateFlipState)
        {
            RotateFlipStateWhenCreated = rotateFlipState;
        }

        public void AddPoints(int y, IEnumerable<KeyValuePair<int, int>> correspondingXsAndColors)
        {
            if (y < 0) return; //TODO: Log?

            if (!_Points.TryGetValue(y, out Dictionary<int, int> xsAndColors))
            {
                xsAndColors = new Dictionary<int, int>();
                _Points[y] = xsAndColors;
            }

            foreach (KeyValuePair<int, int> xAndColor in correspondingXsAndColors)
            {
                if (xAndColor.Key < 0) continue; //TODO: Log?
                xsAndColors.Add(xAndColor.Key, xAndColor.Value);
            }
        }

        public void AddPoint(int y, int x, int argb)
        {
            if (y < 0) return; //TODO: Log?
            if (x < 0) return; //TODO: Log?

            if (!_Points.TryGetValue(y, out Dictionary<int, int> xsAndColors))
            {
                xsAndColors = new Dictionary<int, int>();
                _Points[y] = xsAndColors;
            }

            xsAndColors[x] = argb;
        }

        public unsafe Bitmap ApplyTo(Bitmap bmp, RotateFlipType oldRotateFlipType, out RotateFlipType newRotateFlipState)
        {
            newRotateFlipState = RotateFlipStateWhenCreated;

            BitmapData bmpData = bmp.LockBits(Rectangle.FromLTRB(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            int* ptr = (int*)bmpData.Scan0;

            foreach (KeyValuePair<int, Dictionary<int, int>> fixedYMultiplyX in _Points)
            {
                int yOffset = fixedYMultiplyX.Key * bmpData.Width;
                foreach (KeyValuePair<int, int> xAndArgb in fixedYMultiplyX.Value)
                {
                    int pixelPosition = yOffset + xAndArgb.Key;
                    ptr[pixelPosition] = ColorUtil.MergeColors(ptr[pixelPosition], xAndArgb.Value);
                }
            }

            bmp.UnlockBits(bmpData);
            return bmp;
        }

        public bool Contains(int x, int y, int bitmapWidth, int bitmapHeight, RotateFlipType currentRotateFlipState)
        {
            Util.TranslatePoint(bitmapWidth, bitmapHeight, currentRotateFlipState, RotateFlipStateWhenCreated, x, y, out int targetX, out int targetY);
            return _Points.TryGetValue(targetY, out Dictionary<int, int> xValues) &&
                xValues.Keys.Contains(targetX);
        }

        public static unsafe BmpModColorsPixels FromBitmap(Bitmap bitmap, int unmodifiedPixelColor, RotateFlipType rotateFlipState)
        {
            BmpModColorsPixels bmpModColorPixels = new BmpModColorsPixels(rotateFlipState);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int* pixelData = (int*)bmpData.Scan0;

                List<KeyValuePair<int, int>> targetXsAndArgb = new List<KeyValuePair<int, int>>();

                int pixelIndex = 0;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    targetXsAndArgb.Clear();
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        if (pixelData[pixelIndex] != unmodifiedPixelColor)
                        {
                            targetXsAndArgb.Add(new KeyValuePair<int, int>(x, pixelData[pixelIndex]));
                        }
                        pixelIndex++;
                    }

                    if (targetXsAndArgb.Count > 0)
                    {
                        bmpModColorPixels.AddPoints(y, targetXsAndArgb);
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bmpData);
            }

            return bmpModColorPixels;
        }
    }
}
