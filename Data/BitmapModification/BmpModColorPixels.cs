using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data.BitmapModification
{
    internal class BmpModColorPixels : IBitmapModifyStep
    {
        private readonly Color _Color;
        private readonly Dictionary<int, HashSet<int>> _Points = new Dictionary<int, HashSet<int>>();

        public RotateFlipType RotateFlipStateWhenCreated { get; }

        public BmpModColorPixels(RotateFlipType rotateFlipState, Color color)
        {
            RotateFlipStateWhenCreated = rotateFlipState;
            _Color = color;
        }
        //public BmpModColorPixels(RotateFlipType rotateFlipState, Color color, IEnumerable<Point> points) : this(rotateFlipState, color)
        //{
        //    _Points.AddRange(points);
        //}

        public void AddPoints(int y, IEnumerable<int> correspondingXs)
        {
            if (y < 0) return; //TODO: Log?

            if (!_Points.TryGetValue(y, out HashSet<int> xValues))
            {
                xValues = new HashSet<int>();
                _Points[y] = xValues;
            }

            foreach(int x in correspondingXs)
            {
                if (x < 0) continue; //TODO: Log?
                xValues.Add(x);
            }
        }

        public void AddPoint(int y, int x)
        {
            if (y < 0) return; //TODO: Log?
            if (x < 0) return; //TODO: Log?

            if (!_Points.TryGetValue(y, out HashSet<int> xValues))
            {
                xValues = new HashSet<int>();
                _Points[y] = xValues;
            }

            xValues.Add(x);
        }

        public unsafe Bitmap ApplyTo(Bitmap bmp, RotateFlipType oldRotateFlipType, out RotateFlipType newRotateFlipState)
        {
            newRotateFlipState = RotateFlipStateWhenCreated;
            int color = _Color.ToArgb();

            BitmapData bmpData = bmp.LockBits(Rectangle.FromLTRB(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
            // Get the address of the first line.
            int* ptr = (int*)bmpData.Scan0;

            foreach (KeyValuePair<int, HashSet<int>> fixedYMultiplyX in _Points)
            {
                int yOffset = fixedYMultiplyX.Key * bmpData.Width;
                foreach (int x in fixedYMultiplyX.Value)
                {
                    int pixelPosition = yOffset + x;
                    ptr[pixelPosition] = color;
                }
            }

            bmp.UnlockBits(bmpData);
            return bmp;
        }
        
        //public Bitmap ApplyTo(Bitmap bmp, out RotateFlipType newRotateFlipState)
        //{
        //    newRotateFlipState = RotateFlipState;

        //    BitmapData bmpData = bmp.LockBits(Rectangle.FromLTRB(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * bmpData.Height;
        //    byte[] rgbValues = new byte[bytes];

        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        //    foreach (KeyValuePair<int, HashSet<int>> fixedYMultiplyX in _Points)
        //    {
        //        int yOffset = fixedYMultiplyX.Key * 4 * bmpData.Width;
        //        foreach (int x in fixedYMultiplyX.Value)
        //        {
        //            int pixelPosition = yOffset + x * 4;
        //            rgbValues[pixelPosition] = _Color.B;
        //            rgbValues[pixelPosition + 1] = _Color.G;
        //            rgbValues[pixelPosition + 2] = _Color.R;
        //            rgbValues[pixelPosition + 3] = _Color.A;
        //        }
        //    }

        //    // Copy the RGB values back to the bitmap
        //    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

        //    bmp.UnlockBits(bmpData);
        //    return bmp;
        //}

        public bool Contains(int x, int y, int bitmapWidth, int bitmapHeight, RotateFlipType currentRotateFlipState)
        {
            Util.TranslatePoint(bitmapWidth, bitmapHeight, currentRotateFlipState, RotateFlipStateWhenCreated, x, y, out int targetX, out int targetY);
            return _Points.TryGetValue(targetY, out HashSet<int> xValues) &&
                xValues.Contains(targetX);
        }

        public static unsafe BmpModColorPixels FromBitmap(Bitmap bitmap, int unmodifiedPixelColor, RotateFlipType rotateFlipState, Color color)
        {
            BmpModColorPixels bmpModColorPixels = new BmpModColorPixels(rotateFlipState, color);
            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                int* pixelData = (int*)bmpData.Scan0;

                List<int> targetXs = new List<int>();

                int pixelIndex = 0;
                for (int y = 0; y < bmpData.Height; y++)
                {
                    targetXs.Clear();
                    for (int x = 0; x < bmpData.Width; x++)
                    {
                        if (pixelData[pixelIndex] != unmodifiedPixelColor)
                        {
                            targetXs.Add(x);
                        }
                        pixelIndex++;
                    }

                    if (targetXs.Count > 0)
                    {
                        bmpModColorPixels.AddPoints(y, targetXs);
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
