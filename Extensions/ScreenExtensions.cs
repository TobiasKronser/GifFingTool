using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Extensions
{
    public static class ScreenExtensions
    {
        
        public static Bitmap ScreenshotAllScreens()
        {
            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;
            int globalScreenWidth = SystemInformation.VirtualScreen.Width;
            int globalScreenHeight = SystemInformation.VirtualScreen.Height;

            Bitmap bitmap = new Bitmap(globalScreenWidth, globalScreenHeight);

            // Draw the screenshot into our bitmap.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(globalScreenLeft, globalScreenTop, 0, 0, bitmap.Size);
            }

            return bitmap;
        }

        public static Bitmap CreateScreenshot(this Screen screen, bool excludeTaskbar = false)
        {
            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;

            Rectangle targetArea = excludeTaskbar ? screen.WorkingArea : screen.Bounds;

            Bitmap targetScreenBitmap = new Bitmap(targetArea.Width, targetArea.Height);

            using (Graphics gfx = Graphics.FromImage(targetScreenBitmap))
            {
                gfx.CopyFromScreen(targetArea.Left, targetArea.Top, 0, 0, targetScreenBitmap.Size);
            }

            return targetScreenBitmap;
        }

        public static Bitmap ScreenshotArea(Rectangle targetArea)
        {
            int globalScreenLeft = SystemInformation.VirtualScreen.Left;
            int globalScreenTop = SystemInformation.VirtualScreen.Top;
            int globalScreenWidth = SystemInformation.VirtualScreen.Width;
            int globalScreenHeight = SystemInformation.VirtualScreen.Height;
            Bitmap bitmap;
            // Create a bitmap of the appropriate size to receive the screenshot.
            if (targetArea == null || targetArea.Width < 1 || targetArea.Height < 1)
            {
                targetArea = Rectangle.FromLTRB(globalScreenLeft, globalScreenTop, globalScreenWidth, globalScreenHeight);
            }

            bitmap = new Bitmap(targetArea.Width, targetArea.Height);

            // Draw the screenshot into our bitmap.
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(targetArea.Left, targetArea.Top, 0, 0, bitmap.Size);
            }

            return bitmap;
        }

    }
}
