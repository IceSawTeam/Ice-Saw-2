
namespace IceSaw2.Settings
{
    public class Manager
    {
        private static Manager? _instance;
        public static Manager Instance
        {
            get
            {
                return _instance ?? throw new InvalidOperationException("Settings manager not initialized");
            }
        }
        private Manager() { }
        public static void Init()
        {
            if (_instance != null)
                throw new InvalidOperationException("Settings manager already initialized");
            _instance = new Manager();

            Instance.General = new();
            Instance.Hotkey = new();

        }

        public GeneralSettings? General;
        public HotkeySettings? Hotkey;


        public static void LoadSettings()
        {
            string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }

            Instance.General = GeneralSettings.Load(Path.Combine(SaveFolder, "Settings.json"));
            Instance.Hotkey = HotkeySettings.Load(Path.Combine(SaveFolder, "Hotkeys.json"));

            //Incase of version Change save back to latest version
            SaveSettings();
        }


        public static void SaveSettings()
        {
            string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            Instance.General?.CreateJson(Path.Combine(SaveFolder, "Settings.json"));
            Instance.Hotkey?.CreateJson(Path.Combine(SaveFolder, "Hotkeys.json"));
        }
    }
}