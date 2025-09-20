using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raylib_cs;

namespace IceSaw2.Settings
{
    public class HotkeySettings
    {
        //Convert to use a list so we can have multi hotkeys
        //General
        public KeyboardKey LevelWindow = KeyboardKey.L;
        public KeyboardKey MaterialWindow = KeyboardKey.M;
        public KeyboardKey PrefabWindow = KeyboardKey.P;

        public KeyboardKey OpenProject = KeyboardKey.F;

        //Level Viewer
        public MouseButton ActivateCamera = MouseButton.Right;

        public KeyboardKey Forward = KeyboardKey.W;
        public KeyboardKey Back = KeyboardKey.S;
        public KeyboardKey Left = KeyboardKey.A;
        public KeyboardKey Right = KeyboardKey.D;
        public KeyboardKey Up = KeyboardKey.E;
        public KeyboardKey Down = KeyboardKey.Q;
        public KeyboardKey Boost = KeyboardKey.LeftShift;

        //Custom Hotkey Voids for Pressing 2 or more keys
    }
}
