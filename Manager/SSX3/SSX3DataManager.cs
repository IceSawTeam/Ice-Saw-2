
using Raylib_cs;
using SSXLibrary.JsonFiles.SSX3;
using IceSaw2.LevelObject.SSX3;
using static IceSaw2.Manager.DataManagerBase;

namespace IceSaw2.Manager.SSX3
{
    public class SSX3DataManager
    {
        //Texture Data
        public static List<TextureData> worldTextureData = new List<TextureData>();
        public static List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        public static List<LevelData> levelDatas = new List<LevelData>();

        public static string LoadPath = "";
        public static string SavePath = "";

        public static void LoadProject(string ConfigPath)
        {
            UnloadProject();

            levelDatas = new List<LevelData>();

            LoadPath = Path.GetDirectoryName(ConfigPath);
            LevelJsonHandler config = LevelJsonHandler.Load(Path.Combine(LoadPath, "levels.json"));

            IceSaw2.Manager.SelectionManager.Clear();

            worldTextureData = LoadImages(Path.Combine(LoadPath, "Textures"));

            lightmapTexture2Ds = LoadImages(Path.Combine(LoadPath, "lightmaps"));

            for (int i = 0; i < config.LevelInfoList.Count; i++)
            {
                LevelData levelData = new LevelData();
                levelData.LevelName = config.LevelInfoList[i].LevelName;
                levelData.LevelID = i;
                levelData.LoadLevel(Path.Combine(LoadPath, "Levels", config.LevelInfoList[i].LevelName));
                levelDatas.Add(levelData);
            }

            List<Image>? LightmapArray = new();

            for (int i = 0; i < lightmapTexture2Ds.Count; i++)
            {
                LightmapArray.Add(Raylib.LoadImageFromTexture(lightmapTexture2Ds[i].texture2D));
            }

            Renderer.TessellatedPatch.Instance.Init(LightmapArray, true);
        }

        public static void UnloadProject()
        {
            for (int i = 0; i < worldTextureData.Count; i++)
            {
                Raylib.UnloadTexture(worldTextureData[i].texture2D);
            }

            for (int i = 0; i < lightmapTexture2Ds.Count; i++)
            {
                Raylib.UnloadTexture(lightmapTexture2Ds[i].texture2D);
            }
        }

        public static string[] GenerateLevelList()
        {
            string[] List = new string[levelDatas.Count];
            for (int i = 0; i < List.Length; i++)
            {
                List[i] = levelDatas[i].LevelName;
            }

            return List;
        }

        public static Texture2D ReturnTexture(string FileName)
        {
            for (int i = 0; i < worldTextureData.Count; i++)
            {
                if (worldTextureData[i].Name == FileName)
                {
                    return worldTextureData[i].texture2D;
                }
            }
            return SSX3WorldManager.instance.ErrorTexture;
        }

        public class LevelData
        {
            public int LevelID;
            public string LevelName;
            public List<SSX3PatchObject> ssx3PatchObjects;
            public List<SSX3MaterialObject> ssx3MaterialObjects;


            public void LoadLevel(string LevelDirectory)
            {
                //Patches
                ssx3PatchObjects = new List<SSX3PatchObject>();
                PatchesJsonHandler jsonHandler = PatchesJsonHandler.Load(Path.Combine(LevelDirectory, "Patches.json"));
                for (int i = 0; i < jsonHandler.Patches.Count; i++)
                {
                    SSX3PatchObject patchObject = new SSX3PatchObject(LevelID);

                    patchObject.LoadPatch(jsonHandler.Patches[i]);

                    ssx3PatchObjects.Add(patchObject);
                }

                //Materials
                ssx3MaterialObjects = new List<SSX3MaterialObject>();
                MaterialJsonHandler materialsJsonHandler = MaterialJsonHandler.Load(Path.Combine(LevelDirectory, "Materials.json"));
                for (int i = 0; i < materialsJsonHandler.Materials.Count; i++)
                {
                    SSX3MaterialObject ssx3MaterialObject = new SSX3MaterialObject();

                    ssx3MaterialObject.LoadMaterial(materialsJsonHandler.Materials[i]);

                    ssx3MaterialObjects.Add(ssx3MaterialObject);
                }

                //Models
                //Instances
                //Splines

                //ETC


            }
        }
    }
}
