using Newtonsoft.Json;
using Raylib_cs;
using System.Diagnostics;

// TODO:
// - Add Window position, size, and if maximized to the general settings. This means I
// have to load settings after initializing the raylib window.
// - Settings should only hold keybindings, not input checking. 
// - Re-Save settings before shutdown.


namespace IceSaw2.Settings
{
    public class Hotkey
    {
        private Hotkey() { }
        private static readonly Hotkey _instance = new();
        public static Hotkey Instance { get { return _instance; } }

        public DataClass data = new();
        public class DataClass
        {
            //TODO: Convert to use a list so we can have multi hotkeys
            //General
            public KeyboardKey LevelWindow = KeyboardKey.L;
            public KeyboardKey LogicWindow = KeyboardKey.M;
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
        }


        public void Load()
        {
            string loadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string loadPath = Path.Combine(loadFolder, "Hotkeys.json");
            if (File.Exists(loadPath))
            {
                string stream = File.ReadAllText(loadPath);
                DataClass? loadedData = JsonConvert.DeserializeObject<DataClass>(stream);
                Debug.Assert(loadedData != null, "Container is null");
                data = loadedData;
                return;
            }
            Instance.Save();
            return;
        }

        public void Save()
        {
            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string savePath = Path.Combine(saveFolder, "Hotkeys.json");
            var serializer = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(savePath, serializer);
        }


    }
}
