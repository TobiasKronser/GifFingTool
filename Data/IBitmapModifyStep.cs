using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    public interface IBitmapModifyStep
    {
        RotateFlipType RotateFlipStateWhenCreated { get; }
        Bitmap ApplyTo(Bitmap target, RotateFlipType targetRotateFlipState, out RotateFlipType newRotateFlipState);
        bool Contains(int x, int y, int bitmapWidth, int bitmapHeight, RotateFlipType currentRotateFlipState);
    }
}
