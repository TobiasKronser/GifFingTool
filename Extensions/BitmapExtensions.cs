using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace GifFingTool.Extensions
{
    public static class BitmapExtensions
    {
        public static readonly Bitmap NO_IMAGE = new Bitmap(1, 1);

        #region Tint
        public static void ApplyTint(this Bitmap bitmap, Color color, byte alpha) => ApplyTint(bitmap, Color.FromArgb(alpha, color.R, color.G, color.B));
        public static void ApplyTint(this Bitmap bitmap, Color color)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                using (Brush cloud_brush = new SolidBrush(color))
                {
                    Rectangle bounds = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);
                    g.FillRectangle(cloud_brush, bounds);
                }
            }
        }
        #endregion Tint

        #region CopyShapes
        public static void CopyRectangleFrom(this Bitmap bitmap, Bitmap rectangleSource, Rectangle area)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(rectangleSource, area, area, GraphicsUnit.Pixel);
            }
        }
        public static Bitmap ExtractRectangle(this Bitmap rectangleSource, Rectangle area)
        {
            Bitmap result = new Bitmap(area.Width, area.Height);
            Rectangle destRect = new Rectangle(0, 0, area.Width, area.Height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(rectangleSource, destRect, area, GraphicsUnit.Pixel);
            }
            return result;
        }
        #endregion CopyShapes

        #region FillShapes
        public static void FillRectangle(this Bitmap bitmap, Rectangle area, Color color)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //Rectangle borderRect = Rectangle.FromLTRB(targetRect.Left - 1, targetRect.Top - 1,
                //                                          targetRect.Left + targetRect.Width + 1, // + 2,
                //                                          targetRect.Top + targetRect.Height + 1);// + 2);
                using (SolidBrush brush = new SolidBrush(Properties.Settings.Default.ColorTargetAreaOverlay))
                {
                    g.FillRectangle(brush, area);
                }
            }
        }
        #endregion FillShapes

        #region DrawShapes
        public static void DrawBorderAroundArea(this Bitmap bitmap, Rectangle area, Color color)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                int left = area.Left - 1;
                int top = area.Top - 1;
                int right = area.Left + area.Width;
                int bottom = area.Top + area.Height;
                using (Pen pen = new Pen(Color.Red))
                {
                    g.DrawLine(pen, left, top, right, top);
                    g.DrawLine(pen, left, bottom, right, bottom);
                    g.DrawLine(pen, left, top, left, bottom);
                    g.DrawLine(pen, right, top, right, bottom);
                }
            }
        }
        #endregion DrawShapes

        #region Rescale
        public static Bitmap RescaleKeepProportions(this Bitmap bitmap, int newMaxWidth, int newMaxHeight)
        {
            //Get the image current width  
            int sourceWidth = bitmap.Width;
            //Get the image current height  
            int sourceHeight = bitmap.Height;
            float nPercent = 0;
            //Calulate  width with new desired size  
            float nPercentW = ((float)newMaxWidth / (float)sourceWidth);
            //Calculate height with new desired size  
            float nPercentH = ((float)newMaxHeight / (float)sourceHeight);
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
                g.DrawImage(bitmap, 0, 0, destWidth, destHeight);
            }
            return b;
        }
        #endregion Rescale

    }
}
