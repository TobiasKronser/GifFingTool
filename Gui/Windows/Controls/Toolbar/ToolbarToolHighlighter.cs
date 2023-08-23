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
    internal class ToolbarToolHighlighter : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Highlighter;
        private static readonly Bitmap IMAGE_SIZE_WIDTH = Properties.Resources.Size_Width;
        private static readonly Bitmap IMAGE_SIZE_HEIGHT = Properties.Resources.Size_Height;
        private static readonly Bitmap IMAGE_OPACITY = Properties.Resources.Opacity;

        private readonly ScrollableIntValue _HeightValueProvider;
        private readonly ScrollableIntValue _WidthValueProvider;
        private readonly ScrollableIntValue _OpacityValueProvider;
        private readonly HighlighterToolV2 HighlighterTool;

        private ToolbarToolHighlighter(Action<IToolbarTool> activateTool, HighlighterToolV2 actualTool,
            ScrollableIntValue heightValueProvider, ScrollableIntValue widthValueProvider, ScrollableIntValue opacityValueProvider) : base(activateTool, actualTool, DISPLAY_WIDTH, IMAGE_MAIN, heightValueProvider, widthValueProvider, opacityValueProvider)
        {
            HighlighterTool = actualTool;
            _HeightValueProvider = heightValueProvider;
            _WidthValueProvider = widthValueProvider;
            _OpacityValueProvider = opacityValueProvider;

            _HeightValueProvider.InvokeOnUpdate = (int value) => HighlighterTool.SetHeight(value);
            _WidthValueProvider.InvokeOnUpdate = (int value) => HighlighterTool.SetWidth(value);
            _OpacityValueProvider.InvokeOnUpdate = (int value) => HighlighterTool.SetOpacity(value);

            _HeightValueProvider.Value = 20;
            _WidthValueProvider.Value = 10;
            _OpacityValueProvider.Value = 128;
        }


        public static ToolbarToolHighlighter Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            ScrollableIntValue heightValueProvider = ScrollableIntValue.Create(IMAGE_SIZE_HEIGHT, 1, 999);
            ScrollableIntValue widthValueProvider = ScrollableIntValue.Create(IMAGE_SIZE_WIDTH, 1, 999);
            ScrollableIntValue opacityValueProvider = ScrollableIntValue.Create(IMAGE_OPACITY, 1, 255);
            return new ToolbarToolHighlighter(activateTool, new HighlighterToolV2(editingContext), heightValueProvider, widthValueProvider, opacityValueProvider);
        }
    }
}
