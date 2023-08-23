using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using GifFingTool.Extensions;
using GifFingTool.Util;

namespace GifFingTool.Gui.Shortcuts
{
    sealed class ShortcutManager
    {
        private readonly IInputElement _InputElement;
        private readonly IDictionary<Key, ISet<IShortcut>> _KeyDownTriggers = new ConcurrentDictionary<Key, ISet<IShortcut>>();
        private readonly IDictionary<Key, ISet<IShortcut>> _KeyUpTriggers = new ConcurrentDictionary<Key, ISet<IShortcut>>();
        private readonly ConcurrentDictionary<Key, bool> _heldKeys = new ConcurrentDictionary<Key, bool>();

        public event Action OnChangeNotify;

        public ShortcutManager(IInputElement inputElement)
        {
            _InputElement = inputElement;
            _InputElement.KeyDown += _InputElement_KeyDown;
            _InputElement.KeyUp += _InputElement_KeyUp;
        }

        public ISet<IShortcut> GetShortcuts()
        {
            ISet<IShortcut> shortcuts = new HashSet<IShortcut>();
            foreach (ISet<IShortcut> shortcutSet in _KeyDownTriggers.Values)
            {
                shortcuts.AddAll(shortcutSet);
            }
            foreach (ISet<IShortcut> shortcutSet in _KeyUpTriggers.Values)
            {
                shortcuts.AddAll(shortcutSet);
            }
            return shortcuts;
        }
        public void RegisterKeyDownShortcut(string title, string description, Action action, Key triggerKey, bool exactMatchRequired, params Key[] requiredKeys) =>
            RegisterShortcut(title, description, action, triggerKey, true, exactMatchRequired, requiredKeys);

        public void RegisterKeyUpShortcut(string title, string description, Action action, Key triggerKey, bool exactMatchRequired, params Key[] requiredKeys) =>
            RegisterShortcut(title, description, action, triggerKey, false, exactMatchRequired, requiredKeys);


        public void RegisterShortcut(string title, string description, Action action, Key triggerKey, bool triggerOnKeyDown, bool exactMatchRequired, params Key[] requiredKeys)
        {
            IList<KeyGroup> keyGroups = new List<KeyGroup>();
            bool[] keyHandled = ArrayUtil.CreateAndInitialize(requiredKeys.Length, false);

            for (int i = 0; i < requiredKeys.Length; i++)
            {
                if (keyHandled[i]) continue;

                KeyGroup keyGroup = KeyGroup.GetDefaultKeyGroup(requiredKeys[i]);
                //Be sure to ignore future occurences
                foreach (Key key in keyGroup.Keys)
                {
                    for (int j = i + 1; j < requiredKeys.Length; j++)
                    {
                        if (requiredKeys[j] == key)
                        {
                            keyHandled[j] = true;
                        }
                    }
                }

                keyGroups.Add(keyGroup);
            }

            RegisterShortcut(title, description, action, triggerKey, triggerOnKeyDown, exactMatchRequired, keyGroups.ToArray());
        }
        public void RegisterKeyDownShortcut(string title, string description, Action action, Key triggerKey, bool exactMatchRequired, params KeyGroup[] requiredKeyGroups) =>
            RegisterShortcut(title, description, action, triggerKey, true, exactMatchRequired, requiredKeyGroups);

        public void RegisterKeyUpShortcut(string title, string description, Action action, Key triggerKey, bool exactMatchRequired, params KeyGroup[] requiredKeyGroups) =>
            RegisterShortcut(title, description, action, triggerKey, false, exactMatchRequired, requiredKeyGroups);


        public void RegisterShortcut(string title, string description, Action action, Key triggerKey, bool triggerOnKeyDown, bool exactMatchRequired, params KeyGroup[] requiredKeys)
        {
            if (triggerOnKeyDown)
            {
                if (!_KeyDownTriggers.TryGetValue(triggerKey, out ISet<IShortcut> shortcuts))
                {
                    shortcuts = new HashSet<IShortcut>();
                    _KeyDownTriggers.Add(triggerKey, shortcuts);
                }
                shortcuts.Add(new SimpleShortcut(title, description, action, triggerKey, triggerOnKeyDown, exactMatchRequired, requiredKeys));
            }
            else
            {
                if (!_KeyUpTriggers.TryGetValue(triggerKey, out ISet<IShortcut> shortcuts))
                {
                    shortcuts = new HashSet<IShortcut>();
                    _KeyUpTriggers.Add(triggerKey, shortcuts);
                }
                shortcuts.Add(new SimpleShortcut(title, description, action, triggerKey, triggerOnKeyDown, exactMatchRequired, requiredKeys));
            }
        }



        private void _InputElement_KeyDown(object sender, KeyEventArgs e)
        {
            _heldKeys[e.Key] = true;
            if (_KeyDownTriggers.TryGetValue(e.Key, out ISet<IShortcut> shortcuts))
            {
                foreach (IShortcut shortcut in shortcuts)
                {
                    shortcut.AttemptTrigger(sender, e, _heldKeys);
                }
            }
        }

        private void _InputElement_KeyUp(object sender, KeyEventArgs e)
        {
            _heldKeys.TryRemove(e.Key, out bool _);
            if (_KeyUpTriggers.TryGetValue(e.Key, out ISet<IShortcut> shortcuts))
            {
                foreach (IShortcut shortcut in shortcuts)
                {
                    shortcut.AttemptTrigger(sender, e, _heldKeys);
                }
            }
        }

        private sealed class SimpleShortcut : IShortcut
        {
            public Key TriggerKey { get; private set; }
            public bool TriggerOnKeyDown { get; private set; }

            public bool ExactMatchRequired { get; private set; }
            private HashSet<Key> _AllUsedKeys;
            private KeyGroup[] _RequiredKeys;
            public ReadOnlySpan<KeyGroup> RequiredKeys => new ReadOnlySpan<KeyGroup>(_RequiredKeys);

            public string Title { get; private set; }
            public string Description { get; private set; }

            private Action _Action;

            public SimpleShortcut(string title, string description, Action action, Key triggerKey, bool triggerOnKeyDown, bool exactMatchRequired, KeyGroup[] requiredKeys)
            {
                Title = title;
                Description = description;
                _Action = action;
                TriggerKey = triggerKey;
                TriggerOnKeyDown = triggerOnKeyDown;
                ExactMatchRequired = exactMatchRequired;
                _RequiredKeys = requiredKeys;
                _AllUsedKeys = new HashSet<Key>();
                _AllUsedKeys.Add(triggerKey);
                foreach (KeyGroup keyGroup in _RequiredKeys)
                {
                    foreach (Key key in keyGroup.Keys)
                    {
                        _AllUsedKeys.Add(key);
                    }
                }
            }

            public void AttemptTrigger(object sender, KeyEventArgs keyEventArgs, IDictionary<Key, bool> heldKeys)
            {
                if (keyEventArgs.Key != TriggerKey || keyEventArgs.IsDown != TriggerOnKeyDown)
                {
                    return;
                }

                if (ExactMatchRequired)
                {
                    foreach (Key key in heldKeys.Keys)
                    {
                        if (!_AllUsedKeys.Contains(key))
                        {
                            return;
                        }
                    }
                }

                foreach (KeyGroup keyGroup in _RequiredKeys)
                {
                    foreach (Key key in keyGroup.Keys)
                    {
                        if (Keyboard.IsKeyDown(key))
                        {
                            goto nextKeyGroup;
                        }
                    }
                    return;
                nextKeyGroup:;
                }

                _Action.Invoke();
            }
        }
    }

    interface IShortcut
    {
        Key TriggerKey { get; }
        bool TriggerOnKeyDown { get; }
        ReadOnlySpan<KeyGroup> RequiredKeys { get; }
        bool ExactMatchRequired { get; }
        string Title { get; }
        string Description { get; }
        void AttemptTrigger(object sender, KeyEventArgs keyEventArgs, IDictionary<Key, bool> heldKeys);
    }

    public class KeyGroup
    {
        private static ConcurrentDictionary<Key, KeyGroup> s_DefaultKeyGroups = GenerateDefaultKeyGroups();

        private static ConcurrentDictionary<Key, KeyGroup> GenerateDefaultKeyGroups()
        {
            ConcurrentDictionary<Key, KeyGroup> result = new ConcurrentDictionary<Key, KeyGroup>();

            KeyGroup shift = new KeyGroup("shift", "Shift", Key.LeftShift, Key.RightShift);
            KeyGroup alt = new KeyGroup("alt", "Alt", Key.LeftAlt, Key.RightAlt);
            KeyGroup ctrl = new KeyGroup("ctrl", "Strg", Key.LeftCtrl, Key.RightCtrl);
            KeyGroup win = new KeyGroup("Win", "Win", Key.LWin, Key.RWin);

            Type type = typeof(Key);
            foreach (Key key in Enum.GetValues(type))
            {
                switch (key)
                {
                    case Key.LeftShift:
                    case Key.RightShift:
                        result.TryAdd(key, shift);
                        break;
                    case Key.LeftAlt:
                    case Key.RightAlt:
                        result.TryAdd(key, alt);
                        break;
                    case Key.LeftCtrl:
                    case Key.RightCtrl:
                        result.TryAdd(key, ctrl);
                        break;
                    case Key.LWin:
                    case Key.RWin:
                        result.TryAdd(key, win);
                        break;
                    default:
                        string name = Enum.GetName(type, key);
                        result.TryAdd(key, new KeyGroup(name, name, key));
                        break;
                }
            }
            return result;
        }

        private static ConcurrentDictionary<string, KeyGroup> s_KeyGroups = new ConcurrentDictionary<string, KeyGroup>();


        public static bool TryGetKeyGroup(string name, out KeyGroup keyGroup)
        {
            return s_KeyGroups.TryGetValue(name, out keyGroup);
        }

        public static KeyGroup GetDefaultKeyGroup(Key key)
        {
            return s_DefaultKeyGroups[key];
        }

        public static bool TryCreateKeyGroup(string name, string title, out KeyGroup keyGroup, params Key[] keys)
        {
            if (s_KeyGroups.TryGetValue(name, out keyGroup)) return false;

            keyGroup = new KeyGroup(name, title, keys);
            return s_KeyGroups.TryAdd(name, keyGroup);
        }

        public readonly string Name;
        public readonly string Title;

        private readonly Key[] _Keys;
        public ReadOnlySpan<Key> Keys { get => new ReadOnlySpan<Key>(_Keys); }

        private KeyGroup(string name, string title, params Key[] keys)
        {
            Name = name;
            Title = title;
            _Keys = keys;
        }
    }
}
