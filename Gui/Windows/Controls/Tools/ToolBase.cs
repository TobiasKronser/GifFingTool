//using GifFingTool.Data;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//namespace GifFingTool.Gui.Windows.Controls.Tools
//{
//    internal abstract class ToolBase
//    {
//        private protected Bitmap CurrentTarget;
//        private protected RotateFlipType CurrentRotateFlipState;
//        private protected byte[] bytes;
//        private protected abstract IBitmapModifyStep BitmapModifyStep { get; }

//        public IBitmapModifyStep DoneTryGetModifier()
//        {
//            IBitmapModifyStep result = BitmapModifyStep;
//            Reset();
//            return result;
//        }

//        public void SetNewTarget(Bitmap target, RotateFlipType rotateFlipType)
//        {
//            Reset();
//            CurrentTarget = target;
//        }

//        public abstract void Reset();
//        public abstract void MouseLeftButtonDown(int x, int y);
//        public abstract void MouseLeftButtonUp(int x, int y);
//        public abstract void MouseRightButtonDown(int x, int y);
//        public abstract void MouseRightButtonUp(int x, int y);
//        public abstract void MouseMove(int x, int y);
//        public abstract void MouseLeave(int x, int y);
//        public abstract void MouseEnter(int x, int y);
//        public abstract void MouseClick(int x, int y);

//        //protected bool IsInRange(double x1, double y1, double x2, double y2, double range)
//        //{

//        //}

//    }
//}
