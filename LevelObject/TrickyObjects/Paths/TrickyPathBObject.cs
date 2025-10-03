using IceSaw2.LevelObject;
using Raylib_cs;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

public class TrickyPathBObject : LineBaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.PathB; }
    }

    public float DistanceToFinish;

    public List<Vector3> PathPoints;
    public List<Vector3> VectorPoints;

    public List<PathEvent> PathEvents;

    public override Vector3 Colour => new Vector3(0, 0, 0);

    public void LoadPathB(AIPSOPJsonHandler.PathB pathB)
    {
        Name = pathB.Name;
        DistanceToFinish = pathB.DistanceToFinish;

        Position = JsonUtil.ArrayToVector3(pathB.PathPos);

        PathPoints = new List<Vector3>();
        VectorPoints = new List<Vector3>();
        for (int i = 0; i < pathB.PathPoints.GetLength(0); i++)
        {
            VectorPoints.Add(new Vector3(pathB.PathPoints[i, 0], pathB.PathPoints[i, 1], pathB.PathPoints[i, 2]));
            PathPoints.Add(VectorPoints[i]);
            if (i != 0)
            {
                PathPoints[i] += PathPoints[i - 1];
            }
        }

        PathEvents = new List<PathEvent>();
        for (int i = 0; i < pathB.PathEvents.Count; i++)
        {
            var NewStruct = new PathEvent();

            NewStruct.EventType = pathB.PathEvents[i].EventType;
            NewStruct.EventValue = pathB.PathEvents[i].EventValue;
            NewStruct.EventStart = pathB.PathEvents[i].EventStart;
            NewStruct.EventEnd = pathB.PathEvents[i].EventEnd;

            PathEvents.Add(NewStruct);
        }

        GenerateWorldPathPoints();
    }

    public void GenerateWorldPathPoints()
    {
        WorldLinePoints = new List<Vector3>();

        WorldLinePoints.Add(Position * WorldScale);

        for (int i = 0; i < PathPoints.Count; i++)
        {
            WorldLinePoints.Add((PathPoints[i] + Position) * WorldScale);
        }
    }

    public Vector3 FindPathLocalPoint(float FindDistance)
    {
        float OldDistance = 0f;
        float TestDistance = 0f;
        for (int i = 0; i < VectorPoints.Count; i++)
        {
            TestDistance += Vector3.Distance(VectorPoints[i], new Vector3(0, 0, 0));
            if (TestDistance >= FindDistance)
            {
                //Get Size
                float Size = TestDistance - OldDistance;
                //Get Pos
                float position = TestDistance - FindDistance;
                //Get Percentage
                float Percentage = (position / Size);

                //Return Local Point
                if(i - 1<0)
                {
                    return ((1f-Percentage) * VectorPoints[i]);
                }
                return ((1f - Percentage) * VectorPoints[i]) + PathPoints[i - 1];
            }
            OldDistance = TestDistance;
        }

        return new Vector3 (0, 0, 0);
    }

    public float DistanceAlongPath(Vector3 vector3)
    {
        //Finds the nearest disatance along the path


        return 0f;
    }

    public AIPSOPJsonHandler.PathB GeneratePathB()
    {
        AIPSOPJsonHandler.PathB pathB = new AIPSOPJsonHandler.PathB();

        pathB.Name = Name;
        pathB.DistanceToFinish = DistanceToFinish;

        pathB.PathPos = JsonUtil.Vector3ToArray(Position);

        pathB.PathPoints = new float[PathPoints.Count, 3];

        for (int i = 0; i < PathPoints.Count; i++)
        {
            pathB.PathPoints[i, 0] = PathPoints[i].X;
            pathB.PathPoints[i, 1] = PathPoints[i].Y;
            pathB.PathPoints[i, 2] = PathPoints[i].Z;

            if (i != 0)
            {
                pathB.PathPoints[i, 0] -= PathPoints[i-1].X;
                pathB.PathPoints[i, 1] -= PathPoints[i-1].Y;
                pathB.PathPoints[i, 2] -= PathPoints[i-1].Z;
            }
        }

        pathB.PathEvents = new List<AIPSOPJsonHandler.PathEvent>();
        for (int i = 0; i < PathEvents.Count; i++)
        {
            var NewStruct = new AIPSOPJsonHandler.PathEvent();

            NewStruct.EventType = PathEvents[i].EventType;
            NewStruct.EventValue = PathEvents[i].EventValue;
            NewStruct.EventStart = PathEvents[i].EventStart;
            NewStruct.EventEnd = PathEvents[i].EventEnd;

            pathB.PathEvents.Add(NewStruct);
        }


        return pathB;
    }

    public void PathPointsUpdate()
    {
        GenerateVectors();
    }

    public void GenerateVectors()
    {
        VectorPoints = new List<Vector3>();
        for (int i = 0; i < PathPoints.Count;i++)
        {
            if(i==0)
            {
               VectorPoints.Add(PathPoints[i]);
            }
            else
            {
                VectorPoints.Add(PathPoints[i] - PathPoints[i-1]);
            }

        }
    }

    [System.Serializable]
    public struct PathEvent
    {
        public int EventType;
        public int EventValue;
        public float EventStart;
        public float EventEnd;
    }
}
