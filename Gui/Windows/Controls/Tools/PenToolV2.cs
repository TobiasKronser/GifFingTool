using GifFingTool.Data;
using GifFingTool.Data.BitmapModification;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class PenToolV2 : ToolBaseV2
    {
        private BmpModColorPixels ActiveBitmapModification = null;
        private int _size = 50;
        private Color _color = Color.Red;
        private MinimalRamBoolArray pixelFlags;
        
        private protected override IBitmapModifyStep BitmapModifyStep => ActiveBitmapModification;

        public PenToolV2(GifBitmapEditingContext editingContext) : base(editingContext)
        {
        }

        public void SetColor(Color color)
        {
            _color = color;
        }
        
        public void SetSize(int size)
        {
            _size = size;
        }

        public override void Reset()
        {
            _previousPoint = null;
            ActiveBitmapModification = null;
        }

        public override void MouseLeave(int x, int y)
        {
            _previousPoint = null;
            //ActiveBitmapModification = null;
        }

        private int AssureBounds(int value, int min, int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(ref int value, int min, int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(int value, ref int min, int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(int value, int min, ref int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(ref int value, ref int min, int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(ref int value, int min, ref int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(int value, ref int min, ref int max) => AssureBounds(ref value, ref min, ref max);
        private int AssureBounds(ref int value, ref int min, ref int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        System.Drawing.Point? _previousPoint = null;
        private unsafe void ApplyPaint(int x, int y)
        {
            if (ActiveBitmapModification is null) return;
            if (Mouse.LeftButton == MouseButtonState.Released) return;

            System.Drawing.Point currentPoint = new System.Drawing.Point(x, y);

            int maxXBound = EditingContext.Bitmap.Width - 1;
            int maxYBound = EditingContext.Bitmap.Height - 1;

            int* pixelData = (int*)EditingContext.BitmapData.Scan0;

            int radius = _size / 2;

            int min = -radius;
            int max = radius + _size % 2;

            int minX = Math.Max(x + min, 0);
            int maxX = Math.Min(x + max, maxXBound);

            int minY = Math.Max(y + min, 0);
            int maxY = Math.Min(y + max, maxYBound);

            ActualApplyPaint(x, y, minX, maxX, minY, maxY, radius, pixelData);

            if (_previousPoint is System.Drawing.Point previoisPoint)
            {
                int offsetX = previoisPoint.X - currentPoint.X;
                int offsetY = previoisPoint.Y - currentPoint.Y;
                int stepCount = Math.Max(Math.Abs(offsetX), Math.Abs(offsetY));

                float stepOffsetX = ((float)offsetX) / stepCount;
                float stepOffsetY = ((float)offsetY) / stepCount;

                float movingX = currentPoint.X;
                float movingY = currentPoint.Y;
                float movingMinX = minX;
                float movingMaxX = maxX;
                float movingMinY = minY;
                float movingMaxY = maxY;

                for(int i = 0; i < stepCount; i++)
                {
                    movingX += stepOffsetX;
                    movingMinX += stepOffsetX;
                    movingMaxX += stepOffsetX;

                    movingY += stepOffsetY;
                    movingMinY += stepOffsetY;
                    movingMaxY += stepOffsetY;

                    ActualApplyPaint(
                        (int)movingX,
                        (int)movingY,
                        Math.Max((int)movingMinX, 0),
                        Math.Min((int)movingMaxX, maxXBound),
                        Math.Max((int)movingMinY, 0),
                        Math.Min((int)movingMaxY, maxYBound),
                        radius,
                        pixelData);
                }
            }

            _previousPoint = currentPoint;
            EditingContext.ProcessChangesToBitmap();
        }

        private unsafe void ActualApplyPaint(int x, int y, int minX, int maxX, int minY, int maxY, int radius, int* pixelData)
        {
            for (int targetY = minY; targetY < maxY; targetY++)
            {
                int yIndexOffset = targetY * EditingContext.BitmapData.Width;
                int yOff = y - targetY;
                int yOffSqr = yOff * yOff;

                List<int> targetXs = new List<int>();

                for (int targetX = minX; targetX < maxX; targetX++)
                {
                    int xOff = x - targetX;
                    if (Math.Sqrt(xOff * xOff + yOffSqr) <= radius)
                    {
                        int pixelIndex = yIndexOffset + targetX;
                        if (pixelFlags[pixelIndex]) continue;
                        pixelFlags[pixelIndex] = true;
                        pixelData[pixelIndex] = _color.ToArgb();
                        targetXs.Add(targetX);
                    }
                }

                if (targetXs.Count == 0) continue;
                ActiveBitmapModification.AddPoints(targetY, targetXs);
            }
        }

        public override void MouseMove(int x, int y)
        {
            ApplyPaint(x, y);
        }

        public override void MouseLeftButtonUp(int x, int y)
        {
            if (!(ActiveBitmapModification is null))
            {
                EditingContext.ProcessToolOperationDone();
                Reset();
            }
        }

        public override void MouseLeftButtonDown(int x, int y)
        {
            ActiveBitmapModification = new BmpModColorPixels(EditingContext.GifBitmap.RotateFlipState, _color);
            int pixelCount = EditingContext.Bitmap.Height * EditingContext.Bitmap.Width;
            int intBlockCount = (pixelCount >> 5) + 1;
            pixelFlags = new MinimalRamBoolArray(intBlockCount);
            ApplyPaint(x, y);
        }
        public override void MouseClick(int x, int y) { }

        public override void MouseRightButtonDown(int x, int y)
        {

        }

        public override void MouseRightButtonUp(int x, int y)
        {

        }

        public override void MouseEnter(int x, int y)
        {
            if (!(ActiveBitmapModification is null) &&
                Mouse.LeftButton == MouseButtonState.Released)
            {
                EditingContext.ProcessToolOperationDone();
                Reset();
            }
        }
        public override void KeyDown(Key key)
        {

        }
    }
}
