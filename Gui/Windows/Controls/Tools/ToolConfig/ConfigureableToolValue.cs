using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Gui.Windows.Controls.Tools.ToolConfig
{
    public abstract class ConfigureableToolValue<T>
    {
        public T Value { get => _value; set => SetValue(value); }
        private T _value;

        public ConfigureableToolValue(T initialValue) {
            _value = initialValue;
        }
        public event Action<T> ValueChanged;

        private protected abstract bool TrySetValue(ref T value);
        private void SetValue(T value)
        {
            if (TrySetValue(ref value))
            {
                _value = value;
            }
            ValueChanged?.Invoke(_value);
        }
    }
}
