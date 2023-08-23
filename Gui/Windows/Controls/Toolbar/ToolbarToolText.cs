using GifFingTool.Data;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    internal class ToolbarToolText : ToolbarStandardToolBase
    {
        private const int DISPLAY_WIDTH = 50;
        private static readonly Bitmap IMAGE_MAIN = Properties.Resources.Text;
        private static readonly Bitmap IMAGE_FONT_SIZE = Properties.Resources.Size_Height;

        private readonly TextTool TextTool;
        private readonly ScrollableIntValue FontSizeProvider;
        private readonly FontPicker FontPicker;
        private readonly CheckBox BoldProvider;
        private readonly CheckBox ItalicProvider;
        private readonly CheckBox UnderlineProvider;
        private readonly CheckBox StrikeoutProvider;


        private ToolbarToolText(Action<IToolbarTool> activateTool, TextTool actualTool,
            ScrollableIntValue fontSizeProvider,
            FontPicker fontPicker,
            TextBox inputTextProvider,
            CheckBox boldProvider,
            CheckBox italicProvider,
            CheckBox underlineProvider,
            CheckBox strikeoutProvider,
            CheckBox obliqueProvider
            ) : base(activateTool, actualTool, DISPLAY_WIDTH, IMAGE_MAIN, fontSizeProvider, fontPicker, inputTextProvider, boldProvider, italicProvider, strikeoutProvider, underlineProvider, obliqueProvider)
        {
            FontSizeProvider = fontSizeProvider;
            BoldProvider = boldProvider;
            ItalicProvider = italicProvider;
            UnderlineProvider = underlineProvider;
            StrikeoutProvider = strikeoutProvider;

            TextTool = actualTool;
            TextTool.InitCaptureKeyboardInput = () => inputTextProvider.Focus();

            fontSizeProvider.InvokeOnUpdate = actualTool.SetFontSize;
            inputTextProvider.AcceptsTab = true;
            inputTextProvider.AcceptsReturn = true;
            inputTextProvider.TextChanged += InputTextProvider_TextChanged;
            inputTextProvider.PreviewKeyDown += InputTextProvider_KeyDown;
            fontPicker.FontFamilyChanged += FontPicker_FontFamilyChanged;
            fontPicker.FontChanged += FontPicker_FontChanged;

            boldProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetBold(true);
            italicProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetItalic(true);
            underlineProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetUnderline(true);
            strikeoutProvider.Checked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetStrikeout(true);

            boldProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetBold(false);
            italicProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetItalic(false);
            underlineProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetUnderline(false);
            strikeoutProvider.Unchecked += (object _, System.Windows.RoutedEventArgs _1) => TextTool.SetStrikeout(false);
        }

        private void FontPicker_FontChanged(FontFamily fontFamily, Color color, float fontSize, bool bold, bool italic, bool underline, bool strikeout)
        {
            TextTool.SetFontFamily(fontFamily);
            FontSizeProvider.SetValue((int)fontSize);
            BoldProvider.IsChecked = bold;
            ItalicProvider.IsChecked = italic;
            UnderlineProvider.IsChecked = underline;
            StrikeoutProvider.IsChecked = strikeout;
            //Commented to let the events handle setting the values to prevent async states of UI and Tool
            //TextTool.SetFont(fontFamily, fontSize, bold, italic, underline, strikethrough, color);
            
        }

        private void FontPicker_FontFamilyChanged(FontFamily fontFamily)
        {
            TextTool.SetFontFamily(fontFamily);
        }

        private void InputTextProvider_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Shift) == 0)
                {
                    e.Handled = true;
                    TextTool.DoneAndReset();
                    TextBox textBox = (TextBox)sender;
                    textBox.Clear();
                    Keyboard.ClearFocus();
                    //textBox.Focusable = false;
                    //textBox.Focusable = true;
                }
            }
        }

        private void InputTextProvider_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextTool.SetString(((TextBox)sender).Text);
        }

        public static ToolbarToolText Create(GifBitmapEditingContext editingContext, Action<IToolbarTool> activateTool)
        {
            ScrollableIntValue fontSizeValueProvider = ScrollableIntValue.Create(IMAGE_FONT_SIZE, 1, 999);
            TextBox inputTextProvider = new TextBox();
            FontPicker fontPicker = new FontPicker();
            CheckBox boldProvider = new CheckBox() { Content = "B", FontWeight = FontWeights.Bold };
            CheckBox italicProvider = new CheckBox() { Content = "I", FontStyle = FontStyles.Italic };
            CheckBox strikeoutProvider = new CheckBox() { 
                Content = new TextBlock()
                {
                    Text = "S",
                    TextDecorations = TextDecorations.Strikethrough,
                }
            };
            CheckBox underlineProvider = new CheckBox()
            {
                Content = new TextBlock()
                {
                    Text = "U",
                    TextDecorations = TextDecorations.Underline,
                }
            };
            CheckBox obliqueProvider = new CheckBox()
            {
                Content = new TextBlock()
                {
                    Text = "O",
                    FontStyle = FontStyles.Oblique,
                }
            };
            return new ToolbarToolText(activateTool, new TextTool(editingContext), fontSizeValueProvider, fontPicker, inputTextProvider, boldProvider, italicProvider, underlineProvider, strikeoutProvider, obliqueProvider);
        }
    }
}
