using LiteDB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Nodindow.myPackages
{
    public class Hotkey
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        //Event that will be triggered when a hotkey is pressed
        public event EventHandler HotkeyPressed;
        public bool blockHotkeyPress = false;

        // Window handle
        private IntPtr hWnd;

        // Constants for modifiers
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        public List<HotkeyO> hotkeys = new List<HotkeyO>();

        public Hotkey(IntPtr hWnd)
        {
            this.hWnd = hWnd;
        }

        public string getJsonSave()
        {
            return JsonConvert.SerializeObject(hotkeys, Formatting.Indented);
        }
        public void loadJsonSave(string json)
        {
            this.hotkeys = JsonConvert.DeserializeObject<List<HotkeyO>>(json);
        }

        public bool Register(int id, string name, Keys key, uint modifiers, EventHandler eventHandler)
        {
            if (RegisterHotKey(hWnd, id, modifiers, (uint)key))
            {
                hotkeys.Add(new HotkeyO(id, name, key, modifiers, eventHandler));
                return true;
            }
            return false;
        }
        public bool Register(HotkeyO hotkey)
        {
            if (RegisterHotKey(hWnd, hotkey.id, hotkey.modifiers, (uint)hotkey.keys))
            {
                hotkeys.Add(hotkey);
                return true;
            }
            return false;
        }
        public bool Register(uint modifiers, EventHandler eventHandler, string name)
        {
            Keys defaultKey = Keys.None;
            int idFree = getIdFreeHotkeys();
            if (RegisterHotKey(hWnd, idFree, modifiers, (uint)defaultKey))
            {
                hotkeys.Add(new HotkeyO(idFree, name, defaultKey, modifiers, eventHandler));
                return true;
            }
            return false;
        }
        public bool Unregister(int id)
        {
            HotkeyO hotkey = this.hotkeys.Find(hotkey1 => hotkey1.id == id);
            this.hotkeys = hotkeys.Where(hotkey1 => hotkey1.id != id).ToList();
            return UnregisterHotKey(hWnd, hotkey.id);
        }

        protected void OnHotkeyPressed(int hotkeyId)
        {
            hotkeys.Find(hotkey => hotkey.id == hotkeyId).eventHandler?.Invoke(this, EventArgs.Empty);
        }

        public void ProcessMessage(Message m)
        {
            const int WM_HOTKEY = 0x0312;
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                if (hotkeys.FindIndex(hotkey => hotkey.id == id) != -1)
                {
                    if (!blockHotkeyPress)
                    {
                        OnHotkeyPressed(id);
                    }
                }
            }
        }
        private int getIdFreeHotkeys()
        {
            for (int id = 0; ; id++)
            {
                if (this.hotkeys.FindIndex(hotkey => hotkey.id == id) == -1)
                {
                    return id;
                }
            }
        }
        public static string FormatHotkey(Keys key, uint modifiers)
        {
            List<string> parts = new List<string>();

            if ((modifiers & MOD_WIN) != 0) parts.Add("Win");
            if ((modifiers & MOD_CONTROL) != 0) parts.Add("Ctrl");
            if ((modifiers & MOD_ALT) != 0) parts.Add("Alt");
            if ((modifiers & MOD_SHIFT) != 0) parts.Add("Shift");

            if (key != Keys.None)
                parts.Add(key.ToString());

            return string.Join(" + ", parts);
        }
        public HotkeyO UpdateHotkey(HotkeyO currentHotkey, Keys key, uint modifiers)
        {
            UnregisterHotKey(hWnd, currentHotkey.id);
            Register(currentHotkey.id, currentHotkey.name, key, modifiers, currentHotkey.eventHandler);
            int index = this.hotkeys.FindIndex(hotkey => hotkey.name == currentHotkey.name);
            this.hotkeys[index].keys = key;
            this.hotkeys[index].modifiers = modifiers;
            return this.hotkeys[index];
        }
        public void DisableHotkeys()
        {
            blockHotkeyPress = true;
        }

        public void EnableHotkeys()
        {
            blockHotkeyPress = false;
        }
        public class HotkeyO
        {
            public int id;
            public string name;
            public Keys keys { get; set; }
            public uint modifiers { get; set; }
            [JsonIgnore]
            public EventHandler eventHandler { get; set; }
            [JsonConstructor]
            public HotkeyO(int id, string name, Keys keys, uint modifiers, EventHandler eventHandler = null)
            {
                this.id = id;
                this.name = name;
                this.keys = keys;
                this.modifiers = modifiers;
                this.eventHandler = eventHandler;
            }
            public HotkeyO(HotkeyO hotkeyO, EventHandler eventHandler)
            {
                this.id = hotkeyO.id;
                this.name = hotkeyO.name;
                this.keys = hotkeyO.keys;
                this.modifiers = hotkeyO.modifiers;
                this.eventHandler = eventHandler;
            }
            public string GetStrHotkey()
            {
                List<string> parts = new List<string>();

                if ((modifiers & myPackages.Hotkey.MOD_WIN) != 0)
                {
                    parts.Add("Win");
                }
                if ((modifiers & myPackages.Hotkey.MOD_CONTROL) != 0)
                {
                    parts.Add("Ctrl");
                }
                if ((modifiers & myPackages.Hotkey.MOD_ALT) != 0)
                {
                    parts.Add("Alt");
                }
                if ((modifiers & myPackages.Hotkey.MOD_SHIFT) != 0)
                {
                    parts.Add("Shift");
                }

                if (keys != Keys.None)
                {
                    parts.Add(keys.ToString());
                }

                return string.Join(" + ", parts);
            }
        }
    }
}
