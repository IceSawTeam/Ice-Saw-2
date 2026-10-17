using IceSaw2.Batch;
using IceSaw2.LevelObject;
using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using Raylib_cs;
using SSXLibrary.FileHandlers.LevelFiles.SSX3PS2.SSBData;
using SSXLibrary.JsonFiles.SSX3;
using SSXMultiTool.Utilities;
using System.Numerics;
using static IceSaw2.LevelObject.BaseObject;
using static SSXLibrary.JsonFiles.SSX3.MDRJsonHandler;
namespace IceSaw2.LevelObject.SSX3
{
    public class SSX3ModelMeshObject : BaseObject
    {
        public int ParentID;

        public UnknownS2 unknownS2;
        public UnknownS3 unknownS3;

        public List<Meshes> meshes = new List<Meshes>();
        private List<SSX3RenderCache> renderCaches = new List<SSX3RenderCache>();
        public void LoadPrefab(MDRJsonHandler.ModelObject model)
        {
            ParentID = model.ParentID;

            Position = JsonUtil.ArrayToVector3(model.Position);
            Rotation = JsonUtil.ArrayToQuaternion(model.Rotation);
            Scale = JsonUtil.ArrayToVector3(model.Scale);

            if (Scale == Vector3.Zero)
            {
                Scale = Vector3.One;
            }

            unknownS2 = model.unknownS2;
            unknownS3 = model.unknownS3;

            for (int i = 0; i < model.unknownS2.ModelHeaderOffset.Count; i++)
            {
                var mesh = new Meshes();

                mesh.MeshPath = model.unknownS2.ModelHeaderOffset[i].ModelPath;
                mesh.MaterialIndex = model.unknownS2.ModelHeaderOffset[i].MaterialID;

                mesh.U1 = model.unknownS2.ModelHeaderOffset[i].U1;
                mesh.U4 = model.unknownS2.ModelHeaderOffset[i].U4;

                mesh.U00 = model.unknownS2.ModelHeaderOffset[i].U00;
                mesh.U01 = model.unknownS2.ModelHeaderOffset[i].U01;
                mesh.U02 = model.unknownS2.ModelHeaderOffset[i].U02;
                mesh.U03 = model.unknownS2.ModelHeaderOffset[i].U03;

                mesh.U04 = model.unknownS2.ModelHeaderOffset[i].U04;

                meshes.Add(mesh);
            }

            GenerateRenderCacheNew();
        }

        public override void Render()
        {
            if (SSX3WorldManager.instance.windowMode == SSX3WorldManager.WindowMode.World)
            {
                    for (int i = 0; i < renderCaches.Count; i++)
                    {
                        Raylib.DrawMeshInstanced(renderCaches[i].meshRef.Mesh, renderCaches[i].materialRef.Material, renderCaches[i].matrix4X4Array, renderCaches[i].matrix4X4Array.Length);
                    }
            }
            else
            {
                Matrix4x4[] matrixArray = { worldMatrix4x4 };
                for (int i = 0; i < meshes.Count; i++)
                {
                    Raylib.DrawMeshInstanced(meshes[i].meshRef.Mesh, meshes[i].materialRef.Material, matrixArray, 1);
                }
            }
        }

        public void GenerateRenderCacheNew()
        {
            renderCaches = new List<SSX3RenderCache>();

            for (global::System.Int32 j = 0; j < meshes.Count; j++)
            {
                var TempCache = new SSX3RenderCache();

                TempCache.meshRef = meshes[j].meshRef;
                TempCache.materialRef = meshes[j].materialRef;
                TempCache.matrix4X4s = new List<Matrix4x4>();
                TempCache.ssx3InstanceObjects = new List<SSX3InstanceObject>();

                TempCache.matrix4X4Array = TempCache.matrix4X4s.ToArray();

                renderCaches.Add(TempCache);
            }
        }

        public void AddToRenderCache(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                renderCaches[i].ssx3InstanceObjects.Add(trickyInstanceObject);
                renderCaches[i].matrix4X4s.Add(trickyInstanceObject.worldMatrix4x4 * localMatrix4X4);
                RebuildMatrixArray(i);
            }
        }

        public void RemoveFromRenderCache(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                if (renderCaches[i].ssx3InstanceObjects.Contains(trickyInstanceObject))
                {
                    int Value = renderCaches[i].ssx3InstanceObjects.IndexOf(trickyInstanceObject);

                    renderCaches[i].ssx3InstanceObjects.RemoveAt(Value);
                    renderCaches[i].matrix4X4s.RemoveAt(Value);
                    RebuildMatrixArray(i);
                }
            }
        }

        // renderCaches holds RenderCache structs by value, so the flattened array cache has to be
        // written back into the list explicitly rather than mutated through the indexer directly.
        private void RebuildMatrixArray(int i)
        {
            var cache = renderCaches[i];
            cache.matrix4X4Array = cache.matrix4X4s.ToArray();
            renderCaches[i] = cache;
        }

        // AddToRenderCache snapshots instance.worldMatrix4x4 into matrix4X4s once, at add-time - it
        // isn't a live reference, so anything that moves/rotates/scales an already-placed instance
        // (e.g. the gizmo) needs to explicitly resync that snapshot or the render cache silently goes
        // stale and the drawn mesh stops matching the instance's actual transform.
        public void UpdateRenderCacheTransform(SSX3InstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                int idx = renderCaches[i].ssx3InstanceObjects.IndexOf(trickyInstanceObject);
                if (idx < 0) continue;

                renderCaches[i].matrix4X4s[idx] = trickyInstanceObject.worldMatrix4x4 * localMatrix4X4;
                RebuildMatrixArray(i);
            }
        }

        public struct Meshes
        {
            private string _meshPath;

            public string MeshPath
            {
                get
                { return _meshPath; }
                set
                {
                    _meshPath = value;
                    GenerateModel();
                }
            }

            private ObjectID _MaterialIndex;
            public ObjectID MaterialIndex
            {
                get
                { return _MaterialIndex; }
                set
                {
                    _MaterialIndex = value;
                    GenerateModel();
                }
            }

            public int U1;
            public int U4;

            public int U00;
            public int U01;
            public int U02;
            public int U03;

            public float[] U04;

            public MeshRef meshRef;
            public MaterialRef materialRef;

            public void GenerateModel()
            {
                meshRef = SSX3DataManager.ReturnMesh(_meshPath, MaterialIndex.TrackID);

                materialRef = SSX3DataManager.ReturnMaterial(MaterialIndex.RID,MaterialIndex.TrackID);
            }
        }
    }
}