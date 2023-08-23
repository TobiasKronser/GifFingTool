using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data.BitmapModification
{
    internal class BmpModRotateFlip : IBitmapModifyStep
    {
        private RotateFlipType _rotateFlipOperation;
        private RotateFlipType _rotateFlipState;
        public RotateFlipType RotateFlipStateWhenCreated => _rotateFlipState;

        public BmpModRotateFlip(RotateFlipType rotateFlipState, RotateFlipType rotateFlipOperation)
        {
            _rotateFlipState = rotateFlipState;
            _rotateFlipOperation = rotateFlipOperation;
        }

        public Bitmap ApplyTo(Bitmap target, RotateFlipType oldRotateFlipType, out RotateFlipType newRotateFlipState)
        {
            target.RotateFlip(_rotateFlipState);
            newRotateFlipState = _rotateFlipOperation ^ oldRotateFlipType;
            return target;
        }

        public bool Contains(int x, int y, int bitmapWidth, int bitmapHeight, RotateFlipType currentRotateFlipState)
        {
            return false;
        }
    }
}
