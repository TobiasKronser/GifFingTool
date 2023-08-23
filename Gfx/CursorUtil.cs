﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gfx
{
    internal class CursorUtil
    {
        public struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);
        [DllImport("user32.dll")]
        public static extern IntPtr CreateIconIndirect(ref IconInfo icon);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool DestroyIcon(IntPtr handle);

        /// <summary>
        /// Create a cursor from a bitmap without resizing and with the specified
        /// hot spot
        /// </summary>
        public static Cursor CreateCursorNoResize(Bitmap bmp, int xHotSpot, int yHotSpot)
        {
            IntPtr ptr = bmp.GetHicon();
            IconInfo tmp = new IconInfo();
            GetIconInfo(ptr, ref tmp);
            tmp.xHotspot = xHotSpot;
            tmp.yHotspot = yHotSpot;
            tmp.fIcon = false;
            return new Cursor(CreateIconIndirect(ref tmp));
        }


        ///// <summary>
        ///// Create a 32x32 cursor from a bitmap, with the hot spot in the middle
        ///// </summary>
        //public static Cursor CreateCursor(Bitmap bmp)
        //{
        //    int xHotSpot = 16;
        //    int yHotSpot = 16;

        //    IntPtr ptr = ((Bitmap)ResizeImage(bmp, 32, 32)).GetHicon();
        //    IconInfo tmp = new IconInfo();
        //    GetIconInfo(ptr, ref tmp);
        //    tmp.xHotspot = xHotSpot;
        //    tmp.yHotspot = yHotSpot;
        //    tmp.fIcon = false;
        //    ptr = CreateIconIndirect(ref tmp);
        //    return new Cursor(ptr);
        //}
    }
}
