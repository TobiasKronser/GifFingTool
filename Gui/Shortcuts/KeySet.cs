using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GifFingTool.Gui.Shortcuts
{
    public class KeySet
    {
        protected internal List<Keys> _keys = new List<Keys>();

        public KeySet()
        {

        }
        public KeySet(IEnumerable<Keys> keys)
        {
            foreach(Keys key in keys)
            {
                _keys.Add(key);
            }
        }

        public bool Matches(KeySet keys)
        {
            List<Keys> keysCopy = new List<Keys>();
            keysCopy.AddRange(keys._keys);
            foreach (Keys key in _keys)
            {
                if (keysCopy.Contains(key))
                {
                    keysCopy.Remove(key);
                }
                else
                {
                    return false;
                }
            }

            return keysCopy.Count == 0;
        }

        public void Add(Keys key)
        {
            if (!_keys.Contains(key))
            {
                _keys.Add(key);
            }
        }

    }
}
