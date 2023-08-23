using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolShapes : Grid, IToolbarTool
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Shapes;
        private static readonly Bitmap IMAGE_SIZE_WIDTH = Properties.Resources.Size_Width;

        private readonly ShapesTool ShapesTool;

        public ToolBaseV2 ActualTool => ShapesTool;

        private bool _active = false;
        public bool Active { get => _active; set => SetActive(value); }

        public UIElement RootUIElement => this;

        public double DisplayWidth => DISPLAY_WIDTH;
        Action<IToolbarTool> _activateTool;

        private ShapePicker ShapePicker;

        private void SetActive(bool value)
        {
            _active = value;
            if (!_active)
            {
                ShapePicker.Deselect();
            }
        }

        internal ToolbarToolShapes(Action<IToolbarTool> activateTool, ShapesTool actualTool, ShapePicker shapePicker, ScrollableIntValue penBorderSizeProvider) : base()
        {
            this.Children.Add(shapePicker);
            ShapesTool = actualTool;
            ShapePicker = shapePicker;
            _activateTool = activateTool;
            ShapePicker.ShapeChanged += OnShapePickerSelection;
            penBorderSizeProvider.InvokeOnUpdate = (int value) => ShapesTool.SetPenBorderSize(value); 
        }

        private void OnShapePickerSelection(ShapesTool.Shape shape)
        {
            if (!_active)
            {
                _activateTool(this);
            } else
            {
                ShapesTool.DoneAndReset();
            }
            ShapesTool.SetShape(shape);
        }


        public static ToolbarToolShapes Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            ScrollableIntValue penBorderSizeValueProvider = ScrollableIntValue.Create(IMAGE_SIZE_WIDTH, 1, 999);
            //TextBox inputTextProvider = new TextBox();
            //FontPicker fontPicker = new FontPicker();
            return new ToolbarToolShapes(activateTool, new ShapesTool(editingContext), new ShapePicker(), penBorderSizeValueProvider);
        }
    }
}
