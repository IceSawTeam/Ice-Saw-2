using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using IceSaw2.LevelObject;
using SSXMultiTool.JsonFiles.Tricky;
using SSXMultiTool.Utilities;

public class TrickyPathAObject : BaseObject
{
    public override ObjectType Type
    {
        get { return ObjectType.PathA; }
    }

    public bool Respawnable;

    public List<Vector3> PathPoints;
    public List<Vector3> VectorPoints;

    public List<PathEvent> PathEvents;

    public void LoadPathA(AIPSOPJsonHandler.PathA pathA)
    {
        Name = pathA.Name;

        Respawnable = pathA.Respawnable;

        Position = JsonUtil.ArrayToVector3(pathA.PathPos);

        PathPoints = new List<Vector3>();
        VectorPoints = new List<Vector3>();
        for (int i = 0; i < pathA.PathPoints.GetLength(0); i++)
        {
            VectorPoints.Add(new Vector3(pathA.PathPoints[i, 0], pathA.PathPoints[i, 1], pathA.PathPoints[i, 2]));
            PathPoints.Add(VectorPoints[i]);
            if (i != 0)
            {
                PathPoints[i] += PathPoints[i - 1];
            }
        }

        PathEvents = new List<PathEvent>();
        for (int i = 0; i < pathA.PathEvents.Count; i++)
        {
            var NewStruct = new PathEvent();

            NewStruct.EventType = pathA.PathEvents[i].EventType;
            NewStruct.EventValue = pathA.PathEvents[i].EventValue;
            NewStruct.EventStart = pathA.PathEvents[i].EventStart;
            NewStruct.EventEnd = pathA.PathEvents[i].EventEnd;

            PathEvents.Add(NewStruct);
        }
    }

    public Vector3 FindPathLocalPoint(float FindDistance)
    {
        float OldDistance = 0f;
        float TestDistance = 0f;
        for (int i = 0; i < VectorPoints.Count; i++)
        {
            TestDistance += Vector3.Distance(VectorPoints[i], new Vector3(0, 0,0));
            if (TestDistance >= FindDistance)
            {
                //Get Size
                float Size = TestDistance - OldDistance;
                //Get Pos
                float position = TestDistance - FindDistance;
                //Get Percentage
                float Percentage = (position / Size);

                //Return Local Point
                if (i - 1 < 0)
                {
                    return ((1f - Percentage) * VectorPoints[i]);
                }
                return ((1f - Percentage) * VectorPoints[i]) + PathPoints[i - 1];
            }
            OldDistance = TestDistance;
        }

        return new Vector3(0, 0, 0);
    }

    public AIPSOPJsonHandler.PathA GeneratePathA()
    {
        AIPSOPJsonHandler.PathA NewPathA = new AIPSOPJsonHandler.PathA();

        NewPathA.Name = Name;

        NewPathA.Respawnable = Respawnable;

        NewPathA.PathPos = JsonUtil.Vector3ToArray(Position);

        NewPathA.PathPoints = new float[PathPoints.Count, 3];

        for (int i = 0; i < PathPoints.Count; i++)
        {
            NewPathA.PathPoints[i, 0] = PathPoints[i].X;
            NewPathA.PathPoints[i, 1] = PathPoints[i].Y;
            NewPathA.PathPoints[i, 2] = PathPoints[i].Z;

            if (i != 0)
            {
                NewPathA.PathPoints[i, 0] -= PathPoints[i - 1].X;
                NewPathA.PathPoints[i, 1] -= PathPoints[i - 1].Y;
                NewPathA.PathPoints[i, 2] -= PathPoints[i - 1].Z;
            }
        }

        NewPathA.PathEvents = new List<AIPSOPJsonHandler.PathEvent>();

        for (int i = 0; i < PathEvents.Count; i++)
        {
            var NewStruct = new AIPSOPJsonHandler.PathEvent();

            NewStruct.EventType = PathEvents[i].EventType;
            NewStruct.EventValue = PathEvents[i].EventValue;
            NewStruct.EventStart = PathEvents[i].EventStart;
            NewStruct.EventEnd = PathEvents[i].EventEnd;

            NewPathA.PathEvents.Add(NewStruct);
        }

        return NewPathA;
    }

    public void PathPointsUpdate()
    {
        GenerateVectors();
    }


    public void GenerateVectors()
    {
        VectorPoints = new List<Vector3>();
        for (int i = 0; i < PathPoints.Count; i++)
        {
            if (i == 0)
            {
                VectorPoints.Add(PathPoints[i]);
            }
            else
            {
                VectorPoints.Add(PathPoints[i] - PathPoints[i - 1]);
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