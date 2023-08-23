using GifFingTool.Data;
using GifFingTool.GifEncoding;
using GifFingTool.IO.FileTypeSaving;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GifFingTool.IO
{
    public static class SaveFileHelper
    {
        private static List<SaveFileTypeHelper> s_saveOptions = new List<SaveFileTypeHelper>()
        {
            new SaveFileTypeHelper(SaveGif, "Gif", "gif"),
            new SaveFileTypeHelper(SaveBmp, "Bitmap", "bmp"),
            new SaveFileTypeHelper(SavePng, "PNG", "png"),
            new SaveFileTypeHelper(SaveJpeg, "JPEG", "jpg", "jpeg"),
        };

        private static string ComposeFilter()
        {
            bool first = true;
            StringBuilder result = new StringBuilder();
            foreach(SaveFileTypeHelper saveFileTypeHelper in s_saveOptions)
            {
                if (!first)
                {
                    result.Append('|');
                }
                result.Append(saveFileTypeHelper.FileDialogFilterString);
                first = false;
            }
            return result.ToString();
        }

        private static bool TryGetSaveHelper(string saveFileExtension, out SaveFileTypeHelper result)
        {
            foreach(SaveFileTypeHelper saveFileTypeHelper in s_saveOptions)
            {
                if (saveFileTypeHelper.FileExtensions.Contains(saveFileExtension))
                {
                    result = saveFileTypeHelper;
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static bool TrySave(IList<GifBitmap> gifBitmaps, Bitmap selectedBitmap, bool copyToClipboard, out string savedFileFullPath)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Title = "Speichern . . .";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true; //TODO: Make this a setting
            //saveFileDialog.DefaultExt = "*.gif";
            saveFileDialog.FilterIndex = 0;
            saveFileDialog.Filter = ComposeFilter();
            if (!(bool)saveFileDialog.ShowDialog())
            {
                savedFileFullPath = string.Empty;
                return false;
            }

            string saveFileExtension = Path.GetExtension(saveFileDialog.SafeFileName).Substring(1);
            if (!TryGetSaveHelper(saveFileExtension, out SaveFileTypeHelper saveFileTypeHelper))
            {
                savedFileFullPath = string.Empty;
                return false;
            }

            savedFileFullPath = saveFileDialog.FileName;

            try
            {
                saveFileTypeHelper.SaveFile(savedFileFullPath, gifBitmaps, selectedBitmap);
                if (copyToClipboard)
                {
                    Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { savedFileFullPath });
                }
            } catch (Exception ex)
            {
                //TODO: Make this output more readable
                MessageBox.Show($"Failed to save File:\n{ex}");
            }

            return true;
        }


        private static void SaveBmp(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Bmp);
        private static void SavePng(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Png);
        private static void SaveJpeg(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Jpeg);
        private static void SaveIcon(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Icon);
        private static void SaveTiff(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Tiff);
        private static void SaveExif(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Exif);
        private static void SaveEmf(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Emf);
        private static void SaveWmf(string fileName, IList<GifBitmap> allBitmaps, Bitmap selectedBitmap) => SaveSingleImage(fileName, selectedBitmap, ImageFormat.Wmf);
        private static void SaveSingleImage(string fileName, Image image, ImageFormat imageFormat)
        {
            image.Save(fileName, imageFormat);
        }

        private static void SaveGif(string fileName, IList<GifBitmap> gifBitmaps, Bitmap selectedBitmap)
        {
            GifWriterExtensions.WriteGif(fileName, gifBitmaps, 1000);
        }
    }
}
