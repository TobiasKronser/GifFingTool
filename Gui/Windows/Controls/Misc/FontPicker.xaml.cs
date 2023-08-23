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
//using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GifFingTool.Gui.Windows.Controls.Misc
{
    /// <summary>
    /// Interaktionslogik für FontPicker.xaml
    /// </summary>
    public partial class FontPicker : UserControl
    {
        public delegate void FontFamilyChangedEventArgs(FontFamily fontFamily);  // delegate
        public delegate void FontChangedEventArgs(FontFamily fontFamily, Color color, float fontSize, bool bold, bool italic, bool underline, bool strikethrough);  // delegate
        public event FontFamilyChangedEventArgs FontFamilyChanged;
        public event FontChangedEventArgs FontChanged;

        public FontPicker()
        {
            InitializeComponent();

            this.FontNameComboBox.DisplayMemberPath = "Name";
            foreach (FontFamily fontFamilyName in System.Drawing.FontFamily.Families)
            {
                this.FontNameComboBox.Items.Add(fontFamilyName);
            }
            this.FontNameComboBox.MouseRightButtonUp += FontPicker_MouseRightButtonDown;
            this.FontNameComboBox.SelectionChanged += FontNameComboBox_SelectionChanged;
        }

        private void FontPicker_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.FontDialog fontDialog = new System.Windows.Forms.FontDialog()
            {
                ShowColor = true,
                ShowApply = true,
            };
            if ((fontDialog.ShowDialog() & System.Windows.Forms.DialogResult.OK | System.Windows.Forms.DialogResult.Yes) > 0)
            {
                FontChanged?.Invoke(fontDialog.Font.FontFamily, fontDialog.Color, fontDialog.Font.Size, fontDialog.Font.Bold, fontDialog.Font.Italic, fontDialog.Font.Underline, fontDialog.Font.Strikeout);
            }
        }

        private void FontNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FontNameComboBox.SelectedItem is FontFamily fontFamily)
            {
                FontFamilyChanged?.Invoke(fontFamily);
            }
        }


    }
}
