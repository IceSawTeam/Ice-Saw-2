using IceSaw2.Batch;
using IceSaw2.LevelObject;
using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using IceSaw2.Utilities;
using Raylib_cs;
using SSXLibrary.JsonFiles.SSX3;

namespace IceSaw2.LevelObject.SSX3
{
    public class SSX3MaterialObject : MeshBaseObject
    {
        public int TrackID;
        public int RID;

        public override ObjectType Type
        {
            get { return ObjectType.Material; }
        }

        private string _texturePath = "";
        public string TexturePath
        {
            get
            {
                return _texturePath;
            }
            set
            {
                if (_texturePath != value)
                {
                    _texturePath = value;
                    GenerateMaterial();
                }
            }
        }
        public int U1;
        public int U2;
        public int U3;
        public int U4;
        public int U5;
        public int U6;
        public int U7;
        public int U8;
        public int U9;
        public int U10;


        public void LoadMaterial(MaterialJsonHandler.Material bin0)
        {
            TrackID = bin0.TrackID;
            RID = bin0.RID;

            Name = RID.ToString();

            TexturePath = bin0.TextureName;
            U1 = bin0.U1;
            U2 = bin0.U2;
            U3 = bin0.U3;
            U4 = bin0.U4;
            U5 = bin0.U5;
            U6 = bin0.U6;
            U7 = bin0.U7;
            U8 = bin0.U8;
            U9 = bin0.U9;
            U10 = bin0.U10;
        }

        public void GenerateMaterial()
        {
            Texture2D ReturnTexture = SSX3DataManager.ReturnTexture(TexturePath);

            if (materialRef == null)
            {
                materialRef = new IceSaw2.RayWarp.MaterialRef(Raylib.LoadMaterialDefault());
            }

            Raylib.SetMaterialTexture(ref materialRef.Material, MaterialMapIndex.Diffuse, ReturnTexture);

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

            materialRef.Material.Shader = shader;
        }
    }
}