using CommunityToolkit.HighPerformance.Helpers;
using IceSaw2.Batch;
using IceSaw2.EditorWindows;
using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using Raylib_cs;
using SSXLibrary.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Numerics;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyModelMeshObject : BaseObject
    {
        bool Skybox;

        public int ParentID;
        public int Flags;

        public ObjectAnimation Animation = new ObjectAnimation();

        public bool IncludeAnimation;
        public bool IncludeMatrix;

        public List<Meshes> meshes = new List<Meshes>();

        private List<RenderCache> renderCaches = new List<RenderCache>();

        public void LoadModelMeshObject(ModelJsonHandler.ObjectHeader objectHeader, bool skybox)
        {
            Name = objectHeader.ObjectName;

            Skybox = skybox;

            ParentID = objectHeader.ParentID;
            Flags = objectHeader.Flags;

            IncludeAnimation = objectHeader.IncludeAnimation;
            IncludeMatrix = objectHeader.IncludeMatrix;
            Animation = new ObjectAnimation();

            if (IncludeAnimation)
            {
                Animation.U1 = objectHeader.Animation.Value.U1;
                Animation.U2 = objectHeader.Animation.Value.U2;
                Animation.U3 = objectHeader.Animation.Value.U3;
                Animation.U4 = objectHeader.Animation.Value.U4;
                Animation.U5 = objectHeader.Animation.Value.U5;
                Animation.U6 = objectHeader.Animation.Value.U6;
                Animation.AnimationAction = objectHeader.Animation.Value.AnimationAction;
                Animation.AnimationEntries = new List<AnimationEntry>();
                for (int a = 0; a < objectHeader.Animation.Value.AnimationEntries.Count; a++)
                {
                    var TempEntry = new AnimationEntry();
                    TempEntry.AnimationMaths = new List<AnimationMath>();

                    for (int b = 0; b < objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths.Count; b++)
                    {
                        var TempMaths = new AnimationMath();
                        TempMaths.Value1 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value1;
                        TempMaths.Value2 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value2;
                        TempMaths.Value3 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value3;
                        TempMaths.Value4 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value4;
                        TempMaths.Value5 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value5;
                        TempMaths.Value6 = objectHeader.Animation.Value.AnimationEntries[a].AnimationMaths[b].Value6;
                        TempEntry.AnimationMaths.Add(TempMaths);
                    }
                    Animation.AnimationEntries.Add(TempEntry);
                }
            }


            if (IncludeMatrix)
            {
                Position = JsonUtil.ArrayToVector3(objectHeader.Position);
                Scale = JsonUtil.ArrayToVector3(objectHeader.Scale);
                Rotation = JsonUtil.ArrayToQuaternion(objectHeader.Rotation);
            }

            meshes = new List<Meshes>();
            //trickyModelMaterialObjects = new List<TrickyModelMaterialObject>();

            //Load MeshHeaders
            for (int i = 0; i < objectHeader.MeshData.Count; i++)
            {
                var mesh = new Meshes();

                mesh.Skybox = skybox;

                mesh.MeshPath = objectHeader.MeshData[i].MeshPath;
                mesh.MaterialIndex = objectHeader.MeshData[i].MaterialID;

                meshes.Add(mesh);
            }

            GenerateRenderCacheNew();
        }

        public ModelJsonHandler.ObjectHeader GenerateModelMeshJson()
        {
            ModelJsonHandler.ObjectHeader objectHeader = new ModelJsonHandler.ObjectHeader();

            objectHeader.ObjectName = Name;

            objectHeader.ParentID = ParentID;
            objectHeader.Flags = Flags;

            objectHeader.IncludeAnimation = IncludeAnimation;
            objectHeader.IncludeMatrix = IncludeMatrix;

            objectHeader.Animation = new ModelJsonHandler.ObjectAnimation();
            if(objectHeader.IncludeAnimation)
            {
                var NewAnimation = new ModelJsonHandler.ObjectAnimation();

                NewAnimation.U1 = Animation.U1;
                NewAnimation.U2 = Animation.U2;
                NewAnimation.U3 = Animation.U3;
                NewAnimation.U4 = Animation.U4;
                NewAnimation.U5 = Animation.U5;
                NewAnimation.U6 = Animation.U6;
                NewAnimation.AnimationAction = Animation.AnimationAction;
                NewAnimation.AnimationEntries = new List<ModelJsonHandler.AnimationEntry>();
                for (int i = 0; i < Animation.AnimationEntries.Count; i++)
                {
                    var NewAnimationEntry = new ModelJsonHandler.AnimationEntry();
                    NewAnimationEntry.AnimationMaths = new List<ModelJsonHandler.AnimationMath>();
                    for (int j = 0; j < Animation.AnimationEntries[i].AnimationMaths.Count; j++)
                    {
                        var NewMaths = new ModelJsonHandler.AnimationMath();

                        NewMaths.Value1 = Animation.AnimationEntries[i].AnimationMaths[j].Value1;
                        NewMaths.Value2 = Animation.AnimationEntries[i].AnimationMaths[j].Value2;
                        NewMaths.Value3 = Animation.AnimationEntries[i].AnimationMaths[j].Value3;
                        NewMaths.Value4 = Animation.AnimationEntries[i].AnimationMaths[j].Value4;
                        NewMaths.Value5 = Animation.AnimationEntries[i].AnimationMaths[j].Value5;
                        NewMaths.Value6 = Animation.AnimationEntries[i].AnimationMaths[j].Value6;

                        NewAnimationEntry.AnimationMaths.Add(NewMaths);
                    }
                    NewAnimation.AnimationEntries.Add(NewAnimationEntry);
                }

                objectHeader.Animation = NewAnimation;
            }

            if(objectHeader.IncludeMatrix)
            {
                objectHeader.Position = JsonUtil.Vector3ToArray(Position);
                objectHeader.Scale = JsonUtil.Vector3ToArray(Scale);
                objectHeader.Rotation = JsonUtil.QuaternionToArray(Rotation);
            }

            objectHeader.MeshData = new List<ModelJsonHandler.MeshHeader>();
            for (int i = 0; i < meshes.Count; i++)
            {
                ModelJsonHandler.MeshHeader meshHeader = new ModelJsonHandler.MeshHeader();

                meshHeader.MeshPath = meshes[i].MeshPath;
                meshHeader.MaterialID = meshes[i].MaterialIndex;

                objectHeader.MeshData.Add(meshHeader);
            }

            return objectHeader;
        }

        public override void Render()
        {
            if (TrickyWorldManager.instance.windowMode == TrickyWorldManager.WindowMode.World)
            {
                if (!Skybox)
                {
                    for (int i = 0; i < renderCaches.Count; i++)
                    {
                        Raylib.DrawMeshInstanced(renderCaches[i].meshRef.Mesh, renderCaches[i].materialRef.Material, renderCaches[i].matrix4X4Array, renderCaches[i].matrix4X4Array.Length);
                    }
                }
                else
                {
                    Matrix4x4 matrix4X4 = Default;
                    matrix4X4.M14 = TrickyWorldManager.instance.levelEditorWindow.viewCamera3D.Position.X;
                    matrix4X4.M24 = TrickyWorldManager.instance.levelEditorWindow.viewCamera3D.Position.Y;
                    matrix4X4.M34 = TrickyWorldManager.instance.levelEditorWindow.viewCamera3D.Position.Z;

                    for (int i = 0; i < renderCaches.Count; i++)
                    {
                        Raylib.DrawMesh(renderCaches[i].meshRef.Mesh, renderCaches[i].materialRef.Material, matrix4X4);
                    }
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
            renderCaches = new List<RenderCache>();

            for (global::System.Int32 j = 0; j < meshes.Count; j++)
            {
                var TempCache = new RenderCache();

                TempCache.meshRef = meshes[j].meshRef;
                TempCache.materialRef = meshes[j].materialRef;
                TempCache.matrix4X4s = new List<Matrix4x4>();
                TempCache.trickyInstanceObjects = new List<TrickyInstanceObject>();

                if(Skybox)
                {
                    TempCache.matrix4X4s.Add(worldMatrix4x4);
                }

                TempCache.matrix4X4Array = TempCache.matrix4X4s.ToArray();

                renderCaches.Add(TempCache);
            }
        }

        public void AddToRenderCache(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                renderCaches[i].trickyInstanceObjects.Add(trickyInstanceObject);
                renderCaches[i].matrix4X4s.Add(trickyInstanceObject.worldMatrix4x4 * localMatrix4X4);
                RebuildMatrixArray(i);
            }
        }

        public void RemoveFromRenderCache(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                if (renderCaches[i].trickyInstanceObjects.Contains(trickyInstanceObject))
                {
                    int Value = renderCaches[i].trickyInstanceObjects.IndexOf(trickyInstanceObject);

                    renderCaches[i].trickyInstanceObjects.RemoveAt(Value);
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
        public void UpdateRenderCacheTransform(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                int idx = renderCaches[i].trickyInstanceObjects.IndexOf(trickyInstanceObject);
                if (idx < 0) continue;

                renderCaches[i].matrix4X4s[idx] = trickyInstanceObject.worldMatrix4x4 * localMatrix4X4;
                RebuildMatrixArray(i);
            }
        }

        [Serializable]
        public struct ObjectAnimation
        {
            public float U1;
            public float U2;
            public float U3;
            public float U4;
            public float U5;
            public float U6;

            public int AnimationAction;
            public List<AnimationEntry> AnimationEntries;
        }
        [Serializable]
        public struct AnimationEntry
        {
            public List<AnimationMath> AnimationMaths;
        }
        [Serializable]
        public struct AnimationMath
        {
            public float Value1;
            public float Value2;
            public float Value3;
            public float Value4;
            public float Value5;
            public float Value6;
        }
        [Serializable]
        public struct Meshes
        {
            public bool Skybox;
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

            private int _MaterialIndex;
            public int MaterialIndex
            {
                get
                { return _MaterialIndex; }
                set
                {
                    _MaterialIndex = value;
                    GenerateModel();
                }
            }

            public MeshRef meshRef;
            public MaterialRef materialRef;

            public void GenerateModel()
            {
                meshRef = TrickyDataManager.ReturnMesh(_meshPath, Skybox);

                if (!Skybox)
                {
                    materialRef = TrickyDataManager.trickyMaterialObject[_MaterialIndex].materialRef;
                }
                else
                {
                    materialRef = TrickyDataManager.trickySkyboxMaterialObject[_MaterialIndex].materialRef;
                }
            }
        }

    }
}
