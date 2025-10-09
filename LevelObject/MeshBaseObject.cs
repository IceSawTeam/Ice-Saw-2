using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.LevelObject
{
    public class MeshBaseObject : BaseObject
    {
        public Material material;

        protected Mesh _mesh;
        public Mesh mesh
        {
            get
            {
                return _mesh;
            }

            set
            {
                _mesh = value;
                GenerateBBox(value);
            }
        }

        public override void Render()
        {
            if (Visable && Enabled)
            {
                if (mesh.VertexCount != 0)
                {
                    Raylib.DrawMesh(mesh, material, worldMatrix4x4);
                }
            }
        }

        public override void Render(Matrix4x4 matrix4X4)
        {
            if (Visable && Enabled)
            {
                if (mesh.VertexCount != 0)
                {
                    Raylib.DrawMesh(mesh, material, matrix4X4);
                }
            }
        }

        public void GenerateBBox(Mesh mesh)
        {
            // Convert mesh vertex data to a float array
            var vertices = mesh.VerticesAs<float>().ToArray();

            // Vertex count * 3 (since each vertex has x, y, z)
            int vertexCount = mesh.VertexCount;

            // Initialize min and max vectors with extreme values
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < vertexCount; i++)
            {
                float x = vertices[i * 3 + 0];
                float y = vertices[i * 3 + 1];
                float z = vertices[i * 3 + 2];

                Vector3 vertex = new Vector3(x, y, z);

                // Update min
                min.X = Math.Min(min.X, vertex.X);
                min.Y = Math.Min(min.Y, vertex.Y);
                min.Z = Math.Min(min.Z, vertex.Z);

                // Update max
                max.X = Math.Max(max.X, vertex.X);
                max.Y = Math.Max(max.Y, vertex.Y);
                max.Z = Math.Max(max.Z, vertex.Z);
            }

            boundingBox = new BoundingBox(min, max);
        }

    }
}
