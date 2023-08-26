using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace GifFingTool.Gui.GlobalHooks
{
    //core code (just modified a lot) from https://docs.microsoft.com/en-us/archive/blogs/toub/low-level-keyboard-hook-in-c
    public static class InterceptKeys
    {
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private static readonly Dictionary<Keys, (HashSet<Hook>, HashSet<TimedHook>, HashSet<TimeframeDependantHook>)> s_Hooks = new Dictionary<Keys, (HashSet<Hook>, HashSet<TimedHook>, HashSet<TimeframeDependantHook>)>();
        private static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static int[] TriggerTimeframes = new int[0];

        public static void AssureTimeframeCount(int count)
        {
            if (TriggerTimeframes.Length < count)
            {
                int[] newBackingArray = new int[count];
                for(int i = 0; i < TriggerTimeframes.Length; i++)
                {
                    newBackingArray[i] = TriggerTimeframes[i];
                }
                TriggerTimeframes = newBackingArray;
            }
        }

        public static void EnableTimeframe(int timeframeIndex)
        {
            TriggerTimeframes[timeframeIndex] = int.MaxValue;
        }
        public static void EnableTimeframe(int timeframeIndex, int millisDuration)
        {
            TriggerTimeframes[timeframeIndex] = Environment.TickCount + millisDuration;
        }

        public static void ToggleTimeframe(int timeframeIndex)
        {
            if (TriggerTimeframes[timeframeIndex] == int.MaxValue)
            {
                TriggerTimeframes[timeframeIndex] = int.MinValue;
            } else
            {
                TriggerTimeframes[timeframeIndex] = int.MaxValue;
            }
        }

        public static void DisableTimeframe(int timeframeId)
        {
            TriggerTimeframes[timeframeId] = 0;
        }

        public static void InitHook()
        {
            
            if (_hookID == IntPtr.Zero)
            {
                _hookID = SetHook(_proc);
                return;

                IntPtr SetHook(LowLevelKeyboardProc proc)
                {
                    using (Process curProcess = Process.GetCurrentProcess())
                    using (ProcessModule curModule = curProcess.MainModule)
                    {
                        return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                            GetModuleHandle(curModule.ModuleName), 0);
                    }
                }
            }
            Debug.WriteLine("InitHook was called while hooks are already initialized!");
        }
        public static void Unhook()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
            }
        }

        public static void RegisterHook(Hook hook)
        {
            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for(int i = 0; i < keys.Length; i++)
            {
                if(!s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets = (new HashSet<Hook>(), new HashSet<TimedHook>(), new HashSet<TimeframeDependantHook>());
                    s_Hooks[keys[i]] = hookSets;
                }

                hookSets.hooks.Add(hook);
            }
        }

        public static void RegisterHook(TimedHook hook)
        {
            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (!s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets = (new HashSet<Hook>(), new HashSet<TimedHook>(), new HashSet<TimeframeDependantHook>());
                    s_Hooks[keys[i]] = hookSets;
                }

                hookSets.timedHooks.Add(hook);
            }
        }

        public static void RegisterHook(TimeframeDependantHook hook)
        {
            if (hook.TimeframeIndex < 0 || TriggerTimeframes.Length <= hook.TimeframeIndex)
                throw new ArgumentOutOfRangeException($"A hook cannot be registered with timeframe-index:{hook.TimeframeIndex}. If the index is correct, {nameof(AssureTimeframeCount)} has to be called (with minimum {hook.TimeframeIndex + 1}) before creating this hook!");

            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (!s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets = (new HashSet<Hook>(), new HashSet<TimedHook>(), new HashSet<TimeframeDependantHook>());
                    s_Hooks[keys[i]] = hookSets;
                }

                hookSets.timeframeDependantHooks.Add(hook);
            }
        }

        public static void UnregisterHook(Hook hook)
        {
            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets.hooks.Remove(hook);
                }
            }
        }
        public static void UnregisterHook(TimedHook hook)
        {
            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets.timedHooks.Remove(hook);
                }
            }
        }
        public static void UnregisterHook(TimeframeDependantHook hook)
        {
            ReadOnlySpan<Keys> keys = hook.RequiredKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                if (s_Hooks.TryGetValue(keys[i], out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
                {
                    hookSets.timeframeDependantHooks.Remove(hook);
                }
            }
        }

        private static bool CheckForMatches(Keys pressedKey)
        {
            if (!s_Hooks.TryGetValue(pressedKey, out (HashSet<Hook> hooks, HashSet<TimedHook> timedHooks, HashSet<TimeframeDependantHook> timeframeDependantHooks) hookSets))
            {
                return false;
            }

            bool result = false;
            bool noShift = !IsKeyDown(Keys.ShiftKey);
            bool noControl = !IsKeyDown(Keys.ControlKey);
            bool noAlt = !IsKeyDown(Keys.Alt);

            int triggerTimeMillis = Environment.TickCount;
            foreach (TimedHook hook in hookSets.timedHooks)
            {
                if ((hook.ShiftRequired && noShift) ||
                    (hook.ControlRequired && noControl) ||
                    (hook.AltRequired && noAlt) )
                {
                    //The required modifiers are not pressed!
                    goto NextTimedHook;
                }

                foreach (Keys requiredKey in hook.RequiredKeys)
                {
                    if (requiredKey == pressedKey) continue;

                    if (!IsKeyDown(requiredKey))
                    {
                        goto NextTimedHook;
                    }
                }

                //Hook conditions are met
                if (hook.CheckPress(ref triggerTimeMillis))
                {
                    hook.HookAction();
                }

            NextTimedHook:;
            }

            foreach (TimeframeDependantHook hook in hookSets.timeframeDependantHooks)
            {
                if (TriggerTimeframes[hook.TimeframeIndex] < triggerTimeMillis)
                {
                    goto NextHook;
                }

                if ((hook.ShiftRequired && noShift) ||
                    (hook.ControlRequired && noControl) ||
                    (hook.AltRequired && noAlt))
                {
                    //The required modifiers are not pressed!
                    goto NextHook;
                }

                foreach (Keys requiredKey in hook.RequiredKeys)
                {
                    if (requiredKey == pressedKey) continue;

                    if (!IsKeyDown(requiredKey))
                    {
                        goto NextHook;
                    }
                }

                //Hook conditions are met
                hook.HookAction();
                result = result | hook.SupressOtherHooks;
            NextHook:;
            }

            foreach (Hook hook in hookSets.hooks)
            {
                if ((hook.ShiftRequired && noShift) ||
                    (hook.ControlRequired && noControl) ||
                    (hook.AltRequired && noAlt))
                {
                    //The required modifiers are not pressed!
                    goto NextHook;
                }

                foreach (Keys requiredKey in hook.RequiredKeys)
                {
                    if (requiredKey == pressedKey) continue;

                    if (!IsKeyDown(requiredKey))
                    {
                        goto NextHook;
                    }
                }

                //Hook conditions are met
                hook.HookAction();

            NextHook:;
            }

            return result;
        }

        private static bool IsKeyDown(Keys key)
        {
            short result = GetAsyncKeyState(key);
            return result < 0;
        }



        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (
                    (wParam == (IntPtr)WM_KEYDOWN) || (wParam == (IntPtr)WM_SYSKEYDOWN))
                    )
            {

                int vkCode = Marshal.ReadInt32(lParam);
                Keys pressedKey = (Keys)vkCode;
                if (CheckForMatches(pressedKey))
                {
                    return new IntPtr(1);
                }
            }

            //return new IntPtr(1); //TODO: CAREFULLY test if this literally blocks the user from the keyboard, if yes, figure out why Defender did not interveine
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys vKey);
    }
}
