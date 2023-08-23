using GifFingTool.Extensions;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolNone : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 0;
        internal ToolbarToolNone(Action<IToolbarTool> activateTool) : base(activateTool, NoTool.Instance, DISPLAY_WIDTH, BitmapExtensions.NO_IMAGE)
        {
        }
    }
}
