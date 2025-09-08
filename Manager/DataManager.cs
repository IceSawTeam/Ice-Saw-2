using IceSaw2.LevelObject;
using IceSaw2.LevelObject.Materials;
using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.Manager.WorldManager;

namespace IceSaw2.Manager
{
    public static class DataManager
    {
        //Skybox Data
        public static List<TrickyMaterialObject> trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
        public static List<TrickyPrefabObject> trickySkyboxPrefabObjects = new List<TrickyPrefabObject>();

        //Object Data
        public static List<TrickyPatchObject> trickyPatchObjects = new List<TrickyPatchObject>();
        public static List<TrickyMaterialObject> trickyMaterialObject = new List<TrickyMaterialObject>();
        public static List<TrickyPrefabObject> trickyPrefabObjects = new List<TrickyPrefabObject>();
        public static List<TrickyInstanceObject> trickyInstanceObjects = new List<TrickyInstanceObject>();

        //Texture Data
        public static List<TextureData> worldTextureData = new List<TextureData>();
        public static List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        public static List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        public static string LoadPath = "";

        public static void LoadProject(string ConfigPath)
        {
            LoadPath = Path.GetDirectoryName(ConfigPath);

            //Texture Data
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

            //Object Data
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

                materialObject.LoadMaterial(matJsonHandler.Materials[i], false);

                trickyMaterialObject.Add(materialObject);
            }

            trickyPrefabObjects = new List<TrickyPrefabObject>();
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
        }

        public static Texture2D ReturnTexture(string FileName, bool Skybox)
        {
            if (!Skybox)
            {
                for (int i = 0; i < worldTextureData.Count; i++)
                {
                    if (worldTextureData[i].Name == FileName)
                    {
                        return worldTextureData[i].texture2D;
                    }
                }
                return worldTextureData[0].texture2D;
            }
            else
            {
                for (int i = 0; i < skyboxTexture2Ds.Count; i++)
                {
                    if (skyboxTexture2Ds[i].Name == FileName)
                    {
                        return skyboxTexture2Ds[i].texture2D;
                    }
                }
                return skyboxTexture2Ds[0].texture2D;
            }
        }

        public struct TextureData
        {
            public string Name;
            public Texture2D texture2D;
        }
    }
}
