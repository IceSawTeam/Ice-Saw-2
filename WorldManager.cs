using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2
{
    public class WorldManager
    {
        public static WorldManager instance = new WorldManager();

        Camera3D camera3D = new Camera3D();
        string LoadPath;

        //Object Data
        List<PatchObject> patchObjects = new List<PatchObject>();

        //Texture Data
        List<TextureData> worldTextureData = new List<TextureData>();
        List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        public void Initalise()
        {
            instance = this;

            Raylib.InitWindow(800, 480, "Ice Saw 2");

            camera3D.Position = new System.Numerics.Vector3(0, 100, 100);
            camera3D.Target = Vector3.Zero;
            camera3D.Up = new Vector3(0, 0, 1);
            camera3D.FovY = 45f;
            camera3D.Projection = CameraProjection.Perspective;

            //Test Load
            LoadProject("G:\\SSX Modding\\disk\\SSX Tricky\\DATA\\MODELS\\Gari\\ConfigTricky.ssx");

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

                worldTextureData.Add(textureData);
            }

            lightmapTexture2Ds = new List<TextureData>();

            string[] LightmapFiles = Directory.GetFiles(LoadPath + "\\Lightmaps", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < LightmapFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(LightmapFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(LightmapFiles[i]);

                lightmapTexture2Ds.Add(textureData);
            }

            skyboxTexture2Ds = new List<TextureData>();

            string[] SkyboxFiles = Directory.GetFiles(LoadPath + "\\Skybox\\Textures", "*.png", SearchOption.AllDirectories);

            for (int i = 0; i < SkyboxFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(SkyboxFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(SkyboxFiles[i]);

                skyboxTexture2Ds.Add(textureData);
            }

            patchObjects = new List<PatchObject>();

            PatchesJsonHandler jsonHandler = PatchesJsonHandler.Load(LoadPath + "\\patches.json");

            for (int i = 0; i < jsonHandler.Patches.Count; i++)
            {
                PatchObject patchObject = new PatchObject();

                patchObject.LoadPatch(jsonHandler.Patches[i]);

                patchObjects.Add(patchObject);
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

        public void UpdateLogic()
        {
            //Update Camera
            Raylib.UpdateCamera(ref camera3D, CameraMode.Free);

            //Object Collision
        }

        public void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Gray);

            //Render 3D
            Raylib.BeginMode3D(camera3D);

            //Render Skybox

            //Render Default
            Raylib.DrawGrid(100, 1);

            //Render Objects

            for (int i = 0; i < patchObjects.Count; i++)
            {
                patchObjects[i].Render();
            }

            //Raylib.DrawModelEx(model, Vector3.Zero, Vector3.UnitX, 0, Vector3.One*0.01f, Color.White);
            //Raylib.DrawModelEx(model1, Vector3.Zero, Vector3.UnitX, 0, Vector3.One * 0.01f, Color.White);

            //Render Wires

            Raylib.EndMode3D();

            //Render UI

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

    }
}
