using GifFingTool.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.IO.FileTypeSaving
{
    internal class SaveFileTypeHelper
    {
        public readonly string Description;
        public readonly string[] FileExtensions;
        public readonly string FileDialogFilterString;
        public readonly Action<string, IList<GifBitmap>, Bitmap> SaveFile;

        public SaveFileTypeHelper(Action<string, IList<GifBitmap>, Bitmap> saveFile, string description, params string[] fileExtensions)
        {
            if (fileExtensions.Length == 0) throw new ArgumentException("Each instance of SaveFileTypeHelper must contain at least one file extension, but got an empty array!");
            Description = description ?? throw new ArgumentNullException("A SaveFileTypeHelper's cannot be initialized with null!");
            SaveFile = saveFile ?? throw new ArgumentNullException("");
            
            FileExtensions = new string[fileExtensions.Length];
            
            StringBuilder filterBuilder = new StringBuilder();
            filterBuilder.Append(description);
            filterBuilder.Append('|');

            FileExtensions[0] = fileExtensions[0].ToLower();
            filterBuilder.Append("*.");
            filterBuilder.Append(FileExtensions[0]);


            for (int i = 1; i < fileExtensions.Length; i++)
            {
                FileExtensions[i] = fileExtensions[i].ToLower();
                filterBuilder.Append(';');
                filterBuilder.Append("*.");
                filterBuilder.Append(FileExtensions[i]);
            }

            FileDialogFilterString = filterBuilder.ToString();
        }

    }
}
