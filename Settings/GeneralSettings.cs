using Newtonsoft.Json;
using System.Diagnostics;


namespace IceSaw2.Settings
{
    public class GeneralSettings
    {
        public int ScreenWidth = 1280;
        public int ScreenHeight = 720;
        public string LastLoad = "";
        public int PatchResolution = 7;

        public void CreateJson(string path, bool pretty = false)
        {
            var TempFormating = pretty? Formatting.Indented : Formatting.None;
            var serializer = JsonConvert.SerializeObject(this, TempFormating);
            File.WriteAllText(path, serializer);
        }

        public static GeneralSettings Load(string path)
        {
            string loadPath = path;
            if (File.Exists(loadPath))
            {
                string stream = File.ReadAllText(loadPath);
                GeneralSettings? container = JsonConvert.DeserializeObject<GeneralSettings>(stream);
                Debug.Assert(container != null, "Container is null");
                return container;
            }
            return new GeneralSettings();
        }
    }
}
