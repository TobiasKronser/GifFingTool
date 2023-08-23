using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Util
{
    internal static class MathStk
    {
        public static void MinMax(int a, int b, out int min, out int max)
        {
            if (a < b)
            {
                min = a;
                max = b;
            } else
            {
                min = b;
                max = a;
            }
        }

        public static int MinAbs(int x, int y)
        {
            if (Math.Abs(x) < Math.Abs(y))
            {
                return x;
            }
            return y;
        }
        public static int MaxAbs(int x, int y)
        {
            if (Math.Abs(x) > Math.Abs(y))
            {
                return x;
            }
            return y;
        }
    }
}
