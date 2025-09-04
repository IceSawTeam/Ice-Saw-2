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
    public class PatchObject : BaseObject
    {
        public Vector4 LightMapPoint;
        public List<Vector2> UVPoints;
        public Vector3[,] Points;

        public int SurfaceType;
        public bool TrickOnlyPatch;
        public string TexturePath;
        public int LightmapID;

        public Model Model;
        NURBS.Surface surface;

        public void LoadPatch(PatchesJsonHandler.PatchJson patchJson)
        {
            LightMapPoint = new Vector4(patchJson.LightMapPoint[0], patchJson.LightMapPoint[1], patchJson.LightMapPoint[2], patchJson.LightMapPoint[3]);

            UVPoints = new List<Vector2>();

            for (int i = 0; i < 4; i++)
            {
                UVPoints.Add(new Vector2(patchJson.UVPoints[i, 0], patchJson.UVPoints[i, 1]));
            }

            Points = new Vector3[4,4];

            for (int y = 0; y < 4; y++)
            {
                for (global::System.Int32 x = 0; x < 4; x++)
                {
                    Points[x, y] = new Vector3(patchJson.Points[x + y*4, 0], patchJson.Points[x + y * 4, 1], patchJson.Points[x + y * 4, 2]);
                }
            }

            SurfaceType = patchJson.SurfaceType;
            TrickOnlyPatch = patchJson.TrickOnlyPatch;
            TexturePath = patchJson.TexturePath;
            LightmapID = patchJson.LightmapID;

            GeneratePatch();
        }

        public override void Render()
        {
            Raylib.DrawModelEx(Model, Vector3.Zero, Vector3.UnitX, 0, Vector3.One * 0.001f, Color.White);
        }

        public void GeneratePatch()
        {
            Vector3[,] vertices = new Vector3[4, 4];

            //Control points
            NURBS.ControlPoint[,] cps = new NURBS.ControlPoint[4, 4];
            int c = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 pos = Points[j, i];
                    cps[i, j] = new NURBS.ControlPoint(pos.X, pos.Y, pos.Z, 1);
                    c++;
                }
            }

            int degreeU = 3;
            int degreeV = 3;

            int resolutionU = 7; //7;
            int resolutionV = 7; //7; ()

            surface = new NURBS.Surface(cps, degreeU, degreeV);

            //Build mesh (reusing Mesh to save GC allocation)
            var mesh = surface.BuildMesh(resolutionU, resolutionV);


            

            cps = new NURBS.ControlPoint[2, 2];

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
            Model = Raylib.LoadModelFromMesh(mesh);

            var Texture = WorldManager.instance.ReturnTexture(TexturePath);

            Raylib.SetMaterialTexture(ref Model, 0, MaterialMapIndex.Diffuse, ref Texture);
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

        //public void LoadUVMap()
        //{
        //    //Build UV Points

        //    NURBS.ControlPoint[,] cps = new NURBS.ControlPoint[2, 2];

        //    List<Vector2> vector2s = new List<Vector2>();
        //    vector2s.Add(UVPoint1);
        //    vector2s.Add(UVPoint2);
        //    vector2s.Add(UVPoint3);
        //    vector2s.Add(UVPoint4);

        //    vector2s = PointCorrection(vector2s);

        //    cps[0, 0] = new NURBS.ControlPoint(vector2s[0].x, vector2s[0].y, 0, 1);
        //    cps[1, 0] = new NURBS.ControlPoint(vector2s[1].x, vector2s[1].y, 0, 1);
        //    cps[0, 1] = new NURBS.ControlPoint(vector2s[2].x, vector2s[2].y, 0, 1);
        //    cps[1, 1] = new NURBS.ControlPoint(vector2s[3].x, vector2s[3].y, 0, 1);

        //    surface = new NURBS.Surface(cps, 1, 1);

        //    int resolutionU = 7; //7;
        //    int resolutionV = 7; //7; ()

        //    Vector3[] UV = surface.ReturnVertices(resolutionU, resolutionV);

        //    Vector2[] UV2 = new Vector2[UV.Length];

        //    for (int i = 0; i < UV.Length; i++)
        //    {
        //        UV2[i] = new Vector2(UV[i].x, UV[i].y);
        //    }
        //    meshFilter.sharedMesh.uv = UV2;
        //}

        //public void LoadLightmap()
        //{
        //    //Build Lightmap Points
        //    NURBS.ControlPoint[,] cps = new NURBS.ControlPoint[2, 2];

        //    cps[0, 0] = new NURBS.ControlPoint(0.1f, 0.1f, 0, 1);
        //    cps[1, 0] = new NURBS.ControlPoint(0.1f, 0.9f, 0, 1);
        //    cps[0, 1] = new NURBS.ControlPoint(0.9f, 0.1f, 0, 1);
        //    cps[1, 1] = new NURBS.ControlPoint(0.9f, 0.9f, 0, 1);

        //    surface = new NURBS.Surface(cps, 1, 1);

        //    int resolutionU = 7; //7;
        //    int resolutionV = 7; //7; ()

        //    Vector3[] UV = surface.ReturnVertices(resolutionU, resolutionV);

        //    Vector2[] UV2 = new Vector2[UV.Length];

        //    for (int i = 0; i < UV.Length; i++)
        //    {
        //        UV2[i] = new Vector2(UV[i].x, UV[i].y);
        //    }

        //    meshFilter.sharedMesh.uv2 = UV2;
        //}
    }
}
