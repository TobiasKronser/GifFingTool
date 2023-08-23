using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Util
{
    internal static class TriangleUtil
    {
        public static Point[] CreatePointsForTriangleLeft(Point p1, Point p2)
        {

            MathStk.MinMax(p1.X, p2.X, out int minX, out int maxX);
            MathStk.MinMax(p1.Y, p2.Y, out int minY, out int maxY);
            int halfWidth = (maxX - minX) / 2;
            int halfHeight = (maxY - minY) / 2;

            return new Point[] {
                new Point(minX, minY + halfHeight),
                new Point(maxX, minY),
                new Point(maxX, maxY),
            };
        }
        public static Point[] CreatePointsForTriangleUp(Point p1, Point p2)
        {

            MathStk.MinMax(p1.X, p2.X, out int minX, out int maxX);
            MathStk.MinMax(p1.Y, p2.Y, out int minY, out int maxY);
            int halfWidth = (maxX - minX) / 2;
            int halfHeight = (maxY - minY) / 2;

            return new Point[] {
                new Point(minX + halfWidth, minY),
                new Point(minX, maxY),
                new Point(maxX, maxY),
            };
        }
        public static Point[] CreatePointsForTriangleRight(Point p1, Point p2)
        {

            MathStk.MinMax(p1.X, p2.X, out int minX, out int maxX);
            MathStk.MinMax(p1.Y, p2.Y, out int minY, out int maxY);
            int halfWidth = (maxX - minX) / 2;
            int halfHeight = (maxY - minY) / 2;

            return new Point[] {
                new Point(minX, minY),
                new Point(minX, maxY),
                new Point(maxX, minY + halfHeight),
            };
        }
        public static Point[] CreatePointsForTriangleDown(Point p1, Point p2)
        {

            MathStk.MinMax(p1.X, p2.X, out int minX, out int maxX);
            MathStk.MinMax(p1.Y, p2.Y, out int minY, out int maxY);
            int halfWidth = (maxX - minX) / 2;
            int halfHeight = (maxY - minY) / 2;

            return new Point[] {
                new Point(minX, minY),
                new Point(maxX, minY),
                new Point(minX + halfWidth, maxY),
            };
        }

    }
}
