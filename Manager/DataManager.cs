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
        public static List<TrickySplineObject> trickySplineObjects = new List<TrickySplineObject>();
        public static List<TrickyParticlePrefabObject> trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
        public static List<TrickyPaticleInstanceObject> trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();
        public static List<TrickyCameraObject> trickyCameraObjects = new List<TrickyCameraObject>();
        public static List<TrickyLightObject> trickyLightObjects = new List<TrickyLightObject>();

        //Pathing Data
        //AIP RaceLine
        //AIP AI
        //SOP Raceline
        //SOP AI

        //Effect Data
        public static List<TrickyEffectSlotObject> trickyEffectSlotObjects = new List<TrickyEffectSlotObject>();
        //Physics
        public static List<TrickyFunctionHeader> trickyFunctionHeaders = new List<TrickyFunctionHeader>();
        public static List<TrickyEffectHeader> trickyEffectHeaders = new List<TrickyEffectHeader>();

        //Texture Data
        public static List<TextureData> worldTextureData = new List<TextureData>();
        public static List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        public static List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        //Mesh Data
        public static List<MeshData> worldMeshes = new List<MeshData>();
        public static List<MeshData> skyboxMeshes = new List<MeshData>();

        public static string LoadPath = "";

        public static void LoadProject(string ConfigPath)
        {
            UnloadProject();

            LoadPath = Path.GetDirectoryName(ConfigPath);

            LoadTextureMesh();

            LoadLevelObjects();

            //Skybox
            trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
            MaterialJsonHandler matskyJsonHandler = MaterialJsonHandler.Load(LoadPath + "\\Skybox\\materials.json");
            for (int i = 0; i < matskyJsonHandler.Materials.Count; i++)
            {
                TrickyMaterialObject materialObject = new TrickyMaterialObject();

                materialObject.LoadMaterial(matskyJsonHandler.Materials[i], false);

                trickySkyboxMaterialObject.Add(materialObject);
            }

            trickySkyboxPrefabObjects = new List<TrickyPrefabObject>();
            PrefabJsonHandler prefabSkyboxJsonHandler = PrefabJsonHandler.Load(LoadPath + "\\Skybox\\prefabs.json");
            for (int i = 0; i < prefabSkyboxJsonHandler.Prefabs.Count; i++)
            {
                var NewPrefab = new TrickyPrefabObject();

                NewPrefab.LoadPrefab(prefabSkyboxJsonHandler.Prefabs[i], true);

                trickySkyboxPrefabObjects.Add(NewPrefab);
            }

            //Effects
            SSFJsonHandler sSFJsonHandler = SSFJsonHandler.Load(LoadPath + "\\SSFLogic.json");

            trickyEffectHeaders = new List<TrickyEffectHeader>();
            for (int i = 0; i < sSFJsonHandler.EffectHeaders.Count; i++)
            {
                var NewEffect = new TrickyEffectHeader();

                NewEffect.LoadEffectList(sSFJsonHandler.EffectHeaders[i]);

                trickyEffectHeaders.Add(NewEffect);
            }

            trickyFunctionHeaders = new List<TrickyFunctionHeader>();
            for (int i = 0; i < sSFJsonHandler.Functions.Count; i++)
            {
                var NewEffect = new TrickyFunctionHeader();

                NewEffect.LoadFunction(sSFJsonHandler.Functions[i]);

                trickyFunctionHeaders.Add(NewEffect);
            }

            trickyEffectSlotObjects = new List<TrickyEffectSlotObject>();
            for (int i = 0; i < sSFJsonHandler.EffectSlots.Count; i++)
            {
                var NewEffectSlot = new TrickyEffectSlotObject();

                NewEffectSlot.LoadEffectSlot(sSFJsonHandler.EffectSlots[i]);

                trickyEffectSlotObjects.Add(NewEffectSlot);
            }
        }

        public static void LoadTextureMesh()
        {
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

            worldMeshes = new List<MeshData>();
            string[] MeshFiles = Directory.GetFiles(LoadPath + "\\Models\\", "*.obj", SearchOption.AllDirectories);
            for (int i = 0; i < MeshFiles.Length; i++)
            {
                var TempMesh = new MeshData();

                TempMesh.Name = Path.GetFileName(MeshFiles[i]);
                TempMesh.mesh = ObjImporter.ObjLoad(MeshFiles[i]);

                Raylib.UploadMesh(ref TempMesh.mesh, false);

                worldMeshes.Add(TempMesh);
            }

            skyboxMeshes = new List<MeshData>();
            string[] SkyboxMeshFiles = Directory.GetFiles(LoadPath + "\\Skybox\\Models\\", "*.obj", SearchOption.AllDirectories);
            for (int i = 0; i < SkyboxMeshFiles.Length; i++)
            {
                var TempMesh = new MeshData();

                TempMesh.Name = Path.GetFileName(SkyboxMeshFiles[i]);
                TempMesh.mesh = ObjImporter.ObjLoad(SkyboxMeshFiles[i]);

                Raylib.UploadMesh(ref TempMesh.mesh, false);

                skyboxMeshes.Add(TempMesh);
            }
        }

        public static void LoadLevelObjects()
        {
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

            trickySplineObjects = new List<TrickySplineObject>();
            SplineJsonHandler splineJsonHandler = SplineJsonHandler.Load(LoadPath + "\\splines.json");
            for (int i = 0; i < splineJsonHandler.Splines.Count; i++)
            {
                var Spline = new TrickySplineObject();

                Spline.LoadSpline(splineJsonHandler.Splines[i]);

                trickySplineObjects.Add(Spline);
            }

            trickyLightObjects = new List<TrickyLightObject>();
            LightJsonHandler lightJsonHandler = LightJsonHandler.Load(LoadPath + "\\Lights.json");
            for (int i = 0; i < lightJsonHandler.Lights.Count; i++)
            {
                var Light = new TrickyLightObject();

                Light.LoadLight(lightJsonHandler.Lights[i]);

                trickyLightObjects.Add(Light);
            }

            trickyCameraObjects = new List<TrickyCameraObject>();
            CameraJSONHandler cameraJSONHandler = CameraJSONHandler.Load(LoadPath + "\\Cameras.json");
            for (int i = 0; i < cameraJSONHandler.Cameras.Count; i++)
            {
                var Camera = new TrickyCameraObject();

                Camera.LoadCamera(cameraJSONHandler.Cameras[i]);

                trickyCameraObjects.Add(Camera);
            }

            trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
            ParticleModelJsonHandler particleModelJsonHandler = ParticleModelJsonHandler.Load(LoadPath + "\\ParticlePrefabs.json");
            for (int i = 0; i < particleModelJsonHandler.ParticlePrefabs.Count; i++)
            {
                var ParticlePrefab = new TrickyParticlePrefabObject();

                ParticlePrefab.LoadParticle(particleModelJsonHandler.ParticlePrefabs[i]);

                trickyParticlePrefabObjects.Add(ParticlePrefab);
            }

            trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();
            ParticleInstanceJsonHandler particleInstanceJsonHandler = ParticleInstanceJsonHandler.Load(LoadPath + "\\ParticleInstances.json");
            for (int i = 0; i < particleInstanceJsonHandler.Particles.Count; i++)
            {
                var ParticleInstance = new TrickyPaticleInstanceObject();

                ParticleInstance.LoadPaticleInstance(particleInstanceJsonHandler.Particles[i]);

                trickyPaticleInstanceObjects.Add(ParticleInstance);
            }
        }

        public static void UnloadProject()
        {
            trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
            trickySkyboxPrefabObjects = new List<TrickyPrefabObject>();

            for (int i = 0; i < trickyPatchObjects.Count; i++)
            {
                Raylib.UnloadMesh(trickyPatchObjects[i].mesh);
            }

            trickyPatchObjects = new List<TrickyPatchObject>();
            trickyMaterialObject = new List<TrickyMaterialObject>();
            trickyPrefabObjects = new List<TrickyPrefabObject>();
            trickyInstanceObjects = new List<TrickyInstanceObject>();
            trickyLightObjects = new List<TrickyLightObject>();
            trickyCameraObjects = new List<TrickyCameraObject>();
            trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
            trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();

            trickyEffectHeaders = new List<TrickyEffectHeader>();
            trickyFunctionHeaders = new List<TrickyFunctionHeader>();
            trickyEffectSlotObjects = new List<TrickyEffectSlotObject>();

            for (int i = 0; i < worldTextureData.Count; i++)
            {
                Raylib.UnloadTexture(worldTextureData[i].texture2D);
            }

            for (int i = 0; i < skyboxTexture2Ds.Count; i++)
            {
                Raylib.UnloadTexture(skyboxTexture2Ds[i].texture2D);
            }

            for (int i = 0; i < lightmapTexture2Ds.Count; i++)
            {
                Raylib.UnloadTexture(lightmapTexture2Ds[i].texture2D);
            }

            worldTextureData = new List<TextureData>();
            skyboxTexture2Ds = new List<TextureData>();
            lightmapTexture2Ds = new List<TextureData>();

            for (int i = 0; i < worldMeshes.Count; i++)
            {
                Raylib.UnloadMesh(worldMeshes[i].mesh);
            }

            for (int i = 0; i < skyboxMeshes.Count; i++)
            {
                Raylib.UnloadMesh(skyboxMeshes[i].mesh);
            }

            worldMeshes = new List<MeshData>();
            skyboxMeshes = new List<MeshData>();
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

        public static Mesh ReturnMesh(string FileName, bool Skybox)
        {
            if (!Skybox)
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
            else
            {
                for (int i = 0; i < skyboxMeshes.Count; i++)
                {
                    if (skyboxMeshes[i].Name == FileName)
                    {
                        return skyboxMeshes[i].mesh;
                    }
                }
                return skyboxMeshes[0].mesh;
            }
        }

        public struct TextureData
        {
            public string Name;
            public Texture2D texture2D;
        }

        public struct MeshData
        {
            public string Name;
            public Mesh mesh;
        }
    }
}
