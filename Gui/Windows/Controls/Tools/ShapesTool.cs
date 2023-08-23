using GifFingTool.Data;
using GifFingTool.Data.BitmapModification;
using GifFingTool.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class ShapesTool : ToolBaseV2
    {
        public enum Shape {
            Line,
            Rectangle,
            Ellipse,
            TriangleTop,
            TriangleLeft,
            TriangleBottom,
            TriangleRight,
            TriangleTopLeft,
            TriangleTopRight,
            TriangleBottomLeft,
            TriangleBottomRight,
        }

        private protected override IBitmapModifyStep BitmapModifyStep => GetModifyStepOrNull();
        private Color _color = Color.Yellow;
        private SolidBrush _brush = new SolidBrush(Color.Yellow);
        private Pen _pen = new Pen(Color.Yellow, 3) { Alignment = System.Drawing.Drawing2D.PenAlignment.Inset };
        private bool _drawing = false;
        private Point? _startPoint;
        private Point? _endPoint;
        private bool _fill;
        private Shape _shape = Shape.Ellipse;

        public ShapesTool(GifBitmapEditingContext editingContext) : base(editingContext)
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
            ApplyShapeToBitmap(bmp, _shape, startPoint, endPoint);

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
            } else
            {
                int xOffset = x - startPoint.X;
                int yOffset = y - startPoint.Y;

                int minOffset = Math.Min(Math.Abs(xOffset), Math.Abs(yOffset));
                int maxOffset = Math.Max(Math.Abs(xOffset), Math.Abs(yOffset));

                switch(_shape)
                {
                    case Shape.Line:
                        int xAdd = maxOffset * Math.Sign(xOffset);
                        int yAdd = maxOffset * Math.Sign(yOffset);
                        if (minOffset * 2 < maxOffset)
                        {
                            if (Math.Abs(xOffset) < Math.Abs(yOffset))
                            {
                                xAdd = 0;
                            } else
                            {
                                yAdd = 0;
                            }
                        }
                        _endPoint = new Point(startPoint.X + xAdd, startPoint.Y + yAdd);
                        break;
                    default:
                        _endPoint = new Point(startPoint.X + (maxOffset * Math.Sign(xOffset)), startPoint.Y + (maxOffset * Math.Sign(yOffset)));
                        break;
                }
            }
            Redraw();
        }

        public override void MouseRightButtonDown(int x, int y)
        {
            _fill = !_fill;
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

        public void SetShape(Shape shape)
        {
            DoneAndReset();
            _shape = shape;
        }

        internal void SetPenBorderSize(int value)
        {
            _pen.Width = value;
            Redraw();
        }

        private unsafe IBitmapModifyStep GetModifyStepOrNull()
        {
            if (!(_startPoint is Point startPoint)) return null;
            if (!(_endPoint is Point endPoint)) return null;

            using (Bitmap bitmap = new Bitmap(EditingContext.Bitmap.Width, EditingContext.Bitmap.Height))
            {
                int unmodifiedPixelColor = bitmap.GetPixel(0, 0).ToArgb();
                ApplyShapeToBitmap(bitmap, _shape, startPoint, endPoint);
                return BmpModColorPixels.FromBitmap(bitmap, unmodifiedPixelColor, EditingContext.GifBitmap.RotateFlipState, _color);
            }
        }

        private void ApplyShapeToBitmap(Bitmap bmp, Shape shape, Point startPoint, Point endPoint)
        {
            MathStk.MinMax(startPoint.X, endPoint.X, out int minX, out int maxX);
            MathStk.MinMax(startPoint.Y, endPoint.Y, out int minY, out int maxY);

            Point drawStartPoint = new Point(minX, minY);
            Point drawEndPoint = new Point(maxX, maxY);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                switch (_shape)
                {
                    case Shape.Line:
                        g.DrawLine(_pen, startPoint, endPoint);
                        break;
                    case Shape.Rectangle:
                        if (_fill) g.FillRectangle(_brush, drawStartPoint.X, drawStartPoint.Y, drawEndPoint.X - drawStartPoint.X, drawEndPoint.Y - drawStartPoint.Y);
                        else g.DrawRectangle(_pen, drawStartPoint.X, drawStartPoint.Y, drawEndPoint.X - drawStartPoint.X, drawEndPoint.Y - drawStartPoint.Y);
                        break;
                    case Shape.Ellipse:
                        if (_fill) g.FillEllipse(_brush, drawStartPoint.X, drawStartPoint.Y, drawEndPoint.X - drawStartPoint.X, drawEndPoint.Y - drawStartPoint.Y);
                        else g.DrawEllipse(_pen, drawStartPoint.X, drawStartPoint.Y, drawEndPoint.X - drawStartPoint.X, drawEndPoint.Y - drawStartPoint.Y);
                        break;
                    case Shape.TriangleLeft:
                        {
                            Point[] triangle = TriangleUtil.CreatePointsForTriangleLeft(startPoint, endPoint);
                            if (_fill) g.FillPolygon(_brush, triangle);
                            else g.DrawPolygon(_pen, triangle);
                        }
                        break;
                    case Shape.TriangleTop:
                        {
                            Point[] triangle = TriangleUtil.CreatePointsForTriangleUp(startPoint, endPoint);
                            if (_fill) g.FillPolygon(_brush, triangle);
                            else g.DrawPolygon(_pen, triangle);
                        }
                        break;
                    case Shape.TriangleRight:
                        {
                            Point[] triangle = TriangleUtil.CreatePointsForTriangleRight(startPoint, endPoint);
                            if (_fill) g.FillPolygon(_brush, triangle);
                            else g.DrawPolygon(_pen, triangle);
                        }
                        break;
                    case Shape.TriangleBottom:
                        {
                            Point[] triangle = TriangleUtil.CreatePointsForTriangleDown(startPoint, endPoint);
                            if (_fill) g.FillPolygon(_brush, triangle);
                            else g.DrawPolygon(_pen, triangle);
                        }
                        break;
                }
            }
        }

    }
}
