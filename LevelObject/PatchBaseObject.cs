using IceSaw2.Manager.SSX3;
using IceSaw2.RayWarp;
using IceSaw2.Renderer;
using NURBS;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using static IceSaw2.LevelObject.TrickyObjects.TrickyPatchObject;

namespace IceSaw2.LevelObject
{
    public class PatchBaseObject : BaseObject
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
                    TessellatedPatch.Instance.UpdatePatchLightmap(TesPatchID, LightmapID, ReturnLightmapPoints());
                }
            }
        }
        public ControlPoints controlPoints;

        public Vector2[] UVPoints = new Vector2[4];

        private string _texturePath = "";
        public string TexturePath
        {
            get { return _texturePath; }
            set
            {
                if (_texturePath != value)
                {
                    _texturePath = value;
                    TessellatedPatch.Instance.UpdatePatchTexture(TesPatchID, ReturnTexture(TexturePath).Value, UVPoints);
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


        public PatchBaseObject(int Layer = -1)
        {
            TesPatchID = IceSaw2.Renderer.TessellatedPatch.Instance.AddPatch(new Vector3[16], ReturnTexture(TexturePath).Value, UVPoints, 0, ReturnLightmapPoints(), false);

            controlPoints = new ControlPoints(TesPatchID);
        }

        public virtual Texture2D? ReturnTexture(string Path)
        {
            return null;
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


        public Vector2[] ReturnLightmapPoints()
        {
            Vector2[] vector2s = new Vector2[4];

            vector2s[0] = new Vector2(LightMapPoint.X, LightMapPoint.Y);
            vector2s[1] = new Vector2(LightMapPoint.X, LightMapPoint.Y + LightMapPoint.W);
            vector2s[2] = new Vector2(LightMapPoint.X + LightMapPoint.Z, LightMapPoint.Y);
            vector2s[3] = new Vector2(LightMapPoint.X + LightMapPoint.Z, LightMapPoint.Y + LightMapPoint.W);

            return vector2s;
        }

        public struct ControlPoints
        {
            int _tesPatchID = -1;
            int _layer = -1;
            public ControlPoints(int TesPatchID = -1, int Layer = -1)
            {
                _tesPatchID = TesPatchID;
                _layer = Layer;
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

            public Vector3[] ReturnControlPoints(bool NoScale = false)
            {
                Vector3[] tempPoints = new Vector3[16];

                if (NoScale)
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
