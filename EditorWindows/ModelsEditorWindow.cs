using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;

namespace IceSaw2.EditorWindows
{
    public class ModelsEditorWindow : BaseEditorWindow
    {
        bool Skybox;
        bool Material;

        Camera3D camera3D = new Camera3D();
        int PrefabSelection = 0;
        int SkyboxSelection = 0;

        // Store selected tab index persistently
        int selectedTab = 0; // Make this a field or property in your UI state

        string[] tabs = { "Models", "Materials", "Skybox Models", "Skybox Materials" };

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
                        PrefabSelection = TrickyDataManager.trickyModelObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    PrefabSelection += 1;
                    if (PrefabSelection == TrickyDataManager.trickyModelObjects.Count)
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
                if (TrickyDataManager.trickyModelObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickyModelObjects[PrefabSelection].Name, 12, 60, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (TrickyDataManager.trickyModelObjects.Count != 0)
                {
                    TrickyDataManager.trickyModelObjects[PrefabSelection].Render();
                }

                Raylib.EndMode3D();
            }
            else
            {
                if (TrickyDataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickySkyboxPrefabObjects[SkyboxSelection].Name, 12, 60, 20, Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (TrickyDataManager.trickySkyboxPrefabObjects.Count != 0)
                {
                    TrickyDataManager.trickySkyboxPrefabObjects[SkyboxSelection].Render();
                }

                Raylib.EndMode3D();
            }

            // Calculate position below main menu bar
            float yOffset = ImGui.GetFrameHeight(); // Main menu height

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, yOffset), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(ImGui.GetIO().DisplaySize.X, yOffset-20));

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(8, 9));

            ImGui.Begin("TabStrip", ImGuiWindowFlags.NoTitleBar |
                                     ImGuiWindowFlags.NoResize |
                                     ImGuiWindowFlags.NoMove |
                                     ImGuiWindowFlags.NoScrollbar |
                                     ImGuiWindowFlags.NoSavedSettings |
                                     ImGuiWindowFlags.NoCollapse);

            for (int i = 0; i < tabs.Length; i++)
            {
                if (i > 0) ImGui.SameLine();

                bool isSelected = (selectedTab == i);

                if (ImGui.Selectable(tabs[i], isSelected, ImGuiSelectableFlags.None, new System.Numerics.Vector2(120, 0)))
                {
                    selectedTab = i;

                    if(selectedTab>=2)
                    {
                        Skybox = true;
                    }
                    else
                    {
                        Skybox = false;
                    }

                    if(selectedTab==1 && selectedTab == 3)
                    {
                        Material = true;
                    }
                    else
                    {
                        Material = false;
                    }
                }
            }

            ImGui.End();
            ImGui.PopStyleVar(3);
        }
    }
}
