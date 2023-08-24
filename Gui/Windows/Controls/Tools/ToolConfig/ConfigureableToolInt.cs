using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    public sealed class ConfigureableToolInt : ConfigureableToolValue<int>
    {
        public readonly int MinValue;
        public readonly int MaxValue;

        public ConfigureableToolInt(int initialValue, int minValue, int maxValue) : base(initialValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        private protected override bool TrySetValue(ref int value)
        {
            if (value < MinValue)
            {
                value = MinValue;
            } else if (value > MaxValue)
            {
                value = MaxValue;
            }

            return true;
        }
    }
}
