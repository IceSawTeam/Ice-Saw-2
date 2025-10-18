﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Raylib_cs;
using System.Diagnostics;


namespace IceSaw2.Settings
{
    public class General
    {
        private General() { }
        private static readonly General _instance = new();
        public static General Instance { get { return _instance; } }

        public const string GeneralSettingsVersion = "2";

        public DataClass data = new();
        public class DataClass
        {
            public string version = GeneralSettingsVersion;
            public float windowPositionX = 100;
            public float windowPositionY = 100;
            public float windowWidth = 1280;
            public float windowHeight = 720;
            public bool isMaximized = true;
            public string LastLoad = "";
            public int PatchResolution = 8;
            public bool ConsoleWindow = false;
        }


        public void Load()
        {
            string loadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string loadPath = Path.Combine(loadFolder, "GeneralSettings.json");
            if (File.Exists(loadPath))
            {
                string fileText = File.ReadAllText(loadPath);
                var parsedJson = JObject.Parse(fileText);
                if (parsedJson.TryGetValue("version", StringComparison.Ordinal, out JToken? ver))
                {
                    if (ver.Type == JTokenType.String && ver.ToString() == GeneralSettingsVersion)
                    {
                        DataClass? loadedData = JsonConvert.DeserializeObject<DataClass>(fileText);
                        Debug.Assert(loadedData != null, "DeserializeObject is null");
                        data = loadedData;
                        return;
                    }
                }
            }

            // Save if file doesn't exist, or if it has a different version.
            Console.WriteLine("Disk General settings were incompatible. Overritten with newer version.");
            Instance.Save();
            return;
        }


        public void Save()
        {
            data.windowPositionX = Raylib.GetWindowPosition().X;
            data.windowPositionY = Raylib.GetWindowPosition().Y;
            data.windowWidth = Raylib.GetScreenWidth();
            data.windowHeight = Raylib.GetScreenHeight();
            data.isMaximized = Raylib.IsWindowMaximized();

            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string savePath = Path.Combine(saveFolder, "GeneralSettings.json");
            var serializer = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(savePath, serializer);
        }
    }
}
