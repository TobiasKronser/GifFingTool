using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolEraser : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Eraser;

        private readonly EraserTool EraserTool;

        private ToolbarToolEraser(Action<IToolbarTool> activateTool, EraserTool actualTool) : base(activateTool, actualTool, DISPLAY_WIDTH, IMAGE_MAIN)
        {
            EraserTool = actualTool;
        }


        public static ToolbarToolEraser Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            return new ToolbarToolEraser(activateTool, new EraserTool(editingContext));
        }
    }
}
