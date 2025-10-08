using IceSaw2.LevelObject;
using IceSaw2.Manager.Tricky;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class TrickyCameraObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.Camera; }
    }

    public int CameraType;
    public float FocalLength;
    public float AspectRatio;
    public float[]? Aperture;
    public float[]? ClipPlane;
    public float[]? IntrestPoint;
    public float[]? UpVector;
    public float AnimTime;

    public float[]? InitialPosition;
    public float[]? InitalRotation;
    public float U0; //Big ?
    public List<CameraAnimationHeader> AnimationHeaders = [];

    public int Hash;


    public void LoadCamera(CameraJSONHandler.CameraInstance cameraInstance)
    {
        Name = cameraInstance.CameraName;
        Position = JsonUtil.ArrayToVector3(cameraInstance.Translation);
        EulerAngles = JsonUtil.ArrayToVector3(cameraInstance.Rotation);

        CameraType = cameraInstance.Type;
        FocalLength = cameraInstance.FocalLength;
        AspectRatio = cameraInstance.AspectRatio;
        Aperture = cameraInstance.Aperture;
        ClipPlane = cameraInstance.ClipPlane;
        IntrestPoint = cameraInstance.IntrestPoint;
        UpVector = cameraInstance.UpVector;
        AnimTime = cameraInstance.AnimTime;

        InitialPosition = cameraInstance.InitialPosition;
        InitalRotation = cameraInstance.InitalRotation;
        U0 = cameraInstance.U0;

        AnimationHeaders = new List<CameraAnimationHeader>();

        for (int i = 0; i < cameraInstance.AnimationHeaders.Count; i++)
        {
            var NewHeader = new CameraAnimationHeader();

            NewHeader.Action = cameraInstance.AnimationHeaders[i].Action;
            NewHeader.AnimationDatas = new List<CameraAnimationData>();

            for (int a = 0; a < cameraInstance.AnimationHeaders[i].AnimationDatas.Count; a++)
            {
                var NewAnimationData = new CameraAnimationData();

                NewAnimationData.Translation = cameraInstance.AnimationHeaders[i].AnimationDatas[a].Translation;

                NewAnimationData.Rotation = cameraInstance.AnimationHeaders[i].AnimationDatas[a].Rotation;

                NewHeader.AnimationDatas.Add(NewAnimationData);
            }

            AnimationHeaders.Add(NewHeader);
        }


        Hash = cameraInstance.Hash;

    }

    public CameraJSONHandler.CameraInstance GenerateCamera()
    {
        if (Aperture == null) return new CameraJSONHandler.CameraInstance();
        if (ClipPlane == null) return new CameraJSONHandler.CameraInstance();
        if (IntrestPoint == null) return new CameraJSONHandler.CameraInstance();
        if (UpVector == null) return new CameraJSONHandler.CameraInstance();
        if (InitialPosition == null) return new CameraJSONHandler.CameraInstance();
        if (InitalRotation == null) return new CameraJSONHandler.CameraInstance();

        CameraJSONHandler.CameraInstance cameraInstance = new CameraJSONHandler.CameraInstance
        {
            CameraName = Name,
            Translation = JsonUtil.Vector3ToArray(Position),
            Rotation = JsonUtil.Vector3ToArray(EulerAngles),
            Type = CameraType,
            FocalLength = FocalLength,
            AspectRatio = AspectRatio,
            Aperture = Aperture,
            ClipPlane = ClipPlane,
            IntrestPoint = IntrestPoint,
            UpVector = UpVector,
            AnimTime = AnimTime,

            InitialPosition = InitialPosition,
            InitalRotation = InitalRotation,
            U0 = U0,

            AnimationHeaders = new List<CameraJSONHandler.CameraAnimationHeader>()
        };

        for (int i = 0; i < AnimationHeaders.Count; i++)
        {
            var TempHeader = new CameraJSONHandler.CameraAnimationHeader();

            TempHeader.Action = AnimationHeaders[i].Action;
            TempHeader.AnimationDatas = new List<CameraJSONHandler.CameraAnimationData>();

            for (int a = 0; a < AnimationHeaders[i].AnimationDatas.Count; a++)
            {
                var TempAnimData = new CameraJSONHandler.CameraAnimationData();

                TempAnimData.Translation = AnimationHeaders[i].AnimationDatas[a].Translation;
                TempAnimData.Rotation = AnimationHeaders[i].AnimationDatas[a].Rotation;

                TempHeader.AnimationDatas.Add(TempAnimData);

            }

            cameraInstance.AnimationHeaders.Add(TempHeader);
        }

        cameraInstance.Hash = Hash;

        return cameraInstance;

    }

    public override void Render()
    {
        Rectangle sourceRec = new Rectangle(0, 0, TrickyWorldManager.instance.CameraIcon.Width, TrickyWorldManager.instance.CameraIcon.Height);
        Vector2 size = new Vector2(1.0f, (float)TrickyWorldManager.instance.CameraIcon.Height / TrickyWorldManager.instance.CameraIcon.Width); // maintain aspect
        Vector2 origin = new Vector2(size.X / 2, size.Y / 2);
        Raylib.DrawBillboardPro(TrickyWorldManager.instance.levelEditorWindow.viewCamera3D, TrickyWorldManager.instance.CameraIcon, sourceRec, Position * WorldScale, new Vector3(0, 0, 1), size, origin, 0f, Color.White);
    }

    public struct CameraAnimationHeader
    {
        public int Action; //Could also be offset

        public List<CameraAnimationData> AnimationDatas;
    }

    public struct CameraAnimationData
    {
        //Probably Wrong I'll figure it out
        public float[] Translation;
        public float[] Rotation;
    }

}
