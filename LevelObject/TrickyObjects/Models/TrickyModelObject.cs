using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;


namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyModelObject : BaseObject
    {
        bool Skybox;

        public override ObjectType Type
        {
            get { return ObjectType.Prefab; }
        }

        public int Unknown3;
        public float AnimTime;

        public List<TrickyModelMeshObject> trickyModelMeshObjects = new List<TrickyModelMeshObject>();

        bool IsCachedTexture = false;
        bool IsCachedMesh = false;
        public Mesh CachedMesh;
        public Material CachedMaterial;
        public Texture2D CachedTexture;
        public List<TrickyInstanceObject> RenderingInstances = new List<TrickyInstanceObject>();
        public List<Matrix4x4> RenderingMatrixs = new List<Matrix4x4>();

        ~TrickyModelObject()
        {
            if (IsCachedMesh && !Skybox)
            {
                Raylib.UnloadMesh(CachedMesh);
            }
            if (IsCachedTexture && !Skybox)
            {
                Raylib.UnloadTexture(CachedTexture);
            }
        }

        public void LoadPrefab(ModelJsonHandler.ModelJson prefabJson, bool skybox = false)
        {
            Skybox = skybox;

            Name = prefabJson.ModelName;
            Unknown3 = prefabJson.Unknown3;
            AnimTime = prefabJson.AnimTime;

            trickyModelMeshObjects = new List<TrickyModelMeshObject>();

            for (int i = 0; i < prefabJson.ModelObjects.Count; i++)
            {
                var TrickyPrefabMeshObject = new TrickyModelMeshObject();

                TrickyPrefabMeshObject.parent = this;

                TrickyPrefabMeshObject.LoadModelMeshObject(prefabJson.ModelObjects[i], Skybox);

                trickyModelMeshObjects.Add(TrickyPrefabMeshObject);
            }

            GenerateRenderCacheNew();
        }

        public override void Render()
        {
            if (TrickyWorldManager.WindowMode.Prefabs == TrickyWorldManager.instance.windowMode)
            {
                for (int i = 0; i < trickyModelMeshObjects.Count; i++)
                {
                    trickyModelMeshObjects[i].Render();
                }
            }
            else
            {
                Raylib.DrawMeshInstanced(CachedMesh, CachedMaterial, RenderingMatrixs.ToArray(), RenderingInstances.Count);
            }
        }

        public void RenderInstances()
        {
        }

        public void GenerateRenderCacheNew()
        {
            if (Skybox)
            {
                var TempCache = Batch.Skybox.FromLoaded(this);

                CachedMesh = TempCache.Item1;
                Raylib.UploadMesh(ref CachedMesh, false);
                CachedTexture = Raylib.LoadTextureFromImage(TempCache.Item2);
                CachedMaterial = Raylib.LoadMaterialDefault();
                Raylib.SetMaterialTexture(ref CachedMaterial, MaterialMapIndex.Diffuse, CachedTexture);
            }
            else
            {
                var TempCache = Batch.ModelBatch.FromLoaded(this);

                CachedMesh = TempCache.Item1;
                Raylib.UploadMesh(ref CachedMesh, false);
                CachedTexture = TempCache.Item2;
                CachedMaterial = Raylib.LoadMaterialDefault();
                Raylib.SetMaterialTexture(ref CachedMaterial, MaterialMapIndex.Diffuse, CachedTexture);
                Raylib.SetTextureFilter(CachedTexture, TextureFilter.Bilinear);
                var shader = Raylib.LoadShaderFromMemory(LoadEmbeddedFile.LoadText("Shaders.Instance.vs", System.Text.Encoding.UTF8),
                                                LoadEmbeddedFile.LoadText("Shaders.Instance.fs", System.Text.Encoding.UTF8));

                unsafe
                {
                    int* locs = shader.Locs;
                    locs[(int)ShaderLocationIndex.MatrixModel] = Raylib.GetShaderLocationAttrib(
                        shader,
                        "instanceTransform"
                    );
                }
                CachedMaterial.Shader = shader;

                IsCachedMesh = TempCache.CachedModel;
                IsCachedTexture = TempCache.CachedTexture;
            }
        }

        //public List<RenderCache> GenerateRenderCache()
        //{
        //    List<RenderCache> cache = new List<RenderCache>();

        //    for (int i = 0; i < Children.Count; i++)
        //    {
        //        cache.AddRange(((TrickyModelMeshObject)Children[i]).GenerateRenderCache());
        //    }

        //    return cache;
        //}
    }
}
