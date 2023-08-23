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
    internal class ToolbarToolPen : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap MAIN_IMAGE = Properties.Resources.Pen;
        private static readonly Bitmap SIZE_IMAGE = Properties.Resources.Size_Width;

        private readonly ScrollableIntValue _SizeValueProvider;
        private readonly PenToolV2 PenTool;

        private ToolbarToolPen(Action<IToolbarTool> activateTool, PenToolV2 actualTool, ScrollableIntValue sizeValueProvider) : base(activateTool, actualTool, DISPLAY_WIDTH, MAIN_IMAGE, sizeValueProvider)
        {
            PenTool = actualTool;
            _SizeValueProvider = sizeValueProvider;
            _SizeValueProvider.InvokeOnUpdate = (int newSize) => { PenTool.SetSize(newSize); };

            _SizeValueProvider.SetValue(3);
        }

        public static ToolbarToolPen Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            return new ToolbarToolPen(activateTool, new PenToolV2(editingContext), ScrollableIntValue.Create(SIZE_IMAGE, 1, 999));
        }
    }
}
