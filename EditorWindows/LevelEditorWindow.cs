using IceSaw2.LevelObject;
using IceSaw2.Manager;
using IceSaw2.Manager.Tricky;
using IceSaw2.Renderer;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;

namespace IceSaw2.EditorWindows
{
    public class LevelEditorWindow : BaseEditorWindow
    {
        private const float BOOST_MULTIPLIER = 2.0f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float mouseSensitivity = 0.003f;
        private float moveSpeed = 0.1f;
        private const float moveSpeedStep = 0.008f;
        private int screenWidth { get { return Raylib.GetScreenWidth(); } }
        private int screenHeight { get { return Raylib.GetScreenHeight(); } }
        public Vector2 winPos;
        public Vector2 winSize;

        private float axisLineSize = 1000f;
        private float nearClip = 0.1f;
        private float farClip = 100000f;

        public Camera3D viewCamera3D = new Camera3D();
        public bool Open = true;

        public ShadingType shadingType;
        public bool showWireframeOverlay = false;
        public bool showLightColors = true;
        public bool backFaceCulling = false;

        public List<BaseObject> RenderItems = new List<BaseObject>();

        // Selection (click + box select in the viewport)
        private const float BoxSelectDragThresholdPx = 4f;
        private bool isBoxSelecting = false;
        private Vector2 boxSelectStartScreen;
        private Vector2 boxSelectCurrentScreen;

        public void Initilize()
        {
            viewCamera3D.Position = new Vector3(0f, 0f, 0.1f);
            viewCamera3D.Target = viewCamera3D.Position + new Vector3(0, 1, 0);
            viewCamera3D.Up = new Vector3(0, 0, 1);
            viewCamera3D.FovY = 65f;
            viewCamera3D.Projection = CameraProjection.Perspective;
        }

        public override void RenderUpdate()
        {
            if (backFaceCulling)
            {
                Rlgl.EnableBackfaceCulling();
            }
            else
            {
                Rlgl.DisableBackfaceCulling();
            }

            Rlgl.SetClipPlanes(nearClip, farClip);

            //Render 3D
            Raylib.BeginMode3D(viewCamera3D);
            FrustumCulling.UpdateFrustum(viewCamera3D);
            Rlgl.DisableDepthMask();
            //Render Skybox

            if (TrickyDataManager.trickySkyboxModelObjects.Count != 0)
            {
                //TrickyDataManager.trickySkyboxPrefabObjects[0].Render();
                Raylib.DrawMesh(TrickyDataManager.meshSkybox.Mesh, TrickyDataManager.materialSkybox.Material, Raymath.MatrixTranslate(viewCamera3D.Position.X, viewCamera3D.Position.Y, viewCamera3D.Position.Z) * BaseObject.Default);
            }
            Rlgl.EnableDepthMask();            

            //Render Default
            Raylib.DrawLine3D(new Vector3(-axisLineSize, 0, 0), new Vector3(axisLineSize, 0, 0), new Color(212, 28, 4));
            Raylib.DrawLine3D(new Vector3(0, -axisLineSize, 0), new Vector3(0, axisLineSize, 0), new Color(17, 212, 4));
            Raylib.DrawLine3D(new Vector3(0, 0, -axisLineSize), new Vector3(0, 0, axisLineSize), new Color(2, 99, 224));

            Profiler.SetStartTime("-Patches");
            TessellatedPatch.Instance.Render();
            Profiler.UpdateTime("-Patches");

            Profiler.SetStartTime("-Instances");
            for (int i = 0; i < TrickyDataManager.trickyModelObjects.Count; i++)
            {
                TrickyDataManager.trickyModelObjects[i].Render();
            }
            Profiler.UpdateTime("-Instances");

            var RenderList = CollectionsMarshal.AsSpan(RenderItems);

            //Render Objects
            for (int i = 0; i < RenderList.Length; i++)
            {
                RenderList[i].Render();
            }

            SelectionManager.RenderHighlights();

            Raylib.EndMode3D();
        }

        public void RenderUI()
        {
            //Render UI

            // Dimensions
            var io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            var vpPos = vp.Pos;
            var vpSize = vp.Size;
            float menuBarHeight = ImGui.GetFrameHeight();
            int outlinerWidth = 300;
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
            ImGui.Text("Outliner");

            for (int i = 0; i < TrickyDataManager.LevelNodeTree.Count; i++)
            {
                TrickyDataManager.LevelNodeTree[i].HierarchyRender(true);
            }

            ImGui.End();
            //ImGui.PopStyleVar(2);

            // --- INSPECTOR ---
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(vpPos.X + vpSize.X - inspectorWidth, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(inspectorWidth, vpSize.Y /*Raylib.GetScreenHeight() - menuBarHeight*/), ImGuiCond.Always);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            //ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Inspector Panel", flags);
            ImGui.Text("Inspector");
            ImGui.Separator();

            if (SelectionManager.Selected.Count == 0)
            {
                ImGui.TextDisabled("Nothing selected");
            }
            else if (SelectionManager.Selected.Count == 1)
            {
                ImGui.Text(SelectionManager.Selected[0].Name);
            }
            else
            {
                ImGui.Text(SelectionManager.Selected.Count + " objects selected");
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
            ImGui.PushStyleColor(ImGuiCol.MenuBarBg, new Vector4(0.0f, 0.0f, 0.0f, 0.6f));
            ImGui.Begin("Viewport", flags |= ImGuiWindowFlags.MenuBar);

            var drawList = ImGui.GetWindowDrawList();
            winPos = ImGui.GetWindowPos();
            winSize = ImGui.GetWindowSize();

            //Vector2 headerTL = new Vector2(winPos.X, winPos.Y);
            //Vector2 headerBR = new Vector2(winPos.X + winSize.X, winPos.Y + viewportHeaderHeight);
            //uint headerCol = ImGui.ColorConvertFloat4ToU32(new Vector4(0.08f, 0.08f, 0.08f, 0.5f));
            //drawList.AddRectFilled(headerTL, headerBR, headerCol);

            //ImGui.SetCursorScreenPos(new Vector2(winPos.X + 8, winPos.Y + 4));
            //ImGui.Text("Viewport Header");

            //ImGui.SameLine();
            
            if (ImGui.BeginMenuBar())
            {
                ImGui.Text("Viewport Header");

                ImGui.SetCursorScreenPos(new Vector2((winPos.X + winSize.X) - 88, winPos.Y));

                if (ImGui.BeginMenu("VPS")) // Viewport quick settings. TODO: replace with icon
                {
                    ImGui.Text("Viewport General");

                    ImGui.Separator();

                    ImGui.PushItemFlag(ImGuiItemFlags.AutoClosePopups, false);

                    if (ImGui.MenuItem("Solid", "", shadingType == ShadingType.Solid))
                    {
                        shadingType = ShadingType.Solid;
                    }
                    if (ImGui.MenuItem("Textured", "", shadingType == ShadingType.Textured))
                    {
                        shadingType = ShadingType.Textured;
                    }
                    if (ImGui.MenuItem("Textured Baked", "", shadingType == ShadingType.TexturedBaked))
                    {
                        shadingType = ShadingType.TexturedBaked;
                    }
                    if (ImGui.MenuItem("Wireframe", "", shadingType == ShadingType.Wireframe)) // wireframe only. no solids.
                    {
                        shadingType = ShadingType.Wireframe;
                        //showWireframeOverlay = false;
                    }

                    ImGui.PopItemFlag();

                    ImGui.Separator();

                    ImGui.BeginDisabled(shadingType == ShadingType.Wireframe);
                    ImGui.Checkbox("Wireframe Overlay", ref showWireframeOverlay);
                    ImGui.EndDisabled();

                    ImGui.Checkbox("Backface Culling", ref backFaceCulling);
                    ImGui.Checkbox("Light Colors", ref showLightColors);


                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("CMS")) // Camera quick settings. TODO: replace with icon
                {
                    ImGui.Text("Viewport Camera");

                    ImGui.Separator();

                    if (ImGui.RadioButton("Perspective", viewCamera3D.Projection == CameraProjection.Perspective))
                    {
                        viewCamera3D.Projection = CameraProjection.Perspective;
                    }
                    ImGui.SameLine();
                    if (ImGui.RadioButton("Orthographic", viewCamera3D.Projection == CameraProjection.Orthographic))
                    {
                        viewCamera3D.Projection = CameraProjection.Orthographic;
                    }

                    ImGui.DragFloat("Near Clip", ref nearClip, 0.01f, 0.000001f, 10000f);
                    ImGui.DragFloat("Far Clip", ref farClip, 1f, 0.000002f, 100000f);

                    if(nearClip<=0)
                    {
                        nearClip = 0.001f;
                    }

                    if(farClip <= 0)
                    {
                        farClip = 0.001f;
                    }

                    ImGui.EndMenu();
                }

                ImGui.EndMenuBar();
            }


            ImGui.SetCursorScreenPos(new Vector2(winPos.X + 8, winPos.Y + viewportHeaderHeight + 8));

            // Here you can render your scene texture / draw calls.
            // Example placeholder: show a child area representing the render target region
            //ImGui.BeginChild("viewport_content", new Vector2(winSize.X - 16, winSize.Y - viewportHeaderHeight - 16), ImGuiChildFlags.None,
            //                 ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            ImGui.BeginChild("viewport_content", new Vector2(0, -ImGuiNative.igGetFrameHeightWithSpacing()), ImGuiChildFlags.None,
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            //ImGui.TextWrapped("This is the viewport area! Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test Test ");
            ImGui.EndChild();

            // Box-select rectangle overlay
            if (isBoxSelecting && Vector2.Distance(boxSelectStartScreen, boxSelectCurrentScreen) >= BoxSelectDragThresholdPx)
            {
                uint fillCol = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0.67f, 0f, 0.15f));
                uint borderCol = ImGui.ColorConvertFloat4ToU32(new Vector4(1f, 0.67f, 0f, 0.9f));
                drawList.AddRectFilled(boxSelectStartScreen, boxSelectCurrentScreen, fillCol);
                drawList.AddRect(boxSelectStartScreen, boxSelectCurrentScreen, borderCol);
            }

            ImGui.End();
            ImGui.PopStyleColor(2);
            ImGui.PopStyleVar(2);
        }

        public void GenerateRenderList()
        {
            RenderItems = new List<BaseObject>();

            // Patches and instances don't render through this list - patches go through the shared
            // TessellatedPatch batch and instances through TrickyModelMeshObject's render cache, both
            // driven directly in RenderUpdate() - but both have a no-op BaseObject.Render(), so adding
            // them here is safe and makes them reachable for viewport picking (see Picking.cs).
            //RenderItems.AddRange(TrickyDataManager.trickyModelObjects);
            RenderItems.AddRange(TrickyDataManager.trickyPatchObjects);
            RenderItems.AddRange(TrickyDataManager.trickyInstanceObjects);
            RenderItems.AddRange(TrickyDataManager.trickySplineObjects);
            RenderItems.AddRange(TrickyDataManager.trickyAIPAIPath);
            RenderItems.AddRange(TrickyDataManager.trickyAIPRaceLine);
            RenderItems.AddRange(TrickyDataManager.trickySOPAIPath);
            RenderItems.AddRange(TrickyDataManager.trickySOPRaceLine);
            RenderItems.AddRange(TrickyDataManager.trickyLightObjects);
            RenderItems.AddRange(TrickyDataManager.trickyCameraObjects);
            RenderItems.AddRange(TrickyDataManager.trickyPaticleInstanceObjects);
        }

        public override void LogicUpdate()
        {
            HandleSelectionInput();

            //Update Camera
            //Raylib.UpdateCamera(ref viewCamera3D, CameraMode.Free);
            // Viewport Camera
            if (Input.IsActionDown("CameraActivate"))
            {
                Raylib.HideCursor();

                // Look
                Vector2 mouseDelta = Raylib.GetMouseDelta();
                yaw += mouseDelta.X * mouseSensitivity;
                pitch -= mouseDelta.Y * mouseSensitivity;
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);

                Raylib.SetMousePosition(screenWidth / 2, screenHeight / 2);

                Vector3 forward = new Vector3(MathF.Cos(pitch) * MathF.Sin(yaw), MathF.Cos(pitch) * MathF.Cos(yaw), MathF.Sin(pitch));
                Vector3 right = new Vector3(MathF.Sin(yaw - MathF.PI / 2f), MathF.Cos(yaw - MathF.PI / 2f), 0f);
                Vector3 up = Raymath.Vector3CrossProduct(forward, right);

                viewCamera3D.Up = up;

                // Movement
                Vector3 newPosition = new Vector3(0, 0, 0);
                // Vector3 newPosition = Raymath.Vector3Zero();
                float currentSpeed = moveSpeed;
                if (Input.IsActionDown("CameraBoost")) currentSpeed *= BOOST_MULTIPLIER;
                if (Input.IsActionDown("CameraMoveForward")) newPosition += forward;
                if (Input.IsActionDown("CameraMoveBack")) newPosition -= forward;
                if (Input.IsActionDown("CameraMoveRight")) newPosition -= right;
                if (Input.IsActionDown("CameraMoveLeft")) newPosition += right;
                if (Input.IsActionDown("CameraMoveUp")) newPosition += up;
                if (Input.IsActionDown("CameraMoveDown")) newPosition -= up;
                viewCamera3D.Position += Raymath.Vector3Normalize(newPosition) * currentSpeed;

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    moveSpeed += wheel * moveSpeedStep;
                    moveSpeed = Math.Clamp(moveSpeed, 0.008f, 200f);
                    //Debug.WriteLine(moveSpeed, currentSpeed.ToString());
                }
                viewCamera3D.Target = viewCamera3D.Position + forward;
            }
            if (Input.IsActionReleased("CameraActivate"))
            {
                Raylib.ShowCursor();
            }
        }

        private void HandleSelectionInput()
        {
            // Right-click flycam owns mouse input while active (left click doubles as its speed boost).
            if (Input.IsActionDown("CameraActivate"))
            {
                isBoxSelecting = false;
                return;
            }

            Vector2 mouseScreen = Raylib.GetMousePosition();
            bool overViewport = winSize.X > 0 && winSize.Y > 0
                && mouseScreen.X >= winPos.X && mouseScreen.X <= winPos.X + winSize.X
                && mouseScreen.Y >= winPos.Y && mouseScreen.Y <= winPos.Y + winSize.Y;

            // Note: ImGui's WantCaptureMouse is true just from hovering the (transparent) Viewport
            // window itself, not only when hovering an actual widget - it can't be used to guard
            // this. IsAnyItemHovered() only fires for real widgets (the VPS/CMS menu buttons etc.),
            // so it correctly leaves empty viewport space free for picking.
            if (Input.IsActionPressed("Click") && overViewport && !ImGui.IsAnyItemHovered())
            {
                isBoxSelecting = true;
                boxSelectStartScreen = mouseScreen;
                boxSelectCurrentScreen = mouseScreen;
            }

            if (!isBoxSelecting) return;

            boxSelectCurrentScreen = mouseScreen;

            if (Input.IsActionReleased("Click"))
            {
                isBoxSelecting = false;

                bool ctrl = Raylib.IsKeyDown(KeyboardKey.LeftControl) || Raylib.IsKeyDown(KeyboardKey.RightControl);
                bool shift = Raylib.IsKeyDown(KeyboardKey.LeftShift) || Raylib.IsKeyDown(KeyboardKey.RightShift);

                if (Vector2.Distance(boxSelectStartScreen, mouseScreen) < BoxSelectDragThresholdPx)
                {
                    // Simple click
                    BaseObject? hit = Picking.Pick(mouseScreen, winPos, winSize, viewCamera3D, RenderItems);

                    if (hit != null)
                    {
                        SelectionManager.Click(hit, ctrl, shift);
                    }
                    else if (!ctrl && !shift)
                    {
                        SelectionManager.Clear();
                    }
                }
                else
                {
                    // Box select
                    var hits = Picking.PickBox(boxSelectStartScreen, mouseScreen, winPos, winSize, viewCamera3D, RenderItems);
                    SelectionManager.Box(hits, ctrl || shift);
                }
            }
        }

        public enum ShadingType
        {
            Solid,
            Textured,
            TexturedBaked,
            Wireframe,
        }
    }
}