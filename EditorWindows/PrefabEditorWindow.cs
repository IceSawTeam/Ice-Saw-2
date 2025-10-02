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
        bool Skybox;

        Camera3D camera3D = new Camera3D();
        int PrefabSelection = 0;
        int SkyboxSelection = 0;

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
            if (!Skybox)
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
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    SkyboxSelection -= 1;
                    if (SkyboxSelection == -1)
                    {
                        SkyboxSelection = DataManager.trickySkyboxPrefabObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    SkyboxSelection += 1;
                    if (SkyboxSelection == DataManager.trickySkyboxPrefabObjects.Count)
                    {
                        SkyboxSelection = 0;
                    }
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Slash))
            {
                Skybox = !Skybox;
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public override void RenderUpdate()
        {
            if (!Skybox)
            {
                if (DataManager.trickyPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(DataManager.trickyPrefabObjects[PrefabSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);
                Raylib.ClearBackground(new Color(51, 115, 195));


                Raylib.DrawGrid(10, 1);

                if (DataManager.trickyPrefabObjects.Count != 0)
                {
                    DataManager.trickyPrefabObjects[PrefabSelection].Render();
                }

                Raylib.EndMode3D();
            }
            else
            {
                if (DataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(DataManager.trickySkyboxPrefabObjects[SkyboxSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);
                Raylib.ClearBackground(new Color(51, 115, 195));


                Raylib.DrawGrid(10, 1);

                if (DataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    DataManager.trickySkyboxPrefabObjects[SkyboxSelection].Render();
                }

                Raylib.EndMode3D();
            }
        }
    }
}
