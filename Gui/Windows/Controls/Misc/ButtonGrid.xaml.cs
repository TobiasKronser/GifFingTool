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
    /// Interaktionslogik für ButtonGrid.xaml
    /// </summary>
    public partial class ButtonGrid : Grid
    {
        private readonly Dictionary<ImageButton, Action> _items = new Dictionary<ImageButton, Action>();
        private ImageButton _selectedItem = null;
        private int _rowCount = 1;

        public ButtonGrid()
        {
            InitializeComponent();
        }

        public void SetRowCount(int rowCount)
        {
            Grid.SetIsSharedSizeScope(this, true);
            _rowCount = rowCount;
        }

        public void Deselect()
        {
            if (!(_selectedItem is null))
            {
                _selectedItem.Background = Brushes.Transparent;
            }
        }

        private void GridButtonClick(object sender, RoutedEventArgs e)
        {
            if (!(_selectedItem is null))
            {
                _selectedItem.Background = Brushes.Transparent;
            }
            _selectedItem = (ImageButton)sender;
            _selectedItem.Background = Brushes.DeepSkyBlue;
            if (_items.TryGetValue(_selectedItem, out Action clickAction)) {
                clickAction();
            }
        }

        public void AddItem(System.Drawing.Image image, Action clickAction)
        {
            ImageButton imgButton = new ImageButton() { Background = Brushes.Transparent, BorderBrush = Brushes.Transparent };
            Temp.printImage(imgButton.Image, image);
            imgButton.Click += GridButtonClick;
            _items.Add(imgButton, clickAction);
        }

        public void Rerender()
        {
            if (this.RowDefinitions.Count != _rowCount)
            {
                this.RowDefinitions.Clear();
                GridLength gridLength = new GridLength(1, GridUnitType.Star);
                for (int i = 0; i < _rowCount; i++)
                {
                    this.RowDefinitions.Add(new RowDefinition() { Height = gridLength });
                }
            }

            int requiredColumnCount = (_items.Count + 2) / _rowCount;
            
            if (this.ColumnDefinitions.Count != requiredColumnCount)
            {
                this.ColumnDefinitions.Clear();
                GridLength gridLength = new GridLength(1, GridUnitType.Star);

                for (int i = 0; i < requiredColumnCount; i++)
                {
                    this.ColumnDefinitions.Add(new ColumnDefinition() { SharedSizeGroup = "A" });
                }
            }

            this.Children.Clear();

            int col = 0;
            int row = 0;
            foreach(ImageButton imageButton in _items.Keys)
            {
                this.Children.Add(imageButton);
                Grid.SetColumn(imageButton, col);
                Grid.SetRow(imageButton, row);
                row++;
                if (row >= _rowCount)
                {
                    row = 0;
                    col++;
                }
            }
        }

    }
}
