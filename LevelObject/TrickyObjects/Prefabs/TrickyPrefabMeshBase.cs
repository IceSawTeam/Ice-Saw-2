using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPrefabMeshBase : BaseObject
    {
        public int ParentID;
        public int Flags;

        public ObjectAnimation Animation = new ObjectAnimation();

        public bool IncludeAnimation;
        public bool IncludeMatrix;

        [Serializable]
        public struct ObjectAnimation
        {
            public float U1;
            public float U2;
            public float U3;
            public float U4;
            public float U5;
            public float U6;

            public int AnimationAction;
            public List<AnimationEntry> AnimationEntries;
        }
        [Serializable]
        public struct AnimationEntry
        {
            public List<AnimationMath> AnimationMaths;
        }
        [Serializable]
        public struct AnimationMath
        {
            public float Value1;
            public float Value2;
            public float Value3;
            public float Value4;
            public float Value5;
            public float Value6;
        }
    }
}
