using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    public sealed class ConfigureableToolValueList<T> : ConfigureableToolValue<T>
    {
        public readonly ImmutableArray<T> ValidValues;

        public ConfigureableToolValueList(T initialValue, params T[] validValues) : base(initialValue)
        {
            ValidValues = validValues.ToImmutableArray();
        }

        private protected override bool TrySetValue(ref T value)
        {
            return ValidValues.Contains(value);
        }
    }
}
