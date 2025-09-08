using IceSaw2.EditorWindows;
using IceSaw2.LevelObject;
using IceSaw2.LevelObject.Materials;
using IceSaw2.LevelObject.TrickyObjects;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Numerics;

namespace IceSaw2
{
    public class WorldManager
    {
        public static WorldManager instance = new WorldManager();

        public LevelEditorWindow levelEditorWindow = new LevelEditorWindow();
        public PrefabEditorWindow prefabEditorWindow = new PrefabEditorWindow();
        public MaterialEditorWindow materialEditorWindow = new MaterialEditorWindow();

        public int heightScreen = 1080;
        public int widthScreen = 608;

        public WindowMode windowMode = WindowMode.World;

        public string LoadPath;

        //Skybox Data

        //Object Data
        public List<TrickyPatchObject> trickyPatchObjects = new List<TrickyPatchObject>();
        public List<TrickyPrefabObject> trickyPrefabObjects = new List<TrickyPrefabObject>();
        public List<TrickyInstanceObject> trickyInstanceObjects = new List<TrickyInstanceObject>();

        //Texture Data
        public List<TextureData> worldTextureData = new List<TextureData>();
        public List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        public List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        public List<TrickyMaterialObject> trickyMaterialObject = new List<TrickyMaterialObject>();

        public void Initalise()
        {
            instance = this;

            levelEditorWindow.Initilize();
            prefabEditorWindow.Initilize();
            materialEditorWindow.Initilize();

            Raylib.InitWindow(heightScreen, widthScreen, "Ice Saw 2");
            rlImGui.Setup(true);

            Rlgl.DisableBackfaceCulling();

            //Test Load
            //LoadProject("G:\\SSX Modding\\disk\\SSX Tricky\\DATA\\MODELS\\Gari\\ConfigTricky.ssx");

            Update();

            Raylib.CloseWindow();
        }

        public void LoadProject(string ConfigPath)
        {
            LoadPath = Path.GetDirectoryName(ConfigPath);

            worldTextureData = new List<TextureData>();

            string[] TextureFiles = Directory.GetFiles(LoadPath + "\\Textures", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < TextureFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(TextureFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(TextureFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                worldTextureData.Add(textureData);
            }

            lightmapTexture2Ds = new List<TextureData>();

            string[] LightmapFiles = Directory.GetFiles(LoadPath + "\\Lightmaps", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < LightmapFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(LightmapFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(LightmapFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                lightmapTexture2Ds.Add(textureData);
            }

            skyboxTexture2Ds = new List<TextureData>();

            string[] SkyboxFiles = Directory.GetFiles(LoadPath + "\\Skybox\\Textures", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < SkyboxFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(SkyboxFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(SkyboxFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                skyboxTexture2Ds.Add(textureData);
            }

            trickyPatchObjects = new List<TrickyPatchObject>();

            PatchesJsonHandler jsonHandler = PatchesJsonHandler.Load(LoadPath + "\\patches.json");

            for (int i = 0; i < jsonHandler.Patches.Count; i++)
            {
                TrickyPatchObject patchObject = new TrickyPatchObject();

                patchObject.LoadPatch(jsonHandler.Patches[i]);

                trickyPatchObjects.Add(patchObject);
            }

            trickyMaterialObject = new List<TrickyMaterialObject>();

            MaterialJsonHandler matJsonHandler = MaterialJsonHandler.Load(LoadPath + "\\materials.json");

            for (int i = 0; i < matJsonHandler.Materials.Count; i++)
            {
                TrickyMaterialObject materialObject = new TrickyMaterialObject();

                materialObject.LoadMaterial(matJsonHandler.Materials[i]);

                trickyMaterialObject.Add(materialObject);
            }

            trickyPrefabObjects = new List<TrickyPrefabObject>();
            //
            PrefabJsonHandler prefabJsonHandler = PrefabJsonHandler.Load(LoadPath + "\\prefabs.json");

            for (int i = 0; i < prefabJsonHandler.Prefabs.Count; i++)
            {
                var NewPrefab = new TrickyPrefabObject();

                NewPrefab.LoadPrefab(prefabJsonHandler.Prefabs[i]);

                trickyPrefabObjects.Add(NewPrefab);
            }

            trickyInstanceObjects = new List<TrickyInstanceObject>();

            InstanceJsonHandler instanceJsonHandler = InstanceJsonHandler.Load(LoadPath + "\\instances.json");

            for (int i = 0; i < instanceJsonHandler.Instances.Count; i++)
            {
                var Instance = new TrickyInstanceObject();

                Instance.LoadInstance(instanceJsonHandler.Instances[i]);

                trickyInstanceObjects.Add(Instance);
            }

            //Mesh Test = ObjImporter.ObjLoad("G:\\SSX Modding\\disk\\SSX Tricky\\DATA\\MODELS\\Gari\\Models\\0.obj");

            //Raylib.UploadMesh(ref Test, false);
            //model = Raylib.LoadModelFromMesh(Test);

            //var Texture = worldTextureData[0].texture2D;

            //Raylib.SetMaterialTexture(ref model, 0, MaterialMapIndex.Diffuse, ref Texture);

            //Mesh Test1 = ObjImporter.ObjLoad("G:\\SSX Modding\\disk\\SSX Tricky\\DATA\\MODELS\\Gari\\Models\\1.obj");

            //Raylib.UploadMesh(ref Test1, false);
            //model1 = Raylib.LoadModelFromMesh(Test1);

            //var Texture1 = worldTextureData[1].texture2D;

            //Raylib.SetMaterialTexture(ref model1, 0, MaterialMapIndex.Diffuse, ref Texture1);
        }


        public void Update()
        {
            while (!Raylib.WindowShouldClose())
            {
                //Logic Update

                if (windowMode == WindowMode.World)
                {
                    levelEditorWindow.LogicUpdate();
                }
                if(windowMode == WindowMode.Prefabs)
                {
                    prefabEditorWindow.LogicUpdate();
                }
                if (windowMode == WindowMode.Materials)
                {
                    materialEditorWindow.LogicUpdate();
                }

                UpdateLogic();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);

                if (windowMode == WindowMode.World)
                {
                    levelEditorWindow.RenderUpdate();
                }
                if (windowMode == WindowMode.Prefabs)
                {
                    prefabEditorWindow.RenderUpdate();
                }
                if (windowMode == WindowMode.Materials)
                {
                    materialEditorWindow.RenderUpdate();
                }

                Render();

                Raylib.DrawText("Beta Test", 12, 12, 20, Color.Black);
                Raylib.EndDrawing();
            }
        }

        public void UpdateLogic()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.M))
            {
                windowMode = WindowMode.Materials;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.P))
            {
                windowMode = WindowMode.Prefabs;
            }
            if (Raylib.IsKeyPressed(KeyboardKey.L))
            {
                windowMode = WindowMode.World;
            }

        }

        public void Render()
        {

        }

        public Texture2D ReturnTexture(string FileName)
        {
            for (int i = 0; i < worldTextureData.Count; i++)
            {
                if (worldTextureData[i].Name==FileName)
                {
                    return worldTextureData[i].texture2D;
                }
            }
            return worldTextureData[0].texture2D;
        }

        public struct TextureData
        {
            public string Name;
            public Texture2D texture2D;
        }


        public enum WindowMode
        {
            World,
            Materials,
            Prefabs
        }

    }
}
