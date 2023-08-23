using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gui.Dialogs
{
    public static class UtilDialogs
    {
        public static bool ShowColorDialog(bool allowAllColors, ref Color color)
        {
            ColorDialog dialog = new ColorDialog
            {
                AllowFullOpen = allowAllColors,
                // Allows the user to get help. (The default is false.)
                ShowHelp = true,
                // Sets the initial color select to the current text color.
                Color = color
            };

            // Update the text box color if the user clicks OK 
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                color = dialog.Color;
                return true;
            }

            return false;
        }
        public static bool ShowFontDialog(ref Font font)
        {
            FontDialog dialog = new FontDialog
            {
                ShowColor = false,
                ShowHelp = true,
                Font = font
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                font = dialog.Font;
                return true;
            }

            return false;
        }
        public static bool ShowFontDialog(ref Font font, ref Color color)
        {
            FontDialog dialog = new FontDialog
            {
                ShowColor = true,
                ShowHelp = true,
                Font = font,
                Color = color
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                font = dialog.Font;
                color = dialog.Color;
                return true;
            }

            return false;
        }
    }
}
