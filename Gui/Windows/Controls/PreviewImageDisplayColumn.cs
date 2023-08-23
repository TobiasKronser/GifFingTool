using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GifFingTool.Data;
using System.Windows.Media;

namespace GifFingTool.Gui.Windows.Controls
{
    internal class PreviewImageDisplayColumn : DataGridBoundColumn
    {
        private static readonly Brush DELAY_BACKGROUND_BRUSH = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200));

        public PreviewImageDisplayColumn()
        {
            CanUserSort = false;
        }

        private FrameworkElement GenerateCellElement(object dataItem)
        {
            Grid grid = new Grid();
            grid.Margin = new Thickness(5);
            if (!(dataItem is GifBitmap gifBitmap))
            {
                grid.Children.Add(new TextBlock() { Text = $"Error, got {dataItem.GetType()}." });
                return grid;
            }

            //Thickness contentMargin = new Thickness(1);

            Image image = new Image()
            {
                //Margin = contentMargin,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            TextBlock textBlock = new TextBlock()
            {
                //Margin = contentMargin,
                Padding = new Thickness(4, 0, 4, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Background = DELAY_BACKGROUND_BRUSH,
                Text = "1000 ms",
            };
            
            grid.Children.Add(image);
            grid.Children.Add(textBlock);

            Temp.printBitmap(image, gifBitmap.PreviewImage);
            return grid;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return GenerateCellElement(dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return GenerateCellElement(dataItem);
        }
    }
}
