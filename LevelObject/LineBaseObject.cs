using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.LevelObject
{
    public class LineBaseObject : BaseObject
    {
        public List<Vector3> WorldLinePoints = new List<Vector3>();

        public virtual Vector3 Colour { get; }

        public override void Render()
        {
            for (int i = 0; i < WorldLinePoints.Count - 1; i++)
            {
                Raylib.DrawCylinderEx(WorldLinePoints[i], WorldLinePoints[i + 1], 3f * WorldScale, 3f*WorldScale,4, new Color(Colour.X, Colour.Y, Colour.Z));
            }

            //Rlgl.PushMatrix();

            //Rlgl.Begin(DrawMode.Lines);
            //Rlgl.Color3f(Colour.X, Colour.Y, Colour.Z);

            //for (int i = 0; i < WorldLinePoints.Count - 1; i++)
            //{
            //    Rlgl.Vertex3f(WorldLinePoints[i].X, WorldLinePoints[i].Y, WorldLinePoints[i].Z);
            //    Rlgl.Vertex3f(WorldLinePoints[i + 1].X, WorldLinePoints[i + 1].Y, WorldLinePoints[i + 1].Z);
            //}

            //Rlgl.End();

            //Rlgl.PopMatrix();
        }
    }
}
