using GifFingTool.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//using System.Windows.Shapes;

namespace GifFingTool.Gui.Windows
{
    /// <summary>
    /// Interaktionslogik für TargetSelectionWindow.xaml
    /// </summary>
    public partial class TargetSelectionWindow : Window
    {
        private TargetArea _TargetArea;

        private Bitmap _ImageBase;
        private Bitmap _ImageWhitened;
        private Bitmap _ImageTargetArea;

        private bool _IsMouseSelectingActive = false;
        private bool _WasMovedMouseWhileSelecting = false;

        internal TargetSelectionWindow(TargetArea targetArea)
        {
            _TargetArea = targetArea;
            InitializeComponent();
            //this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(HandleKeyInput);
        }

        private Rectangle ToScreenTargetArea(Screen targetScreen, Rectangle globalTargetArea)
        {
            return Rectangle.FromLTRB(globalTargetArea.Left - targetScreen.WorkingArea.Left,
                                                     globalTargetArea.Top - targetScreen.WorkingArea.Top,
                                                     globalTargetArea.Right - targetScreen.WorkingArea.Left,
                                                     globalTargetArea.Bottom - targetScreen.WorkingArea.Top);
        }

        /// <summary>
        /// Opens the selection window in the targetScreen
        /// </summary>
        /// <param name="targetScreen"></param>
        /// <param name="currentGlobalTargetArea"></param>
        /// <returns></returns>
        public Rectangle ShowModalTargetSelect(Screen targetScreen, Rectangle currentGlobalTargetArea)
        {
            SetScreen(targetScreen);
            this.RenderSelection();

            this.ShowDialog();

            return _TargetArea.GetTargetArea(); //_TargetArea.CalculateGlobalTargetArea();

            void SetScreen(Screen screen)
            {
                _TargetArea.SetTargetScreen(screen);

                this.WindowStyle = WindowStyle.None;
                //this.WindowState = WindowState.Normal;
                //System.Threading.Thread.Sleep(200);
                this.Top = screen.Bounds.Top;
                this.Left = screen.Bounds.Left;
                //System.Threading.Thread.Sleep(200);
                this.Height = screen.Bounds.Height;
                this.Width = screen.Bounds.Width;
                this.WindowState = WindowState.Maximized;
                //System.Threading.Thread.Sleep(200);
                targetAreaPreview.Height = this.Height;
                targetAreaPreview.Width = this.Width;

                this.Topmost = true;


                this._ImageBase?.Dispose();
                this._ImageBase = screen.CreateScreenshot();


                this._ImageWhitened?.Dispose();
                this._ImageWhitened = (Bitmap)this._ImageBase.Clone();
                //this._ImageWhitened.ApplyTint(Properties.Settings.Default.ColorTargetAreaOverlay, Properties.Settings.Default.AlphaTargetAreaOverlay);
                this._ImageWhitened.ApplyTint(System.Drawing.Color.White, 180);
            }
        }

        private void GenerateCurrent()
        {
            this._ImageTargetArea?.Dispose();
            this._ImageTargetArea = (Bitmap)this._ImageWhitened.Clone();

            Rectangle targetRect = _TargetArea.GetTargetArea();
            Console.WriteLine($"TargetRect: ({targetRect.Width}, {targetRect.Height})");
            this._ImageTargetArea.DrawBorderAroundArea(targetRect, System.Drawing.Color.Red);
            this._ImageTargetArea.CopyRectangleFrom(_ImageBase, targetRect);
        }
        
        private void PrintCurrent()
        {
            Temp.printBitmap(targetAreaPreviewImage, _ImageTargetArea);
        }

        private void TargetAreaPreviewImage_Click(object sender, RoutedEventArgs e)
        {
            //this.Hide();
        }

        private System.Windows.Point toIntPoint(System.Windows.Point point)
        {
            return new System.Windows.Point((int)point.X, (int)point.Y);
        }

        private void RenderSelection()
        {
            GenerateCurrent();
            PrintCurrent();
        }


        private void TargetAreaPreview_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _IsMouseSelectingActive = true;
            _WasMovedMouseWhileSelecting = false;

            System.Windows.Point targetPoint = e.GetPosition(this);
            _TargetArea.SetPointA((int)targetPoint.X, (int)targetPoint.Y);
            _TargetArea.SetPointB((int)targetPoint.X, (int)targetPoint.Y);

            RenderSelection();
        }

        private void TargetAreaPreview_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _IsMouseSelectingActive = false;
            if (_WasMovedMouseWhileSelecting)
            {
                System.Windows.Point targetPoint = e.GetPosition(this);
                _TargetArea.SetPointB((int)targetPoint.X, (int)targetPoint.Y);
            }
            RenderSelection();
        }

        private void TargetAreaPreview_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_IsMouseSelectingActive)
            {
                System.Windows.Point targetPoint = e.GetPosition(this);
                _TargetArea.SetPointB((int)targetPoint.X, (int)targetPoint.Y);
                RenderSelection();
            }
        }

        private void TargetAreaPreview_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point targetPoint = e.GetPosition(this);
            _TargetArea.SetPointB((int)targetPoint.X, (int)targetPoint.Y);
            RenderSelection();
        }

        //KeyHandling

        //private void HandleKeyInput(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (Keyboard.Modifiers == ModifierKeys.Control)
        //    {
        //        switch (e.Key)
        //        {
        //            case Key.A:
        //                handleControlA(e);
        //                break;
        //            case Key.Q:
        //                handleControlQ(e);
        //                break;
        //        }
        //    }
        //    else if (Keyboard.Modifiers == ModifierKeys.Shift)
        //    {
        //        switch (e.Key)
        //        {
        //            case Key.Q:
        //                handleShiftQ(e);
        //                break;
        //            case Key.Left:
        //                handleShiftLeft(e);
        //                break;
        //            case Key.Up:
        //                handleShiftUp(e);
        //                break;
        //            case Key.Right:
        //                handleShiftRight(e);
        //                break;
        //            case Key.Down:
        //                handleShiftDown(e);
        //                break;

        //        }
        //    }
        //    else
        //    {
        //        switch (e.Key)
        //        {
        //            case Key.Escape:
        //                handleEscape(e);
        //                break;
        //            case Key.D1:
        //                handleOne(e);
        //                break;
        //            case Key.NumPad1:
        //                handleOne(e);
        //                break;
        //            case Key.D2:
        //                handleTwo(e);
        //                break;
        //            case Key.NumPad2:
        //                handleTwo(e);
        //                break;
        //            case Key.Tab:
        //                handleTab(e);
        //                break;
        //        }

        //        switch (e.Key)
        //        {
        //            case Key.Left:
        //                handleLeft(false, Keyboard.Modifiers == ModifierKeys.Control);
        //                break;
        //            case Key.Up:
        //                handleUp(false, Keyboard.Modifiers == ModifierKeys.Control);
        //                break;
        //            case Key.Right:
        //                handleRight(false, Keyboard.Modifiers == ModifierKeys.Control);
        //                break;
        //            case Key.Down:
        //                handleDown(false, Keyboard.Modifiers == ModifierKeys.Control);
        //                break;
        //        }
        //    }
        //}

        //private void handleControlA(System.Windows.Input.KeyEventArgs e)
        //{
        //    _TargetArea.SetPointA(0, 0);
        //    _TargetArea.SetPointB((int)this.Width, (int)this.Height);
        //    RenderSelection();
        //}

        //private System.Drawing.Color GetColorAt(byte[] bgra, int x, int y, int imgWidth)
        //{
        //    int pos = ((y * imgWidth) + x) * 3;
        //    return System.Drawing.Color.FromArgb(bgra[pos + 2], bgra[pos + 1], bgra[pos]);
        //}

        //private void handleShiftLeft(System.Windows.Input.KeyEventArgs e)
        //{
        //    int xStart = 0;
        //    int y = 0;
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            xStart = (int)_PointA.X;
        //            y = (int)_PointA.Y;
        //            break;
        //        case 1:
        //            xStart = (int)_PointB.X;
        //            y = (int)_PointB.Y;
        //            break;
        //    }

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color color = GetColorAt(rgbValues, xStart, y, width);

        //    for (int i = xStart - 1; i >= 0; i--)
        //    {
        //        if (GetColorAt(rgbValues, i, y, width).Equals(color))
        //        {
        //            xStart = i;
        //        }
        //        else
        //        {
        //            xStart = i;
        //            break;
        //        }
        //    }

        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            _PointA.X = xStart;
        //            break;
        //        case 1:
        //            _PointB.X = xStart;
        //            break;
        //    }

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}

        //private void handleShiftRight(System.Windows.Input.KeyEventArgs e)
        //{
        //    int xStart = 0;
        //    int y = 0;
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            xStart = (int)_PointA.X;
        //            y = (int)_PointA.Y;
        //            break;
        //        case 1:
        //            xStart = (int)_PointB.X;
        //            y = (int)_PointB.Y;
        //            break;
        //    }

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color color = GetColorAt(rgbValues, xStart, y, width);

        //    for (int i = xStart + 1; i < _ImageBase.Width; i++)
        //    {
        //        if (GetColorAt(rgbValues, i, y, width).Equals(color))
        //        {
        //            xStart = i;
        //        }
        //        else
        //        {
        //            xStart = i;
        //            break;
        //        }
        //    }

        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            _PointA.X = xStart;
        //            break;
        //        case 1:
        //            _PointB.X = xStart;
        //            break;
        //    }

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}

        //private void handleShiftUp(System.Windows.Input.KeyEventArgs e)
        //{
        //    int x = 0;
        //    int yStart = 0;
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            x = (int)_PointA.X;
        //            yStart = (int)_PointA.Y;
        //            break;
        //        case 1:
        //            x = (int)_PointB.X;
        //            yStart = (int)_PointB.Y;
        //            break;
        //    }

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color color = GetColorAt(rgbValues, x, yStart, width);

        //    for (int i = yStart - 1; i >= 0; i--)
        //    {
        //        if (GetColorAt(rgbValues, x, i, width).Equals(color))
        //        {
        //            yStart = i;
        //        }
        //        else
        //        {
        //            yStart = i;
        //            break;
        //        }
        //    }

        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            _PointA.Y = yStart;
        //            break;
        //        case 1:
        //            _PointB.Y = yStart;
        //            break;
        //    }

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}

        //private void handleShiftDown(System.Windows.Input.KeyEventArgs e)
        //{
        //    int x = 0;
        //    int yStart = 0;
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            x = (int)_PointA.X;
        //            yStart = (int)_PointA.Y;
        //            break;
        //        case 1:
        //            x = (int)_PointB.X;
        //            yStart = (int)_PointB.Y;
        //            break;
        //    }

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color color = GetColorAt(rgbValues, x, yStart, width);

        //    for (int i = yStart + 1; i < _ImageBase.Width; i++)
        //    {
        //        if (GetColorAt(rgbValues, x, i, width).Equals(color))
        //        {
        //            yStart = i;
        //        }
        //        else
        //        {
        //            yStart = i;
        //            break;
        //        }
        //    }

        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            _PointA.Y = yStart;
        //            break;
        //        case 1:
        //            _PointB.Y = yStart;
        //            break;
        //    }

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}

        //private void handleControlQ(System.Windows.Input.KeyEventArgs e)
        //{
        //    int x1 = (int)Math.Min(_PointA.X, _PointB.X);
        //    int x2 = (int)Math.Max(_PointA.X, _PointB.X);
        //    int y1 = (int)Math.Min(_PointA.Y, _PointB.Y);
        //    int y2 = (int)Math.Max(_PointA.Y, _PointB.Y);

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color colorP1 = GetColorAt(rgbValues, x1, y1, width);
        //    System.Drawing.Color colorP2 = GetColorAt(rgbValues, x2, y2, width);

        //    for (int i = x1 + 1; i < x2; i++)
        //    {
        //        if (GetColorAt(rgbValues, i, y1, width).Equals(colorP1))
        //        {
        //            x1 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = y1 + 1; i <= y2; i++)
        //    {
        //        if (GetColorAt(rgbValues, x1, i, width).Equals(colorP1))
        //        {
        //            y1 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = x2 - 1; i > x1; i--)
        //    {
        //        if (GetColorAt(rgbValues, i, y2, width).Equals(colorP2))
        //        {
        //            x2 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = y2 - 1; i > y1; i--)
        //    {
        //        if (GetColorAt(rgbValues, x2, i, width).Equals(colorP2))
        //        {
        //            y2 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    _PointA.X = x1;
        //    _PointA.Y = y1;
        //    _PointB.X = x2;
        //    _PointB.Y = y2;

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}


        //private void handleShiftQ(System.Windows.Input.KeyEventArgs e)
        //{
        //    int x1 = (int)Math.Min(_PointA.X, _PointB.X);
        //    int x2 = (int)Math.Max(_PointA.X, _PointB.X);
        //    int y1 = (int)Math.Min(_PointA.Y, _PointB.Y);
        //    int y2 = (int)Math.Max(_PointA.Y, _PointB.Y);

        //    System.Drawing.Imaging.BitmapData bmpData = _ImageBase.LockBits(_AreaScreen, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = Math.Abs(bmpData.Stride) * _ImageBase.Height;
        //    byte[] rgbValues = new byte[bytes];
        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
        //    int len = rgbValues.Length;

        //    int width = _ImageBase.Width;
        //    System.Drawing.Color colorP1 = GetColorAt(rgbValues, x1, y1, width);
        //    System.Drawing.Color colorP2 = GetColorAt(rgbValues, x2, y2, width);

        //    for (int i = x1 - 1; i >= 0; i--)
        //    {
        //        if (GetColorAt(rgbValues, i, y1, width).Equals(colorP1))
        //        {
        //            x1 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = y1 - 1; i >= 0; i--)
        //    {
        //        if (GetColorAt(rgbValues, x1, i, width).Equals(colorP1))
        //        {
        //            y1 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = x2 + 1; i < _ImageBase.Width; i++)
        //    {
        //        if (GetColorAt(rgbValues, i, y2, width).Equals(colorP2))
        //        {
        //            x2 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    for (int i = y2 + 1; i < _ImageBase.Height; i++)
        //    {
        //        if (GetColorAt(rgbValues, x2, i, width).Equals(colorP2))
        //        {
        //            y2 = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    _PointA.X = x1;
        //    _PointA.Y = y1;
        //    _PointB.X = x2;
        //    _PointB.Y = y2;

        //    // Unlock the bits.
        //    _ImageBase.UnlockBits(bmpData);

        //    RenderSelection();
        //}

        //private void handleEscape(System.Windows.Input.KeyEventArgs e)
        //{
        //    Hide();
        //}

        //private int getPointMoveDistance(bool shift, bool control)
        //{
        //    if (control)
        //    {
        //        if (shift)
        //        {
        //            //Appears to not work, at least in the testing environment
        //            return int.MaxValue;
        //        }
        //        else
        //        {
        //            return 10;
        //        }
        //    }
        //    else if (shift)
        //    {
        //        return 100;
        //    }
        //    else
        //    {
        //        return 1;
        //    }
        //}

        //private void handleLeft(bool shift, bool control)
        //{
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            decreasePointX(ref _PointA, getPointMoveDistance(shift, control));
        //            break;
        //        case 1:
        //            decreasePointX(ref _PointB, getPointMoveDistance(shift, control));
        //            break;
        //    }
        //    RenderSelection();
        //}
        //private void handleRight(bool shift, bool control)
        //{
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            increasePointX(ref _PointA, getPointMoveDistance(shift, control));
        //            break;
        //        case 1:
        //            increasePointX(ref _PointB, getPointMoveDistance(shift, control));
        //            break;
        //    }
        //    RenderSelection();
        //}
        //private void handleUp(bool shift, bool control)
        //{
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            decreasePointY(ref _PointA, getPointMoveDistance(shift, control));
        //            break;
        //        case 1:
        //            decreasePointY(ref _PointB, getPointMoveDistance(shift, control));
        //            break;
        //    }
        //    RenderSelection();
        //}
        //private void handleDown(bool shift, bool control)
        //{
        //    switch (_LastSetPointNr)
        //    {
        //        case 0:
        //            increasePointY(ref _PointA, getPointMoveDistance(shift, control));
        //            break;
        //        case 1:
        //            increasePointY(ref _PointB, getPointMoveDistance(shift, control));
        //            break;
        //    }
        //    RenderSelection();
        //}
        //private void handleOne(System.Windows.Input.KeyEventArgs e)
        //{
        //    _LastSetPointNr = 0;
        //    RenderSelection();
        //}
        //private void handleTwo(System.Windows.Input.KeyEventArgs e)
        //{
        //    _LastSetPointNr = 1;
        //    RenderSelection();
        //}
        //private void handleTab(System.Windows.Input.KeyEventArgs e)
        //{
        //    _LastSetPointNr = (_LastSetPointNr + 1) % 2;
        //    RenderSelection();
        //}

        private void CutFieldPreview_Activated(object sender, EventArgs e)
        {
            //System.Threading.Thread.Sleep(100);
            this.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void TargetAreaPreview_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }
    }
}
