using Newtonsoft.Json;
using Raylib_cs;
using System.Diagnostics;


// TODO:
// - Integrate the keybinding system config json.
// - Work on Input.cs using the ActionMap.


namespace IceSaw2.Settings
{
    public class KeyBinding
    {
        private KeyBinding() { }
        private static readonly KeyBinding _instance = new();
        public static KeyBinding Instance { get { return _instance; } }

        public enum InputActionType
        {
            // Camera movement
            CameraMoveLeft,
            CameraMoveRight,
            CameraMoveForward,
            CameraMoveBack,
            CameraMoveUp,
            CameraMoveDown,
            CameraBoost,
            CameraActivate,

            // Tab switching
            LevelSwitch,
            ModelsSwitch,
            LogicSwitch,

            // Other
            Save,
            Exit,
        }

        public struct InputAction
        {
            public Type inputTag = typeof(KeyboardKey);
            public KeyboardKey key;
            public MouseButton button;

            public InputAction(KeyboardKey key)
            {
                inputTag = typeof(KeyboardKey);
                this.key = key;
            }

            public InputAction(MouseButton button)
            {
                inputTag = typeof(MouseButton);
                this.button = button;
            }
        }

        public Dictionary<InputActionType, List<InputAction>> ActionMap = new()
        {
            { InputActionType.CameraMoveLeft, [new InputAction(KeyboardKey.A)] },
            { InputActionType.CameraMoveRight, [new InputAction(KeyboardKey.D)] },
            { InputActionType.CameraMoveForward, [new InputAction(KeyboardKey.W)] },
            { InputActionType.CameraMoveBack, [new InputAction(KeyboardKey.S)] },
            { InputActionType.CameraMoveUp, [new InputAction(KeyboardKey.E)] },
            { InputActionType.CameraMoveDown, [new InputAction(KeyboardKey.Q)] },
            { InputActionType.CameraBoost, [new InputAction(MouseButton.Left)] },
            { InputActionType.CameraActivate, [new InputAction(MouseButton.Right)] },
            { InputActionType.LevelSwitch, [new InputAction(KeyboardKey.L)] },
            { InputActionType.ModelsSwitch, [new InputAction(KeyboardKey.P)] },
            { InputActionType.LogicSwitch, [new InputAction(KeyboardKey.M)] },
            { InputActionType.Save, [new InputAction(KeyboardKey.LeftControl), new InputAction(KeyboardKey.S)] },
            { InputActionType.Exit, [new InputAction(KeyboardKey.E)] },
        };



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
            string loadPath = Path.Combine(loadFolder, "KeyBinding.json");
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
            string savePath = Path.Combine(saveFolder, "KeyBinding.json");
            var serializer = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(savePath, serializer);
        }


    }
}
