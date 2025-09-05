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

        public TrickyPrefabMeshObject parent = null;

        public Model Model;

        public Material material;

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

            if (parent.IncludeMatrix)
            {
                //Matrix4x4 scale = Matrix4x4.CreateScale(parent.Scale);
                //Matrix4x4 Rotation = Matrix4x4.CreateFromQuaternion(parent.Rotation);
                //Matrix4x4 matrix4X4 = Matrix4x4.Multiply(scale, Rotation);
                //Model.Transform.Translation = parent.Position;

                //Model.Transform = matrix4X4;
            }

            material = Raylib.LoadMaterialDefault();

            var Texture = WorldManager.instance.ReturnTexture(TexturePath);

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, Texture);
        }

        public override void Render()
        {
            Matrix4x4 scale = MatrixScale(parent.Scale.X, parent.Scale.Y, parent.Scale.Z);
            Matrix4x4 Rotation = QuaternionToMatrix(parent.Rotation);
            Matrix4x4 matrix4X4 = MatrixMultiply(scale, Rotation);
            matrix4X4 = MatrixMultiply(matrix4X4, MatrixTranslate(parent.Position.X, parent.Position.Y, parent.Position.Z));

            matrix4X4 = MatrixMultiply(matrix4X4, MatrixScale(0.01f, 0.01f, 0.01f));

            Raylib.DrawMesh(mesh, material , matrix4X4);
        }
    }
}
