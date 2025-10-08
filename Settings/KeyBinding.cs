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
                InputMap.Add(new InputAction("CameraMoveLeft"));
                Instance.GetInputActionByName("CameraMoveLeft").AddInputEvent(KeyboardKey.A);
                InputMap.Add(new InputAction("CameraMoveRight"));
                Instance.GetInputActionByName("CameraMoveRight").AddInputEvent(KeyboardKey.D);
                InputMap.Add(new InputAction("CameraMoveForward"));
                Instance.GetInputActionByName("CameraMoveForward").AddInputEvent(KeyboardKey.W);
                InputMap.Add(new InputAction("CameraMoveBack"));
                Instance.GetInputActionByName("CameraMoveBack").AddInputEvent(KeyboardKey.S);
                InputMap.Add(new InputAction("CameraMoveUp"));
                Instance.GetInputActionByName("CameraMoveUp").AddInputEvent(KeyboardKey.E);
                InputMap.Add(new InputAction("CameraMoveDown"));
                Instance.GetInputActionByName("CameraMoveDown").AddInputEvent(KeyboardKey.Q);
                InputMap.Add(new InputAction("CameraBoost"));
                Instance.GetInputActionByName("CameraBoost").AddInputEvent(KeyboardKey.LeftShift);
                Instance.GetInputActionByName("CameraBoost").AddInputEvent(MouseButton.Left);
                InputMap.Add(new InputAction("CameraActivate"));
                Instance.GetInputActionByName("CameraActivate").AddInputEvent(MouseButton.Right);

                // Tab switching
                InputMap.Add(new InputAction("LevelSwitch"));
                Instance.GetInputActionByName("LevelSwitch").AddInputEvent(KeyboardKey.L);
                InputMap.Add(new InputAction("ModelsSwitch"));
                Instance.GetInputActionByName("ModelsSwitch").AddInputEvent(KeyboardKey.P);
                InputMap.Add(new InputAction("LogicSwitch"));
                Instance.GetInputActionByName("LogicSwitch").AddInputEvent(KeyboardKey.N);

                // Other
                InputMap.Add(new InputAction("Click"));
                Instance.GetInputActionByName("Click").AddInputEvent(MouseButton.Left);
                InputMap.Add(new InputAction("Save"));
                Instance.GetInputActionByName("Save").AddInputEvent(KeyboardKey.LeftControl, KeyboardKey.S);
                InputMap.Add(new InputAction("Exit"));
                Instance.GetInputActionByName("Exit").AddInputEvent(KeyboardKey.Escape);

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
