using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Util
{
    internal class PolygonUtil
    {
        public static Point[] CalculatePoints(in int cornerCount, Point p1, Point p2, double baseAngle = 0)
        {
            Point[] result = new Point[cornerCount];
            MathStk.MinMax(p1.X, p2.X, out int minX, out int maxX);
            MathStk.MinMax(p1.Y, p2.Y, out int minY, out int maxY);
            double halfWidth = (maxX - minX) / 2;
            double halfHeight = (maxY - minY) / 2;

            double centerX = ((double)maxX + (double)minX) / 2d;
            double centerY = ((double)maxY + (double)minY) / 2d;

            double radiansMultiplier = (Math.PI / 180);
            double angle = baseAngle * radiansMultiplier;
            double addAngle = (360d / (double)cornerCount) * radiansMultiplier;
            for(int i = 0; i < cornerCount; i++)
            {
                double sin = Math.Sin(angle);
                double cos = Math.Cos(angle);
                angle += addAngle;
                result[i] = new Point(
                    (int)(centerX + Math.Round(sin * halfWidth)),
                    (int)(centerY + Math.Round(cos * halfHeight)));
            }

            return result;
        }
    }
}
