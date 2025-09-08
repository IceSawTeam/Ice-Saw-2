using IceSaw2.LevelObject.Materials;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2.EditorWindows
{
    public class MaterialEditorWindow
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

        public void LogicUpdate()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                MaterialSelection -= 1;
                if (MaterialSelection == -1)
                {
                    MaterialSelection = WorldManager.instance.trickyMaterialObject.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                MaterialSelection += 1;
                if (MaterialSelection == WorldManager.instance.trickyMaterialObject.Count)
                {
                    MaterialSelection = 0;
                }
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public void RenderUpdate()
        {
            if (WorldManager.instance.trickyMaterialObject.Count != 0)
            {
                Raylib.DrawText(WorldManager.instance.trickyMaterialObject[MaterialSelection].Name, 12, 30, 20, Color.Black);
            }

            Raylib.BeginMode3D(camera3D);

            Raylib.DrawGrid(10, 1);

            if (WorldManager.instance.trickyMaterialObject.Count != 0)
            {
                WorldManager.instance.trickyMaterialObject[MaterialSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
