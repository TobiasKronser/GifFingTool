using GifFingTool.Gui.GlobalHooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GifFingTool
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            InterceptKeys.InitHook();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            InterceptKeys.Unhook();
        }

    }
}
