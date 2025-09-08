using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.WorldManager;

namespace IceSaw2.EditorWindows
{
    public class PrefabEditorWindow
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

        public void LogicUpdate()
        {
            if (Raylib.IsKeyPressed(KeyboardKey.Left))
            {
                PrefabSelection -= 1;
                if (PrefabSelection == -1)
                {
                    PrefabSelection = WorldManager.instance.trickyPrefabObjects.Count - 1;
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Right))
            {
                PrefabSelection += 1;
                if (PrefabSelection == WorldManager.instance.trickyPrefabObjects.Count)
                {
                    PrefabSelection = 0;
                }
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public void RenderUpdate()
        {
            if (WorldManager.instance.trickyPrefabObjects.Count != 0)
            {
                Raylib.DrawText(WorldManager.instance.trickyPrefabObjects[PrefabSelection].Name, 12, 30, 20, Color.Black);
            }

            Raylib.BeginMode3D(camera3D);

            Raylib.DrawGrid(10, 1);

            if (WorldManager.instance.trickyPrefabObjects.Count != 0)
            {
                WorldManager.instance.trickyPrefabObjects[PrefabSelection].Render();
            }

            Raylib.EndMode3D();
        }
    }
}
