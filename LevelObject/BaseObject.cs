using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Raylib_cs.Raymath;

namespace IceSaw2.LevelObject
{
    public class BaseObject
    {
        public string Name = "Null";

        public BaseObject parent;

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        public Matrix4x4 matrix4X4 
        { 
            get
            {
                Matrix4x4 scale = MatrixScale(Scale.X, Scale.Y, Scale.Z);
                Matrix4x4 rotation = QuaternionToMatrix(Rotation);
                Matrix4x4 matrix4X4 = MatrixMultiply(scale, rotation);
                matrix4X4 = MatrixMultiply(matrix4X4, MatrixTranslate(Position.X, Position.Y, Position.Z));

                if (parent != null)
                {
                    matrix4X4 = MatrixMultiply(matrix4X4, parent.matrix4X4);
                }
                else
                {
                    matrix4X4 = MatrixMultiply(matrix4X4, MatrixScale(0.01f, 0.01f, 0.01f));
                }

                return matrix4X4;
            }
        }

        public Material material;
        public Mesh mesh;
        public virtual ObjectType Type
        {
            get { return ObjectType.None; }
        }

        public virtual void UpdateLogic()
        {

        }

        public virtual void Render()
        {
            if(mesh.VertexCount!=0)
            {
                Raylib.DrawMesh(mesh, material, matrix4X4);
            }
        }

        public enum ObjectType
        {
            None,
            Patch,
            Instance,
            Light,
            Particle,
            Physics,
            Camera,
            EffectSlot,
            Material,
            Spline,
            Prefab,
            PrefabSub,
            PrefabMesh,
            ParticlePrefab,
            SkyboxMaterial,
            SkyboxPrefab,
            SkyboxPrefabSub,
            SkyboxPrefabMesh,
            PathManager,
            Effect,
            Function
        }
    }
}
