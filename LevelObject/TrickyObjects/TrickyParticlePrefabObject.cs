using IceSaw2.LevelObject;
using IceSaw2.Manager;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class TrickyParticlePrefabObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.ParticlePrefab; }
    }

    public List<ParticleObject> ParticleObjects;

    public void LoadParticle(ParticleModelJsonHandler.ParticleModelJson particleModel)
    {
        Name = particleModel.ParticleModelName;
        ParticleObjects = new List<ParticleObject>();
        for (int i = 0; i < particleModel.ParticleObjectHeaders.Count; i++)
        {
            var NewHeader = new ParticleObject();

            NewHeader.LowestXYZ = JsonUtil.ArrayToVector3(particleModel.ParticleObjectHeaders[i].ParticleObject.LowestXYZ);
            NewHeader.HighestXYZ = JsonUtil.ArrayToVector3(particleModel.ParticleObjectHeaders[i].ParticleObject.HighestXYZ);
            NewHeader.U1 = particleModel.ParticleObjectHeaders[i].ParticleObject.U1;

            NewHeader.AnimationFrames = new List<AnimationFrames>();

            for (int a = 0; a < particleModel.ParticleObjectHeaders[i].ParticleObject.AnimationFrames.Count; a++)
            {
                var NewAnimation = new AnimationFrames();

                NewAnimation.Position = JsonUtil.ArrayToVector3(particleModel.ParticleObjectHeaders[i].ParticleObject.AnimationFrames[a].Position);
                NewAnimation.Rotation = JsonUtil.ArrayToVector3(particleModel.ParticleObjectHeaders[i].ParticleObject.AnimationFrames[a].Rotation);
                NewAnimation.Unknown = particleModel.ParticleObjectHeaders[i].ParticleObject.AnimationFrames[a].Unknown;
                NewHeader.AnimationFrames.Add(NewAnimation);
            }


            ParticleObjects.Add(NewHeader);
        }
    
    }

    public ParticleModelJsonHandler.ParticleModelJson GenerateParticle()
    {
        ParticleModelJsonHandler.ParticleModelJson jsonHandler = new ParticleModelJsonHandler.ParticleModelJson();

        jsonHandler.ParticleModelName = Name;

        jsonHandler.ParticleObjectHeaders = new List<ParticleModelJsonHandler.ParticleObjectHeader>();

        for (int i = 0; i < ParticleObjects.Count; i++)
        {
            var TempParticle = new ParticleModelJsonHandler.ParticleObjectHeader();

            TempParticle.ParticleObject = new ParticleModelJsonHandler.ParticleObject();

            TempParticle.ParticleObject.LowestXYZ = JsonUtil.Vector3ToArray(ParticleObjects[i].LowestXYZ);
            TempParticle.ParticleObject.HighestXYZ = JsonUtil.Vector3ToArray(ParticleObjects[i].HighestXYZ);
            TempParticle.ParticleObject.U1 = ParticleObjects[i].U1;

            TempParticle.ParticleObject.AnimationFrames = new List<ParticleModelJsonHandler.AnimationFrames>();

            for (int a = 0; a < ParticleObjects[i].AnimationFrames.Count; a++)
            {
                var NewAnimationFrame = new ParticleModelJsonHandler.AnimationFrames();

                NewAnimationFrame.Rotation = JsonUtil.Vector3ToArray(ParticleObjects[i].AnimationFrames[a].Rotation);
                NewAnimationFrame.Position = JsonUtil.Vector3ToArray(ParticleObjects[i].AnimationFrames[a].Position);
                NewAnimationFrame.Unknown = ParticleObjects[i].AnimationFrames[a].Unknown;

                TempParticle.ParticleObject.AnimationFrames.Add(NewAnimationFrame);
            }
            jsonHandler.ParticleObjectHeaders.Add(TempParticle);
        }

        return jsonHandler;
    }

    public override void Render()
    {
        Rectangle sourceRec = new Rectangle(0, 0, WorldManager.instance.ParticleIcon.Width, WorldManager.instance.ParticleIcon.Height);
        Vector2 size = new Vector2(1.0f, (float)WorldManager.instance.ParticleIcon.Height / WorldManager.instance.ParticleIcon.Width); // maintain aspect
        Vector2 origin = new Vector2(size.X / 2, size.Y / 2);
        Raylib.DrawBillboardPro(WorldManager.instance.levelEditorWindow.viewCamera3D, WorldManager.instance.ParticleIcon, sourceRec, Position * WorldScale, new Vector3(0, 0, 1), size, origin, 0f, Color.White);
    }

    [System.Serializable]
    public struct ParticleObject
    {
        public Vector3 LowestXYZ;
        public Vector3 HighestXYZ;
        public int U1;

        public List<AnimationFrames> AnimationFrames;
    }

    [System.Serializable]
    public struct AnimationFrames
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public float Unknown;
    }
}
