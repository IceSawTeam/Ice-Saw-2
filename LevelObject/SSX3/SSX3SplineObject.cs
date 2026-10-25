using IceSaw2.LevelObject;
using SSXLibrary.JsonFiles.SSX3;
using SSXMultiTool.Utilities;
using System.Collections.Generic;
using System.Numerics;

public class SSX3SplineObject : LineBaseObject
{
    public int U0;
    public int U1;
    public float U2;
    public int U3;
    public int U4;

    public List<SplineSegment> splineSegments = new List<SplineSegment>();

    int SEGMENT_COUNT = 7;

    public override Vector3 Colour => new Vector3(1, 0, 0);

    public void LoadSpline(SplineJsonHandler.SplineJson spline)
    {
        Name = spline.Name;

        U0 = spline.U0;
        U1 = spline.U1;
        U2 = spline.U2;
        U3 = spline.U3;
        U4 = spline.U4;

        Position = JsonUtil.Array2DToVector3(spline.Segments[0].Points, 0);

        for (int i = 0; i < spline.Segments.Count; i++)
        {
            SplineSegment segment = new SplineSegment();

            segment.Point1 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 0);
            segment.Point2 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 1);
            segment.Point3 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 2);
            segment.Point4 = JsonUtil.Array2DToVector3(spline.Segments[i].Points, 3);

            splineSegments.Add(segment);
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
            WorldLinePoints.Add(TempSegment.Point1 * WorldScale);
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

    [System.Serializable]
    public struct SplineSegment
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;
        public Vector3 Point4;

        public Vector3 LocalPoint1;
        public Vector3 LocalPoint2;
        public Vector3 LocalPoint3;
        public Vector3 LocalPoint4;
    }
}