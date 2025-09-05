using IceSaw2.LevelObject.Materials;
using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Numerics;

namespace IceSaw2
{
    public class WorldManager
    {
        public static WorldManager instance = new WorldManager();

        public WindowMode windowMode = WindowMode.World;

        Camera3D worldCamera3D = new Camera3D();
        Camera3D materialCamera3D = new Camera3D();
        public string LoadPath;

        //Skybox Data

        //Object Data
        List<TrickyPatchObject> trickyPatchObjects = new List<TrickyPatchObject>();
        List<TrickyPrefabObject> trickyPrefabObjects = new List<TrickyPrefabObject>();

        //Texture Data
        List<TextureData> worldTextureData = new List<TextureData>();
        List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        public List<TrickyMaterialObject> trickyMaterialObject = new List<TrickyMaterialObject>();

        int MaterialSelection = 0;
        int PrefabSelection = 0;

        public void Initalise()
        {
            instance = this;

            Raylib.InitWindow(1080, 608, "Ice Saw 2");

            Rlgl.DisableBackfaceCulling();

            worldCamera3D.Position = new System.Numerics.Vector3(0, 100, 100);
            worldCamera3D.Target = Vector3.Zero;
            worldCamera3D.Up = new Vector3(0, 0, 1);
            worldCamera3D.FovY = 45f;
            worldCamera3D.Projection = CameraProjection.Perspective;

            materialCamera3D.Position = new System.Numerics.Vector3(0, 10, 3);
            materialCamera3D.Target = Vector3.Zero;
            materialCamera3D.Up = new Vector3(0, 0, 1);
            materialCamera3D.FovY = 45f;
            materialCamera3D.Projection = CameraProjection.Perspective;

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
                //Update Logic Loop

                UpdateLogic();

                //Render Loop

                Render();
            }
        }

        static FilePicker filePicker = new FilePicker();

        public void UpdateLogic()
        {
            if (windowMode == WindowMode.World)
            {
                //Update Camera
                Raylib.UpdateCamera(ref worldCamera3D, CameraMode.Free);

                string picked = filePicker.GetSelectedFile();
                if (picked != null)
                {
                    Console.WriteLine("Picked: " + picked);
                    // You can now do something with the file
                    LoadProject(picked);
                }

                //Object Collision
                if (Raylib.IsKeyPressed(KeyboardKey.F))
                {
                    filePicker.Open();
                }

                filePicker.Update();

            }
            if (windowMode == WindowMode.Materials)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    MaterialSelection -= 1;
                    if(MaterialSelection==-1)
                    {
                        MaterialSelection = trickyMaterialObject.Count-1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    MaterialSelection += 1;
                    if (MaterialSelection == trickyMaterialObject.Count)
                    {
                        MaterialSelection = 0;
                    }
                }

                Raylib.UpdateCamera(ref materialCamera3D, CameraMode.Orbital);
            }
            if (windowMode == WindowMode.Prefabs)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    PrefabSelection -= 1;
                    if (PrefabSelection == -1)
                    {
                        PrefabSelection = trickyPrefabObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    PrefabSelection += 1;
                    if (MaterialSelection == trickyPrefabObjects.Count)
                    {
                        PrefabSelection = 0;
                    }
                }

                Raylib.UpdateCamera(ref materialCamera3D, CameraMode.Orbital);
            }

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
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.White);

            if (windowMode == WindowMode.World)
            {
                //Render 3D
                Raylib.BeginMode3D(worldCamera3D);

                //Render Skybox

                //Render Default
                Raylib.DrawGrid(100, 1);

                //Render Objects

                for (int i = 0; i < trickyPatchObjects.Count; i++)
                {
                    trickyPatchObjects[i].Render();
                }

                //Render Wires

                Raylib.EndMode3D();

                //Render UI
                filePicker.Draw();

            }
            if(windowMode == WindowMode.Materials)
            {
                if (trickyMaterialObject.Count != 0)
                {
                    Raylib.DrawText(trickyMaterialObject[MaterialSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(materialCamera3D);

                Raylib.DrawGrid(10, 1);

                if (trickyMaterialObject.Count != 0)
                {
                    trickyMaterialObject[MaterialSelection].Render();
                }

                Raylib.EndMode3D();
            }

            if (windowMode == WindowMode.Prefabs)
            {
                if (trickyPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(trickyPrefabObjects[PrefabSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(materialCamera3D);

                Raylib.DrawGrid(10, 1);

                if (trickyPrefabObjects.Count != 0)
                {
                    trickyPrefabObjects[PrefabSelection].Render();
                }

                Raylib.EndMode3D();
            }

            //Raylib.DrawTexture(skyboxTexture2Ds[0], 100, 100, Color.White);

            Raylib.DrawText("Beta Test", 12, 12, 20, Color.Black);

            Raylib.EndDrawing();
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

        struct TextureData
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
