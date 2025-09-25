using System.Collections;
using System.Collections.Generic;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using IceSaw2.LevelObject;
using System.Numerics;
using Raylib_cs;
using IceSaw2.Manager;

public class TrickyLightObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.Light; }
    }

    public LightType lightType;
    public int SpriteRes;
    public float UnknownFloat1;
    public int UnknownInt1;
    public Vector3 Colour;
    public Vector3 LowestXYZ;
    public Vector3 HighestXYZ;
    public float UnknownFloat2;
    public int UnknownInt2;
    public float UnknownFloat3;
    public int UnknownInt3;

    public int Hash;

    public void LoadLight(LightJsonHandler.LightJson lightJson)
    {
        Name = lightJson.LightName;

        lightType = (LightType)lightJson.Type;
        SpriteRes = lightJson.SpriteRes;
        UnknownFloat1 = lightJson.UnknownFloat1;
        UnknownInt1 = lightJson.UnknownInt1;
        Colour = JsonUtil.ArrayToVector3(lightJson.Colour);

        LowestXYZ = JsonUtil.ArrayToVector3(lightJson.LowestXYZ);
        HighestXYZ = JsonUtil.ArrayToVector3(lightJson.HighestXYZ);
        UnknownFloat2 = lightJson.UnknownFloat2;
        UnknownInt2 = lightJson.UnknownInt2;
        UnknownFloat3 = lightJson.UnknownFloat3;
        UnknownInt3 = lightJson.UnknownInt3;
        Hash = lightJson.Hash;

        Position = JsonUtil.ArrayToVector3(lightJson.Postion);
        Rotation = Raymath.QuaternionFromVector3ToVector3(JsonUtil.ArrayToVector3(lightJson.Direction), Vector3.UnitX);
    }

    public LightJsonHandler.LightJson GenerateLight()
    {
        var NewLight = new LightJsonHandler.LightJson();

        NewLight.LightName = Name;
        NewLight.Postion = JsonUtil.Vector3ToArray(Position);
        NewLight.Type = (int)lightType;
        NewLight.SpriteRes = SpriteRes;
        NewLight.UnknownFloat1 = UnknownFloat1;
        NewLight.UnknownInt1 = UnknownInt1;
        NewLight.Colour = JsonUtil.Vector3ToArray(Colour);
        NewLight.Direction = JsonUtil.Vector3ToArray(Raymath.Vector3Normalize(Raymath.Vector3Transform(Vector3.UnitX, localMatrix4X4)));
        NewLight.LowestXYZ = JsonUtil.Vector3ToArray(LowestXYZ);
        NewLight.HighestXYZ = JsonUtil.Vector3ToArray(HighestXYZ);
        NewLight.UnknownFloat2 = UnknownFloat2;
        NewLight.UnknownInt2 = UnknownInt2;
        NewLight.UnknownFloat3 = UnknownFloat3;
        NewLight.UnknownInt3 = UnknownInt3;
        NewLight.Hash = Hash;

        return NewLight;
    }

    public override void Render()
    {
        Rectangle sourceRec = new Rectangle(0, 0, WorldManager.instance.LightIcon.Width, WorldManager.instance.LightIcon.Height);
        Vector2 size = new Vector2(1.0f, (float)WorldManager.instance.LightIcon.Height / WorldManager.instance.LightIcon.Width); // maintain aspect
        Vector2 origin = new Vector2(size.X / 2, size.Y / 2);
        Raylib.DrawBillboardPro(WorldManager.instance.levelEditorWindow.viewCamera3D, WorldManager.instance.LightIcon, sourceRec, Position * WorldScale, new Vector3(0,0,1) , size, origin,0f, Color.White);
    }

    public enum LightType
    {
        Directional,
        U0,
        U1,
        Ambient,
    }
}
