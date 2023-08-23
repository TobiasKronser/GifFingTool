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
    /// Interaktionslogik für ImageButton.xaml
    /// </summary>
    public partial class ImageButton : Button
    {
        public ImageSource ImageSource { get => DisplayImage.Source; set => DisplayImage.Source = value; }
        public Image Image { get => DisplayImage; }

        public ImageButton()
        {
            InitializeComponent();
        }
    }
}
