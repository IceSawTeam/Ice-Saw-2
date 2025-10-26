using IceSaw2.Batch;
using IceSaw2.LevelObject;
using IceSaw2.LevelObject.Materials;
using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using IceSaw2.RayWarp;

namespace IceSaw2.Manager.Tricky
{
    public static class TrickyDataManager
    {
        //Skybox Data
        public static List<TrickyMaterialObject> trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
        public static List<TrickyModelObject> trickySkyboxPrefabObjects = new List<TrickyModelObject>();

        //Object Data
        public static List<TrickyPatchObject> trickyPatchObjects = new List<TrickyPatchObject>();
        public static List<TrickyMaterialObject> trickyMaterialObject = new List<TrickyMaterialObject>();
        public static List<TrickyModelObject> trickyModelObjects = new List<TrickyModelObject>();
        public static List<TrickyInstanceObject> trickyInstanceObjects = new List<TrickyInstanceObject>();
        public static List<TrickySplineObject> trickySplineObjects = new List<TrickySplineObject>();
        public static List<TrickyParticlePrefabObject> trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
        public static List<TrickyPaticleInstanceObject> trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();
        public static List<TrickyCameraObject> trickyCameraObjects = new List<TrickyCameraObject>();
        public static List<TrickyLightObject> trickyLightObjects = new List<TrickyLightObject>();

        //Pathing Data
        public static List<int> AIPStartPos = new List<int>();
        public static List<TrickyPathBObject> trickyAIPRaceLine = new List<TrickyPathBObject>();
        public static List<TrickyPathAObject> trickyAIPAIPath = new List<TrickyPathAObject>();
        public static List<int> SOPStartPos = new List<int>();
        public static List<TrickyPathBObject> trickySOPRaceLine = new List<TrickyPathBObject>();
        public static List<TrickyPathAObject> trickySOPAIPath = new List<TrickyPathAObject>();

        //Effect Data
        public static List<TrickyEffectSlotObject> trickyEffectSlotObjects = new List<TrickyEffectSlotObject>();
        public static List<TrickyPhysicsObject> trickyPhysicsObjects = new List<TrickyPhysicsObject>();
        public static List<TrickyFunctionHeader> trickyFunctionHeaders = new List<TrickyFunctionHeader>();
        public static List<TrickyEffectHeader> trickyEffectHeaders = new List<TrickyEffectHeader>();

        //Texture Data
        public static List<TextureData> worldTextureData = new List<TextureData>();
        public static List<TextureData> skyboxTexture2Ds = new List<TextureData>();
        public static List<TextureData> lightmapTexture2Ds = new List<TextureData>();

        //Mesh Data
        public static List<MeshData> worldMeshes = new List<MeshData>();
        public static List<MeshData> skyboxMeshes = new List<MeshData>();

        //Cached Skybox Mesh Data
        public static MeshRef meshSkybox;
        public static MaterialRef materialSkybox;
        public static Texture2D textureSkybox;


        public static List<BaseObject> LevelNodeTree = new List<BaseObject>();

        public static string LoadPath = "";

        public static void LoadProject(string ConfigPath)
        {
            UnloadProject();

            LoadPath = Path.GetDirectoryName(ConfigPath);

            LoadTextureMesh();

            LoadLevelObjects();

            LoadPaths();

            LoadEffects();

            LoadSkybox();

            LoadTreeNode();
        }

        public static void LoadTextureMesh()
        {
            //Texture Data
            worldTextureData = new List<TextureData>();
            string[] TextureFiles = Directory.GetFiles(Path.Combine(LoadPath,"Textures"), "*.png", SearchOption.AllDirectories);
            for (int i = 0; i < TextureFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(TextureFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(TextureFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                worldTextureData.Add(textureData);
            }

            lightmapTexture2Ds = new List<TextureData>();
            string[] LightmapFiles = Directory.GetFiles(Path.Combine(LoadPath, "Lightmaps"), "*.png", SearchOption.AllDirectories);
            for (int i = 0; i < LightmapFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(LightmapFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(LightmapFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                lightmapTexture2Ds.Add(textureData);
            }

            skyboxTexture2Ds = new List<TextureData>();
            string[] SkyboxFiles = Directory.GetFiles(Path.Combine(LoadPath, "Skybox", "Textures"), "*.png", SearchOption.AllDirectories);
            for (int i = 0; i < SkyboxFiles.Length; i++)
            {
                TextureData textureData = new TextureData();

                textureData.Name = Path.GetFileName(SkyboxFiles[i]);
                textureData.texture2D = Raylib.LoadTexture(SkyboxFiles[i]);

                Raylib.SetTextureFilter(textureData.texture2D, TextureFilter.Bilinear);

                skyboxTexture2Ds.Add(textureData);
            }

            worldMeshes = new List<MeshData>();
            string[] MeshFiles = Directory.GetFiles(Path.Combine(LoadPath, "Meshes"), "*.obj", SearchOption.AllDirectories);
            for (int i = 0; i < MeshFiles.Length; i++)
            {
                var TempMesh = new MeshData();

                TempMesh.Name = Path.GetFileName(MeshFiles[i]);
                TempMesh.mesh = new MeshRef(ObjImporter.ObjLoad(MeshFiles[i]));

                Raylib.UploadMesh(ref TempMesh.mesh.Mesh, false);

                worldMeshes.Add(TempMesh);
            }

            skyboxMeshes = new List<MeshData>();
            string[] SkyboxMeshFiles = Directory.GetFiles(Path.Combine(LoadPath, "Skybox", "Meshes"), "*.obj", SearchOption.AllDirectories);
            for (int i = 0; i < SkyboxMeshFiles.Length; i++)
            {
                var TempMesh = new MeshData();

                TempMesh.Name = Path.GetFileName(SkyboxMeshFiles[i]);
                TempMesh.mesh = new MeshRef(ObjImporter.ObjLoad(SkyboxMeshFiles[i]));

                Raylib.UploadMesh(ref TempMesh.mesh.Mesh, false);

                skyboxMeshes.Add(TempMesh);
            }
        }

        public static void LoadLevelObjects()
        {
            //Object Data
            trickyPatchObjects = new List<TrickyPatchObject>();
            PatchesJsonHandler jsonHandler = PatchesJsonHandler.Load(Path.Combine(LoadPath, "Patches.json"));
            for (int i = 0; i < jsonHandler.Patches.Count; i++)
            {
                TrickyPatchObject patchObject = new TrickyPatchObject();

                patchObject.LoadPatch(jsonHandler.Patches[i]);

                trickyPatchObjects.Add(patchObject);
            }


            trickyMaterialObject = new List<TrickyMaterialObject>();
            MaterialJsonHandler matJsonHandler = MaterialJsonHandler.Load(Path.Combine(LoadPath, "Materials.json"));
            for (int i = 0; i < matJsonHandler.Materials.Count; i++)
            {
                TrickyMaterialObject materialObject = new TrickyMaterialObject();

                materialObject.LoadMaterial(matJsonHandler.Materials[i], false);

                trickyMaterialObject.Add(materialObject);
            }

            trickyModelObjects = new List<TrickyModelObject>();
            ModelJsonHandler prefabJsonHandler = ModelJsonHandler.Load(Path.Combine(LoadPath, "Models.json"));
            for (int i = 0; i < prefabJsonHandler.Models.Count; i++)
            {
                var NewPrefab = new TrickyModelObject();

                NewPrefab.LoadPrefab(prefabJsonHandler.Models[i]);

                trickyModelObjects.Add(NewPrefab);
            }

            trickyInstanceObjects = new List<TrickyInstanceObject>();
            InstanceJsonHandler instanceJsonHandler = InstanceJsonHandler.Load(Path.Combine(LoadPath, "Instances.json"));
            for (int i = 0; i < instanceJsonHandler.Instances.Count; i++)
            {
                var Instance = new TrickyInstanceObject();

                Instance.LoadInstance(instanceJsonHandler.Instances[i]);

                trickyInstanceObjects.Add(Instance);
            }

            trickySplineObjects = new List<TrickySplineObject>();
            SplineJsonHandler splineJsonHandler = SplineJsonHandler.Load(Path.Combine(LoadPath, "Splines.json"));
            for (int i = 0; i < splineJsonHandler.Splines.Count; i++)
            {
                var Spline = new TrickySplineObject();

                Spline.LoadSpline(splineJsonHandler.Splines[i]);

                trickySplineObjects.Add(Spline);
            }

            trickyLightObjects = new List<TrickyLightObject>();
            LightJsonHandler lightJsonHandler = LightJsonHandler.Load(Path.Combine(LoadPath, "Lights.json"));
            for (int i = 0; i < lightJsonHandler.Lights.Count; i++)
            {
                var Light = new TrickyLightObject();

                Light.LoadLight(lightJsonHandler.Lights[i]);

                trickyLightObjects.Add(Light);
            }

            trickyCameraObjects = new List<TrickyCameraObject>();
            CameraJSONHandler cameraJSONHandler = CameraJSONHandler.Load(Path.Combine(LoadPath, "Cameras.json"));
            for (int i = 0; i < cameraJSONHandler.Cameras.Count; i++)
            {
                var Camera = new TrickyCameraObject();

                Camera.LoadCamera(cameraJSONHandler.Cameras[i]);

                trickyCameraObjects.Add(Camera);
            }

            trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
            ParticleModelJsonHandler particleModelJsonHandler = ParticleModelJsonHandler.Load(Path.Combine(LoadPath, "ParticlePrefabs.json"));
            for (int i = 0; i < particleModelJsonHandler.ParticlePrefabs.Count; i++)
            {
                var ParticlePrefab = new TrickyParticlePrefabObject();

                ParticlePrefab.LoadParticle(particleModelJsonHandler.ParticlePrefabs[i]);

                trickyParticlePrefabObjects.Add(ParticlePrefab);
            }

            trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();
            ParticleInstanceJsonHandler particleInstanceJsonHandler = ParticleInstanceJsonHandler.Load(Path.Combine(LoadPath, "ParticleInstances.json"));
            for (int i = 0; i < particleInstanceJsonHandler.Particles.Count; i++)
            {
                var ParticleInstance = new TrickyPaticleInstanceObject();

                ParticleInstance.LoadPaticleInstance(particleInstanceJsonHandler.Particles[i]);

                trickyPaticleInstanceObjects.Add(ParticleInstance);
            }
        }

        public static void LoadPaths()
        {
            AIPSOPJsonHandler aIPSOPJsonHandler = AIPSOPJsonHandler.Load(Path.Combine(LoadPath, "AIP.json"));

            AIPStartPos = aIPSOPJsonHandler.StartPosList;

            trickyAIPAIPath = new List<TrickyPathAObject>();
            for (int i = 0; i < aIPSOPJsonHandler.AIPaths.Count; i++)
            {
                TrickyPathAObject trickyPathAObject = new TrickyPathAObject();

                trickyPathAObject.LoadPathA(aIPSOPJsonHandler.AIPaths[i]);

                trickyAIPAIPath.Add(trickyPathAObject);
            }

            trickyAIPRaceLine = new List<TrickyPathBObject>();
            for (int i = 0; i < aIPSOPJsonHandler.RaceLines.Count; i++)
            {
                TrickyPathBObject trickyPathBObject = new TrickyPathBObject();

                trickyPathBObject.LoadPathB(aIPSOPJsonHandler.RaceLines[i]);

                trickyAIPRaceLine.Add(trickyPathBObject);
            }

            aIPSOPJsonHandler = AIPSOPJsonHandler.Load(Path.Combine(LoadPath, "SOP.json"));

            SOPStartPos = aIPSOPJsonHandler.StartPosList;

            trickySOPAIPath = new List<TrickyPathAObject>();
            for (int i = 0; i < aIPSOPJsonHandler.AIPaths.Count; i++)
            {
                TrickyPathAObject trickyPathAObject = new TrickyPathAObject();

                trickyPathAObject.LoadPathA(aIPSOPJsonHandler.AIPaths[i]);

                trickySOPAIPath.Add(trickyPathAObject);
            }

            trickySOPRaceLine = new List<TrickyPathBObject>();
            for (int i = 0; i < aIPSOPJsonHandler.RaceLines.Count; i++)
            {
                TrickyPathBObject trickyPathBObject = new TrickyPathBObject();

                trickyPathBObject.LoadPathB(aIPSOPJsonHandler.RaceLines[i]);

                trickySOPRaceLine.Add(trickyPathBObject);
            }
        }

        public static void LoadSkybox()
        {
            //Skybox
            trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
            MaterialJsonHandler matskyJsonHandler = MaterialJsonHandler.Load(Path.Combine(LoadPath, "Skybox", "Materials.json"));
            for (int i = 0; i < matskyJsonHandler.Materials.Count; i++)
            {
                TrickyMaterialObject materialObject = new TrickyMaterialObject();

                materialObject.LoadMaterial(matskyJsonHandler.Materials[i], true);

                trickySkyboxMaterialObject.Add(materialObject);
            }

            trickySkyboxPrefabObjects = new List<TrickyModelObject>();
            ModelJsonHandler prefabSkyboxJsonHandler = ModelJsonHandler.Load(Path.Combine(LoadPath, "Skybox", "Models.json"));
            for (int i = 0; i < prefabSkyboxJsonHandler.Models.Count; i++)
            {
                var NewPrefab = new TrickyModelObject();

                NewPrefab.LoadPrefab(prefabSkyboxJsonHandler.Models[i], true);

                trickySkyboxPrefabObjects.Add(NewPrefab);
            }

            if (trickySkyboxPrefabObjects.Count != 0)
            {
                var Skybox = Batch.Skybox.FromLoaded(trickySkyboxPrefabObjects[0]);

                meshSkybox = new MeshRef(Skybox.Item1);

                Raylib.UploadMesh(ref meshSkybox.Mesh, false);

                textureSkybox = Raylib.LoadTextureFromImage(Skybox.Item2);

                Material material = Raylib.LoadMaterialDefault();

                Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Albedo, textureSkybox);

                materialSkybox = new MaterialRef(material);
            }
        }

        public static void LoadEffects()
        {
            //Effects
            SSFJsonHandler sSFJsonHandler = SSFJsonHandler.Load(Path.Combine(LoadPath,"SSFLogic.json"));

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

            trickyPhysicsObjects = new List<TrickyPhysicsObject>();
            for (int i = 0; i < sSFJsonHandler.PhysicsHeaders.Count; i++)
            {
                var NewPhysics = new TrickyPhysicsObject();

                NewPhysics.LoadPhysics(sSFJsonHandler.PhysicsHeaders[i]);

                trickyPhysicsObjects.Add(NewPhysics);
            }
        }

        public static void LoadTreeNode()
        {
            //Attempt to load JSON First if none do base load

            BaseObject baseObject = new BaseObject();
            baseObject.Name = "Patches";

            for (int i = 0; i < trickyPatchObjects.Count; i++)
            {
                baseObject.AddChild(trickyPatchObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Instances";

            for (int i = 0; i < trickyInstanceObjects.Count; i++)
            {
                baseObject.AddChild(trickyInstanceObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Splines";

            for (int i = 0; i < trickySplineObjects.Count; i++)
            {
                baseObject.AddChild(trickySplineObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Lights";

            for (int i = 0; i < trickyLightObjects.Count; i++)
            {
                baseObject.AddChild(trickyLightObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Cameras";

            for (int i = 0; i < trickyCameraObjects.Count; i++)
            {
                baseObject.AddChild(trickyCameraObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Particle Instances";

            for (int i = 0; i < trickyPaticleInstanceObjects.Count; i++)
            {
                baseObject.AddChild(trickyPaticleInstanceObjects[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "General Race Line";

            for (int i = 0; i < trickyAIPRaceLine.Count; i++)
            {
                baseObject.AddChild(trickyAIPRaceLine[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "General AI Path";

            for (int i = 0; i < trickyAIPAIPath.Count; i++)
            {
                baseObject.AddChild(trickyAIPAIPath[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Showoff Race Line";

            for (int i = 0; i < trickySOPRaceLine.Count; i++)
            {
                baseObject.AddChild(trickySOPRaceLine[i]);
            }

            LevelNodeTree.Add(baseObject);

            baseObject = new BaseObject();
            baseObject.Name = "Showoff AI Path";

            for (int i = 0; i < trickySOPAIPath.Count; i++)
            {
                baseObject.AddChild(trickySOPAIPath[i]);
            }

            LevelNodeTree.Add(baseObject);
        }

        public static void UnloadProject()
        {
            LevelNodeTree = new List<BaseObject>();

            for (int i = 0; i < trickySkyboxMaterialObject.Count; i++)
            {
                Raylib.UnloadMaterial(trickySkyboxMaterialObject[i].materialRef.Material);
            }

            trickySkyboxMaterialObject = new List<TrickyMaterialObject>();
            trickySkyboxPrefabObjects = new List<TrickyModelObject>();

            for (int i = 0; i < trickyPatchObjects.Count; i++)
            {
                Raylib.UnloadMesh(trickyPatchObjects[i].meshRef.Mesh);
            }

            trickyPatchObjects = new List<TrickyPatchObject>();

            for (int i = 0; i < trickyMaterialObject.Count; i++)
            {
                Raylib.UnloadMaterial(trickyMaterialObject[i].materialRef.Material);
            }

            trickyMaterialObject = new List<TrickyMaterialObject>();

            trickyModelObjects = new List<TrickyModelObject>();
            trickyInstanceObjects = new List<TrickyInstanceObject>();
            trickyLightObjects = new List<TrickyLightObject>();
            trickyCameraObjects = new List<TrickyCameraObject>();
            trickyParticlePrefabObjects = new List<TrickyParticlePrefabObject>();
            trickyPaticleInstanceObjects = new List<TrickyPaticleInstanceObject>();

            trickyEffectHeaders = new List<TrickyEffectHeader>();
            trickyFunctionHeaders = new List<TrickyFunctionHeader>();
            trickyEffectSlotObjects = new List<TrickyEffectSlotObject>();
            trickyPhysicsObjects = new List<TrickyPhysicsObject>();

            trickyAIPRaceLine = new List<TrickyPathBObject>();
            trickyAIPAIPath = new List<TrickyPathAObject>();
            trickySOPAIPath = new List<TrickyPathAObject>();
            trickySOPRaceLine = new List<TrickyPathBObject>();

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
                Raylib.UnloadMesh(worldMeshes[i].mesh.Mesh);
            }

            for (int i = 0; i < skyboxMeshes.Count; i++)
            {
                Raylib.UnloadMesh(skyboxMeshes[i].mesh.Mesh);
            }

            worldMeshes = new List<MeshData>();
            skyboxMeshes = new List<MeshData>();

            if (meshSkybox != null)
            {
                Raylib.UnloadTexture(textureSkybox);
                Raylib.UnloadMesh(meshSkybox.Mesh);
                Raylib.UnloadMaterial(materialSkybox.Material);
            }

            textureSkybox = new Texture2D();
            meshSkybox = null;
            materialSkybox = null;
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
                return TrickyWorldManager.instance.ErrorTexture;
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
                return TrickyWorldManager.instance.ErrorTexture;
            }
        }

        public static MeshRef ReturnMesh(string FileName, bool Skybox)
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
            public MeshRef mesh;
        }
    }
}
