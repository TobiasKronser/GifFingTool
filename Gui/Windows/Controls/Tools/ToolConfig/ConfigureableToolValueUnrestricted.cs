using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    public sealed class ConfigureableToolValueUnrestricted<T> : ConfigureableToolValue<T>
    {
        public ConfigureableToolValueUnrestricted(T initialValue) : base(initialValue)
        {
        }

        private protected override bool TrySetValue(ref T value)
        {
            return true;
        }
    }
}
