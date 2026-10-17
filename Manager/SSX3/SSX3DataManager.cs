
using IceSaw2.LevelObject;
using IceSaw2.LevelObject.SSX3;
using IceSaw2.RayWarp;
using Raylib_cs;
using SSXLibrary.JsonFiles.SSX3;
using static IceSaw2.Manager.DataManagerBase;
using static IceSaw2.Manager.SSX3.SSX3DataManager;

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
                levelDatas.Add(levelData);
                levelDatas[i].LoadLevel(Path.Combine(LoadPath, "Levels", config.LevelInfoList[i].LevelName));
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

        public static MeshRef ReturnMesh(string FileName, int ID)
        {
            return levelDatas[ID].ReturnMesh(FileName);
        }

        public static MaterialRef ReturnMaterial(int RID, int TrackID)
        {
            return levelDatas[TrackID].ReturnMaterial(RID);
        }

        public class LevelData
        {
            public int LevelID;
            public string LevelName;
            public List<SSX3PatchObject> ssx3PatchObjects;
            public List<SSX3MaterialObject> ssx3MaterialObjects;
            public List<SSX3ModelObject> ssx3ModelObjects;
            public List<SSX3InstanceObject> ssx3Instances;

            public List<MeshData> worldMeshes = new List<MeshData>();


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

                //Meshes
                worldMeshes = new List<MeshData>();
                string[] MeshFiles = Directory.GetFiles(Path.Combine(LevelDirectory, "Models"), "*.obj", SearchOption.AllDirectories);
                for (int i = 0; i < MeshFiles.Length; i++)
                {
                    var TempMesh = new MeshData();

                    TempMesh.Name = Path.GetFileName(MeshFiles[i]);
                    TempMesh.mesh = new MeshRef(ObjImporter.ObjLoad(MeshFiles[i]));

                    Raylib.UploadMesh(ref TempMesh.mesh.Mesh, false);

                    worldMeshes.Add(TempMesh);
                }

                //Models
                ssx3ModelObjects = new List<SSX3ModelObject>();
                MDRJsonHandler prefabJsonHandler = MDRJsonHandler.Load(Path.Combine(LevelDirectory, "Prefabs.json"));
                for (int i = 0; i < prefabJsonHandler.mainModelHeaders.Count; i++)
                {
                    var NewPrefab = new SSX3ModelObject();

                    NewPrefab.LoadModel(prefabJsonHandler.mainModelHeaders[i]);

                    ssx3ModelObjects.Add(NewPrefab);
                }

                //Instances
                ssx3Instances = new List<SSX3InstanceObject>();
                InstanceJsonHandler instanceJsonHandler = InstanceJsonHandler.Load(Path.Combine(LevelDirectory, "Instances.json"));
                for (int i = 0; i < instanceJsonHandler.Instances.Count; i++)
                {
                    var Instance = new SSX3InstanceObject();

                    Instance.LoadBin3(instanceJsonHandler.Instances[i]);

                    ssx3Instances.Add(Instance);
                }

                //Splines

                //ETC


            }

            public MeshRef ReturnMesh(string FileName)
            {
                for (int i = 0; i < worldMeshes.Count; i++)
                {
                    if (worldMeshes[i].Name == FileName)
                    {
                        return worldMeshes[i].mesh;
                    }
                }
                return worldMeshes[0].mesh;
            }

            public MaterialRef ReturnMaterial(int ID)
            {
                for (int i = 0; i < ssx3MaterialObjects.Count; i++)
                {
                    if (ssx3MaterialObjects[i].RID == ID)
                    {
                        return ssx3MaterialObjects[i].materialRef;
                    }
                }
                return ssx3MaterialObjects[0].materialRef;
            }
        }
    }
}
