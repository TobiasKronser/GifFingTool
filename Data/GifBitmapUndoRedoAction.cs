using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    internal class GifBitmapUndoRedoAction
    {
        private bool addItem;
        private IBitmapModifyStep bitmapModifyStep;
        private int _insertIndex;

        public GifBitmapUndoRedoAction(IBitmapModifyStep stepToUndo)
        {
            addItem = false;
            bitmapModifyStep = stepToUndo;
            _insertIndex = 0;
        }

        public GifBitmapUndoRedoAction(IBitmapModifyStep stepToRedo, int insertIndex)
        {
            addItem = true;
            bitmapModifyStep = stepToRedo;
            _insertIndex = insertIndex;
        }

        public void ApplyAndInvert(Action<IBitmapModifyStep, int> insertModifyStep, Func<IBitmapModifyStep, int> removeModifyStep)
        {
            if (addItem)
            {
                addItem = false;
                insertModifyStep(bitmapModifyStep, _insertIndex);
            } else
            {
                addItem = true;
                _insertIndex = removeModifyStep(bitmapModifyStep);
            }
        }

    }
}
