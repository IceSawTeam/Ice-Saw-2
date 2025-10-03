using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using Raylib_cs;
using System.Numerics;

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
                    MaterialSelection = TrickyDataManager.trickyMaterialObject.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                MaterialSelection += 1;
                if (MaterialSelection == TrickyDataManager.trickyMaterialObject.Count)
                {
                    MaterialSelection = 0;
                }
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public override void RenderUpdate()
        {
            if (TrickyDataManager.trickyMaterialObject.Count != 0)
            {
                Raylib.DrawText(TrickyDataManager.trickyMaterialObject[MaterialSelection].Name, 12, 30, 20, Color.Black);
            }

            Raylib.BeginMode3D(camera3D);

            RaylibCustomGrid.DrawBasic3DGrid(10, 1, new Color(51, 51, 51));

            if (TrickyDataManager.trickyMaterialObject.Count != 0)
            {
                TrickyDataManager.trickyMaterialObject[MaterialSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
