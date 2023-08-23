//using GifFingTool.Data;
//using GifFingTool.Data.BitmapModification;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Media;
//using System.Text;
//using System.Threading.Tasks;

//namespace GifFingTool.Gui.Windows.Controls.Tools
//{
//    internal class PenTool : ToolBase
//    {
//        BmpModColorPixels ActiveBitmapModification = null;
//        private Func<int> _sizeProvider;

//        private protected override IBitmapModifyStep BitmapModifyStep => ActiveBitmapModification;

//        public PenTool(Func<int> sizeProvider)
//        {
//            _sizeProvider = sizeProvider;
//        }

//        public override void Reset()
//        {
//            ActiveBitmapModification = null;
//        }

//        public override void MouseLeave(int x, int y)
//        {
//            ActiveBitmapModification = null;
//        }

//        private void ApplyPaint(int x, int y)
//        {
//            if (ActiveBitmapModification is null) return;

//            int size = _sizeProvider();

//            int min = -(size / 2);
//            int max = size / 2 + size % 2;

//            for (int j = min; j < max; j++)
//            {
//                int targetY = y + j;
//                List<int> targetXs = new List<int>();

//                for (int i = min; i < max; i++)
//                {
//                    targetXs.Add(x + i);
//                }
//                ActiveBitmapModification.AddPoints(targetY, targetXs);
//            }
            
//        }

//        public override void MouseMove(int x, int y)
//        {
//            ApplyPaint(x, y);
//        }

//        public override void MouseLeftButtonUp(int x, int y)
//        {
//            ActiveBitmapModification = null;
//        }

//        public override void MouseLeftButtonDown(int x, int y)
//        {   
//            if (CurrentTarget is null) return;

//            ActiveBitmapModification = new BmpModColorPixels(CurrentTarget.RotateFlipState, Color.Red);
//            CurrentTarget.Apply(ActiveBitmapModification);
//            ApplyPaint(x, y);
//        }
//        public override void MouseClick(int x, int y) { }

//        public override void MouseRightButtonDown(int x, int y)
//        {

//        }

//        public override void MouseRightButtonUp(int x, int y)
//        {

//        }

//        public override void MouseEnter(int x, int y)
//        {

//        }
//    }
//}
