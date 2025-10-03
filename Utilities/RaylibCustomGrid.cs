using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.Utilities
{
    public class RaylibCustomGrid
    {
        public static void DrawBasic3DGrid(int Slices, int Spacing, Vector3 Color)
        {
            Rlgl.PushMatrix();
            Rlgl.Rotatef(90, 1, 0, 0);
            int halfSlices = Slices / 2;
            Rlgl.Begin(DrawMode.Lines);
            for (int i = -halfSlices; i <= halfSlices; i++)
            {
                //if (i == 0)
                //{
                    Rlgl.Color3f(Color.X, Color.Y, Color.Z);
                //}
                //else
                //{
                //    Rlgl.Color3f(0.2f, 0.2f, 0.2f);
                //}

                Rlgl.Vertex3f((float)i * Spacing, 0.0f, (float)-halfSlices * Spacing);
                Rlgl.Vertex3f((float)i * Spacing, 0.0f, (float)halfSlices * Spacing);

                Rlgl.Vertex3f((float)-halfSlices * Spacing, 0.0f, (float)i * Spacing);
                Rlgl.Vertex3f((float)halfSlices * Spacing, 0.0f, (float)i * Spacing);
            }
            Rlgl.End();
            Rlgl.PopMatrix();
        }
    }
}
