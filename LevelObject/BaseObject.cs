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
        public static float WorldScale = 0.001f;

        public string Name = "Null";

        private BaseObject _parent;
        public BaseObject parent
        {
            get
            { return _parent; }
            set
            {
                if(_parent!=null)
                {
                    _parent.children.Remove(this);
                }

                if(value!=null)
                {
                    value.children.Add(this);
                }

                _parent = value;

                UpdateMatrix(false);
            }
        }

        public List<BaseObject> children = new List<BaseObject>();

        private Vector3 _position = Vector3.Zero;
        public Vector3 Position
        {
            get
                { return _position; }
            set {
                _position = value;
                UpdateMatrix();
            }
        }
        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get
                { return _rotation; }
            set
            {
                _rotation = value;
                UpdateMatrix();
            }
        }
        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get
                { return _scale; }
            set
            {
                _scale = value;
                UpdateMatrix();
            }
        }
        public Vector3 EulerAngles
        {
            get
            {
                return QuaternionToEuler(Rotation);
            }
            set
            {
                Rotation = QuaternionFromEuler(EulerAngles.Z, EulerAngles.Y, EulerAngles.X);
            }
        }

        public Matrix4x4 localMatrix4X4
        {
            get; private set;
        }

        public Matrix4x4 worldMatrix4x4
        {
            get; private set;
            //get
            //{
            //    if (_parent != null)
            //    {
            //        return MatrixMultiply(localMatrix4X4, _parent.worldMatrix4x4);
            //    }
            //    else
            //    {
            //        return MatrixMultiply(localMatrix4X4, MatrixScale(WorldScale, WorldScale, WorldScale));
            //    }
            //}
        }

        public bool Visable = true;
        public bool Enabled = true;

        public Material material;
        public Mesh mesh;

        public virtual ObjectType Type
        {
            get { return ObjectType.None; }
        }

        public BaseObject()
        {
            UpdateMatrix();
        }

        public virtual void UpdateLogic()
        {

        }

        public virtual void Render()
        {
            if (Visable && Enabled)
            {
                if (mesh.VertexCount != 0)
                {
                    Raylib.DrawMesh(mesh, material, worldMatrix4x4);
                }
            }
        }

        void UpdateMatrix(bool UpdateLocal = true)
        {
            if (UpdateLocal)
            {
                Matrix4x4 scale = MatrixScale(_scale.X, _scale.Y, _scale.Z);
                Matrix4x4 rotation = QuaternionToMatrix(_rotation);
                Matrix4x4 TempMatrix4X4 = MatrixMultiply(scale, rotation);
                TempMatrix4X4 = MatrixMultiply(TempMatrix4X4, MatrixTranslate(_position.X, _position.Y, _position.Z));

                localMatrix4X4 = TempMatrix4X4;
            }

            //Check Parent
            if (_parent == null)
            {
                worldMatrix4x4 = MatrixMultiply(localMatrix4X4, MatrixScale(WorldScale, WorldScale, WorldScale));
            }
            else
            {
                worldMatrix4x4 = MatrixMultiply(localMatrix4X4, _parent.worldMatrix4x4);
            }

            //Update Children
            for (global::System.Int32 i = 0; i < children.Count; i++)
            {
                children[i].UpdateMatrix(false);
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
            Function,
            PathA,
            PathB
        }
    }
}
