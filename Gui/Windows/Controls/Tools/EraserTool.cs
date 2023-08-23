using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class EraserTool : ToolBaseV2
    {
        private bool _eraseOnMove = false;

        public EraserTool(GifBitmapEditingContext editingContext) : base(editingContext)
        {
        }

        private protected override IBitmapModifyStep BitmapModifyStep => null;

        public override void KeyDown(Key key)
        {

        }

        public override void MouseClick(int x, int y)
        {
            
        }

        public override void MouseEnter(int x, int y)
        {
            _eraseOnMove = false;
        }

        public override void MouseLeave(int x, int y)
        {
            _eraseOnMove = false;
        }

        public override void MouseLeftButtonDown(int x, int y)
        {
            _eraseOnMove = true;
            AttemptErase(x, y);
        }

        private void AttemptErase(int x, int y)
        {
            IReadOnlyList<IBitmapModifyStep> bitmapModifySteps = EditingContext.GifBitmap.GetModifySteps();
            if (bitmapModifySteps.Count == 0) return;

            for (int i = bitmapModifySteps.Count - 1; i >= 0; i--)
            {
                IBitmapModifyStep bitmapModifyStep = bitmapModifySteps[i];
                if (bitmapModifyStep.Contains(x, y, EditingContext.Bitmap.Width, EditingContext.Bitmap.Height, EditingContext.GifBitmap.RotateFlipState))
                {
                    EditingContext.GifBitmap.RemoveModifyStep(bitmapModifyStep);
                    EditingContext.ProcessChangesToGifBitmap();
                    break;
                }
            }
        }

        public override void MouseLeftButtonUp(int x, int y)
        {
            _eraseOnMove = false;
        }

        public override void MouseMove(int x, int y)
        {
            if (_eraseOnMove)
            {
                //AttemptErase(x, y);
            }
        }

        public override void MouseRightButtonDown(int x, int y)
        {

        }

        public override void MouseRightButtonUp(int x, int y)
        {

        }

        public override void Reset()
        {

        }
    }
}
