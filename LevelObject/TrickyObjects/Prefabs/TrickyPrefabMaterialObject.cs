using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Numerics;
using static Raylib_cs.Raymath;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMaterialObject : TrickyPrefabMaterialBase
    {
        public override ObjectType Type
        {
            get { return ObjectType.PrefabMesh; }
        }

        public void LoadPrefabMaterialObject(PrefabJsonHandler.MeshHeader objectHeader)
        {
            MeshPath = objectHeader.MeshPath;
            MaterialIndex = objectHeader.MaterialID;

            GenerateModel();
        }

        public void GenerateModel()
        {
            mesh = ObjImporter.ObjLoad(WorldManager.instance.LoadPath + "\\Models\\"+ MeshPath);

            Raylib.UploadMesh(ref mesh, false);

            var TexturePath = WorldManager.instance.trickyMaterialObject[MaterialIndex].TexturePath;

            Texture2D ReturnTexture = WorldManager.instance.ReturnTexture(TexturePath);

            material = Raylib.LoadMaterialDefault();

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, ReturnTexture);
        }
    }
}
