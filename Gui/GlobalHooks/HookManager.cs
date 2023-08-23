using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gui.GlobalHooks
{
    public class HookManager
    {
        public static readonly HookManager Shared = new HookManager();

        private ConcurrentDictionary<Action, Hook> _Hooks = new ConcurrentDictionary<Action, Hook>();

        public HookManager() { }

        public void RegisterOrUpdateHookAction(Action action, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys) =>
            RegisterOrUpdateHookAction(action, keys.Length, shiftRequired, controlRequired, altRequired, keys);
        public void RegisterOrUpdateHookAction(Action action, int triggerKeyCount, bool shiftRequired = false, bool controlRequired = false, bool altRequired = false, params Keys[] keys)
        {
            UnregisterHookAction(action);
            Hook newHook = new Hook(action, triggerKeyCount, shiftRequired, controlRequired, altRequired, keys);
            _Hooks[action] = newHook;
            InterceptKeys.RegisterHook(newHook);
        }

        public void UnregisterHookAction(Action action)
        {
            if (_Hooks.TryGetValue(action, out Hook existingHook))
            {
                InterceptKeys.UnregisterHook(existingHook);
            }
        }


    }
}
