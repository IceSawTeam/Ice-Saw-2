using ImGuiNET;
using Raylib_cs;
using System.Numerics;

namespace IceSaw2.LevelObject
{
    public class BaseObject
    {
        public int ID;
        private static int IDCount = 0;

        public static float WorldScale = 0.001f;

        public string Name = "Null";

        private BaseObject _parent = null;
        public BaseObject parent
        {
            get
            { return _parent; }
            set
            {
                if(_parent!=null)
                {
                    _parent.RemoveChild(this);
                }

                if(value!=null)
                {
                    value.AddChild(this);
                }

                _parent = value;

                UpdateMatrix(false);
            }
        }

        private List<BaseObject> _children = new List<BaseObject>();

        public IReadOnlyList<BaseObject> Children => _children.AsReadOnly();

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
                return Raymath.QuaternionToEuler(Rotation);
            }
            set
            {
                Rotation = Raymath.QuaternionFromEuler(value.Z, value.Y, value.X);
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
        public bool VisableHierarchy = true;


        public virtual ObjectType Type
        {
            get { return ObjectType.None; }
        }

        public BaseObject()
        {
            ID = IDCount;
            IDCount++;
            UpdateMatrix();
        }

        public virtual void UpdateLogic()
        {

        }

        public virtual void Render()
        {

        }

        public virtual void Render(Matrix4x4 matrix4X4)
        {

        }

        public void AddChild(BaseObject baseObject)
        {
            if (baseObject.parent!=this)
            {
                baseObject.parent = this;
            }
            else
            {
                _children.Add(baseObject);
            }
        }

        public void RemoveChild(BaseObject baseObject)
        {
            if (_children.Contains(baseObject))
            {
                if(parent==baseObject)
                {
                    parent = null;
                }
                else
                {
                    _children.Remove(baseObject);
                }
            }
        }

        private void UpdateMatrix(bool UpdateLocal = true)
        {
            if (UpdateLocal)
            {
                Matrix4x4 scale = Raymath.MatrixScale(_scale.X, _scale.Y, _scale.Z);
                Matrix4x4 rotation = Raymath.QuaternionToMatrix(_rotation);
                Matrix4x4 TempMatrix4X4 = Raymath.MatrixMultiply(scale, rotation);
                TempMatrix4X4.M14 = _position.X;
                TempMatrix4X4.M24 = _position.Y;
                TempMatrix4X4.M34 = _position.Z;

                //TempMatrix4X4 = Raymath.MatrixMultiply(TempMatrix4X4, Raymath.MatrixTranslate(_position.X, _position.Y, _position.Z));

                localMatrix4X4 = TempMatrix4X4;
            }

            //Check Parent
            if (_parent == null)
            {
                worldMatrix4x4 = Raymath.MatrixMultiply(localMatrix4X4, Raymath.MatrixScale(WorldScale, WorldScale, WorldScale));
            }
            else
            {
                worldMatrix4x4 = Raymath.MatrixMultiply(localMatrix4X4, _parent.worldMatrix4x4);
            }

            //Update Children
            for (global::System.Int32 i = 0; i < Children.Count; i++)
            {
                Children[i].UpdateMatrix(false);
            }
        }

        public void HierarchyRender()
        {
            if (VisableHierarchy)
            {
                var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

                if (Children.Count == 0)
                    flags |= ImGuiTreeNodeFlags.Leaf;

                bool nodeOpen = ImGui.TreeNodeEx(Name + "###" + ID, flags);

                // Handle selection or context menu if needed
                if (ImGui.IsItemClicked())
                {
                    Console.WriteLine($"Selected: " + Name + "###" + ID);
                }

                if (nodeOpen)
                {
                    for (global::System.Int32 i = 0; i < Children.Count; i++)
                    {
                        Children[i].HierarchyRender();
                    }
                    ImGui.TreePop();
                }
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

        public struct RenderCache
        {
            public BaseObject baseObject;
            public Matrix4x4 WorldMatrix;
        }
    }
}
