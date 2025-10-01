using Newtonsoft.Json;
using SSXMultiTool.JsonFiles.Tricky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.Settings
{
    public class GeneralSettings
    {
        public int ScreenWidth = 1280;
        public int ScreenHeight = 720;

        public string LastLoad = "";

        public int PatchResolution = 7;

        public void CreateJson(string path, bool Inline = false)
        {
            var TempFormating = Formatting.None;
            if (Inline)
            {
                TempFormating = Formatting.Indented;
            }

            var serializer = JsonConvert.SerializeObject(this, TempFormating);
            File.WriteAllText(path, serializer);
        }

        public static GeneralSettings Load(string path)
        {
            string paths = path;
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<GeneralSettings>(stream);
                return container;
            }
            else
            {
                return new GeneralSettings();
            }
        }
    }
}
