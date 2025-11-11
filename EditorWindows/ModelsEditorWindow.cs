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
        int ActiveModelIndex = -1;
        int ActiveSkyboxIndex = -1;
        int ActiveMaterialIndex = -1;

        List<int> selectedModelIndices = new List<int>();

        // Store selected tab index persistently
        int selectedTab = 0; // Make this a field or property in your UI state

        string[] tabs = { "Models", "Materials", "Skybox Models", "Skybox Materials" };

        //private byte[] _nameBuffer = new byte[128];
        //private int _bufferEntityIndex = -1;

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
                    ActiveModelIndex -= 1;
                    if (ActiveModelIndex == -1)
                    {
                        ActiveModelIndex = TrickyDataManager.trickyModelObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    ActiveModelIndex += 1;
                    if (ActiveModelIndex == TrickyDataManager.trickyModelObjects.Count)
                    {
                        ActiveModelIndex = 0;
                    }
                }
            }
            else
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Left))
                {
                    ActiveSkyboxIndex -= 1;
                    if (ActiveSkyboxIndex == -1)
                    {
                        ActiveSkyboxIndex = TrickyDataManager.trickySkyboxModelObjects.Count - 1;
                    }
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Right))
                {
                    ActiveSkyboxIndex += 1;
                    if (ActiveSkyboxIndex == TrickyDataManager.trickySkyboxModelObjects.Count)
                    {
                        ActiveSkyboxIndex = 0;
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
                if (ActiveModelIndex != -1)
                {
                    Raylib.DrawText(TrickyDataManager.trickyModelObjects[ActiveModelIndex].Name, 300, 90, 20, Raylib_cs.Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (ActiveModelIndex != -1)
                {
                    TrickyDataManager.trickyModelObjects[ActiveModelIndex].Render();
                }

                Raylib.EndMode3D();
            }
            else
            {
                if (ActiveSkyboxIndex != -1)
                {
                    Raylib.DrawText(TrickyDataManager.trickySkyboxModelObjects[ActiveSkyboxIndex].Name, 300, 90, 20, Raylib_cs.Color.Black);
                }

                Raylib.BeginMode3D(camera3D);

                RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

                if (ActiveSkyboxIndex != -1)
                {
                    TrickyDataManager.trickySkyboxModelObjects[ActiveSkyboxIndex].Render();
                }

                Raylib.EndMode3D();
            }

            RenderUI();
        }

        public void RenderUI()
        {
            // Render UI
            var io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            var vpPos = vp.Pos;
            var vpSize = vp.Size;
            float menuBarHeight = ImGui.GetFrameHeight();


            ShowSkybox = selectedTab >= 2;
            ShowMaterials = selectedTab == 1 || selectedTab == 3;

            RenderSubtabs(vpPos, vpSize, menuBarHeight);

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

            RenderOutliner(flags,vpSize, menuBarHeight, outlinerWidth);


            RenderInspector(flags, vpPos, vpSize, inspectorWidth, menuBarHeight);



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

        public void RenderSubtabs(Vector2 vpPos, Vector2 vpSize, float menuBarHeight)
        {
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
        }

        public void RenderOutliner(ImGuiWindowFlags flags, Vector2 vpSize, float menuBarHeight, float outlinerWidth)
        {
            // --- OUTLINER ---


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
                            ActiveMaterialIndex = i;
                        }
                    }
                }
                else
                {
                    //for (int i = 0; i < TrickyDataManager.trickyModelObjects.Count; i++)
                    //{
                    //var _id = TrickyDataManager.trickyModelObjects[i].HierarchyRender();
                    //if (_id != -1)
                    //{
                    //    ActiveModelIndex = i;
                    //}

                    //}



                    for (int i = 0; i < TrickyDataManager.trickyModelObjects.Count; i++)
                    {
                        var _modelChildren = TrickyDataManager.trickyModelObjects[i].Children;
                        bool isSelected = selectedModelIndices.Contains(i);

                        var _flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

                        if (_modelChildren.Count == 0)
                            _flags |= ImGuiTreeNodeFlags.Leaf;

                        if (isSelected)
                            _flags |= ImGuiTreeNodeFlags.Selected;

                        if (i == ActiveModelIndex)
                        {
                            ImGui.PushStyleColor(ImGuiCol.Header, new System.Numerics.Vector4(0.717f, 0.435f, 0.156f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.HeaderHovered, new System.Numerics.Vector4(0.9f, 0.6f, 0.3f, 1.0f));
                            ImGui.PushStyleColor(ImGuiCol.HeaderActive, new System.Numerics.Vector4(0.9f, 0.6f, 0.3f, 1.0f));
                        }

                        bool nodeOpen = ImGui.TreeNodeEx($"##Model{i}", _flags, TrickyDataManager.trickyModelObjects[i].Name);

                        if (i == ActiveModelIndex)
                        {
                            ImGui.PopStyleColor(3);
                        }

                        if (ImGui.IsItemClicked())
                        {
                            bool ctrl = ImGui.GetIO().KeyCtrl;
                            bool shift = ImGui.GetIO().KeyShift;

                            if (shift && ActiveModelIndex >= 0)
                            {
                                int start = Math.Min(ActiveModelIndex, i);
                                int end = Math.Max(ActiveModelIndex, i);

                                for (int j = start; j <= end; j++)
                                {
                                    if (!selectedModelIndices.Contains(j))
                                        selectedModelIndices.Add(j);
                                }
                            }
                            else if (ctrl)
                            {
                                if (isSelected)
                                    selectedModelIndices.Remove(i);
                                else
                                    selectedModelIndices.Add(i);
                            }
                            else
                            {
                                selectedModelIndices.Clear();
                                selectedModelIndices.Add(i);
                            }

                            ActiveModelIndex = i;
                        }

                        if (!selectedModelIndices.Contains(ActiveModelIndex) && selectedModelIndices.Count != 0)
                        {
                            ActiveModelIndex = selectedModelIndices[selectedModelIndices.Count - 1];
                        }



                        if (nodeOpen)
                        {
                            for (int j = 0; j < _modelChildren.Count; j++)
                            {
                                _modelChildren[j].HierarchyRender();
                            }
                            ImGui.TreePop();
                        }
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
        }

        public void RenderInspector(ImGuiWindowFlags flags, Vector2 vpPos, Vector2 vpSize, float inspectorWidth, float menuBarHeight)
        {
            // --- INSPECTOR ---
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(vpPos.X + vpSize.X - inspectorWidth, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(inspectorWidth, vpSize.Y /*Raylib.GetScreenHeight() - menuBarHeight*/), ImGuiCond.Always);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Inspector Panel", flags);
            ImGui.Text("Inspector");


            if (ShowMaterials && ActiveMaterialIndex != -1)
            {
                if (!ShowSkybox)
                {
                    var activeMat = TrickyDataManager.trickyMaterialObject[ActiveMaterialIndex];

                    //bool selectionChanged = ActiveMaterialIndex != _bufferEntityIndex;
                    //bool inputActive = ImGui.IsAnyItemActive();

                    //if (selectionChanged && !inputActive)
                    //{
                    //    _bufferEntityIndex = ActiveMaterialIndex;
                    //    Array.Clear(_nameBuffer, 0, _nameBuffer.Length);

                    //    var nameBytes = System.Text.Encoding.UTF8.GetBytes(activeMat.Name);
                    //    Array.Copy(nameBytes, _nameBuffer, Math.Min(nameBytes.Length, _nameBuffer.Length - 1));
                    //}

                    //if (ImGui.InputText("##Material Name", _nameBuffer, (uint)_nameBuffer.Length))
                    //{
                    //    int nullIndex = Array.IndexOf(_nameBuffer, (byte)0);
                    //    if (nullIndex < 0) nullIndex = _nameBuffer.Length;
                    //    activeMat.Name = System.Text.Encoding.UTF8.GetString(_nameBuffer, 0, nullIndex);
                    //}

                    ImGui.SetNextItemWidth(-1);
                    ImGui.InputTextWithHint($"##Material Name {ActiveMaterialIndex}", "Enter material name...", ref activeMat.Name, 128);
                    //ImGui.Text("Name | " + activeMat.Name);
                    ImGui.Text("Main Texture | " + activeMat.TexturePath);
                    rlImGui.Image(TrickyDataManager.ReturnTexture(activeMat.TexturePath, ShowSkybox));
                    activeMat.TexturePath = ImGUIUtil.TextInput(activeMat.TexturePath, 99999);
                    ImGui.DragInt("UnknownInt2", ref activeMat.UnknownInt2, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt3", ref activeMat.UnknownInt3, 0.3f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat1", ref activeMat.UnknownFloat1, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat2", ref activeMat.UnknownFloat2, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat3", ref activeMat.UnknownFloat3, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat4", ref activeMat.UnknownFloat4, 0.25f, -1, 99999);
                    ImGui.DragInt("UnknownInt8", ref activeMat.UnknownInt8, 0.3f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat5", ref activeMat.UnknownFloat5, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat6", ref activeMat.UnknownFloat6, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat7", ref activeMat.UnknownFloat7, 0.25f, -1, 99999);
                    ImGui.DragFloat("UnknownFloat8", ref activeMat.UnknownFloat8, 0.25f, -1, 99999);
                    ImGui.DragInt("UnknownInt13", ref activeMat.UnknownInt13, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt14", ref activeMat.UnknownInt14, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt15", ref activeMat.UnknownInt15, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt16", ref activeMat.UnknownInt16, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt17", ref activeMat.UnknownInt17, 0.3f, -1, 99999);
                    ImGui.DragInt("UnknownInt18", ref activeMat.UnknownInt18, 0.3f, -1, 99999);

                    ImGui.DragInt("UnknownInt20", ref activeMat.UnknownInt20, 0.3f, -1, 99999);

                    ImGui.Text("Texture Flipbook:");

                    foreach (string flipBook in activeMat.TextureFlipbook)
                    {
                        ImGui.Text("\t- " + flipBook);
                    }

                }
            }
            else if (ActiveModelIndex != -1)
            {
                var activeMdl = TrickyDataManager.trickyModelObjects[ActiveModelIndex];
                ImGui.SetNextItemWidth(-1);
                ImGui.InputTextWithHint($"##Model Name {ActiveModelIndex}", "Enter model name...", ref activeMdl.Name, 128);

                ImGui.DragInt("UnknownInt3", ref activeMdl.Unknown3, 0.3f, -1, 99999);
                ImGui.DragFloat("AnimTime", ref activeMdl.AnimTime, 0.025f, 0, 99999);
            }


            ImGui.End();
            //ImGui.PopStyleVar(2);
        }


        public void BetterInputInt(string val_name, ref int val, int min, int max)
        {
            if (ImGui.Button(" - "))
            {
                val -= 1;
            }
            ImGui.SameLine();
            if (ImGui.Button(" + "))
            {
                val += 1;
            }
            ImGui.SameLine();
            ImGui.InputInt(val_name, ref val, min, max);
        }

        //private void UpdateBufferFromEntityName(string name)
        //{
        //    Array.Clear(_nameBuffer, 0, _nameBuffer.Length);
        //    var bytes = System.Text.Encoding.UTF8.GetBytes(name);
        //    Array.Copy(bytes, _nameBuffer, Math.Min(bytes.Length, _nameBuffer.Length - 1));
        //}

        //private string GetStringFromBuffer()
        //{
        //    int nullIndex = Array.IndexOf(_nameBuffer, (byte)0);
        //    if (nullIndex < 0) nullIndex = _nameBuffer.Length;
        //    return System.Text.Encoding.UTF8.GetString(_nameBuffer, 0, nullIndex);
        //}
    }
}
