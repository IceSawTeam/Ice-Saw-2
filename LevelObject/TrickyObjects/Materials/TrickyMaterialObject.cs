using IceSaw2.LevelObject;
using IceSaw2.Manager;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.LevelObject.Materials
{
    public class TrickyMaterialObject : BaseObject
    {
        bool Skybox = false;
        public override ObjectType Type
        {
            get { return ObjectType.Material; }
        }

        public string TexturePath = "";
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

            if (json.MaterialName != "" && json.MaterialName != null)
            {
                Name = json.MaterialName;
            }

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

            GenerateMesh();
        }

        public void GenerateMesh()
        {
            mesh = Raylib.GenMeshCube(2000, 1000, 2000);

            Texture2D ReturnTexture = DataManager.ReturnTexture(TexturePath, Skybox);

            material = Raylib.LoadMaterialDefault();

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, ReturnTexture);
        }


    }
}
