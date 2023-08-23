using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    internal class NoTool : ToolBaseV2
    {
        public static readonly NoTool Instance = new NoTool(null);

        private NoTool(GifBitmapEditingContext editingContextProvider) : base(editingContextProvider)
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

        }

        public override void MouseLeave(int x, int y)
        {

        }

        public override void MouseLeftButtonDown(int x, int y)
        {

        }

        public override void MouseLeftButtonUp(int x, int y)
        {

        }

        public override void MouseMove(int x, int y)
        {

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
