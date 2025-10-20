using IceSaw2.RayWarp;
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
        public MaterialRef materialRef;

        protected MeshRef _meshref;
        public MeshRef meshRef
        {
            get
            {
                return _meshref;
            }

            set
            {
                _meshref = value;
                meshBoundingBox = Raylib.GetMeshBoundingBox(value.Mesh);
                GenerateBBoxLocal();
                GenertateBBoxWorld();
            }
        }

        public override void Render()
        {
            if (Visible && Enabled)
            {
                if (meshRef.Mesh.VertexCount != 0)
                {
                    Raylib.DrawMesh(meshRef.Mesh, materialRef.Material, worldMatrix4x4);
                }
            }
        }

        public override void Render(Matrix4x4 matrix4X4)
        {
            if (Visible && Enabled)
            {
                if (meshRef.Mesh.VertexCount != 0)
                {
                    Raylib.DrawMesh(meshRef.Mesh, materialRef.Material, matrix4X4);
                }
            }
        }

    }
}
