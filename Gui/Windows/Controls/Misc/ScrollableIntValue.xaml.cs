using System;
using System.Collections.Generic;
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

namespace GifFingTool.Gui.Windows.Controls.Misc
{
    /// <summary>
    /// Interaktionslogik für ScrollableIntValue.xaml
    /// </summary>
    public partial class ScrollableIntValue : UserControl
    {
        public int MaxValue { get; set; } = int.MaxValue;
        public int MinValue { get; set; } = int.MinValue;

        private int _value = 100;
        public int Value { get => _value; set => SetValue(value); }
        public int ScrollStep { get; set; } = 1;
        public int ShiftScrollStep { get; set; } = 10;
        public ImageSource ImageSource { get => DisplayImage.Source; set => DisplayImage.Source = value; }
        private Action<int> _InvokeOnUpdate = DoNothing;
        public Action<int> InvokeOnUpdate { set => _InvokeOnUpdate = value; }
        public void SetValue(int value)
        {
            if (value < MinValue)
            {
                value = MinValue;
            } else if (value > MaxValue)
            {
                value = MaxValue;
            }
            _value = value;
            _blockChangeEvent = true;
            ValueTextBlock.Text = value.ToString();
            _blockChangeEvent = false;
            _InvokeOnUpdate(value);
        }
        public void SetValueNoChangeEvent(int value)
        {
            if (value < MinValue)
            {
                value = MinValue;
            }
            else if (value > MaxValue)
            {
                value = MaxValue;
            }
            _value = value;
            _blockChangeEvent = true;
            ValueTextBlock.Text = value.ToString();
            _blockChangeEvent = false;
        }
        public ScrollableIntValue()
        {
            InitializeComponent();
        }
        public ScrollableIntValue(int initialValue)
        {
            InitializeComponent();
            SetValue(initialValue);
        }

        private void BaseGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int stepSize = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? ShiftScrollStep : ScrollStep;
            int stepValue = (e.Delta > 0) ? stepSize : -stepSize;

            long newValue = _value + stepValue;

            if (newValue > (long)MaxValue)
            {
                newValue = MaxValue;
            }
            else if (newValue < (long)MinValue)
            {
                newValue = MinValue;
            }

            e.Handled = true;

            Value = (int)newValue;
        }

        public static ScrollableIntValue Create(System.Drawing.Bitmap image, int minValue, int maxValue)
        {
            return new ScrollableIntValue()
            {
                ImageSource = Temp.GetImageSource(image),
                MinValue = minValue,
                MaxValue = maxValue,
            };
        }
        public static ScrollableIntValue Create(System.Drawing.Bitmap image, int minValue, int maxValue, int initialValue)
        {
            return new ScrollableIntValue(initialValue)
            {
                ImageSource = Temp.GetImageSource(image),
                MinValue = minValue,
                MaxValue = maxValue,
            };
        }

        private static void DoNothing(int value) { }

        private bool _blockChangeEvent = false;
        private void ValueTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_blockChangeEvent) return;

            if (int.TryParse(ValueTextBlock.Text, out int value))
            {
                SetValue(value);
            } else
            {
                SetValue(_value);
            }
        }

    }
}
