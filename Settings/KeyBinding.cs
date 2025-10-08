using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raylib_cs;
using System.Diagnostics;


namespace IceSaw2.Settings
{
    public class KeyBinding
    {
        private KeyBinding() { }
        private static readonly KeyBinding _instance = new();
        public static KeyBinding Instance { get { return _instance; } }
        public string KeyBindingVersion = "1";


        public class InputAction(string name)
        {
            public string Name = name;

            // Holds compinations of Keyboardkey and MouseButton that represent this action.
            public List<List<object>> InputEvents = []; 

            public void AddInputEvent(params object[] inputs)
            {
                List<object> inputList = [];
                foreach (object i in inputs)
                {
                    Debug.Assert(i.GetType() == typeof(KeyboardKey) || i.GetType() == typeof(MouseButton),
                                 "Input action is not a KeyboardKey or MouseButton ");
                    inputList.Add(i);
                }
                InputEvents.Add(inputList);
            }
        }


        public class DataClass
        {
            public string Version = "1";
            public List<InputAction> InputMap = [];

            public DataClass()
            {
                // Camera movement
                InputAction cameraMoveLeft = new("CameraMoveLeft");
                cameraMoveLeft.AddInputEvent(KeyboardKey.A);
                InputMap.Add(cameraMoveLeft);
                InputAction cameraMoveRight = new("CameraMoveRight");
                cameraMoveRight.AddInputEvent(KeyboardKey.D);
                InputMap.Add(cameraMoveRight);
                InputAction cameraMoveForward = new("CameraMoveForward");
                cameraMoveForward.AddInputEvent(KeyboardKey.W);
                InputMap.Add(cameraMoveForward);
                InputAction cameraMoveBack = new("CameraMoveBack");
                cameraMoveBack.AddInputEvent(KeyboardKey.S);
                InputMap.Add(cameraMoveBack);
                InputAction cameraMoveUp = new("CameraMoveUp");
                cameraMoveUp.AddInputEvent(KeyboardKey.E);
                InputMap.Add(cameraMoveUp);
                InputAction cameraMoveDown = new("CameraMoveDown");
                cameraMoveDown.AddInputEvent(KeyboardKey.Q);
                InputMap.Add(cameraMoveDown);
                InputAction cameraBoost = new("CameraBoost");
                cameraBoost.AddInputEvent(KeyboardKey.LeftShift);
                cameraBoost.AddInputEvent(MouseButton.Left);
                InputMap.Add(cameraBoost);
                InputAction cameraActivate = new("CameraActivate");
                cameraActivate.AddInputEvent(MouseButton.Right);
                InputMap.Add(cameraActivate); 

                // Tab switching
                InputAction levelSwitch = new("LevelSwitch");
                levelSwitch.AddInputEvent(KeyboardKey.L);
                InputMap.Add(levelSwitch);
                InputAction modelSwitch = new("ModelSwitch");
                modelSwitch.AddInputEvent(KeyboardKey.P);
                InputMap.Add(modelSwitch);
                InputAction logicSwitch = new("LogicSwitch");
                logicSwitch.AddInputEvent(KeyboardKey.N);
                InputMap.Add(logicSwitch);

                // Other
                InputAction click = new("Click");
                click.AddInputEvent(MouseButton.Left);
                InputMap.Add(click);
                InputAction save = new("Save");
                save.AddInputEvent(KeyboardKey.LeftControl, KeyboardKey.S);
                InputMap.Add(save);
                InputAction exit = new("Exit");
                exit.AddInputEvent(KeyboardKey.Escape);
                InputMap.Add(exit);
            }
        }
        public DataClass Data = new();


        public InputAction GetInputActionByName(string name)
        {
            foreach (InputAction action in Data.InputMap)
            {
                if (action.Name == name)
                {
                    return action;
                }
            }
            Debug.Assert(false, $"Input action '{name}' does not exist");
            return new InputAction("Invalid");
        }


        public void Load()
        {
            string loadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string loadPath = Path.Combine(loadFolder, "KeyBinding.json");
            if (File.Exists(loadPath))
            {
                string fileText = File.ReadAllText(loadPath);
                var parsedJson = JObject.Parse(fileText);
                if (parsedJson.TryGetValue("version", StringComparison.Ordinal, out JToken? ver))
                {
                    if (ver.Type == JTokenType.String && ver.ToString() == KeyBindingVersion)
                    {
                        DataClass? loadedData = JsonConvert.DeserializeObject<DataClass>(fileText);
                        Debug.Assert(loadedData != null, "DeserializeObject is null");
                        Data = loadedData;
                        return;
                    }
                }
            }

            // Save if file doesn't exist, or if it has a different version.
            Console.WriteLine("Disk Keybindings were incompatible. Overritten with newer version.");
            Instance.Save();
            return;
        }


        public void Save()
        {
            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string savePath = Path.Combine(saveFolder, "KeyBinding.json");
            var serializer = JsonConvert.SerializeObject(Data, Formatting.Indented);
            File.WriteAllText(savePath, serializer);
        }
    }
}
