using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gui.Shortcuts
{
    internal class KeyTimers
    {
        private static Dictionary<Keys, long> _lastKeyPress = new Dictionary<Keys, long>();
        private static readonly DateTime Jan1st1970 = new DateTime
        (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long CurrentTimeMillis()
        {
            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public KeyTimers()
        {

        }

        public void KeyPress(Keys key)
        {
            if (_lastKeyPress.ContainsKey(key))
            {
                _lastKeyPress[key] = CurrentTimeMillis();
            }
            else
            {
                _lastKeyPress.Add(key, CurrentTimeMillis());
            }
        }

        public long GetTime(Keys key)
        {
            if (_lastKeyPress.ContainsKey(key))
            {
                return _lastKeyPress[key];
            }
            else
            {
                return 0;
            }
        }

        public bool Matches(KeySet keySet, long millis)
        {
            long now = CurrentTimeMillis();

            foreach (Keys key in keySet._keys)
            {
                if (now - GetTime(key) > millis)
                {
                    return false;
                }
            }

            return true;
        }

        
    }
}
