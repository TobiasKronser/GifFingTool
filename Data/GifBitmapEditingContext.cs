using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    public sealed class GifBitmapEditingContext : IDisposable
    {
        public GifBitmap GifBitmap { get => _GifBitmap; set => ChangeTo(value); }
        public Bitmap Bitmap { get => _Bitmap; set => SetBitmap(value); }
        public BitmapData BitmapData { get => _BitmapData; }

        private GifBitmap _GifBitmap;
        private Bitmap _Bitmap;
        private BitmapData _BitmapData;

        private Action GifBitmapChanged;
        private Action BitmapChanged;
        private Action ToolOperationDone;

        public GifBitmapEditingContext(Action invokeAfterGifBitmapChange, Action invokeAfterDisplayBitmapChange, Action invokeWhenToolOperationDone)
        {
            GifBitmapChanged = invokeAfterGifBitmapChange;
            BitmapChanged = invokeAfterDisplayBitmapChange;
            ToolOperationDone = invokeWhenToolOperationDone;
        }

        public void ProcessToolOperationDone()
        {
            ToolOperationDone.Invoke();
        }

        public void ProcessChangesToBitmap()
        {
            BitmapChanged.Invoke();
        }

        public void ProcessChangesToGifBitmap()
        {
            _GifBitmap.Refresh();
            Bitmap = new Bitmap(_GifBitmap.ModifiedImage);
            GifBitmapChanged.Invoke();
        }

        private void ChangeTo(GifBitmap value)
        {
            _GifBitmap = value;
            if (_GifBitmap is null)
            {
                Dispose();
            } else
            {
                Bitmap = new Bitmap(value.ModifiedImage);
            }
        }

        private void SetBitmap(Bitmap value)
        {
            Dispose();
            _Bitmap = value;
            _BitmapData = _Bitmap.LockBits(new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapChanged.Invoke();
        }

        public void Dispose()
        {
            if (_Bitmap is null) return;
            if (!(BitmapData is null))
            {
                _Bitmap.UnlockBits(_BitmapData);
            }
            _Bitmap.Dispose();
        }

        internal void Undo()
        {
            if (_GifBitmap is null) return;

            _GifBitmap.Undo();
            ProcessChangesToGifBitmap();
        }
        internal void Redo()
        {
            if (_GifBitmap is null) return;

            _GifBitmap.Redo();
            ProcessChangesToGifBitmap();
        }

        internal void ResetBitmap()
        {
            Bitmap = new Bitmap(_GifBitmap.ModifiedImage);
        }
    }
}
