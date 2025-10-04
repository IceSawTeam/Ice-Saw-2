/*
    Manages and centralizes all settings classes.

    This is a singleton. It's Instance can be accessed anywhere.
*/

namespace IceSaw2.Settings
{
    public class Manager
    {
        private Manager() { }
        private static readonly Manager _instance = new();
        public static Manager Instance {get{return _instance;}}

        public GeneralSettings General = new();
        public HotkeySettings Hotkey = new();


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
            Instance.General.CreateJson(Path.Combine(SaveFolder, "Settings.json"));
            Instance.Hotkey.CreateJson(Path.Combine(SaveFolder, "Hotkeys.json"));
        }
    }
}