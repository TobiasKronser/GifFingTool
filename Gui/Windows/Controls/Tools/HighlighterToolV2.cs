using GifFingTool.Data;
using GifFingTool.Data.BitmapModification;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class HighlighterToolV2 : ToolBaseV2
    {
        private BmpModMergePixels ActiveBitmapModification = null;
        private int _height = 12;
        private int _width = 5;
        private int _opacity = 50;
        private Color _color = Color.Red;
        private MinimalRamBoolArray pixelFlags;
        private Point? _previousPoint;
        private int _previousY = 0;

        private protected override IBitmapModifyStep BitmapModifyStep => ActiveBitmapModification;

        public HighlighterToolV2(GifBitmapEditingContext editingContext) : base(editingContext)
        {
        }

        public void SetColor(Color color)
        {
            _color = Color.FromArgb(_opacity, color);
        }

        public void SetHeight(int value)
        {
            _height = value;
        }

        public void SetWidth(int value)
        {
            _width = value;
        }

        public void SetOpacity(int value)
        {
            _opacity = value;
            _color = Color.FromArgb(_opacity, _color);
        }

        public override void Reset()
        {
            ActiveBitmapModification = null;
        }

        public override void MouseLeave(int x, int y)
        {
            _previousPoint = null;
            //ActiveBitmapModification = null;
        }

        private int MergeColorsPeakPerformanceInaccurateLSB(int c1, int c2)
        {
            const int ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK = 0x00FEFEFE;
            int addedValue = (c1 & ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK) + (c2 & ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK);
            addedValue >>= 1;
            return unchecked((int)0xFF000000) | addedValue;
        }

        private int MergeColorsPrecise(int c1, int c2)
        {
            const int ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK = 0x00FEFEFE;
            const int ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK = 0x00010101;
            int addedValue = (c1 & ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK) + (c2 & ALL_RGB_BYTES_EXCEPT_LEAST_SIGNIFICANT_BITS_MASK);
            addedValue >>= 1;

            //int lsbAdded = (c1 & ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK) + (c2 & ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK);
            //lsbAdded >>= 1;
            //lsbAdded &= ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK;
            
            //int lsbAdded = (c1 & ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK) & (c2 & ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK);

            int lsbAdded = c1 & c2 & ALL_RGB_LEAST_SIGNIFICANT_BITS_MASK;

            return unchecked((int)0xFF000000) | addedValue + lsbAdded;
        }

        private unsafe void ApplyPaint(int x, int y)
        {
            if (ActiveBitmapModification is null) return;
            if (Mouse.LeftButton == MouseButtonState.Released) return;

            int maxXBound = EditingContext.Bitmap.Width - 1;
            int maxYBound = EditingContext.Bitmap.Height - 1;

            int* pixelData = (int*)EditingContext.BitmapData.Scan0;

            ActualApplyPaint(x, x + _width - 1, y, y + _height - 1, pixelData);

            Point currentPoint = new Point(x, y);

            if (_previousPoint is Point previoisPoint)
            {
                int offsetX = previoisPoint.X - currentPoint.X;
                int offsetY = previoisPoint.Y - currentPoint.Y;
                int stepCount = Math.Max(Math.Abs(offsetX), Math.Abs(offsetY));

                float stepOffsetX = ((float)offsetX) / stepCount;
                float stepOffsetY = ((float)offsetY) / stepCount;

                float movingX = currentPoint.X;
                float movingY = currentPoint.Y;

                for (int i = 0; i < stepCount; i++)
                {
                    movingX += stepOffsetX;
                    movingY += stepOffsetY;

                    ActualApplyPaint(
                        Math.Max((int)movingX, 0),
                        Math.Min((int)movingX + _width - 1, maxXBound),
                        Math.Max((int)movingY, 0),
                        Math.Min((int)movingY + _height - 1, maxYBound),
                        pixelData);
                }
            }

            _previousY = currentPoint.Y;
            _previousPoint = currentPoint;
            EditingContext.ProcessChangesToBitmap();
        }

        private unsafe void ActualApplyPaint(int minX, int maxX, int minY, int maxY, int* pixelData)
        {
            for (int targetY = minY; targetY < maxY; targetY++)
            {
                List<int> targetXs = new List<int>();
                int pixelIndex = targetY * EditingContext.BitmapData.Width + minX - 1; //-1 so pixelIndex++ and if ... continue do not complicate code

                for (int targetX = minX; targetX < maxX; targetX++)
                {
                    pixelIndex++;
                    if (pixelFlags[pixelIndex]) continue;
                    pixelFlags[pixelIndex] = true;
                    pixelData[pixelIndex] = ColorUtil.MergeColors(pixelData[pixelIndex], _color);
                    targetXs.Add(targetX);
                }

                if (targetXs.Count == 0) continue;
                ActiveBitmapModification.AddPoints(targetY, targetXs);
            }
        }

        public override void MouseMove(int x, int y)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) > 0)
            {
                ApplyPaint(x, _previousY);
            } else
            {
                ApplyPaint(x, y);
            }
        }

        public override void MouseLeftButtonUp(int x, int y)
        {
            _previousPoint = null;
            //ActiveBitmapModification = null;
        }

        public override void MouseLeftButtonDown(int x, int y)
        {
            if (ActiveBitmapModification is null)
            {
                ActiveBitmapModification = new BmpModMergePixels(EditingContext.GifBitmap.RotateFlipState, _color);
                int pixelCount = EditingContext.Bitmap.Height * EditingContext.Bitmap.Width;
                int intBlockCount = (pixelCount >> 5) + 1;
                pixelFlags = new MinimalRamBoolArray(intBlockCount);
            }
            ApplyPaint(x, y);
        }
        public override void MouseClick(int x, int y) { }

        public override void MouseRightButtonDown(int x, int y)
        {
            EditingContext.ProcessToolOperationDone();
        }

        public override void MouseRightButtonUp(int x, int y)
        {

        }

        public override void MouseEnter(int x, int y)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                _previousPoint = null;
            }
        }

        public override void KeyDown(Key key)
        {

        }
    }
}
