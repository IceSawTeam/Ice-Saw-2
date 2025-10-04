using Newtonsoft.Json;
using System.Diagnostics;


namespace IceSaw2.Settings
{
    public class General
    {
        private General() { }
        private static readonly General _instance = new();
        public static General Instance { get { return _instance; } }

        public DataClass data = new();
        public class DataClass
        {
            public  string version = "1";
            public  int ScreenWidth = 1280;
            public  int ScreenHeight = 720;
            public  string LastLoad = "";
            public  int PatchResolution = 7;
        }


        public void Load()
        {
            string loadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string loadPath = Path.Combine(loadFolder, "Settings.json");
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
            const bool PRETTY = true;
            string saveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            string savePath = Path.Combine(saveFolder, "Settings.json");
            var tempFormating = PRETTY ? Formatting.Indented : Formatting.None;
            var serializer = JsonConvert.SerializeObject(data, tempFormating);
            File.WriteAllText(savePath, serializer);
        }
    }
}
