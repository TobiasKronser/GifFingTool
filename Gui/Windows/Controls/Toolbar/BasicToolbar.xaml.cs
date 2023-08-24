using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    /// <summary>
    /// Interaktionslogik für BasicToolbar.xaml
    /// </summary>
    public partial class BasicToolbar : UserControl
    {
        private IToolbarTool ToolbarToolNoTool;
        IToolbarTool ActiveToolbarElement;
        List<IToolbarTool> _tools = new List<IToolbarTool>();
        internal ToolBaseV2 ActiveTool => ActiveToolbarElement.ActualTool;
        private readonly GifBitmapEditingContext _EditingContext;

        public Action TriggerCreateScreenshot { get; set; } = NoAction;
        public Action TriggerSelectTarget { get; set; } = NoAction;

        private Action<Bitmap> _UpdateDisplay = (Bitmap bmp) => { }; 
        private Action<GifBitmap> _UpdateListDisplay = (GifBitmap bmp) => { }; 
        
        public BasicToolbar()
        {
            _EditingContext = new GifBitmapEditingContext(InvokeGifBitmapDisplayUpdate, InvokeDisplayUpdate, ApplyCurrentToolModifyStep);
            ToolbarToolNoTool = new ToolbarToolNone(ActivateTool);
            ActiveToolbarElement = ToolbarToolNoTool;
            InitializeComponent();
            AddTool(new ToolbarButton(() => TriggerCreateScreenshot(), Properties.Resources.Screenshot));
            AddTool(new ToolbarButton(() => TriggerSelectTarget(), Properties.Resources.Selection));
            AddSpacer(10);
            AddTool(ToolbarToolPen.Create(_EditingContext, ActivateTool));
            AddTool(ToolbarToolHighlighter.Create(_EditingContext, ActivateTool));
            AddTool(ToolbarToolEraser.Create(_EditingContext, ActivateTool));
            AddTool(ToolbarToolText.Create(_EditingContext, ActivateTool));
            AddTool(ToolbarToolShapes.Create(_EditingContext, ActivateTool));
            //AddTool(ToolbarToolEnumerate.Create(_EditingContext, ActivateTool));
            AddTool(ToolbarToolPolygon.Create(_EditingContext, ActivateTool));

            foreach (IToolbarTool toolbarTool in _tools)
            {
                toolbarTool.Active = false;
            }

            ActivateTool(_tools[2]); //TODO: Rework xD
        }

        public GifBitmapEditingContext GetEditingContext() { return _EditingContext; }

        public void SetUpdateDisplayMethod(Action<Bitmap> updateDisplay)
        {
            _UpdateDisplay = updateDisplay;
        }
        public void SetUpdateListDisplayMethod(Action<GifBitmap> updateListDisplay)
        {
            _UpdateListDisplay = updateListDisplay;
        }

        private void InvokeDisplayUpdate()
        {
            _UpdateDisplay(_EditingContext.Bitmap);
        }
        private void InvokeGifBitmapDisplayUpdate()
        {
            //Nothing probably?
            _UpdateListDisplay(_EditingContext.GifBitmap);
        }

        private void ApplyCurrentToolModifyStep()
        {
            IBitmapModifyStep bitmapModifyStep = ActiveToolbarElement.ActualTool.DoneTryGetModifier();
            if (!(bitmapModifyStep is null))
            {
                _EditingContext.GifBitmap.Apply(bitmapModifyStep);
                _EditingContext.ProcessChangesToGifBitmap();
            }
        }

        private void ActivateTool(IToolbarTool newActiveToolbarElement)
        {
            ApplyCurrentToolModifyStep();
            ActiveToolbarElement.Active = false;

            ActiveToolbarElement = newActiveToolbarElement;
            ActiveToolbarElement.Active = true;
            ActiveToolbarElement.ActualTool.Reset();

            //foreach (ToolbarToolBase toolbarTool in _tools)
            //{
            //    if (ReferenceEquals(toolbarTool, newActiveToolbarElement))
            //    {
            //        toolbarTool.Active = true;
            //    }
            //    else
            //    {
            //        toolbarTool.Active = false;
            //    }
            //}

        }

        private void AddSpacer(double width)
        {
            ToolbarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(width) });
        }

        private void AddTool(IToolbarTool toolbarTool)
        {
            ToolbarGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(toolbarTool.DisplayWidth) });
            UIElement toolRootElement = toolbarTool.RootUIElement;
            ToolbarGrid.Children.Add(toolRootElement);
            Grid.SetColumn(toolRootElement, ToolbarGrid.ColumnDefinitions.Count - 1);
            _tools.Add(toolbarTool);
        }

        private static void NoAction() { }

        internal void SetNewTarget(GifBitmap newSelection)
        {
            ApplyCurrentToolModifyStep();
            _EditingContext.GifBitmap = newSelection;
            //_CurrentTarget = newSelection;
            //_CurrentlyEditingImage = new Bitmap(newSelection.ModifiedImage);
            //foreach(ToolbarToolBase toolbarTool in _tools)
            //{
            //    toolbarTool.ActualTool.Don(_CurrentlyEditingImage);
            //}
        }

        internal void Undo()
        {
            this.ApplyCurrentToolModifyStep();
            _EditingContext.Undo();
        }

        internal void Redo()
        {
            this.ApplyCurrentToolModifyStep();
            _EditingContext.Redo();
        }
    }
}
