using IceSaw2.Manager;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.LevelObject.TrickyObjects
{
    public class TrickySplineObject : LineBaseObject
    {
        public override ObjectType Type
        {
            get { return ObjectType.Spline; }
        }

        public int U0;
        public int U1;
        public int SplineStyle;

        //[OnChangedCall("DrawCurve")]
        public List<SplineSegment> splineSegments = new List<SplineSegment>();

        int SEGMENT_COUNT = 7;

        public override Vector3 Colour => new Vector3(1, 0, 0);

        public void LoadSpline(SplineJsonHandler.SplineJson spline)
        {
            Name = spline.SplineName;
            U0 = spline.U0;
            U1 = spline.U1;
            SplineStyle = spline.SplineStyle;

            Position = JsonUtil.Array2DToVector3(spline.Segments[0].Points, 0);

            for (int i = 0; i < spline.Segments.Count; i++)
            {
                SplineSegment splineSegment = new SplineSegment();

                splineSegment.Point1 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 0);
                splineSegment.Point2 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 1);
                splineSegment.Point3 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 2);
                splineSegment.Point4 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 3);

                splineSegment.U0 = spline.Segments[i].U0;
                splineSegment.U1 = spline.Segments[i].U1;
                splineSegment.U2 = spline.Segments[i].U2;
                splineSegment.U3 = spline.Segments[i].U3;

                splineSegments.Add(splineSegment);
            }

            for (int i = 0; i < splineSegments.Count - 1; i++)
            {
                SplineSegment splineSegment = splineSegments[i];
                splineSegment.Point4 = splineSegments[i + 1].Point1;
                splineSegments[i] = splineSegment;
            }

            DrawCurve();
        }

        public Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            return p;
        }
        //bool Generated = false;
        public void DrawCurve()
        {
            WorldLinePoints = new List<Vector3>();
            for (int i = 0; i < splineSegments.Count; i++)
            {
                var TempSegment = splineSegments[i];

                //if (!Generated)
                //{
                //    TempSegment.LocalPoint1 = ConvertLocalPoint(TempSegment.Point1);
                //    TempSegment.LocalPoint2 = ConvertLocalPoint(TempSegment.Point2);
                //    TempSegment.LocalPoint3 = ConvertLocalPoint(TempSegment.Point3);
                //    TempSegment.LocalPoint4 = ConvertLocalPoint(TempSegment.Point4);
                //}
                //else
                //{
                //    Generated = false;
                //}
                WorldLinePoints.Add(TempSegment.Point1*WorldScale);
                for (int a = 1; a <= SEGMENT_COUNT; a++)
                {
                    float t = a / (float)SEGMENT_COUNT;
                    Vector3 pixel = CalculateCubicBezierPoint(t, (TempSegment.Point1), (TempSegment.Point2), (TempSegment.Point3), (TempSegment.Point4));
                    WorldLinePoints.Add(pixel * WorldScale);
                }
                WorldLinePoints.Add(TempSegment.Point4 * WorldScale);

                splineSegments[i] = TempSegment;
            }
        }

        //Vector3 ConvertLocalPoint(Vector3 point)
        //{
        //    return transform.InverseTransformPoint(TrickyLevelManager.Instance.transform.TransformPoint(point));
        //}

        //Vector3 ConvertWorldPoint(Vector3 point)
        //{
        //    return TrickyLevelManager.Instance.transform.InverseTransformPoint(transform.TransformPoint(point));
        //}

        [System.Serializable]
        public struct SplineSegment
        {
            public Vector3 Point1;
            public Vector3 Point2;
            public Vector3 Point3;
            public Vector3 Point4;
            public float U0;
            public float U1;
            public float U2;
            public float U3;

            public Vector3 LocalPoint1;
            public Vector3 LocalPoint2;
            public Vector3 LocalPoint3;
            public Vector3 LocalPoint4;
        }
    }
}
