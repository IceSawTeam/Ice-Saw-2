using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Numerics;
using IceSaw2.Manager;

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
            //Have it pull mesh and material from trickyMaterial and mesh instead

            mesh = DataManager.ReturnMesh(MeshPath, false);

            var TexturePath = DataManager.trickyMaterialObject[MaterialIndex].TexturePath;

            Texture2D ReturnTexture = DataManager.ReturnTexture(TexturePath, false);

            material = Raylib.LoadMaterialDefault();

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, ReturnTexture);
        }
    }
}
