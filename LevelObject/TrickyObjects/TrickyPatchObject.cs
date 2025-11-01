using IceSaw2.Manager.Tricky;
using IceSaw2.RayWarp;
using IceSaw2.Renderer;
using NURBS;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Numerics;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickyPatchObject : MeshBaseObject
    {
        public Vector4 LightMapPoint;
        public List<Vector2> UVPoints = [];
        public Vector3[,] WorldPoints = new Vector3[4,4];

        public int SurfaceType;
        public bool TrickOnlyPatch;
        public string TexturePath = "";
        public int LightmapID;

        Surface? surface;

        bool MeshLoaded =false;

        public TrickyPatchObject()
        {
            //Renderer.TessellatedPatch.Instance.AddPatch();
        }

        public void LoadPatch(PatchesJsonHandler.PatchJson patchJson)
        {
            Name = patchJson.PatchName;

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

            List<Vector3> vector3s = new List<Vector3>();

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    vector3s.Add(WorldPoints[x, y]*WorldScale);
                }
            }

            List<Vector2> vector2s = new List<Vector2>();

            vector2s.Add(new Vector2(LightMapPoint.X, LightMapPoint.Y));
            vector2s.Add(new Vector2(LightMapPoint.X+ LightMapPoint.Z, LightMapPoint.Y));
            vector2s.Add(new Vector2(LightMapPoint.X, LightMapPoint.Y + LightMapPoint.W));
            vector2s.Add(new Vector2(LightMapPoint.X + LightMapPoint.Z, LightMapPoint.Y + LightMapPoint.W));

            TessellatedPatch.Instance.AddPatch(vector3s, TrickyDataManager.ReturnTexture(TexturePath, false), UVPoints, LightmapID, vector2s, false);


            //GeneratePatch();
        }

        public void GeneratePatch()
        {
            if(MeshLoaded)
            {
                Raylib.UnloadMesh(meshRef.Mesh);
            }

            // Vector3[,] vertices = new Vector3[4, 4];

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

            int resolutionU = Settings.General.Instance.data.PatchResolution; //7;
            int resolutionV = Settings.General.Instance.data.PatchResolution; //7; ()

            surface = new NURBS.Surface(cps, degreeU, degreeV);

            //Build mesh (reusing Mesh to save GC allocation)
            meshRef = new MeshRef(surface.BuildMesh(resolutionU, resolutionV));

            cps = new ControlPoint[2, 2];

            //UVPoints = PointCorrection(UVPoints);

            cps[0, 0] = new NURBS.ControlPoint(UVPoints[0].X, UVPoints[0].Y, 0, 1);
            cps[1, 0] = new NURBS.ControlPoint(UVPoints[1].X, UVPoints[1].Y, 0, 1);
            cps[0, 1] = new NURBS.ControlPoint(UVPoints[2].X, UVPoints[2].Y, 0, 1);
            cps[1, 1] = new NURBS.ControlPoint(UVPoints[3].X, UVPoints[3].Y, 0, 1);

            surface = new NURBS.Surface(cps, 1, 1);

            Vector3[] UV = surface.ReturnVertices(resolutionU, resolutionV);

            Span<Vector2> NewTextureCords = meshRef.Mesh.TexCoordsAs<Vector2>();
            for (int i = 0; i < UV.Length; i++)
            {
                NewTextureCords[i] = new Vector2(UV[i].X, UV[i].Y);
            }

            Raylib.UploadMesh(ref meshRef.Mesh, false);
            MeshLoaded = true;

            var Texture = TrickyDataManager.ReturnTexture(TexturePath, false);

            materialRef = new MaterialRef(Raylib.LoadMaterialDefault());

            Raylib.SetMaterialTexture(ref materialRef.Material, MaterialMapIndex.Diffuse, Texture);
        }

        public PatchesJsonHandler.PatchJson SavePatch()
        {
            PatchesJsonHandler.PatchJson patch = new PatchesJsonHandler.PatchJson();
            patch.PatchName = Name;
            patch.LightMapPoint = JsonUtil.Vector4ToArray(LightMapPoint);

            patch.UVPoints = new float[4, 2];

            for (int i = 0; i < UVPoints.Count; i++)
            {
                patch.UVPoints[i, 0] = UVPoints[i].X;
                patch.UVPoints[i, 1] = UVPoints[i].Y;
            }

            patch.Points = new float[16, 3];

            for (int y = 0; y < 4; y++)
            {
                for (global::System.Int32 x = 0; x < 4; x++)
                {
                    patch.Points[y * 4 + x, 0] = WorldPoints[y, x].X;
                    patch.Points[y * 4 + x, 1] = WorldPoints[y, x].Y;
                    patch.Points[y * 4 + x, 2] = WorldPoints[y, x].Z;
                }
            }

            patch.SurfaceType = (int)SurfaceType;
            patch.TrickOnlyPatch = TrickOnlyPatch;
            patch.TexturePath = TexturePath;
            patch.LightmapID = LightmapID;

            return patch;
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
