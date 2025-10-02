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
            Raylib.ClearBackground(new Color(51, 115, 195));


            Raylib.DrawGrid(10, 1);

            if (DataManager.trickyMaterialObject.Count != 0)
            {
                DataManager.trickyMaterialObject[MaterialSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
