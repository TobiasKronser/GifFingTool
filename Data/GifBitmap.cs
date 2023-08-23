using GifFingTool.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    public class GifBitmap : INotifyPropertyChanged
    {
        const int MAX_PREVIEW_WIDTH = 160;
        const int MAX_PREVIEW_HEIGHT = 90;
        private readonly Bitmap _BaseImage;
        public int? CustomDelay { get; set; }

        private RotateFlipType _RotateFlipState = RotateFlipType.RotateNoneFlipNone;
        public RotateFlipType RotateFlipState => _RotateFlipState;
        private readonly List<IBitmapModifyStep> _Modifications = new List<IBitmapModifyStep>();

        private readonly Stack<GifBitmapUndoRedoAction> _UndoStack = new Stack<GifBitmapUndoRedoAction>();
        private readonly Stack<GifBitmapUndoRedoAction> _RedoStack = new Stack<GifBitmapUndoRedoAction>();

        private Bitmap _ModifiedBitmap;
        private Bitmap _PreviewImage;

        public event PropertyChangedEventHandler PropertyChanged;

        public Bitmap BaseImage { get => _BaseImage; }
        public Bitmap ModifiedImage { get => _ModifiedBitmap; }
        public Bitmap PreviewImage { get => _PreviewImage; }

        public GifBitmap(Image baseImage) : this(new Bitmap(baseImage))
        {

        }

        private GifBitmap(Bitmap bitmap)
        {
            _BaseImage = bitmap;
            _ModifiedBitmap = bitmap;
            _PreviewImage = _ModifiedBitmap.RescaleKeepProportions(MAX_PREVIEW_WIDTH, MAX_PREVIEW_HEIGHT);
        }

        public static GifBitmap UseExistingBitmap(Bitmap bitmap)
        {
            return new GifBitmap(bitmap);
        }

        private void UpdateModifiedBitmap()
        {
            _ModifiedBitmap = (Bitmap)_BaseImage.Clone();
            for(int i = 0; i < _Modifications.Count; i++)
            {
                _ModifiedBitmap = _Modifications[i].ApplyTo(_ModifiedBitmap, _RotateFlipState, out RotateFlipType newRotateFlipType);
                _RotateFlipState = newRotateFlipType;
            }
            _PreviewImage = _ModifiedBitmap.RescaleKeepProportions(MAX_PREVIEW_WIDTH, MAX_PREVIEW_HEIGHT);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewImage"));
        }

        public void Apply(IBitmapModifyStep step)
        {
            _Modifications.Add(step);
            _ModifiedBitmap = step.ApplyTo(_ModifiedBitmap, _RotateFlipState, out RotateFlipType newRotateFlipState);
            _RotateFlipState = newRotateFlipState;

            _UndoStack.Push(new GifBitmapUndoRedoAction(step));

            _PreviewImage = _ModifiedBitmap.RescaleKeepProportions(MAX_PREVIEW_WIDTH, MAX_PREVIEW_HEIGHT);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PreviewImage"));
        }

        public void Undo()
        {
            if (_UndoStack.Count == 0) return;
            GifBitmapUndoRedoAction undoRedoAction = _UndoStack.Pop();
            undoRedoAction.ApplyAndInvert(InsertModifyStep, RemoveModifyStepReturnIndex);
            _RedoStack.Push(undoRedoAction);
        }

        public void Redo()
        {
            if (_RedoStack.Count == 0) return;
            GifBitmapUndoRedoAction undoRedoAction = _RedoStack.Pop();
            undoRedoAction.ApplyAndInvert(InsertModifyStep, RemoveModifyStepReturnIndex);
            _UndoStack.Push(undoRedoAction);
        }

        internal void Refresh()
        {
            UpdateModifiedBitmap();
        }

        internal int RemoveModifyStepReturnIndex(IBitmapModifyStep bitmapModifyStep)
        {
            //BUG TODO: THIS WAS CALLED WITH ITEM NOT EXISTING WITH MODIFICATIONS
            //somehow caused by a combination of undo redo undo and eraser
            int result = _Modifications.IndexOf(bitmapModifyStep);
            _Modifications.Remove(bitmapModifyStep);
            return result;
        }

        internal void RemoveModifyStep(IBitmapModifyStep bitmapModifyStep)
        {
            int index = _Modifications.IndexOf(bitmapModifyStep);
            if (index < 0) return;

            _Modifications.Remove(bitmapModifyStep);
            _UndoStack.Push(new GifBitmapUndoRedoAction(bitmapModifyStep, index));
        }

        private void InsertModifyStep(IBitmapModifyStep bitmapModifyStep, int insertIndex)
        {
            //BUG TODO: Figure out how insertIndex -1 was possible
            _Modifications.Insert(insertIndex, bitmapModifyStep);
        }

        internal IReadOnlyList<IBitmapModifyStep> GetModifySteps()
        {
            return _Modifications;
        }
    }
}
