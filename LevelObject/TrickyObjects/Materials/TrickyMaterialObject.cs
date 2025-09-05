using IceSaw2.LevelObject;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.LevelObject.Materials
{
    public class TrickyMaterialObject : BaseObject
    {
        public override ObjectType Type
        {
            get { return ObjectType.Material; }
        }

        public string TexturePath;
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

        public List<string> TextureFlipbook;

        public Model Model;

        public void LoadMaterial(MaterialJsonHandler.MaterialsJson json)
        {
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
            Model = Raylib.LoadModelFromMesh(Raylib.GenMeshCube(2, 1, 2));

            Texture2D ReturnTexture = WorldManager.instance.ReturnTexture(TexturePath);

            if (TextureFlipbook.Count != 0)
            {
                ReturnTexture = WorldManager.instance.ReturnTexture(TextureFlipbook[0]);
            }

            Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Diffuse, ref ReturnTexture);
        }

        public override void Render()
        {


            Raylib.DrawModelEx(Model, Vector3.Zero, Vector3.UnitX, 0, Vector3.One * 1f, Color.White);
        }

        static void DrawCubeTexture(
        Texture2D texture,
        Vector3 position,
        float width,
        float height,
        float length,
        Color color
    )
        {
            float x = position.X;
            float y = position.Y;
            float z = position.Z;

            // Set desired texture to be enabled while drawing following vertex data
            Rlgl.SetTexture(texture.Id);

            // Vertex data transformation can be defined with the commented lines,
            // but in this example we calculate the transformed vertex data directly when calling Rlgl.Vertex3f()
            // Rlgl.PushMatrix();
            // NOTE: Transformation is applied in inverse order (scale -> rotate -> translate)
            // Rlgl.Translatef(2.0f, 0.0f, 0.0f);
            // Rlgl.Rotatef(45, 0, 1, 0);
            // Rlgl.Scalef(2.0f, 2.0f, 2.0f);

            Rlgl.Begin(DrawMode.Quads);
            Rlgl.Color4ub(color.R, color.G, color.B, color.A);

            // Front Face
            // Normal Pointing Towards Viewer
            Rlgl.Normal3f(0.0f, 0.0f, 1.0f);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);

            // Back Face
            // Normal Pointing Away From Viewer
            Rlgl.Normal3f(0.0f, 0.0f, -1.0f);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);

            // Top Face
            // Normal Pointing Up
            Rlgl.Normal3f(0.0f, 1.0f, 0.0f);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);

            // Bottom Face
            // Normal Pointing Down
            Rlgl.Normal3f(0.0f, -1.0f, 0.0f);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);

            // Right face
            // Normal Pointing Right
            Rlgl.Normal3f(1.0f, 0.0f, 0.0f);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z - length / 2);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z - length / 2);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y + height / 2, z + length / 2);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x + width / 2, y - height / 2, z + length / 2);

            // Left Face
            // Normal Pointing Left
            Rlgl.Normal3f(-1.0f, 0.0f, 0.0f);
            Rlgl.TexCoord2f(0.0f, 0.0f);
            // Bottom Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z - length / 2);
            Rlgl.TexCoord2f(1.0f, 0.0f);
            // Bottom Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y - height / 2, z + length / 2);
            Rlgl.TexCoord2f(1.0f, 1.0f);
            // Top Right Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z + length / 2);
            Rlgl.TexCoord2f(0.0f, 1.0f);
            // Top Left Of The Texture and Quad
            Rlgl.Vertex3f(x - width / 2, y + height / 2, z - length / 2);
            Rlgl.End();
            //rlPopMatrix();

            Rlgl.SetTexture(0);
        }

    }
}
