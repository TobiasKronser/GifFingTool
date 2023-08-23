using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace GifFingTool.Gui.GlobalHooks
{
    public sealed class Hook
    {
        public readonly int TriggerKeyCount;
        private Keys[] _RequiredKeys;
        public ReadOnlySpan<Keys> RequiredKeys => _RequiredKeys;
        public readonly bool ShiftRequired;
        public readonly bool ControlRequired;
        public readonly bool AltRequired;
        public readonly Action HookAction;

        public Hook(Action hookAction, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
            : this(hookAction, keys.Length, shiftRequired, controlRequired, altRequired, keys) { }

        public Hook(Action hookAction, int triggerKeyCount, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
        {
            TriggerKeyCount = triggerKeyCount;
            if (TriggerKeyCount < 1) throw new ArgumentException($"Failed to create Hook: There must not be at least one trigger key!");
            _RequiredKeys = (Keys[])keys.Clone();
            HookAction = hookAction ?? throw new ArgumentNullException($"Cannot create Hook: Parameter {nameof(hookAction)} was null!");
            ShiftRequired = shiftRequired;
            ControlRequired = controlRequired;
            AltRequired = altRequired;
        }

        public Hook(Action hookAction, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, keys.Length, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

        public Hook(Action hookAction, int triggerKeyCount, ModifierKeys requiredModifiers = ModifierKeys.None, params Keys[] keys)
            : this(hookAction, triggerKeyCount, (requiredModifiers & ModifierKeys.Shift) > 0, (requiredModifiers & ModifierKeys.Control) > 0, (requiredModifiers & ModifierKeys.Alt) > 0, keys) { }

    }
}
