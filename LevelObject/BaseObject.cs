using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.LevelObject
{
    public class BaseObject
    {
        public string Name = "Null";

        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        public virtual ObjectType Type
        {
            get { return ObjectType.None; }
        }

        public virtual void UpdateLogic()
        {

        }

        public virtual void Render()
        {

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
