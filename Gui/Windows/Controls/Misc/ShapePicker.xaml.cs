using GifFingTool.Gui.Windows.Controls.Tools;
using GifFingTool.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GifFingTool.Gui.Windows.Controls.Misc
{
    /// <summary>
    /// Interaktionslogik für ShapePicker.xaml
    /// </summary>
    public partial class ShapePicker : Grid
    {
        internal delegate void SelectedShapeChanged(ShapesTool.Shape shape);

        internal event SelectedShapeChanged ShapeChanged;

        private static readonly Brush s_ImageBrush = new SolidBrush(Color.Black);
        private static Bitmap CreateImage(ShapesTool.Shape shape)
        {
            const int bitmapSize = 50;
            const int shapePadding = 5;
            const int shapeSize = bitmapSize - shapePadding - shapePadding;
            //Point p1 = new Point(shapePadding, shapePadding);
            //Point p2 = new Point(bitmapSize - shapePadding, bitmapSize - shapePadding);

            Bitmap bitmap = new Bitmap(bitmapSize, bitmapSize);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                switch (shape)
                {
                    case ShapesTool.Shape.Line:
                        g.FillRectangle(s_ImageBrush, shapePadding, 23, shapeSize, 5);
                        break;
                    case ShapesTool.Shape.Rectangle:
                        g.FillRectangle(s_ImageBrush, shapePadding, shapePadding, shapeSize, shapeSize);
                        break;
                    case ShapesTool.Shape.Ellipse:
                        g.FillEllipse(s_ImageBrush, shapePadding, shapePadding, shapeSize, shapeSize);
                        break;
                    case ShapesTool.Shape.TriangleLeft:
                        {
                            Point p1 = new Point(0, shapeSize / 2);
                            Point p2 = new Point(shapeSize, 0);
                            Point p3 = new Point(shapeSize, shapeSize);
                            g.FillPolygon(s_ImageBrush, new Point[] { p1, p2, p3 });
                        }
                        break;
                    case ShapesTool.Shape.TriangleTop:
                        {
                            Point p1 = new Point(shapeSize / 2, 0);
                            Point p2 = new Point(0, shapeSize);
                            Point p3 = new Point(shapeSize, shapeSize);
                            g.FillPolygon(s_ImageBrush, new Point[] { p1, p2, p3 });
                        }
                        break;
                    case ShapesTool.Shape.TriangleRight:
                        {
                            Point p1 = new Point(shapeSize, shapeSize / 2);
                            Point p2 = new Point(0, 0);
                            Point p3 = new Point(0, shapeSize);
                            g.FillPolygon(s_ImageBrush, new Point[] { p1, p2, p3 });
                        }
                        break;
                    case ShapesTool.Shape.TriangleBottom:
                        {
                            Point p1 = new Point(shapeSize / 2, shapeSize);
                            Point p2 = new Point(0, 0);
                            Point p3 = new Point(shapeSize, 0);
                            g.FillPolygon(s_ImageBrush, new Point[] { p1, p2, p3 });
                        }
                        break;
                    case ShapesTool.Shape.Pentagon:
                        {
                            Point p1 = new Point(shapePadding, shapePadding);
                            Point p2 = new Point(bitmapSize - shapePadding, bitmapSize - shapePadding);
                            Point[] points = PolygonUtil.CalculatePoints(5, p1, p2);
                            g.FillPolygon(s_ImageBrush, points);
                        }
                        break;
                    case ShapesTool.Shape.Hexagon:
                        {
                            Point p1 = new Point(shapePadding, shapePadding);
                            Point p2 = new Point(bitmapSize - shapePadding, bitmapSize - shapePadding);
                            Point[] points = PolygonUtil.CalculatePoints(6, p1, p2);
                            g.FillPolygon(s_ImageBrush, points);
                        }
                        break;
                    case ShapesTool.Shape.Heptagon:
                        {
                            Point p1 = new Point(shapePadding, shapePadding);
                            Point p2 = new Point(bitmapSize - shapePadding, bitmapSize - shapePadding);
                            Point[] points = PolygonUtil.CalculatePoints(7, p1, p2);
                            g.FillPolygon(s_ImageBrush, points);
                        }
                        break;
                }
            }
            return bitmap;
        }

        private void AddButtonToButtonGrid(ShapesTool.Shape shape)
        {
            _buttonGrid.AddItem(CreateImage(shape), () => ShapeChanged?.Invoke(shape));
        }


        public ShapePicker()
        {
            InitializeComponent();

            AddButtonToButtonGrid(ShapesTool.Shape.Line);
            AddButtonToButtonGrid(ShapesTool.Shape.Rectangle);
            AddButtonToButtonGrid(ShapesTool.Shape.Ellipse);
            AddButtonToButtonGrid(ShapesTool.Shape.TriangleLeft);
            AddButtonToButtonGrid(ShapesTool.Shape.Pentagon);
            AddButtonToButtonGrid(ShapesTool.Shape.Hexagon);
            AddButtonToButtonGrid(ShapesTool.Shape.Heptagon);
            AddButtonToButtonGrid(ShapesTool.Shape.TriangleTop);
            AddButtonToButtonGrid(ShapesTool.Shape.TriangleRight);
            AddButtonToButtonGrid(ShapesTool.Shape.TriangleBottom);
            _buttonGrid.SetRowCount(3);
            _buttonGrid.Rerender();

            this.Background = System.Windows.Media.Brushes.Beige;
            //this.Children.Add(_buttonGrid);
        }

        public void Deselect()
        {
            _buttonGrid.Deselect();
        }

        //private void ShapesListBox_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    //ScrollViewer scrollViewer = (ScrollViewer)sender;
        //    //scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset + e.VerticalChange);
        //}

        //private void ShapesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    FrameworkElement frameworkElement = (FrameworkElement)ShapesListBox.SelectedItem;
        //    //TODO: Refactor this class to not have the ToolTip to be of this significance, for example use a KeyValuePair as Items and Set SelectedValuePath and DisplayMemberPath and fill List via constructor
        //    switch (frameworkElement.ToolTip)
        //    {
        //        case "Line":
        //            ShapeChanged?.Invoke(ShapesTool.Shape.Line);
        //            break;
        //        case "Rectangle":
        //            ShapeChanged?.Invoke(ShapesTool.Shape.Rectangle);
        //            break;
        //        case "Ellipse":
        //            ShapeChanged?.Invoke(ShapesTool.Shape.Ellipse);
        //            break;
        //    }
        //}
    }
}
