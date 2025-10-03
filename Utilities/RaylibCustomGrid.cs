using Raylib_cs;

namespace IceSaw2.Utilities
{
    public class RaylibCustomGrid
    {
        public static void DrawBasic3DGrid(int Slices, int Spacing, Color Color)
        {
            Rlgl.PushMatrix();
            Rlgl.Rotatef(90, 1, 0, 0);
            int halfSlices = Slices / 2;
            Rlgl.Begin(DrawMode.Lines);
            var V3Color = Raylib.ColorNormalize(Color);

            for (int i = -halfSlices; i <= halfSlices; i++)
            {
                Rlgl.Color3f(V3Color.X, V3Color.Y, V3Color.Z);
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
