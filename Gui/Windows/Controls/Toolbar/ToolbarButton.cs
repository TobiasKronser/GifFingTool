using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarButton : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private readonly Action _clickAction;
        internal ToolbarButton(Action runOnClick, Bitmap image) : base(DoNothing, NoTool.Instance, DISPLAY_WIDTH, image)
        {
            _clickAction = runOnClick;
        }

        protected override void MainButton_Click(object sender, RoutedEventArgs e)
        {
            _clickAction();
        }

        private static void DoNothing(IToolbarTool tool) { }
    }
}
