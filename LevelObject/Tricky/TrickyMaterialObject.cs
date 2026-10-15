using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using SSXLibrary.JsonFiles.Tricky;

namespace IceSaw2.LevelObject.Tricky
{
    public class TrickyMaterialObject : MeshBaseObject
    {
        bool Skybox = false;
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
            set {
                if (_texturePath != value)
                {
                    _texturePath = value;
                    GenerateMaterial();
                }
            } 
        }

        public int UnknownInt2;
        public int UnknownInt3;

        public float UnknownFloat1;
        public float UnknownFloat2;
        public float UnknownFloat3;
        public float UnknownFloat4;

        public int UnknownInt8;

        public float UnknownFloat5;
        public float UnknownFloat6;
        public float UnknownFloat7;
        public float UnknownFloat8;

        public int UnknownInt13;
        public int UnknownInt14;
        public int UnknownInt15;
        public int UnknownInt16;
        public int UnknownInt17;
        public int UnknownInt18;

        public int UnknownInt20;

        public List<string> TextureFlipbook = new List<string>();

        public void LoadMaterial(MaterialJsonHandler.MaterialsJson json, bool _skybox)
        {
            Skybox = _skybox;

            Name = json.MaterialName;

            TexturePath = json.TexturePath;
            UnknownInt2 = json.UnknownInt2;
            UnknownInt3 = json.UnknownInt3;

            UnknownFloat1 = json.UnknownFloat1;
            UnknownFloat2 = json.UnknownFloat2;
            UnknownFloat3 = json.UnknownFloat3;
            UnknownFloat4 = json.UnknownFloat4;

            UnknownInt8 = json.UnknownInt8;

            UnknownFloat5 = json.UnknownFloat5;
            UnknownFloat6 = json.UnknownFloat6;
            UnknownFloat7 = json.UnknownFloat7;
            UnknownFloat8 = json.UnknownFloat8;

            UnknownInt13 = json.UnknownInt13;
            UnknownInt14 = json.UnknownInt14;
            UnknownInt15 = json.UnknownInt15;
            UnknownInt16 = json.UnknownInt16;
            UnknownInt17 = json.UnknownInt17;
            UnknownInt18 = json.UnknownInt18;

            TextureFlipbook = json.TextureFlipbook;
            UnknownInt20 = json.UnknownInt20;

            GenerateMaterial();
        }

        public MaterialJsonHandler.MaterialsJson GenerateMaterialJson()
        {
            var NewJson = new MaterialJsonHandler.MaterialsJson();

            NewJson.MaterialName = Name;

            NewJson.TexturePath = TexturePath;
            NewJson.UnknownInt2 = UnknownInt2;
            NewJson.UnknownInt3 = UnknownInt3;

            NewJson.UnknownFloat1 = UnknownFloat1;
            NewJson.UnknownFloat2 = UnknownFloat2;
            NewJson.UnknownFloat3 = UnknownFloat3;
            NewJson.UnknownFloat4 = UnknownFloat4;

            NewJson.UnknownInt8 = UnknownInt8;

            NewJson.UnknownFloat5 = UnknownFloat5;
            NewJson.UnknownFloat6 = UnknownFloat6;
            NewJson.UnknownFloat7 = UnknownFloat7;
            NewJson.UnknownFloat8 = UnknownFloat8;

            NewJson.UnknownInt13 = UnknownInt13;
            NewJson.UnknownInt14 = UnknownInt14;
            NewJson.UnknownInt15 = UnknownInt15;
            NewJson.UnknownInt16 = UnknownInt16;
            NewJson.UnknownInt17 = UnknownInt17;
            NewJson.UnknownInt18 = UnknownInt18;

            NewJson.TextureFlipbook = TextureFlipbook;
            NewJson.UnknownInt20 = UnknownInt20;

            return NewJson;
        }

        public void GenerateMaterial()
        {
            Texture2D ReturnTexture = TrickyDataManager.ReturnTexture(TexturePath, Skybox);

            if (materialRef == null)
            {
                materialRef = new RayWarp.MaterialRef(Raylib.LoadMaterialDefault());
            }

            Raylib.SetMaterialTexture(ref materialRef.Material, MaterialMapIndex.Diffuse, ReturnTexture);

            if (!Skybox)
            {
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

        public static void Draw(IReadOnlyList<TrickyMaterialObject> materials)
        {
            InspectorManager.Text<TrickyMaterialObject>("Name", materials, p => p.Name, (p, v) => p.Name = v);

            InspectorManager.Text<TrickyMaterialObject>("Texture Path", materials, p => p.TexturePath, (p, v) => p.TexturePath = v);

            InspectorManager.Int<TrickyMaterialObject>("U2", materials, p => p.UnknownInt2, (p, v) => p.UnknownInt2 = v);
            InspectorManager.Int<TrickyMaterialObject>("U3", materials, p => p.UnknownInt3, (p, v) => p.UnknownInt3 = v);

            InspectorManager.Float<TrickyMaterialObject>("FU1", materials, p => p.UnknownFloat1, (p, v) => p.UnknownFloat1 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU2", materials, p => p.UnknownFloat2, (p, v) => p.UnknownFloat2 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU3", materials, p => p.UnknownFloat3, (p, v) => p.UnknownFloat3 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU4", materials, p => p.UnknownFloat4, (p, v) => p.UnknownFloat4 = v);

            InspectorManager.Int<TrickyMaterialObject>("U8", materials, p => p.UnknownInt8, (p, v) => p.UnknownInt8 = v);

            InspectorManager.Float<TrickyMaterialObject>("FU5", materials, p => p.UnknownFloat5, (p, v) => p.UnknownFloat5 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU6", materials, p => p.UnknownFloat6, (p, v) => p.UnknownFloat6 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU7", materials, p => p.UnknownFloat7, (p, v) => p.UnknownFloat7 = v);
            InspectorManager.Float<TrickyMaterialObject>("FU8", materials, p => p.UnknownFloat8, (p, v) => p.UnknownFloat8 = v);

            InspectorManager.Int<TrickyMaterialObject>("U13", materials, p => p.UnknownInt13, (p, v) => p.UnknownInt13 = v);
            InspectorManager.Int<TrickyMaterialObject>("U14", materials, p => p.UnknownInt14, (p, v) => p.UnknownInt14 = v);
            InspectorManager.Int<TrickyMaterialObject>("U15", materials, p => p.UnknownInt15, (p, v) => p.UnknownInt15 = v);
            InspectorManager.Int<TrickyMaterialObject>("U16", materials, p => p.UnknownInt16, (p, v) => p.UnknownInt16 = v);
            InspectorManager.Int<TrickyMaterialObject>("U17", materials, p => p.UnknownInt17, (p, v) => p.UnknownInt17 = v);
            InspectorManager.Int<TrickyMaterialObject>("U18", materials, p => p.UnknownInt18, (p, v) => p.UnknownInt18 = v);

            InspectorManager.Int<TrickyMaterialObject>("U20", materials, p => p.UnknownInt20, (p, v) => p.UnknownInt20 = v);

            //FlipBook list
        }
    }
}
