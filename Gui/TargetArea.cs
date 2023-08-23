using GifFingTool.Data;
using GifFingTool.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gui
{
    internal sealed class TargetArea
    {
        private static Screen DefaultScreen = Screen.PrimaryScreen;



        private Screen _TargetScreen = DefaultScreen;
        private Rectangle _AreaScreen = Rectangle.FromLTRB(0,0,0,0);


        private Point _PointA = new Point(0, 0);
        private Point _PointB = new Point(0, 0);
        private Rectangle _TargetArea = Rectangle.FromLTRB(0, 0, 0, 0);
        private Point _ActivePoint;
        public Rectangle GetTargetArea()
        {
            return new Rectangle(_TargetArea.Location, _TargetArea.Size);
        }

        public Rectangle CalculateGlobalTargetArea()
        {
            return Rectangle.FromLTRB(_TargetArea.Left + _TargetScreen.WorkingArea.Left,
                                        _TargetArea.Top + _TargetScreen.WorkingArea.Top,
                                        _TargetArea.Right + _TargetScreen.WorkingArea.Left,
                                        _TargetArea.Bottom + _TargetScreen.WorkingArea.Top);
        }

        public TargetArea()
        {
            _ActivePoint = _PointA;
        }
        
        public void SetTargetScreen(Screen screen)
        {
            _TargetScreen = screen;
            //_AreaScreen = new Rectangle(0, 0, 0, 0);// screen.Bounds.Width, screen.Bounds.Height);
            _AreaScreen = new Rectangle(0, 0, screen.Bounds.Width, screen.Bounds.Height);
            //_AreaScreen = new Rectangle(0, 0, screen.WorkingArea.Width, screen.WorkingArea.Height);
            Console.WriteLine($"AreaScreen: {_AreaScreen}");
        }

        public void SetActivePoint(bool pointAIsActive)
        {
            _ActivePoint = pointAIsActive ? _PointA : _PointB;
        }
        public void InvertActivePoint()
        {
            _ActivePoint = ReferenceEquals(_PointA, _ActivePoint) ? _PointB : _PointA;
        }

        private void AssureThatPointIsInBounds(ref int x, ref int y)
        {
            if (x < 0) x = 0;
            if (x > _AreaScreen.Right) x = _AreaScreen.Right;
            if (y < 0) y = 0;
            if (y > _AreaScreen.Bottom) y = _AreaScreen.Bottom;
        }

        private void UpdateTargetArea()
        {
            _TargetArea = Rectangle.FromLTRB(
                Math.Min(_PointA.X, _PointB.X),
                Math.Min(_PointA.Y, _PointB.Y),
                Math.Max(_PointA.X, _PointB.X),
                Math.Max(_PointA.Y, _PointB.Y));
        }

        private void MovePoint(ref Point point, int xOffset, int yOffset)
        {
            int x = point.X + xOffset;
            int y = point.Y + yOffset;
            SetPoint(ref point, x, y);
        }
        private void SetPoint(ref Point point, int x, int y)
        {
            AssureThatPointIsInBounds(ref x, ref y);
            point.X = x;
            point.Y = y;
            UpdateTargetArea();
        }

        public void MovePointA(int xOffset, int yOffset) => MovePoint(ref _PointB, xOffset, yOffset);
        public void MovePointB(int xOffset, int yOffset) => MovePoint(ref _PointB, xOffset, yOffset);
        public void MoveActivePoint(int xOffset, int yOffset) => MovePoint(ref _ActivePoint, xOffset, yOffset);
        public void SetPointA(int x, int y) => SetPoint(ref _PointA, x, y);
        public void SetPointB(int x, int y) => SetPoint(ref _PointB, x, y);
        public void SetActivePoint(int x, int y) => SetPoint(ref _ActivePoint, x, y);

        public Bitmap CreateScreenshot(bool excludeTaskbar = false)
        {
            Rectangle extractionRect = GetTargetArea();
            Console.WriteLine($"Extracted rect: ({extractionRect.Width}, {extractionRect.Height})");
            if (extractionRect.Width == 0 || extractionRect.Height == 0)
            {
                return _TargetScreen.CreateScreenshot(excludeTaskbar);
            }
            using (Bitmap fullscreenScreenshot = _TargetScreen.CreateScreenshot(excludeTaskbar))
            {
                return fullscreenScreenshot.ExtractRectangle(extractionRect);
            }
                
        }
    }
}
