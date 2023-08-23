//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Windows.Media.Animation;

//namespace GifFingTool.Gui.Shortcuts
//{
//    //core code from https://docs.microsoft.com/en-us/archive/blogs/toub/low-level-keyboard-hook-in-c
//    //a great and fairly easy to read example of global hooks
//    class InterceptKeys
//    {
//        private const int WH_KEYBOARD_LL = 13;
//        private const int WM_KEYDOWN = 0x0100;
//        private const int WM_SYSKEYDOWN = 0x0104;
//        private static Dictionary<KeySet, Func<bool>> runOnHook = new Dictionary<KeySet, Func<bool>>();
//        private static Dictionary<Func<bool>, KeySet> runOnHookReverse = new Dictionary<Func<bool>, KeySet>();
//        private static Func<Keys, bool> _alwaysRunOnKeyDown;
//        private static LowLevelKeyboardProc _proc = HookCallback;
//        private static IntPtr _hookID = IntPtr.Zero;
//        private static KeyTimers keyTimers = new KeyTimers();
//        private static long _lastKeyTime = 0;
//        private static Keys _lastKey = Keys.None;

//        public static void InitHook()
//        {
//            _hookID = SetHook(_proc);
//        }

//        public static void SetAlwaysRunOnKeyDown(Func<Keys, bool> func)
//        {
//            _alwaysRunOnKeyDown = func;
//        }

//        public static void SetHookMethod(KeySet keySet, Func<bool> func)
//        {
//            RemoveHookMethod(func);
//            runOnHook.Add(keySet, func);
//            runOnHookReverse.Add(func, keySet);
//        }
//        public static void RemoveHookMethod(Func<bool> func)
//        {
//            if (runOnHookReverse.ContainsKey(func))
//            {
//                runOnHook.Remove(runOnHookReverse[func]);
//                runOnHookReverse.Remove(func);
//            }
//        }

//        public static void Unhook()
//        {
//            if (_hookID != IntPtr.Zero)
//            {
//                UnhookWindowsHookEx(_hookID);
//            }
//        }

//        //public static void Main()
//        //{
//        //    _hookID = SetHook(_proc);
//        //    Application.Run();
//        //    UnhookWindowsHookEx(_hookID);
//        //}

//        private static IntPtr SetHook(LowLevelKeyboardProc proc)
//        {
//            using (Process curProcess = Process.GetCurrentProcess())
//            using (ProcessModule curModule = curProcess.MainModule)
//            {
//                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
//                    GetModuleHandle(curModule.ModuleName), 0);
//            }
//        }

//        private delegate IntPtr LowLevelKeyboardProc(
//        int nCode, IntPtr wParam, IntPtr lParam);


//        private static void CheckForMatches(KeySet checkKeySet)
//        {
//            List<KeySet> toPause = new List<KeySet>();
//            List<KeySet> toActivate = new List<KeySet>();

//            foreach (KeySet keySet in runOnHook.Keys)
//            {
//                if (keySet.Matches(checkKeySet))
//                {
//                    //Return false -> do not fire again until key/keyCombination released
//                    if (!runOnHook[keySet].Invoke())
//                    {

//                    }
//                }
//            }
//        }



//        private static IntPtr HookCallback(
//            int nCode, IntPtr wParam, IntPtr lParam)
//        {
//            if (nCode >= 0 && (
//                    (wParam == (IntPtr)WM_KEYDOWN) || (wParam == (IntPtr)WM_SYSKEYDOWN))
//                    )
//            {
//                int vkCode = Marshal.ReadInt32(lParam);
//                //keyTimers.KeyPress((Keys)vkCode);
//                //Trace.WriteLine("Press" + (Keys)vkCode);
//                long now = KeyTimers.CurrentTimeMillis();
//                bool doContinue = true;

//                if (_alwaysRunOnKeyDown != null)
//                {
//                    doContinue = _alwaysRunOnKeyDown.Invoke((Keys)vkCode);
//                }

//                if (doContinue)
//                {
//                    if (now - _lastKeyTime < 700 && _lastKey != (Keys)vkCode)
//                    {
//                        //Trace.WriteLine("Check " + _lastKey + "+" + (Keys)vkCode);
//                        KeySet toCheck = new KeySet();
//                        toCheck.Add((Keys)vkCode);
//                        toCheck.Add(_lastKey);
//                        CheckForMatches(toCheck);
//                        _lastKey = Keys.None;
//                        _lastKeyTime = 0;

//                    }
//                    else
//                    {
//                        _lastKey = (Keys)vkCode;
//                        _lastKeyTime = now;
//                    }
//                }
//            }

//            return CallNextHookEx(_hookID, nCode, wParam, lParam);
//        }

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr SetWindowsHookEx(int idHook,
//            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        [return: MarshalAs(UnmanagedType.Bool)]
//        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

//        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
//            IntPtr wParam, IntPtr lParam);

//        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
//        private static extern IntPtr GetModuleHandle(string lpModuleName);
//    }
//}
