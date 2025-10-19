using IceSaw2.Manager.Tricky;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;

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
        }

        public override void Render()
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                Raylib.DrawMesh(meshes[i].mesh, meshes[i].material, worldMatrix4x4);
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
        public class Meshes
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

            public Mesh mesh;
            public Material material;

            public void GenerateModel()
            {
                mesh = TrickyDataManager.ReturnMesh(MeshPath, Skybox);

                var TexturePath = "";

                if (!Skybox)
                {
                    TexturePath = TrickyDataManager.trickyMaterialObject[MaterialIndex].TexturePath;
                }
                else
                {
                    TexturePath = TrickyDataManager.trickySkyboxMaterialObject[MaterialIndex].TexturePath;
                }

                Texture2D ReturnTexture = TrickyDataManager.ReturnTexture(TexturePath, Skybox);

                material = Raylib.LoadMaterialDefault();

                Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, ReturnTexture);
            }
        }
    }
}
