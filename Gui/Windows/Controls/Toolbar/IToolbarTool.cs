using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    public interface IToolbarTool
    {
        UIElement RootUIElement { get; }
        double DisplayWidth { get; }
        ToolBaseV2 ActualTool { get; }
        bool Active { get; set; }
    }
}
