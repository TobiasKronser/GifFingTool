using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data.BitmapModification
{
    internal static class Util
    {
        public static void TranslatePoint(int sourceBmpWidth, int sourceBmpHeight, RotateFlipType sourceRotateFlipType, RotateFlipType targetRotateFlipType, int sourceX, int sourceY, out int targetX, out int targetY)
        {
            RotateFlipType actualRotateFlipType = sourceRotateFlipType ^ targetRotateFlipType;
            sourceBmpWidth--; //convert pixel count to upper index bound
            sourceBmpHeight--;

            targetX = sourceX;
            targetY = sourceY;

            int targetBmpWidth = sourceBmpWidth;
            int targetBmpHeight = sourceBmpHeight;
            if ((int)(actualRotateFlipType & RotateFlipType.Rotate90FlipNone) != 0)
            {
                targetBmpWidth = sourceBmpHeight;
                targetBmpHeight = sourceBmpWidth;

                targetX = targetBmpWidth - sourceY;
                targetY = sourceX;
            }

            if ((int)(actualRotateFlipType & RotateFlipType.Rotate180FlipNone) != 0)
            {
                targetX = targetBmpWidth - targetX;
                targetY = targetBmpHeight - targetY;
            }

            if ((int)(actualRotateFlipType & RotateFlipType.RotateNoneFlipX) != 0)
            {
                targetX = targetBmpWidth - targetX;
            }

            if ((int)(actualRotateFlipType & RotateFlipType.RotateNoneFlipY) != 0)
            {
                targetY = targetBmpHeight - targetY;
            }

        }
    }
}
