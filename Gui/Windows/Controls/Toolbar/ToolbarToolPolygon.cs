using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolPolygon : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Highlighter;
        private static readonly Bitmap IMAGE_SIZE_WIDTH = Properties.Resources.Size_Width;
        private static readonly Bitmap IMAGE_SIZE_HEIGHT = Properties.Resources.Size_Height;
        private static readonly Bitmap IMAGE_OPACITY = Properties.Resources.Opacity;

        private readonly ScrollableIntValue _CornerCountProvider;
        private readonly ScrollableIntValue _BorderWidthProvider;
        private readonly ScrollableIntValue _CornerSkipProvider;
        private readonly PolygonTool PolygonTool;

        private ToolbarToolPolygon(Action<IToolbarTool> activateTool, PolygonTool actualTool,
            ScrollableIntValue cornerCountProvider, ScrollableIntValue borderWidthProvider, ScrollableIntValue cornerSkipProvider) : base(activateTool, actualTool, DISPLAY_WIDTH, IMAGE_MAIN, cornerCountProvider, borderWidthProvider, cornerSkipProvider)
        {
            PolygonTool = actualTool;
            _CornerCountProvider = cornerCountProvider;
            _BorderWidthProvider = borderWidthProvider;
            _CornerSkipProvider = cornerSkipProvider;

            _CornerCountProvider.InvokeOnUpdate = (int value) => PolygonTool.CornerCountConfig.Value = value;
            //_BorderWidthProvider.InvokeOnUpdate = (int value) => PolygonTool.Conf(value);
            _CornerSkipProvider.InvokeOnUpdate = (int value) => PolygonTool.AngleConfig.Value = value;

            _CornerCountProvider.Value = 5;
            _BorderWidthProvider.Value = 3;
            _CornerSkipProvider.Value = 128;
        }


        public static ToolbarToolPolygon Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            ScrollableIntValue cornerCountProvider = ScrollableIntValue.Create(IMAGE_SIZE_HEIGHT, 1, 999);
            ScrollableIntValue borderWidthProvider = ScrollableIntValue.Create(IMAGE_SIZE_WIDTH, 1, 999);
            ScrollableIntValue cornerSkipProvider = ScrollableIntValue.Create(IMAGE_OPACITY, 1, 255);
            return new ToolbarToolPolygon(activateTool, new PolygonTool(editingContext), cornerCountProvider, borderWidthProvider, cornerSkipProvider);
        }
    }
}
