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
