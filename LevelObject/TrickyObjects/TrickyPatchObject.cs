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
    public class TrickyPatchObject : BaseObject
    {
        private int TesPatchID;
        //private bool _holdUpdate;

        private Vector4 _lightmapPoint;
        public Vector4 LightMapPoint 
        {
            get { return _lightmapPoint; }
            set 
            {
                if (_lightmapPoint != value)
                {
                    _lightmapPoint = value;
                    TessellatedPatch.Instance.UpdatePatchLightmap(TesPatchID, LightmapID, ReturnLightmapPoints());
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
                    TessellatedPatch.Instance.UpdatePatchTexture(TesPatchID, TrickyDataManager.ReturnTexture(TexturePath, false), UVPoints);
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
                    TessellatedPatch.Instance.UpdatePatchLightmap(TesPatchID, LightmapID, ReturnLightmapPoints());
                }
            }
        }

        public TrickyPatchObject()
        {
            TesPatchID = Renderer.TessellatedPatch.Instance.AddPatch(new Vector3[16], TrickyDataManager.ReturnTexture(TexturePath, false), UVPoints, 0, ReturnLightmapPoints(), false);

            controlPoints = new ControlPoints(TesPatchID);
        }

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
            //GeneratePatch();
        }

        public Vector2[] ReturnLightmapPoints()
        {
            Vector2[] vector2s = new Vector2[4];

            vector2s[0] = new Vector2(LightMapPoint.X, LightMapPoint.Y);
            vector2s[1] = new Vector2(LightMapPoint.X, LightMapPoint.Y + LightMapPoint.W);
            vector2s[2] = new Vector2(LightMapPoint.X + LightMapPoint.Z, LightMapPoint.Y);
            vector2s[3] = new Vector2(LightMapPoint.X + LightMapPoint.Z, LightMapPoint.Y + LightMapPoint.W);

            return vector2s;
        }

        //public void GeneratePatch()
        //{
        //    if(MeshLoaded)
        //    {
        //        Raylib.UnloadMesh(meshRef.Mesh);
        //    }

        //    // Vector3[,] vertices = new Vector3[4, 4];

        //    //Control points
        //    ControlPoint[,] cps = new ControlPoint[4, 4];
        //    int c = 0;
        //    for (int i = 0; i < 4; i++)
        //    {
        //        for (int j = 0; j < 4; j++)
        //        {
        //            Vector3 pos = WorldPoints[j, i];
        //            cps[i, j] = new NURBS.ControlPoint(pos.X, pos.Y, pos.Z, 1);
        //            c++;
        //        }
        //    }

        //    int degreeU = 3;
        //    int degreeV = 3;

        //    int resolutionU = Settings.General.Instance.data.PatchResolution; //7;
        //    int resolutionV = Settings.General.Instance.data.PatchResolution; //7; ()

        //    surface = new NURBS.Surface(cps, degreeU, degreeV);

        //    //Build mesh (reusing Mesh to save GC allocation)
        //    meshRef = new MeshRef(surface.BuildMesh(resolutionU, resolutionV));

        //    cps = new ControlPoint[2, 2];

        //    //UVPoints = PointCorrection(UVPoints);

        //    cps[0, 0] = new NURBS.ControlPoint(UVPoints[0].X, UVPoints[0].Y, 0, 1);
        //    cps[1, 0] = new NURBS.ControlPoint(UVPoints[1].X, UVPoints[1].Y, 0, 1);
        //    cps[0, 1] = new NURBS.ControlPoint(UVPoints[2].X, UVPoints[2].Y, 0, 1);
        //    cps[1, 1] = new NURBS.ControlPoint(UVPoints[3].X, UVPoints[3].Y, 0, 1);

        //    surface = new NURBS.Surface(cps, 1, 1);

        //    Vector3[] UV = surface.ReturnVertices(resolutionU, resolutionV);

        //    Span<Vector2> NewTextureCords = meshRef.Mesh.TexCoordsAs<Vector2>();
        //    for (int i = 0; i < UV.Length; i++)
        //    {
        //        NewTextureCords[i] = new Vector2(UV[i].X, UV[i].Y);
        //    }

        //    Raylib.UploadMesh(ref meshRef.Mesh, false);
        //    MeshLoaded = true;

        //    var Texture = TrickyDataManager.ReturnTexture(TexturePath, false);

        //    materialRef = new MaterialRef(Raylib.LoadMaterialDefault());

        //    Raylib.SetMaterialTexture(ref materialRef.Material, MaterialMapIndex.Diffuse, Texture);
        //}

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

        public ObjExporter.MassModelData GenerateModel()
        {
            ObjExporter.MassModelData TempModel = new ObjExporter.MassModelData();
            TempModel.Name = Name;

            var Points = controlPoints.ReturnControlPoints(true);

            //Control points
            ControlPoint[,] cps = new ControlPoint[4, 4];
            int c = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 pos = Points[j + (i * 4)];
                    cps[i, j] = new NURBS.ControlPoint(pos.X, pos.Y, pos.Z, 1);
                    c++;
                }
            }

            int degreeU = 3;
            int degreeV = 3;

            int resolutionU = Settings.General.Instance.data.PatchResolution;
            int resolutionV = Settings.General.Instance.data.PatchResolution;

            NURBS.Surface surface = new NURBS.Surface(cps, degreeU, degreeV);

            //Build mesh (reusing Mesh to save GC allocation)
            MeshRef meshRef = new MeshRef(surface.BuildMesh(resolutionU, resolutionV));

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
                NewTextureCords[i] = new Vector2(UV[i].X, -UV[i].Y);
            }

            var Verts = meshRef.Mesh.VerticesAs<Vector3>();
            for (int i = 0; i < Verts.Length; i++)
            {
                Verts[i] = Raymath.Vector3Transform(Verts[i], worldMatrix4x4);
            }

            TempModel.Model = meshRef.Mesh;
            TempModel.TextureName = TexturePath;

            return TempModel;
        }

        public struct ControlPoints
        {
            int _tesPatchID = -1;
            public ControlPoints(int TesPatchID = -1)
            {
                _tesPatchID = TesPatchID;
            }

            private Vector3[] _worldPoints = new Vector3[16];
            public Vector3 this[int index]
            {
                get 
                {
                    if (index >= 0 && index < _worldPoints.Length)
                    {
                        return _worldPoints[index];
                    }
                    throw new IndexOutOfRangeException("Index is out of bounds.");
                }
                set
                {
                    if (index >= 0 && index < _worldPoints.Length)
                    {
                        if (_worldPoints[index] != value)
                        {
                            _worldPoints[index] = value;
                            TessellatedPatch.Instance.UpdatePatchControlPoints(_tesPatchID, _worldPoints);
                        }
                    }
                    else
                    {
                        throw new IndexOutOfRangeException("Index is out of bounds.");
                    }
                }
            }

            public Vector3[] ReturnControlPoints(bool NoScale =false)
            {
                Vector3[] tempPoints = new Vector3[16];

                if(NoScale)
                {
                    return _worldPoints;
                }

                //Replace Tessellated Patch Matrix with Correct One
                if (_tesPatchID != -1)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        tempPoints[i] = _worldPoints[i] * WorldScale;
                    }
                }

                return tempPoints;
            }
        }
    }
}
