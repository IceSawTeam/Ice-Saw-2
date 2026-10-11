using IceSaw2.Manager.SSX3;
using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using IceSaw2.Renderer;
using NURBS;
using Raylib_cs;
using SSXLibrary.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Numerics;
using static IceSaw2.LevelObject.TrickyObjects.TrickyPatchObject;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPatchObject : PatchBaseObject
    {
        public int SurfaceType;
        public bool TrickOnlyPatch;

        public void LoadPatch(PatchesJsonHandler.PatchJson patchJson)
        {
            Name = patchJson.PatchName;

            LightMapPoint = new Vector4(patchJson.LightMapPoint[0], patchJson.LightMapPoint[1], patchJson.LightMapPoint[2], patchJson.LightMapPoint[3]);

            //UVPoints = new Vector2[4];
            
            for (int i = 0; i < 4; i++)
            {
                UVPoints[i] = new Vector2(patchJson.UVPoints[i, 0], patchJson.UVPoints[i, 1]);
            }

            //WorldPoints = new Vector3[16];

            for (int y = 0; y < 16; y++)
            {
                controlPoints[y] = new Vector3(patchJson.Points[y, 0], patchJson.Points[y, 1], patchJson.Points[y, 2]);
            }

            SurfaceType = patchJson.SurfaceType;
            TrickOnlyPatch = patchJson.TrickOnlyPatch;
            TexturePath = patchJson.TexturePath;
            LightmapID = patchJson.LightmapID;
        }

        public override Texture2D? ReturnTexture(string Path)
        {
            return TrickyDataManager.ReturnTexture(TexturePath, false);
        }

        public PatchesJsonHandler.PatchJson SavePatch()
        {
            PatchesJsonHandler.PatchJson patch = new PatchesJsonHandler.PatchJson();
            patch.PatchName = Name;
            patch.LightMapPoint = JsonUtil.Vector4ToArray(LightMapPoint);

            patch.UVPoints = new float[4, 2];

            for (int i = 0; i < UVPoints.Length; i++)
            {
                patch.UVPoints[i, 0] = UVPoints[i].X;
                patch.UVPoints[i, 1] = UVPoints[i].Y;
            }

            patch.Points = new float[16, 3];

            for (int y = 0; y < 16; y++)
            {
                patch.Points[y, 0] = controlPoints[y].X;
                patch.Points[y, 1] = controlPoints[y].Y;
                patch.Points[y, 2] = controlPoints[y].Z;
            }

            patch.SurfaceType = (int)SurfaceType;
            patch.TrickOnlyPatch = TrickOnlyPatch;
            patch.TexturePath = TexturePath;
            patch.LightmapID = LightmapID;

            return patch;
        }
    }
}
