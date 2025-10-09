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
                meshBoundingBox = Raylib.GetMeshBoundingBox(value);
                GenerateBBoxLocal();
                GenertateBBoxWorld();
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

    }
}
