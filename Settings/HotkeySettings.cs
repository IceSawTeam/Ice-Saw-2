using Newtonsoft.Json;
using Raylib_cs;
using System.Diagnostics;


namespace IceSaw2.Settings
{
    public class HotkeySettings
    {
        //TODO: Convert to use a list so we can have multi hotkeys
        //General
        public KeyboardKey LevelWindow = KeyboardKey.L;
        public KeyboardKey MaterialWindow = KeyboardKey.M;
        public KeyboardKey PrefabWindow = KeyboardKey.P;
        public KeyboardKey OpenProject = KeyboardKey.F;

        // Camera movement
        public MouseButton ActivateCamera = MouseButton.Right;
        public KeyboardKey Forward = KeyboardKey.W;
        public KeyboardKey Back = KeyboardKey.S;
        public KeyboardKey Left = KeyboardKey.A;
        public KeyboardKey Right = KeyboardKey.D;
        public KeyboardKey Up = KeyboardKey.E;
        public KeyboardKey Down = KeyboardKey.Q;
        public KeyboardKey Boost = KeyboardKey.LeftShift;


        public void CreateJson(string path, bool pretty = false)
        {
            var TempFormating = pretty ? Formatting.Indented : Formatting.None;
            var serializer = JsonConvert.SerializeObject(this, TempFormating);
            File.WriteAllText(path, serializer);
        }


        public static HotkeySettings Load(string path)
        {
            string loadPath = path;
            if (File.Exists(loadPath))
            {
                string stream = File.ReadAllText(loadPath);
                HotkeySettings? container = JsonConvert.DeserializeObject<HotkeySettings>(stream);
                Debug.Assert(container != null, "Container is null");
                return container;
            }
            return new HotkeySettings();
        }
    }
}
