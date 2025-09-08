using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace IceSaw2.Settings
{
    public static class HotkeySettings
    {
        //Convert to use a list so we can have multi hotkeys

        public static KeyboardKey LevelWindow = KeyboardKey.L;
        public static KeyboardKey MaterialWindow = KeyboardKey.M;
        public static KeyboardKey PrefabWindow = KeyboardKey.P;

        public static KeyboardKey OpenProject = KeyboardKey.F;

        //Custom Hotkey Voids for Pressing 2 or more keys
    }
}
