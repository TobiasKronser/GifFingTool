using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows;

namespace GifFingTool.Gui.Windows
{
    internal class Temp
    {
        public static Bitmap GrayOut(Bitmap bitmap)
        {
            Rectangle rectangle = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmpData.Height;
            byte[] argb = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(ptr, argb, 0, bytes);

            for (int i = 0; i < bytes; i = i + 4)
            {
                Color color = GetColorAt(argb, i, true);
                SetColorAt(argb, i, true, Color.FromArgb(color.A, (color.R + 128) / 2, (color.G + 128) / 2, (color.B + 128) / 2));
            }

            System.Runtime.InteropServices.Marshal.Copy(argb, 0, ptr, bytes);
            bitmap.UnlockBits(bmpData);

            return bitmap;
        }
        public static void GrayOut(ref Bitmap bitmap)
        {
            bitmap = GrayOut(bitmap);
        }

        public static System.Windows.Media.Color ColorDrawingToMedia(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        public static System.Drawing.Color ColorMediaToDrawing(System.Windows.Media.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static Bitmap ScreenshotEverything()
        {
            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;
            int globalScreenWidth = SystemInformation.VirtualScreen.Width;
            int globalScreenHeight = SystemInformation.VirtualScreen.Height;

            System.Drawing.Rectangle bitmapBounds = System.Drawing.Rectangle.FromLTRB(0, 0, globalScreenWidth, globalScreenHeight);

            Bitmap bitmap = new Bitmap(globalScreenWidth, globalScreenHeight);

            // Draw the screenshot into our bitmap.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(globalScreenLeft, globalScreenTop, 0, 0, bitmap.Size);
            }

            return bitmap;
        }

        public static Bitmap ScreenshotScreen(Screen targetScreen)
        {

            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;

            System.Drawing.Rectangle targetScreenWorkingArea = targetScreen.WorkingArea;

            Bitmap targetScreenBitmap = new Bitmap(targetScreenWorkingArea.Width, targetScreenWorkingArea.Height);

            using (Graphics gfx = Graphics.FromImage(targetScreenBitmap))
            {
                gfx.CopyFromScreen(targetScreenWorkingArea.Left, targetScreenWorkingArea.Top, 0, 0, targetScreenBitmap.Size);
            }

            return targetScreenBitmap;
        }
        public static Bitmap ScreenshotArea(System.Drawing.Rectangle targetArea)
        {
            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;
            int globalScreenWidth = SystemInformation.VirtualScreen.Width;
            int globalScreenHeight = SystemInformation.VirtualScreen.Height;
            Bitmap bitmap;
            // Create a bitmap of the appropriate size to receive the screenshot.
            if (targetArea == null || targetArea.Width < 1 || targetArea.Height < 1)
            {
                targetArea = System.Drawing.Rectangle.FromLTRB(globalScreenLeft, globalScreenTop, globalScreenWidth, globalScreenHeight);
            }

            bitmap = new Bitmap(targetArea.Width, targetArea.Height);

            // Draw the screenshot into our bitmap.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(targetArea.Left, targetArea.Top, 0, 0, bitmap.Size);
            }

            return bitmap;
        }

        /// <summary>
        /// Code from https://www.c-sharpcorner.com/UploadFile/ishbandhu2009/resize-an-image-in-C-Sharp/
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Bitmap resizeImage(Bitmap bmpToResize, System.Windows.Size size)
        {
            //Get the image current width  
            int sourceWidth = bmpToResize.Width;
            //Get the image current height  
            int sourceHeight = bmpToResize.Height;
            float nPercent = 0;
            //Calulate  width with new desired size  
            float nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size  
            float nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width  
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height  
            int destHeight = (int)(sourceHeight * nPercent);
            Bitmap b = new Bitmap(destWidth, destHeight);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                // Draw image with new width and height  
                g.DrawImage(bmpToResize, 0, 0, destWidth, destHeight);
            }
            return b;
        }

        public static void printPreviewBitmap(System.Windows.Controls.Image image, Bitmap bitmap)
        {
            if (image.DesiredSize.Width != 0 && image.DesiredSize.Height != 0)
                printAndDeleteHBitmap(image, resizeImage(bitmap, image.DesiredSize).GetHbitmap());
        }

        public static void printBitmap(System.Windows.Controls.Image image, Bitmap bitmap)
        {
            printAndDeleteHBitmap(image, bitmap.GetHbitmap());
        }

        public static void printImage(System.Windows.Controls.Image imageControl, Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                printBitmap(imageControl, bitmap);
            }
        }

        public static void printHBitmap(System.Windows.Controls.Image image, IntPtr hBitmap)
        {
            try
            {
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                image.Source = source;
            }
            finally
            {

            }
        }

        public static System.Windows.Media.ImageSource GetImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
                GC.Collect();
            }
        }

        public static void printAndDeleteHBitmap(System.Windows.Controls.Image image, IntPtr hBitmap)
        {
            try
            {
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                image.Source = source;
            }
            finally
            {
                DeleteObject(hBitmap);
                GC.Collect();
            }
        }

        public static void printBitmap(List<System.Windows.Controls.Image> images, Bitmap bitmap)
        {
            printAndDeleteHBitmap(images, bitmap.GetHbitmap());
        }
        public static void printAndDeleteHBitmap(List<System.Windows.Controls.Image> images, IntPtr hBitmap)
        {
            try
            {
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                foreach (System.Windows.Controls.Image image in images)
                {
                    image.Source = source;
                }
            }
            finally
            {
                DeleteObject(hBitmap);
                GC.Collect();
            }
        }

        public static BitmapSource ToBitmapSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
                GC.Collect();
            }
        }

        public static float MsToFps(int ms)
        {
            return (1000 / ms);
        }

        public static int FpsToMs(float fps)
        {
            return (int)(1000 / fps);
        }

        public static System.Drawing.Color GetColorAt(byte[] bgra, int x, int y, int imgWidth, bool hasAlpha)
        {
            if (hasAlpha)
            {
                int pos = ((y * imgWidth) + x) * 4;
                return System.Drawing.Color.FromArgb(bgra[pos + 3], bgra[pos + 2], bgra[pos + 1], bgra[pos]);
            }
            else
            {
                int pos = ((y * imgWidth) + x) * 3;
                return System.Drawing.Color.FromArgb(bgra[pos + 2], bgra[pos + 1], bgra[pos]);
            }
        }
        public static System.Drawing.Color GetColorAt(byte[] bgra, int pos, bool hasAlpha)
        {
            if (hasAlpha)
            {
                return System.Drawing.Color.FromArgb(bgra[pos + 3], bgra[pos + 2], bgra[pos + 1], bgra[pos]);
            }
            else
            {
                return System.Drawing.Color.FromArgb(bgra[pos + 2], bgra[pos + 1], bgra[pos]);
            }
        }

        public static void SetColorAt(byte[] bgra, int x, int y, int imgWidth, bool hasAlpha, System.Drawing.Color newColor)
        {
            if (hasAlpha)
            {
                int pos = ((y * imgWidth) + x) * 4;
                bgra[pos + 3] = newColor.A;
                bgra[pos + 2] = newColor.R;
                bgra[pos + 1] = newColor.G;
                bgra[pos] = newColor.B;

            }
            else
            {
                int pos = ((y * imgWidth) + x) * 3;
                bgra[pos + 2] = newColor.R;
                bgra[pos + 1] = newColor.G;
                bgra[pos] = newColor.B;
            }
        }
        public static void SetColorAt(byte[] bgra, int position, bool hasAlpha, System.Drawing.Color newColor)
        {
            if (hasAlpha)
            {
                bgra[position + 3] = newColor.A;
                bgra[position + 2] = newColor.R;
                bgra[position + 1] = newColor.G;
                bgra[position] = newColor.B;

            }
            else
            {
                bgra[position + 2] = newColor.R;
                bgra[position + 1] = newColor.G;
                bgra[position] = newColor.B;
            }
        }

        public static bool EqualsIgnoreAlpha(Color color1, Color color2)
        {
            return color1.R == color2.R && color1.G == color2.G && color1.B == color2.B;
        }

        public static Bitmap ReplaceColorIgnoreAlpha(Bitmap bitmap, Color oldColor, Color newColor)
        {
            Bitmap toReturn = new Bitmap(bitmap.Width, bitmap.Height);
            Rectangle rect = new Rectangle(0, 0, toReturn.Width, toReturn.Height);

            System.Drawing.Imaging.BitmapData bmpDataInput = bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData bmpDataOutput = toReturn.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            // Get the address of the first line.

            IntPtr ptrInput = bmpDataInput.Scan0;
            IntPtr ptrOutput = bmpDataOutput.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpDataOutput.Stride) * toReturn.Height;
            byte[] rgbValuesInput = new byte[bytes];
            byte[] rgbValuesOutput = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptrInput, rgbValuesInput, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(ptrOutput, rgbValuesOutput, 0, bytes);
            int len = rgbValuesInput.Length;

            int width = toReturn.Width;

            for (int i = 0; i < toReturn.Width; i++)
            {
                for (int j = 0; j < toReturn.Height; j++)
                {
                    Color curColor = Temp.GetColorAt(rgbValuesInput, i, j, width, true);
                    if (Temp.EqualsIgnoreAlpha(curColor, oldColor))
                    {
                        SetColorAt(rgbValuesOutput, i, j, width, true, newColor);
                    }
                    else
                    {
                        SetColorAt(rgbValuesOutput, i, j, width, true, curColor);
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValuesOutput, 0, ptrOutput, bytes);

            bitmap.UnlockBits(bmpDataInput);
            toReturn.UnlockBits(bmpDataOutput);

            return toReturn;
        }


        public static bool ShowColorDialog(bool allowAllColors, ref Color color)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.AllowFullOpen = allowAllColors;
            // Allows the user to get help. (The default is false.)
            dialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            dialog.Color = color;

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
            FontDialog dialog = new FontDialog();
            dialog.ShowColor = false;

            dialog.ShowHelp = true;
            dialog.Font = font;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                font = dialog.Font;
                return true;
            }

            return false;
        }
        public static bool ShowFontDialog(ref Font font, ref Color color)
        {
            FontDialog dialog = new FontDialog();
            dialog.ShowColor = true;

            dialog.ShowHelp = true;
            dialog.Font = font;
            dialog.Color = color;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                font = dialog.Font;
                color = dialog.Color;
                return true;
            }

            return false;
        }

        public static Screen GetWindowScreen(Window window)
        {
            System.Drawing.Point point = new System.Drawing.Point((int)window.Left, (int)window.Top);
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.Contains(point))
                {
                    return screen;
                }
            }
            return Screen.PrimaryScreen;
        }

        public static void ModifyMargin(System.Windows.Controls.Control target, double? newLeft = null, double? newTop = null, double? newRight = null, double? newBottom = null)
        {
            if (newLeft == null) newLeft = target.Margin.Left;
            if (newTop == null) newTop = target.Margin.Top;
            if (newRight == null) newRight = target.Margin.Right;
            if (newBottom == null) newBottom = target.Margin.Bottom;

            target.Margin = new Thickness((double)newLeft, (double)newTop, (double)newRight, (double)newBottom);
        }
    }
}
