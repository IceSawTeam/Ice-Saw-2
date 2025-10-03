using IceSaw2.LevelObject.Materials;
using IceSaw2.Manager;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.EditorWindows
{
    public class MaterialEditorWindow : BaseEditorWindow
    {
        Camera3D camera3D = new Camera3D();
        int MaterialSelection = 0;

        public void Initilize()
        {
            camera3D.Position = new System.Numerics.Vector3(0, 10, 3);
            camera3D.Target = Vector3.Zero;
            camera3D.Up = new Vector3(0, 0, 1);
            camera3D.FovY = 45f;
            camera3D.Projection = CameraProjection.Perspective;
        }

        public override void LogicUpdate()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                MaterialSelection -= 1;
                if (MaterialSelection == -1)
                {
                    MaterialSelection = DataManager.trickyMaterialObject.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                MaterialSelection += 1;
                if (MaterialSelection == DataManager.trickyMaterialObject.Count)
                {
                    MaterialSelection = 0;
                }
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public override void RenderUpdate()
        {
            if (DataManager.trickyMaterialObject.Count != 0)
            {
                Raylib.DrawText(DataManager.trickyMaterialObject[MaterialSelection].Name, 12, 30, 20, Color.Black);
            }

            Raylib.BeginMode3D(camera3D);
            Raylib.ClearBackground(new Color(120, 120, 120));

            {
                Rlgl.PushMatrix();
                Rlgl.Rotatef(90, 1, 0, 0);
                int slices = 10;
                float spacing = 1;
                int halfSlices = slices / 2;
                Rlgl.Begin(DrawMode.Lines);
                for (int i = -halfSlices; i <= halfSlices; i++)
                {
                    if (i == 0)
                    {
                        Rlgl.Color3f(0.2f, 0.2f, 0.2f);
                    }
                    else
                    {
                        Rlgl.Color3f(0.2f, 0.2f, 0.2f);
                    }

                    Rlgl.Vertex3f((float)i*spacing, 0.0f, (float)-halfSlices*spacing);
                    Rlgl.Vertex3f((float)i*spacing, 0.0f, (float)halfSlices*spacing);

                    Rlgl.Vertex3f((float)-halfSlices*spacing, 0.0f, (float)i*spacing);
                    Rlgl.Vertex3f((float)halfSlices*spacing, 0.0f, (float)i*spacing);
                }
                Rlgl.End();
            }
            Rlgl.PopMatrix();


            if (DataManager.trickyMaterialObject.Count != 0)
            {
                DataManager.trickyMaterialObject[MaterialSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
