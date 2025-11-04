using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Numerics;

namespace IceSaw2.EditorWindows
{
    public class ModelsEditorWindow : BaseEditorWindow
    {
        bool ShowSkybox;
        bool ShowMaterials;

        Camera3D camera3D = new Camera3D();
        int ActiveModel = 0;
        int ActiveSkybox = 0;
        int ActiveMaterial = -1;

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
            if (!ShowSkybox)
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    ActiveModel -= 1;
                    if (ActiveModel == -1) 
                    {
                        ActiveModel = TrickyDataManager.trickyModelObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    ActiveModel += 1;
                    if (ActiveModel == TrickyDataManager.trickyModelObjects.Count)
                    {
                        ActiveModel = 0;
                    }
                }
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    ActiveSkybox -= 1;
                    if (ActiveSkybox == -1)
                    {
                        ActiveSkybox = TrickyDataManager.trickySkyboxModelObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    ActiveSkybox += 1;
                    if (ActiveSkybox == TrickyDataManager.trickySkyboxModelObjects.Count)
                    {
                        ActiveSkybox = 0;
                    }
                }
            }

            if (Raylib.IsKeyPressed(KeyboardKey.Slash))
            {
                ShowSkybox = !ShowSkybox;
            }

            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);
        }

        public override void RenderUpdate()
        {
            if (!ShowSkybox)
            {
                if (TrickyDataManager.trickyModelObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickyModelObjects[ActiveModel].Name, 300, 90, 20, Raylib_cs.Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (TrickyDataManager.trickyModelObjects.Count != 0)
                {
                    TrickyDataManager.trickyModelObjects[ActiveModel].Render();
                }

                Raylib.EndMode3D();
            }
            else
            {
                if (TrickyDataManager.trickySkyboxModelObjects.Count != 0)
                {
                    Raylib.DrawText(TrickyDataManager.trickySkyboxModelObjects[ActiveSkybox].Name, 300, 90, 20, Raylib_cs.Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (TrickyDataManager.trickySkyboxModelObjects.Count != 0)
                {
                    TrickyDataManager.trickySkyboxModelObjects[ActiveSkybox].Render();
                }

                Raylib.EndMode3D();
            }


            // Render UI


            var io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            var vpPos = vp.Pos;
            var vpSize = vp.Size;
            float menuBarHeight = ImGui.GetFrameHeight();


            // --- SUB TABS ---

            ImGui.SetNextWindowPos(new Vector2(vpPos.X, vpPos.Y + menuBarHeight));
            ImGui.SetNextWindowSize(new Vector2(vpSize.X, menuBarHeight + 2));

            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(4, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4, 2));
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowMinSize, new Vector2(0, 0)); // prevent min height override
            ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.GetColorU32(ImGuiCol.MenuBarBg));

            ImGui.Begin("TabStrip", ImGuiWindowFlags.NoTitleBar |
                                    ImGuiWindowFlags.NoResize |
                                    ImGuiWindowFlags.NoMove |
                                    ImGuiWindowFlags.NoScrollbar |
                                    ImGuiWindowFlags.NoSavedSettings |
                                    ImGuiWindowFlags.NoCollapse |
                                    ImGuiWindowFlags.NoBringToFrontOnFocus |
                                    ImGuiWindowFlags.NoNavFocus |
                                    ImGuiWindowFlags.NoDocking);

            // centered
            float totalWidth = 0f;
            for (int i = 0; i < tabs.Length; i++)
            {
                Vector2 textSize = ImGui.CalcTextSize(tabs[i]);
                float buttonWidth = textSize.X + ImGui.GetStyle().FramePadding.X * 2f;
                totalWidth += buttonWidth;
                if (i < tabs.Length - 1)
                    totalWidth += ImGui.GetStyle().ItemSpacing.X;
            }

            float availWidth = ImGui.GetWindowSize().X;
            float startX = (availWidth - totalWidth) * 0.5f;
            if (startX < 0) startX = 0;

            ImGui.SetCursorPosX(startX);


            for (int i = 0; i < tabs.Length; i++)
            {
                if (i == selectedTab)
                    ImGui.PushStyleColor(ImGuiCol.Button, ImGui.GetColorU32(ImGuiCol.Header));
                else
                    ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.0f, 0.0f, 0.0f, 0f));

                if (ImGui.Button(tabs[i]))
                    selectedTab = i;

                ImGui.PopStyleColor();

                if (i < tabs.Length - 1)
                    ImGui.SameLine();
            }

            ImGui.End();

            ImGui.PopStyleColor();
            ImGui.PopStyleVar(5);

            ShowSkybox = selectedTab >= 2;
            ShowMaterials = selectedTab == 1 || selectedTab == 3;



            // --- OUTLINER ---

            // Dimensions
            menuBarHeight += 22; // Note: this includes both top bars.
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

            
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(outlinerWidth, vpSize.Y /*Raylib.GetScreenHeight() - menuBarHeight*/), ImGuiCond.Always);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Outliner Panel", flags);
            ImGui.Text("Outliner");

            if (!ShowSkybox)
            {
                if (ShowMaterials)
                {
                    for (int i = 0; i < TrickyDataManager.trickyMaterialObject.Count; i++)
                    {
                        var _id = TrickyDataManager.trickyMaterialObject[i].HierarchyRender();
                        if (_id != -1)
                        {
                            ActiveMaterial = i;
                            Console.WriteLine(ActiveMaterial);
                        }
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
                if (ShowMaterials)
                {
                    for (int i = 0; i < TrickyDataManager.trickySkyboxMaterialObject.Count; i++)
                    {
                        TrickyDataManager.trickySkyboxMaterialObject[i].HierarchyRender();
                    }
                }
                else
                {
                    for (int i = 0; i < TrickyDataManager.trickySkyboxModelObjects.Count; i++)
                    {
                        TrickyDataManager.trickySkyboxModelObjects[i].HierarchyRender();
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


            if (ShowMaterials && ActiveMaterial != -1)
            {
                if (!ShowSkybox)
                {
                    ImGui.Text("Name | " + TrickyDataManager.trickyMaterialObject[ActiveMaterial].Name);
                    ImGui.Text(TrickyDataManager.trickyMaterialObject[ActiveMaterial].TexturePath);
                    rlImGui.Image(TrickyDataManager.ReturnTexture(TrickyDataManager.trickyMaterialObject[ActiveMaterial].TexturePath, ShowSkybox));

                }
            }


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
            ImGui.TextWrapped("This is the viewport area. Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test End");
            ImGui.EndChild();

            ImGui.End();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar(2);


        }
    }
}
