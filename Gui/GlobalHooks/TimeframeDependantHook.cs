using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GifFingTool.Gui.GlobalHooks
{
    public sealed class TimeframeDependantHook
    {
        public readonly int TriggerKeyCount;
        private Keys[] _RequiredKeys;
        public ReadOnlySpan<Keys> RequiredKeys => _RequiredKeys;
        public readonly bool ShiftRequired;
        public readonly bool ControlRequired;
        public readonly bool AltRequired;
        public readonly Action HookAction;
        public readonly int TimeframeIndex;
        public readonly bool SupressOtherHooks;

        public TimeframeDependantHook(Action hookAction, int timeframeIndex, bool supressOtherHooks, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
            : this(hookAction, timeframeIndex, keys.Length, supressOtherHooks, shiftRequired, controlRequired, altRequired, keys) { }

        public TimeframeDependantHook(Action hookAction, int timeframeIndex, int triggerKeyCount, bool supressOtherHooks, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
        {
            TriggerKeyCount = triggerKeyCount;
            TimeframeIndex = timeframeIndex;
            if (TriggerKeyCount < 1) throw new ArgumentException($"Failed to create Hook: There must not be at least one trigger key!");
            _RequiredKeys = (Keys[])keys.Clone();
            HookAction = hookAction ?? throw new ArgumentNullException($"Cannot create Hook: Parameter {nameof(hookAction)} was null!");
            ShiftRequired = shiftRequired;
            ControlRequired = controlRequired;
            AltRequired = altRequired;
            SupressOtherHooks = supressOtherHooks;
        }

        public TimeframeDependantHook(Action hookAction, int timeframeIndex, bool supressOtherHooks, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, timeframeIndex, keys.Length, supressOtherHooks, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

        public TimeframeDependantHook(Action hookAction, int timeframeIndex, int triggerKeyCount, bool supressOtherHooks, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, timeframeIndex, triggerKeyCount, supressOtherHooks, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

    }
}
