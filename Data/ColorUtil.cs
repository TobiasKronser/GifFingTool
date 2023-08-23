using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    public static class ColorUtil
    {
        public static int MergeColors(int c1, Color c2)
        {

            const int BYTE_MASK = 0xFF;
            float multiplierOverlay = c2.A / 255f;
            float multiplierBase = 1 - multiplierOverlay;
            byte newB = (byte)(c2.B * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);
            c1 >>= 8;
            byte newG = (byte)(c2.G * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);
            c1 >>= 8;
            byte newR = (byte)(c2.R * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);

            c1 = BYTE_MASK;
            c1 <<= 8;
            c1 += newR;
            c1 <<= 8;
            c1 += newG;
            c1 <<= 8;
            c1 += newB;
            return c1;
        }
        public static int MergeColors(int c1, int c2)
        {
            unchecked
            {
                const int BYTE_MASK = 0xFF;
            
                uint c2alpha = unchecked(((uint)c2) >> 24);
                float multiplierOverlay = c2alpha / 255f;
                float multiplierBase = 1 - multiplierOverlay;
                byte newB = (byte)((c2 & BYTE_MASK) * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);
                c1 >>= 8;
                c2 >>= 8;
                byte newG = (byte)((c2 & BYTE_MASK) * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);
                c1 >>= 8;
                c2 >>= 8;
                byte newR = (byte)((c2 & BYTE_MASK) * multiplierOverlay + (c1 & BYTE_MASK) * multiplierBase);

                c1 = BYTE_MASK;
                c1 <<= 8;
                c1 += newR;
                c1 <<= 8;
                c1 += newG;
                c1 <<= 8;
                c1 += newB;
                return c1;
            }
        }
    }
}
