using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    internal class ConfigureableToolEnum<T> : ConfigureableToolValue<T> where T : Enum
    {
        private static readonly T[] s_ValidValues = (T[])Enum.GetValues(typeof(T));

        public ConfigureableToolEnum(T initialValue) : base(initialValue)
        {
        }

        private protected override bool TrySetValue(ref T value)
        {
            return s_ValidValues.Contains(value);
        }
    }
}
