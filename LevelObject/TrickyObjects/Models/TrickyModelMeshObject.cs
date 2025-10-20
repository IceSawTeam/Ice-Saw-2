using IceSaw2.EditorWindows;
using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
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
        public List<RenderCache> renderCaches = new List<RenderCache>();

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

        public override void Render()
        {
            if (TrickyWorldManager.instance.windowMode == TrickyWorldManager.WindowMode.World)
            {
                if (!Skybox)
                {
                    for (int i = 0; i < renderCaches.Count; i++)
                    {
                        Raylib.DrawMeshInstanced(renderCaches[i].mesh, renderCaches[i].material, renderCaches[i].matrix4X4s.ToArray(), renderCaches[i].matrix4X4s.Count);
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
                        Raylib.DrawMesh(renderCaches[i].mesh, renderCaches[i].material, matrix4X4);
                    }
                }
            }
            else
            {
                Matrix4x4[] matrixArray = { worldMatrix4x4 };
                for (int i = 0; i < meshes.Count; i++)
                {
                    Raylib.DrawMeshInstanced(meshes[i].mesh.Mesh, meshes[i].material.Material, matrixArray, 1);
                }
            }
        }

        public void GenerateRenderCacheNew()
        {
            renderCaches = new List<RenderCache>();

            for (global::System.Int32 j = 0; j < meshes.Count; j++)
            {
                var TempCache = new RenderCache();

                TempCache.mesh = meshes[j].mesh.Mesh;
                TempCache.material = meshes[j].material.Material;
                TempCache.matrix4X4s = new List<Matrix4x4>();
                TempCache.trickyInstanceObjects = new List<TrickyInstanceObject>();

                if(Skybox)
                {
                    TempCache.matrix4X4s.Add(worldMatrix4x4);
                }

                renderCaches.Add(TempCache);
            }
        }

        public void AddToRenderCache(TrickyInstanceObject trickyInstanceObject)
        {
            for (int i = 0; i < renderCaches.Count; i++)
            {
                renderCaches[i].trickyInstanceObjects.Add(trickyInstanceObject);
                renderCaches[i].matrix4X4s.Add(trickyInstanceObject.worldMatrix4x4 * localMatrix4X4);
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
                }
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

            public MeshRef mesh;
            public MaterialRef material;

            public void GenerateModel()
            {
                mesh = TrickyDataManager.ReturnMesh(_meshPath, Skybox);

                if (!Skybox)
                {
                    material = TrickyDataManager.trickyMaterialObject[_MaterialIndex].material;
                }
                else
                {
                    material = TrickyDataManager.trickySkyboxMaterialObject[_MaterialIndex].material;
                }
            }
        }

    }
}
