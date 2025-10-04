using IceSaw2.Manager.Tricky;
using NURBS;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPatchObject : MeshBaseObject
    {
        public Vector4 LightMapPoint;
        public List<Vector2> UVPoints;
        public Vector3[,] WorldPoints;

        public int SurfaceType;
        public bool TrickOnlyPatch;
        public string TexturePath = "";
        public int LightmapID;

        Surface surface;

        bool MeshLoaded =false;

        public void LoadPatch(PatchesJsonHandler.PatchJson patchJson)
        {
            LightMapPoint = new Vector4(patchJson.LightMapPoint[0], patchJson.LightMapPoint[1], patchJson.LightMapPoint[2], patchJson.LightMapPoint[3]);

            UVPoints = new List<Vector2>();

            for (int i = 0; i < 4; i++)
            {
                UVPoints.Add(new Vector2(patchJson.UVPoints[i, 0], patchJson.UVPoints[i, 1]));
            }

            WorldPoints = new Vector3[4,4];

            for (int y = 0; y < 4; y++)
            {
                for (global::System.Int32 x = 0; x < 4; x++)
                {
                    WorldPoints[x, y] = new Vector3(patchJson.Points[x + y*4, 0], patchJson.Points[x + y * 4, 1], patchJson.Points[x + y * 4, 2]);
                }
            }

            SurfaceType = patchJson.SurfaceType;
            TrickOnlyPatch = patchJson.TrickOnlyPatch;
            TexturePath = patchJson.TexturePath;
            LightmapID = patchJson.LightmapID;

            GeneratePatch();
        }

        public void GeneratePatch()
        {
            if(MeshLoaded)
            {
                Raylib.UnloadMesh(mesh);
            }

            Vector3[,] vertices = new Vector3[4, 4];

            //Control points
            ControlPoint[,] cps = new ControlPoint[4, 4];
            int c = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 pos = WorldPoints[j, i];
                    cps[i, j] = new NURBS.ControlPoint(pos.X, pos.Y, pos.Z, 1);
                    c++;
                }
            }

            int degreeU = 3;
            int degreeV = 3;

            int resolutionU = Settings.Manager.Instance.General.PatchResolution; //7;
            int resolutionV = Settings.Manager.Instance.General.PatchResolution; //7; ()

            surface = new NURBS.Surface(cps, degreeU, degreeV);

            //Build mesh (reusing Mesh to save GC allocation)
            mesh = surface.BuildMesh(resolutionU, resolutionV);

            cps = new ControlPoint[2, 2];

            //UVPoints = PointCorrection(UVPoints);

            cps[0, 0] = new NURBS.ControlPoint(UVPoints[0].X, UVPoints[0].Y, 0, 1);
            cps[1, 0] = new NURBS.ControlPoint(UVPoints[1].X, UVPoints[1].Y, 0, 1);
            cps[0, 1] = new NURBS.ControlPoint(UVPoints[2].X, UVPoints[2].Y, 0, 1);
            cps[1, 1] = new NURBS.ControlPoint(UVPoints[3].X, UVPoints[3].Y, 0, 1);

            surface = new NURBS.Surface(cps, 1, 1);

            Vector3[] UV = surface.ReturnVertices(resolutionU, resolutionV);

            Span<Vector2> NewTextureCords = mesh.TexCoordsAs<Vector2>();
            for (int i = 0; i < UV.Length; i++)
            {
                NewTextureCords[i] = new Vector2(UV[i].X, UV[i].Y);
            }

            Raylib.UploadMesh(ref mesh, false);
            MeshLoaded = true;

            var Texture = TrickyDataManager.ReturnTexture(TexturePath, false);

            material = Raylib.LoadMaterialDefault();

            Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, Texture);
        }

        public List<Vector2> PointCorrection(List<Vector2> NewList)
        {
            for (int i = 0; i < NewList.Count; i++)
            {
                var TempPoint = NewList[i];
                TempPoint.Y = -TempPoint.Y;
                NewList[i] = TempPoint;
            }

            return NewList;
        }
    }
}
