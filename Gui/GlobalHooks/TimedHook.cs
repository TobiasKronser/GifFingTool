using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GifFingTool.Gui.GlobalHooks
{
    public sealed class TimedHook
    {
        public readonly int TriggerKeyCount;
        private Keys[] _RequiredKeys;
        public ReadOnlySpan<Keys> RequiredKeys => _RequiredKeys;
        public readonly bool ShiftRequired;
        public readonly bool ControlRequired;
        public readonly bool AltRequired;
        public readonly Action HookAction;
        private int CurrentTimeframe = 0;
        private readonly int MillisTimeframe = 0;

        public TimedHook(Action hookAction, int millisTimeframe, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
            : this(hookAction, millisTimeframe, keys.Length, shiftRequired, controlRequired, altRequired, keys) { }

        public TimedHook(Action hookAction, int millisTimeframe, int triggerKeyCount, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
        {
            TriggerKeyCount = triggerKeyCount;
            MillisTimeframe = millisTimeframe;
            if (TriggerKeyCount < 1) throw new ArgumentException($"Failed to create Hook: There must not be at least one trigger key!");
            _RequiredKeys = (Keys[])keys.Clone();
            HookAction = hookAction ?? throw new ArgumentNullException($"Cannot create Hook: Parameter {nameof(hookAction)} was null!");
            ShiftRequired = shiftRequired;
            ControlRequired = controlRequired;
            AltRequired = altRequired;
        }

        public TimedHook(Action hookAction, int millisTimeframe, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, millisTimeframe, keys.Length, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

        public TimedHook(Action hookAction, int millisTimeframe, int triggerKeyCount, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, millisTimeframe, triggerKeyCount, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

        internal bool CheckPress(ref int triggerTime)
        {
            if (CurrentTimeframe > triggerTime)
            {
                CurrentTimeframe = 0;
                return true;
            }
            CurrentTimeframe = triggerTime + MillisTimeframe;
            return false;
        }
    }
}
