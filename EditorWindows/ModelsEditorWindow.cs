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


            #region Secondary Top Bar
            // Calculate position below main menu bar
            float yOffset = ImGui.GetFrameHeight(); // Main menu height

            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, yOffset), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(ImGui.GetIO().DisplaySize.X, yOffset-20));

            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(8, 9));

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

                    if(selectedTab==1 || selectedTab == 3)
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
            ImGui.PopStyleVar(2);
            #endregion

            var io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            var vpPos = vp.Pos;
            var vpSize = vp.Size;

            //Render UI

            float menuBarHeight = ImGui.GetFrameHeight() + 32; // Typically height of main menu bar
            float outlinerWidth = 300;
            float inspectorWidth = 300;
            float viewportWidth = Math.Max(100f, vpSize.X - outlinerWidth - inspectorWidth);
            float viewportHeaderHeight = 28f;

            ImGuiWindowFlags flags = ImGuiWindowFlags.NoTitleBar |
                             ImGuiWindowFlags.NoResize |
                             ImGuiWindowFlags.NoMove |
                             ImGuiWindowFlags.NoCollapse |
                             ImGuiWindowFlags.NoBringToFrontOnFocus |
                             ImGuiWindowFlags.NoFocusOnAppearing;

            // --- OUTLINER ---
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(outlinerWidth, vpSize.Y /*Raylib.GetScreenHeight() - menuBarHeight*/), ImGuiCond.Always);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Outliner Panel", flags);

            if(!Skybox)
            {
                if (Material)
                {
                    for (int i = 0; i < TrickyDataManager.trickyMaterialObject.Count; i++)
                    {
                        TrickyDataManager.trickyMaterialObject[i].HierarchyRender();
                    }
                }
                else
                {
                    for (int i = 0; i < TrickyDataManager.trickyModelObjects.Count; i++)
                    {
                        TrickyDataManager.trickyModelObjects[i].HierarchyRender();
                    }
                }
            }
            else
            {
                if (Material)
                {
                    for (int i = 0; i < TrickyDataManager.trickySkyboxMaterialObject.Count; i++)
                    {
                        TrickyDataManager.trickySkyboxMaterialObject[i].HierarchyRender();
                    }
                }
                else
                {
                    for (int i = 0; i < TrickyDataManager.trickySkyboxPrefabObjects.Count; i++)
                    {
                        TrickyDataManager.trickySkyboxPrefabObjects[i].HierarchyRender();
                    }
                }
            }

                // Add your sidebar content here

            ImGui.End();
            //ImGui.PopStyleVar(2);



            // --- INSPECTOR ---
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(vpPos.X + vpSize.X - inspectorWidth, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(inspectorWidth, vpSize.Y /*Raylib.GetScreenHeight() - menuBarHeight*/), ImGuiCond.Always);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Inspector Panel", flags);
            ImGui.Text("Inspector");
            ImGui.End();
            //ImGui.PopStyleVar(2);




            // --- VIEWPORT ---
            float centerX = vpPos.X + outlinerWidth;
            ImGui.SetNextWindowPos(new Vector2(centerX, vpPos.Y + menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(viewportWidth, vpSize.Y), ImGuiCond.Always);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Viewport", flags);

            var drawList = ImGui.GetWindowDrawList();
            var winPos = ImGui.GetWindowPos();
            var winSize = ImGui.GetWindowSize();

            Vector2 headerTL = new Vector2(winPos.X, winPos.Y);
            Vector2 headerBR = new Vector2(winPos.X + winSize.X, winPos.Y + viewportHeaderHeight);
            uint headerCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.08f, 0.08f, 0.08f, 0.5f));
            drawList.AddRectFilled(headerTL, headerBR, headerCol);

            ImGui.SetCursorScreenPos(new Vector2(winPos.X + 8, winPos.Y + 4));
            ImGui.Text("Viewport Header");

            ImGui.SetCursorScreenPos(new Vector2(winPos.X + 8, winPos.Y + viewportHeaderHeight + 8));

            // Here you can render your scene texture / draw calls.
            // Example placeholder: show a child area representing the render target region
            //ImGui.BeginChild("viewport_content", new Vector2(winSize.X - 16, winSize.Y - viewportHeaderHeight - 16), ImGuiChildFlags.None,
            //                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);

            ImGui.BeginChild("viewport_content", new Vector2(0, -ImGuiNative.igGetFrameHeightWithSpacing()), ImGuiChildFlags.None,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            ImGui.TextWrapped("This is the viewport area! Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ");
            ImGui.EndChild();

            ImGui.End();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar(2);


        }
    }
}
