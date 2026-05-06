using IceSaw2.LevelObject;
using IceSaw2.Manager.Tricky;
using Raylib_cs;
using SSXLibrary.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
public class TrickyPaticleInstanceObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.Particle; }
    }

    public int UnknownInt1;
    public float[] LowestXYZ;
    public float[] HighestXYZ;
    public int UnknownInt8;
    public int UnknownInt9;
    public int UnknownInt10;
    public int UnknownInt11;
    public int UnknownInt12;
    public void LoadPaticleInstance (ParticleInstanceJsonHandler.ParticleJson instanceJsonHandler)
    {
        Name = instanceJsonHandler.ParticleName;

        Position = JsonUtil.ArrayToVector3(instanceJsonHandler.Location);
        Rotation = JsonUtil.ArrayToQuaternion(instanceJsonHandler.Rotation);
        Scale = JsonUtil.ArrayToVector3(instanceJsonHandler.Scale);

        UnknownInt1 = instanceJsonHandler.UnknownInt1;
        LowestXYZ = instanceJsonHandler.LowestXYZ;
        HighestXYZ = instanceJsonHandler.HighestXYZ;
        UnknownInt8 = instanceJsonHandler.UnknownInt8;
        UnknownInt9 = instanceJsonHandler.UnknownInt9;
        UnknownInt10 = instanceJsonHandler.UnknownInt10;
        UnknownInt11 = instanceJsonHandler.UnknownInt11;
        UnknownInt12 = instanceJsonHandler.UnknownInt12;
    }

    public ParticleInstanceJsonHandler.ParticleJson GenerateParticleInstance()
    {
        ParticleInstanceJsonHandler.ParticleJson particleJson = new ParticleInstanceJsonHandler.ParticleJson();

        particleJson.ParticleName = Name;
        particleJson.Location = JsonUtil.Vector3ToArray(Position);
        particleJson.Rotation = JsonUtil.QuaternionToArray(Rotation);
        particleJson.Scale = JsonUtil.Vector3ToArray(Scale);

        particleJson.UnknownInt1 = UnknownInt1;
        particleJson.LowestXYZ = LowestXYZ;
        particleJson.HighestXYZ = HighestXYZ;
        particleJson.UnknownInt8 = UnknownInt8;
        particleJson.UnknownInt9 = UnknownInt9;
        particleJson.UnknownInt10 = UnknownInt10;
        particleJson.UnknownInt11 = UnknownInt11;
        particleJson.UnknownInt12 = UnknownInt12;

        return particleJson;
    }

    public override void Render()
    {
        var worldManInstance = TrickyWorldManager.instance;
        Texture2D particleIcon = worldManInstance.ParticleIcon;

        Rectangle sourceRec = new Rectangle(0, 0, particleIcon.Width, particleIcon.Height);
        Vector2 size = new Vector2(1.0f, (float)particleIcon.Height / particleIcon.Width); // maintain aspect
        Vector2 origin = new Vector2(size.X / 2, size.Y / 2);
        Raylib.DrawBillboardPro(worldManInstance.levelEditorWindow.viewCamera3D, particleIcon, sourceRec, Position * WorldScale, worldManInstance.levelEditorWindow.viewCamera3D.Up, size, origin, 0f, Color.White);
    }
}
