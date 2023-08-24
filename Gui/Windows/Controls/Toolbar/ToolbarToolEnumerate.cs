using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolEnumerate : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Text;
        private static readonly Bitmap IMAGE_FONT_SIZE = Properties.Resources.Size_Height;

        private readonly EnumerateTool EnumerateTool;
        private readonly ScrollableIntValue FontSizeProvider;
        private readonly ScrollableIntValue NumberProvider;
        private readonly FontPicker FontPicker;
        private readonly CheckBox BoldProvider;
        private readonly CheckBox ItalicProvider;
        private readonly CheckBox UnderlineProvider;
        private readonly CheckBox StrikeoutProvider;


        private ToolbarToolEnumerate(Action<IToolbarTool> activateTool, EnumerateTool actualTool,
            ScrollableIntValue numberProvider,
            ScrollableIntValue fontSizeProvider,
            FontPicker fontPicker,
            CheckBox boldProvider,
            CheckBox italicProvider,
            CheckBox underlineProvider
            ) : base(activateTool, actualTool, new double[] { DISPLAY_WIDTH, 60, 30 }, IMAGE_MAIN, numberProvider, fontSizeProvider, fontPicker, boldProvider, italicProvider, underlineProvider)
        {
            NumberProvider = numberProvider;
            FontSizeProvider = fontSizeProvider;
            BoldProvider = boldProvider;
            ItalicProvider = italicProvider;
            UnderlineProvider = underlineProvider;

            EnumerateTool = actualTool;

            NumberProvider.InvokeOnUpdate = actualTool.SetNumber;
            EnumerateTool.NotifyInternalConfigChanged += EnumerateTool_NotifyInternalConfigChanged;

            fontSizeProvider.InvokeOnUpdate = actualTool.SetFontSize;
            fontPicker.FontFamilyChanged += FontPicker_FontFamilyChanged;
            fontPicker.FontChanged += FontPicker_FontChanged;

            boldProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetBold(true);
            italicProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetItalic(true);
            underlineProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetUnderline(true);

            boldProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetBold(false);
            italicProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetItalic(false);
            underlineProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => EnumerateTool.SetUnderline(false);
        }

        private void EnumerateTool_NotifyInternalConfigChanged()
        {
            NumberProvider.SetValueNoChangeEvent(EnumerateTool.Number);
        }

        private void FontPicker_FontChanged(FontFamily fontFamily, Color color, float fontSize, bool bold, bool italic, bool underline, bool strikeout)
        {
            EnumerateTool.SetFontFamily(fontFamily);
            FontSizeProvider.SetValue((int)fontSize);
            BoldProvider.IsChecked = bold;
            ItalicProvider.IsChecked = italic;
            UnderlineProvider.IsChecked = underline;
            StrikeoutProvider.IsChecked = strikeout;
            //Commented to let the events handle setting the values to prevent async states of UI and Tool
            //EnumerateTool.SetFont(fontFamily, fontSize, bold, italic, underline, strikethrough, color);

        }

        private void FontPicker_FontFamilyChanged(FontFamily fontFamily)
        {
            EnumerateTool.SetFontFamily(fontFamily);
        }

        private void InputTextProvider_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    e.Handled = true;
                    EnumerateTool.DoneAndReset();
                    TextBox textBox = (TextBox)sender;
                    textBox.Clear();
                    Keyboard.ClearFocus();
                    //textBox.Focusable = false;
                    //textBox.Focusable = true;
                }
            }
        }

        public static ToolbarToolEnumerate Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            ScrollableIntValue numberProvider = ScrollableIntValue.Create(IMAGE_MAIN, 1, 999);
            ScrollableIntValue fontSizeValueProvider = ScrollableIntValue.Create(IMAGE_FONT_SIZE, 1, 999);
            FontPicker fontPicker = new FontPicker();
            CheckBox boldProvider = new CheckBox() { Content = "B", FontWeight = FontWeights.Bold };
            CheckBox italicProvider = new CheckBox() { Content = "I", FontStyle = FontStyles.Italic };
            CheckBox underlineProvider = new CheckBox()
            {
                Content = new TextBlock()
                {
                    Text = "U",
                    TextDecorations = TextDecorations.Underline,
                }
            };
            return new ToolbarToolEnumerate(activateTool, new EnumerateTool(editingContext), numberProvider, fontSizeValueProvider, fontPicker, boldProvider, italicProvider, underlineProvider);
        }
    }
}
