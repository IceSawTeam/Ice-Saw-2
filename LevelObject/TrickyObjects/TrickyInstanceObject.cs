using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.LevelObject
{
    public class TrickyInstanceObject : BaseObject
    {
        public override ObjectType Type
        {
            get { return ObjectType.Instance; }
        }

        int ModelID;
        //public TrickyPrefabObject PrefabObject;

        int PrevInstance; //Next Connected Model 
        int NextInstance; //Prev Connected Model

        //public TrickyInstanceObject PrevInstanceObject;
        //public TrickyInstanceObject NextInstanceObject;

        public int UnknownInt26;
        public int UnknownInt27;
        public int UnknownInt28;
        public int UnknownInt30;
        public int UnknownInt31;
        public int UnknownInt32;

        public int LTGState;

        public int Hash;
        public bool IncludeSound;
        public SoundData Sounds;

        //Object Properties
        public float U0;
        public float PlayerBounceAmmount;
        public int U2;
        public bool Visable;
        public bool PlayerCollision;
        public bool PlayerBounce;
        public bool Unknown241;
        public bool UVScroll;
        public InstanceSurfaceType SurfaceType;

        public int CollsionMode;
        public string[] CollsionModelPaths;

        int EffectSlotIndex;
        //public EffectSlotObject EffectSlotObject;

        int PhysicsIndex;
        //public PhysicsObject PhysicsObject;

        public int U8;

        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightVector1;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightVector2;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightVector3;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 AmbentLightVector;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightColour1;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightColour2;
        //[OnChangedCall("SetLightingColour")]
        public Vector3 LightColour3;
        //[OnChangedCall("SetLightingColour")]
        public Vector4 AmbentLightColour;

        public TrickyPrefabObject TrickyPrefab;

        public void LoadInstance(InstanceJsonHandler.InstanceJson instance)
        {
            Name = instance.InstanceName;

            Rotation = JsonUtil.ArrayToQuaternion(instance.Rotation);
            Scale = JsonUtil.ArrayToVector3(instance.Scale);
            Position = JsonUtil.ArrayToVector3(instance.Location);

            LightVector1 = JsonUtil.ArrayToVector3(instance.LightVector1);
            LightVector2 = JsonUtil.ArrayToVector3(instance.LightVector2);
            LightVector3 = JsonUtil.ArrayToVector3(instance.LightVector3);
            AmbentLightVector = JsonUtil.ArrayToVector3(instance.AmbentLightVector);

            LightColour1 = JsonUtil.ArrayToVector3(instance.LightColour1);
            LightColour2 = JsonUtil.ArrayToVector3(instance.LightColour2);
            LightColour3 = JsonUtil.ArrayToVector3(instance.LightColour3);
            AmbentLightColour = JsonUtil.ArrayToVector4(instance.AmbentLightColour);

            ModelID = instance.ModelID;
            PrevInstance = instance.PrevInstance;
            NextInstance = instance.NextInstance;

            UnknownInt26 = instance.UnknownInt26;
            UnknownInt27 = instance.UnknownInt27;
            UnknownInt28 = instance.UnknownInt28;
            UnknownInt30 = instance.UnknownInt30;
            UnknownInt31 = instance.UnknownInt31;
            UnknownInt32 = instance.UnknownInt32;

            LTGState = instance.LTGState;

            Hash = instance.Hash;
            IncludeSound = instance.IncludeSound;

            if (IncludeSound)
            {
                SoundData soundData = new SoundData();
                var TempSound = instance.Sounds.Value;
                soundData.CollisonSound = TempSound.CollisonSound;

                soundData.ExternalSounds = new List<ExternalSound>();
                for (int i = 0; i < TempSound.ExternalSounds.Count; i++)
                {
                    var NewTempSound = new ExternalSound();
                    NewTempSound.U0 = TempSound.ExternalSounds[i].U0;
                    NewTempSound.SoundIndex = TempSound.ExternalSounds[i].SoundIndex;
                    NewTempSound.U2 = TempSound.ExternalSounds[i].U2;
                    NewTempSound.U3 = TempSound.ExternalSounds[i].U3;
                    NewTempSound.U4 = TempSound.ExternalSounds[i].U4;
                    NewTempSound.U5 = TempSound.ExternalSounds[i].U5;
                    NewTempSound.U6 = TempSound.ExternalSounds[i].U6;
                    soundData.ExternalSounds.Add(NewTempSound);
                }

                Sounds = soundData;
            }

            U0 = instance.U0;
            PlayerBounceAmmount = instance.PlayerBounceAmmount;
            U2 = instance.U2;
            Visable = instance.Visable;
            PlayerCollision = instance.PlayerCollision;
            PlayerBounce = instance.PlayerBounce;
            Unknown241 = instance.Unknown241;
            UVScroll = instance.UVScroll;

            SurfaceType = (InstanceSurfaceType)(instance.SurfaceType + 1);
            CollsionMode = instance.CollsionMode;
            CollsionModelPaths = instance.CollsionModelPaths;
            EffectSlotIndex = instance.EffectSlotIndex;
            PhysicsIndex = instance.PhysicsIndex;
            U8 = instance.U8;

            LoadPrefab();
        }

        public void LoadPrefab()
        {
            if (ModelID != -1)
            {
                TrickyPrefab = DataManager.trickyPrefabObjects[ModelID];
                GenerateRenderCache();
            }
        }

        List<RenderCache> renderCaches = new List<RenderCache>();

        public void GenerateRenderCache()
        {
            renderCaches = new List<RenderCache>();
            if (TrickyPrefab != null)
            {
                TrickyPrefab.parent = this;
                renderCaches = TrickyPrefab.GenerateRenderCache();
                TrickyPrefab.parent = null;
            }
        }
        
        public override void Render()
        {
            //if (TrickyPrefab != null)
            //{
            //    TrickyPrefab.parent = this;
            //    TrickyPrefab.Render();
            //    TrickyPrefab.parent = null;
            //}
            for (int i = 0; i < renderCaches.Count; i++)
            {
                renderCaches[i].baseObject.Render(renderCaches[i].WorldMatrix);
            }
        }

        [System.Serializable]
        public struct SoundData
        {
            public int CollisonSound;
            public List<ExternalSound> ExternalSounds;
        }
        [System.Serializable]
        public struct ExternalSound
        {
            public int U0;
            public int SoundIndex;
            public float U2;
            public float U3;
            public float U4;
            public float U5; //Radius?
            public float U6;
        }
        [System.Serializable]
        public enum InstanceSurfaceType
        {
            None,
            Reset,
            StandardSnow,
            StandardOffTrack,
            PoweredSnow,
            SlowPoweredSnow,
            IceStandard,
            BounceUnskiable,
            IceWaterNoTrail,
            GlidyPoweredSnow,
            Rock,
            Wall,
            IceNoTrail,
            SmallParticleWake,
            OffTrackMetal,
            MetalGliding,
            Standard1,
            StandardSand,
            NoCollision,
            ShowOffRampMetal
        }
    }
}
