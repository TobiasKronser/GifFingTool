using GifFingTool.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GifFingTool.Gui.Windows.Controls.Tools
{
    public abstract class ToolBaseV2
    {
        private protected readonly GifBitmapEditingContext EditingContext;
        private protected abstract IBitmapModifyStep BitmapModifyStep { get; }

        public event Action NotifyInternalConfigChanged;

        private protected ToolBaseV2(GifBitmapEditingContext editingContext)
        {
            EditingContext = editingContext;
        }

        public void DoneAndReset()
        {
            EditingContext.ProcessToolOperationDone();
            Reset(); //TODO: Consider removal: Technically already called in ProcessToolOperationDone
        }
        public IBitmapModifyStep DoneTryGetModifier()
        {
            IBitmapModifyStep result = BitmapModifyStep;
            Reset();
            return result;
        }

        private protected void InvokeConfigChangedNotifyEvent()
        {
            NotifyInternalConfigChanged?.Invoke();
        }
        public abstract void Reset();
        public abstract void MouseLeftButtonDown(int x, int y);
        public abstract void MouseLeftButtonUp(int x, int y);
        public abstract void MouseRightButtonDown(int x, int y);
        public abstract void MouseRightButtonUp(int x, int y);
        public abstract void MouseMove(int x, int y);
        public abstract void MouseLeave(int x, int y);
        public abstract void MouseEnter(int x, int y);
        public abstract void MouseClick(int x, int y);
        public abstract void KeyDown(Key key);
    }
}
