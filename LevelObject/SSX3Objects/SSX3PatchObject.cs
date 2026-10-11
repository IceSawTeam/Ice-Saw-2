//using SSXMultiTool.JsonFiles.SSX3;
//using SSXMultiTool.Utilities;
//using System.Collections.Generic;
//using System.Drawing;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;

//[ExecuteInEditMode]
using IceSaw2.LevelObject;
using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using IceSaw2.Renderer;
using SSXLibrary.JsonFiles.SSX3;
using SSXMultiTool.Utilities;
using System.Numerics;
using static IceSaw2.LevelObject.TrickyObjects.TrickyPatchObject;

public class SSX3PatchObject : BaseObject
{
    public int TesPatchID { get; private set; }
    private Vector4 _lightmapPoint;
    public Vector4 LightMapPoint
    {
        get { return _lightmapPoint; }
        set
        {
            if (_lightmapPoint != value)
            {
                _lightmapPoint = value;
                TessellatedPatch.Instance.UpdatePatchLightmap(TesPatchID, LightmapID, ReturnLightmapPoints(_lightmapPoint));
            }
        }
    }
    public ControlPoints controlPoints;

    public Vector2[] UVPoints = new Vector2[4];

    public int SurfaceType;
    public bool TrickOnlyPatch;

    public string _texturePath = "";
    public string TexturePath
    {
        get { return _texturePath; }
        set
        {
            if (_texturePath != value)
            {
                _texturePath = value;
                TessellatedPatch.Instance.UpdatePatchTexture(TesPatchID, SSX3DataManager.ReturnTexture(TexturePath), UVPoints);
            }
        }
    }

    private int _lightmapID;
    public int LightmapID
    {
        get { return _lightmapID; }
        set
        {
            if (_lightmapID != value)
            {
                _lightmapID = value;
                TessellatedPatch.Instance.UpdatePatchLightmap(TesPatchID, LightmapID, ReturnLightmapPoints(_lightmapPoint));
            }
        }
    }

    public SSX3PatchObject()
    {
        TesPatchID = IceSaw2.Renderer.TessellatedPatch.Instance.AddPatch(new Vector3[16], SSX3DataManager.ReturnTexture(TexturePath), UVPoints, 0, ReturnLightmapPoints(_lightmapPoint), false);

        controlPoints = new ControlPoints(TesPatchID);
    }


    public void LoadPatch(PatchesJsonHandler.PatchJson import)
    {
        Name = import.Name;
        LightMapPoint = JsonUtil.ArrayToVector4(import.LightMapPoint);

        LightMapPoint = new Vector4(import.LightMapPoint[0], import.LightMapPoint[1], import.LightMapPoint[2], import.LightMapPoint[3]);

        //UVPoints = new Vector2[4];

        for (int i = 0; i < 4; i++)
        {
            UVPoints[i] = new Vector2(import.UVPoints[i, 0], import.UVPoints[i, 1]);
        }

        //WorldPoints = new Vector3[16];

        for (int y = 0; y < 16; y++)
        {
            controlPoints[y] = new Vector3(import.Points[y, 0], import.Points[y, 1], import.Points[y, 2]);
        }

        TexturePath = import.TextureRID.ToString() + ".png";
        LightmapID = import.LightmapRID;
    }

    //    public PatchesJsonHandler.PatchJson GeneratePatch()
    //    {
    //        PatchesJsonHandler.PatchJson patch = new PatchesJsonHandler.PatchJson();
    //        //patch.PatchName = transform.name;
    //        patch.LightMapPoint = JsonUtil.Vector4ToArray(LightMapPoint);

    //        patch.UVPoints = new float[4, 2];

    //        patch.UVPoints[0, 0] = UVPoint1.x;
    //        patch.UVPoints[0, 1] = UVPoint1.y;
    //        patch.UVPoints[1, 0] = UVPoint2.x;
    //        patch.UVPoints[1, 1] = UVPoint2.y;
    //        patch.UVPoints[2, 0] = UVPoint3.x;
    //        patch.UVPoints[2, 1] = UVPoint3.y;
    //        patch.UVPoints[3, 0] = UVPoint4.x;
    //        patch.UVPoints[3, 1] = UVPoint4.y;

    //        patch.Points = new float[16, 3];

    //        patch.Points[0, 0] = RawControlPoint.x;
    //        patch.Points[0, 1] = RawControlPoint.y;
    //        patch.Points[0, 2] = RawControlPoint.z;

    //        patch.Points[1, 0] = RawR1C2.x;
    //        patch.Points[1, 1] = RawR1C2.y;
    //        patch.Points[1, 2] = RawR1C2.z;

    //        patch.Points[2, 0] = RawR1C3.x;
    //        patch.Points[2, 1] = RawR1C3.y;
    //        patch.Points[2, 2] = RawR1C3.z;

    //        patch.Points[3, 0] = RawR1C4.x;
    //        patch.Points[3, 1] = RawR1C4.y;
    //        patch.Points[3, 2] = RawR1C4.z;

    //        patch.Points[4, 0] = RawR2C1.x;
    //        patch.Points[4, 1] = RawR2C1.y;
    //        patch.Points[4, 2] = RawR2C1.z;

    //        patch.Points[5, 0] = RawR2C2.x;
    //        patch.Points[5, 1] = RawR2C2.y;
    //        patch.Points[5, 2] = RawR2C2.z;

    //        patch.Points[6, 0] = RawR2C3.x;
    //        patch.Points[6, 1] = RawR2C3.y;
    //        patch.Points[6, 2] = RawR2C3.z;

    //        patch.Points[7, 0] = RawR2C4.x;
    //        patch.Points[7, 1] = RawR2C4.y;
    //        patch.Points[7, 2] = RawR2C4.z;

    //        patch.Points[8, 0] = RawR3C1.x;
    //        patch.Points[8, 1] = RawR3C1.y;
    //        patch.Points[8, 2] = RawR3C1.z;

    //        patch.Points[9, 0] = RawR3C2.x;
    //        patch.Points[9, 1] = RawR3C2.y;
    //        patch.Points[9, 2] = RawR3C2.z;

    //        patch.Points[10, 0] = RawR3C3.x;
    //        patch.Points[10, 1] = RawR3C3.y;
    //        patch.Points[10, 2] = RawR3C3.z;

    //        patch.Points[11, 0] = RawR3C4.x;
    //        patch.Points[11, 1] = RawR3C4.y;
    //        patch.Points[11, 2] = RawR3C4.z;

    //        patch.Points[12, 0] = RawR4C1.x;
    //        patch.Points[12, 1] = RawR4C1.y;
    //        patch.Points[12, 2] = RawR4C1.z;

    //        patch.Points[13, 0] = RawR4C2.x;
    //        patch.Points[13, 1] = RawR4C2.y;
    //        patch.Points[13, 2] = RawR4C2.z;

    //        patch.Points[14, 0] = RawR4C3.x;
    //        patch.Points[14, 1] = RawR4C3.y;
    //        patch.Points[14, 2] = RawR4C3.z;

    //        patch.Points[15, 0] = RawR4C4.x;
    //        patch.Points[15, 1] = RawR4C4.y;
    //        patch.Points[15, 2] = RawR4C4.z;

    //        return patch;
    //    }


    //    public void UpdateTexture()
    //    {
    //        try
    //        {
    //            bool Found = false;
    //            for (int i = 0; i < SSX3WorldManager.Instance.texture2ds.Count; i++)
    //            {
    //                if (SSX3WorldManager.Instance.texture2ds[i].Name.ToLower() == TextureAssigment.ToLower())
    //                {
    //                    Found = true;
    //                    meshRenderer.sharedMaterial.SetTexture("_MainTexture", SSX3WorldManager.Instance.texture2ds[i].Texture);

    //                    var Lightmap = SSX3WorldManager.Instance.GrabLightmapTexture(LightMapPoint, LightmapID);

    //                    ImageSize = 1f / Lightmap.Size().x;

    //                    meshRenderer.sharedMaterial.SetTexture("_Lightmap", Lightmap);
    //                    return;
    //                }
    //            }

    //            if (!Found)
    //            {
    //                meshRenderer.sharedMaterial.SetTexture("_MainTexture", SSX3WorldManager.Instance.Error);
    //            }
    //            else
    //            {

    //            }
    //        }
    //        catch
    //        {
    //            meshRenderer.sharedMaterial.SetTexture("_MainTexture", SSX3WorldManager.Instance.Error);
    //        }
    //    }

    //    public enum PatchSurfaceType
    //    {
    //        Reset,
    //        StandardSnow,
    //        StandardOffTrack,
    //        PoweredSnow,
    //        SlowPoweredSnow,
    //        IceStandard,
    //        BounceUnskiable,
    //        IceWaterNoTrail,
    //        GlidyPoweredSnow,
    //        Rock,
    //        Wall,
    //        IceNoTrail,
    //        SmallParticleWake,
    //        OffTrackMetal,
    //        MetalGliding,
    //        Standard1,
    //        StandardSand,
    //        NoCollision,
    //        ShowOffRampMetal
    //    }
}