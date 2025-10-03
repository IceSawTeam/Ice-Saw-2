using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using Raylib_cs;
using System.Numerics;

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
                        PrefabSelection = TrickyDataManager.trickyPrefabObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    PrefabSelection += 1;
                    if (PrefabSelection == TrickyDataManager.trickyPrefabObjects.Count)
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
                        SkyboxSelection = TrickyDataManager.trickySkyboxPrefabObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    SkyboxSelection += 1;
                    if (SkyboxSelection == TrickyDataManager.trickySkyboxPrefabObjects.Count)
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
                if (TrickyDataManager.trickyPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickyPrefabObjects[PrefabSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, new Color(51, 51, 51));

                if (TrickyDataManager.trickyPrefabObjects.Count != 0)
                {
                    TrickyDataManager.trickyPrefabObjects[PrefabSelection].Render();
                }

                Raylib.EndMode3D();
            }
            else
            {
                if (TrickyDataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickySkyboxPrefabObjects[SkyboxSelection].Name, 12, 30, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);
                Raylib.ClearBackground(new Color(51, 115, 195));


                Raylib.DrawGrid(10, 1);

                if (TrickyDataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    TrickyDataManager.trickySkyboxPrefabObjects[SkyboxSelection].Render();
                }

                Raylib.EndMode3D();
            }
        }
    }
}
