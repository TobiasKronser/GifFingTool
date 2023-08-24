using GifFingTool.Data.BitmapModification;
using GifFingTool.Data;
using GifFingTool.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GifFingTool.Gui.Windows.Controls.Tools.ToolConfig;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class PolygonTool : ToolBaseV2
    {
        public enum PolygonDrawMode
        {
            Fill,
            Border,
            CornerSkip
        }

        private protected override IBitmapModifyStep BitmapModifyStep => GetModifyStepOrNull();
        private Color _color = Color.Yellow;
        private SolidBrush _brush = new SolidBrush(Color.Yellow);
        private Pen _pen = new Pen(Color.Yellow, 3) { Alignment = System.Drawing.Drawing2D.PenAlignment.Inset };
        private bool _drawing = false;
        private Point? _startPoint;
        private Point? _endPoint;

        public readonly ConfigureableToolInt AngleConfig = new ConfigureableToolInt(0, 0, 359);
        public readonly ConfigureableToolInt CornerCountConfig = new ConfigureableToolInt(5, 3, 999);
        public readonly ConfigureableToolInt CornerSkipConfig = new ConfigureableToolInt(0, 0, 999);
        public readonly ConfigureableToolEnum<PolygonDrawMode> DrawModeConfig = new ConfigureableToolEnum<PolygonDrawMode>(PolygonDrawMode.Fill);

        public PolygonTool(GifBitmapEditingContext editingContext) : base(editingContext)
        {
        }

        public void SetColor(Color color)
        {
            _color = color;
            _pen.Color = _color;
            _brush.Color = _color;
        }

        public void Redraw()
        {
            if (!(_startPoint is Point startPoint)) return;
            if (!(_endPoint is Point endPoint)) return;

            Bitmap bmp = new Bitmap(EditingContext.GifBitmap.ModifiedImage);
            ApplyPolygonToBitmap(bmp, CornerCountConfig.Value, DrawModeConfig.Value, AngleConfig.Value, startPoint, endPoint);

            EditingContext.Bitmap = bmp;
        }

        public override void KeyDown(Key key)
        {
        }

        public override void MouseClick(int x, int y)
        {
        }

        public override void MouseEnter(int x, int y)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                EditingContext.ProcessToolOperationDone();
                _drawing = false;
            }
        }

        public override void MouseLeave(int x, int y)
        {
        }

        public override void MouseLeftButtonDown(int x, int y)
        {
            EditingContext.ProcessToolOperationDone();
            _startPoint = new Point(x, y);
            _endPoint = _startPoint;
            _drawing = true;
            Redraw();
        }

        public override void MouseLeftButtonUp(int x, int y)
        {
            _drawing = false;
        }

        public override void MouseMove(int x, int y)
        {
            if (_drawing == false) return;
            if (!(_startPoint is Point startPoint)) return;

            bool shift = (Keyboard.Modifiers & ModifierKeys.Shift) > 0;
            if (!shift)
            {
                _endPoint = new Point(x, y);
            }
            else
            {
                int xOffset = x - startPoint.X;
                int yOffset = y - startPoint.Y;

                int minOffset = Math.Min(Math.Abs(xOffset), Math.Abs(yOffset));
                int maxOffset = Math.Max(Math.Abs(xOffset), Math.Abs(yOffset));

                _endPoint = new Point(startPoint.X + (maxOffset * Math.Sign(xOffset)), startPoint.Y + (maxOffset * Math.Sign(yOffset)));
            }
            Redraw();
        }

        public override void MouseRightButtonDown(int x, int y)
        {
            DrawModeConfig.Value = (PolygonDrawMode)(((int)DrawModeConfig.Value + 1) % DrawModeConfig.ValidValues.Length);
            Redraw();
        }

        public override void MouseRightButtonUp(int x, int y)
        {
        }

        public override void Reset()
        {
            _drawing = false;
            _startPoint = null;
            _endPoint = null;
        }

        private unsafe IBitmapModifyStep GetModifyStepOrNull()
        {
            if (!(_startPoint is Point startPoint)) return null;
            if (!(_endPoint is Point endPoint)) return null;

            using (Bitmap bitmap = new Bitmap(EditingContext.Bitmap.Width, EditingContext.Bitmap.Height))
            {
                int unmodifiedPixelColor = bitmap.GetPixel(0, 0).ToArgb();
                ApplyPolygonToBitmap(bitmap, CornerCountConfig.Value, DrawModeConfig.Value, AngleConfig.Value, startPoint, endPoint);
                return BmpModColorPixels.FromBitmap(bitmap, unmodifiedPixelColor, EditingContext.GifBitmap.RotateFlipState, _color);
            }
        }

        private void ApplyPolygonToBitmap(Bitmap bmp, int cornerCount, PolygonDrawMode drawMode, int baseAngle, Point startPoint, Point endPoint)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Point[] points = PolygonUtil.CalculatePoints(cornerCount, startPoint, endPoint, baseAngle);
                switch (drawMode)
                {
                    case PolygonDrawMode.Fill:
                        g.FillPolygon(_brush, points);
                        break;
                    case PolygonDrawMode.Border:
                        g.DrawPolygon(_pen, points);
                        break;
                    case PolygonDrawMode.CornerSkip:
                        
                        break;
                }
            }
        }

    }
}
