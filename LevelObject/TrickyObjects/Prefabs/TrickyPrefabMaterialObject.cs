using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMaterialObject : TrickyPrefabMaterialBase
    {
        public override ObjectType Type
        {
            get { return ObjectType.PrefabMesh; }
        }

        public Model Model;

        public void LoadPrefabMaterialObject(PrefabJsonHandler.MeshHeader objectHeader)
        {
            MeshPath = objectHeader.MeshPath;
            MaterialIndex = objectHeader.MaterialID;

            GenerateModel();
        }

        public void GenerateModel()
        {
            mesh = ObjImporter.ObjLoad(WorldManager.instance.LoadPath + "\\Models\\"+ MeshPath);

            var TexturePath = WorldManager.instance.trickyMaterialObject[MaterialIndex].TexturePath;

            Raylib.UploadMesh(ref mesh, false);
            Model = Raylib.LoadModelFromMesh(mesh);

            var Texture = WorldManager.instance.ReturnTexture(TexturePath);

            Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Diffuse, ref Texture);
        }

        public override void Render()
        {
            Raylib.DrawModelEx(Model, Vector3.Zero, Vector3.UnitX, 0, Vector3.One * 0.001f, Color.White);
        }
    }
}
