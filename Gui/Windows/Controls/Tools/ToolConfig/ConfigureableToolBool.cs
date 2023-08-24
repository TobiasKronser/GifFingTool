using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    internal class ConfigureableToolBool : ConfigureableToolValue<bool>
    {

        public ConfigureableToolBool(bool initialValue) : base(initialValue)
        {

        }

        private protected override bool TrySetValue(ref bool value)
        {
            return true;
        }
    }
}