using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.Manager.WorldManager;

namespace IceSaw2.EditorWindows
{
    public class PrefabEditorWindow : BaseEditorWindow
    {
        Camera3D camera3D = new Camera3D();
        int PrefabSelection = 0;

        public void Initilize()
        {
            camera3D.Position = new System.Numerics.Vector3(0, 15, 3);
            camera3D.Target = Vector3.Zero;
            camera3D.Up = new Vector3(0, 0, 1);
            camera3D.FovY = 45f;
            camera3D.Projection = CameraProjection.Perspective;
        }

        public override void LogicUpdate()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                PrefabSelection -= 1;
                if (PrefabSelection == -1)
                {
                    PrefabSelection = DataManager.trickyPrefabObjects.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                PrefabSelection += 1;
                if (PrefabSelection == DataManager.trickyPrefabObjects.Count)
                {
                    PrefabSelection = 0;
                }
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public override void RenderUpdate()
        {
            if (DataManager.trickyPrefabObjects.Count != 0)
            {
                Raylib.DrawText(DataManager.trickyPrefabObjects[PrefabSelection].Name, 12, 30, 20, Color.Black);
            }

            Raylib.BeginMode3D(camera3D);

            Raylib.DrawGrid(10, 1);

            if (DataManager.trickyPrefabObjects.Count != 0)
            {
                DataManager.trickyPrefabObjects[PrefabSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
