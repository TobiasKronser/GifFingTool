using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.GifEncoding
{
    public static class GifWriterExtensions
    {

        internal static void WriteGif(string savePath, IEnumerable<GifBitmap> gifBitmaps, int defaultDelay)
        {
            using (GifWriter gifWriter = new GifWriter(savePath))
            {
                gifWriter.DefaultFrameDelay = defaultDelay;
                foreach(GifBitmap gifBitmap in gifBitmaps)
                {
                    if (gifBitmap.CustomDelay is int customDelay)
                    {
                        gifWriter.WriteFrame(gifBitmap.ModifiedImage, customDelay);
                    } else
                    {
                        gifWriter.WriteFrame(gifBitmap.ModifiedImage);
                    }
                }
            }
        }

    }
}
