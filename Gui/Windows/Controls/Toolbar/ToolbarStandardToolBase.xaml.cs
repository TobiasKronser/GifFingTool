using GifFingTool.Data;
using GifFingTool.Gfx;
using GifFingTool.Gui.Windows.Controls.Misc;
using GifFingTool.Gui.Windows.Controls.Tools;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace GifFingTool.Gui.Windows.Controls.Toolbar
{
    /// <summary>
    /// Interaktionslogik für ToolbarToolBase.xaml
    /// </summary>
    public partial class ToolbarStandardToolBase : UserControl, IToolbarTool
    {
        private static readonly Brush BACKGROUND_ACTIVE = Brushes.DeepSkyBlue; // new SolidColorBrush(Colors.DeepSkyBlue);
        private static readonly Brush BACKGROUND_INACTIVE = Brushes.LightGray; // new SolidColorBrush(Colors.LightGray);

        internal static readonly System.Drawing.Bitmap s_NoImage = new System.Drawing.Bitmap(1, 1);

        private readonly ToolBaseV2 _actualTool;
        //ToolBaseV2 ActualTool { get => _actualTool; }
        ToolBaseV2 IToolbarTool.ActualTool => _actualTool;
        internal readonly double DisplayWidth;

        internal readonly System.Drawing.Bitmap MainImage;

        private List<UIElement> _UserControls = new List<UIElement>();
        protected readonly Action<IToolbarTool> ActivateTool;


        private bool _active;
        //public bool Active { get => _active; set => SetActive(value); }
        bool IToolbarTool.Active { get => _active; set => SetActive(value); }

        public UIElement RootUIElement => this;

        double IToolbarTool.DisplayWidth => DisplayWidth;

        internal ToolbarStandardToolBase(Action<IToolbarTool> activateTool, ToolBaseV2 actualTool, double displayWidth, System.Drawing.Bitmap mainImage = null, params UIElement[] subControls)
        {
            const int SUBCONTROL_WIDTH = 50;
            int subControlColumnCount = (int)Math.Floor((double)((subControls.Length + 2) / 3));

            InitializeComponent();
            _actualTool = actualTool;
            DisplayWidth = displayWidth + SUBCONTROL_WIDTH * subControlColumnCount;
            MainImage = mainImage ?? s_NoImage;
            Temp.printBitmap(MainButton.Image, MainImage);
            ActivateTool = activateTool;

            GridLength subControlWidth = new GridLength(SUBCONTROL_WIDTH);
            for(int i = 0; i < subControlColumnCount; i++)
            {
                ToolBaseGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = subControlWidth });
            }

            int column = 1;
            int row = 0;
            for(int i = 0; i < subControls.Length; i++)
            {
                UIElement userControl = subControls[i];
                ToolBaseGrid.Children.Add(userControl);
                Grid.SetColumn(userControl, column);
                Grid.SetRow(userControl, row);
                if (i % 3 == 2)
                {
                    column++;
                    row = 0;
                } else
                {
                    row++;
                }
            }
        }



        private void SetActive(bool value)
        {
            _active = value;
            this.MainButton.Background = _active ? BACKGROUND_ACTIVE : BACKGROUND_INACTIVE;
        }

        protected virtual void MainButton_Click(object sender, RoutedEventArgs e)
        {
            ActivateTool(this);
        }
    }
}
