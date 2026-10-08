using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using SharpGLTF.Schema2;
using System.Numerics;
using static ICSharpCode.SharpZipLib.Zip.ExtendedUnixData;

namespace IceSaw2.EditorWindows
{
    public class LogicEditorWindow : BaseEditorWindow
    {
        private int screenWidth { get { return Raylib.GetScreenWidth(); } }
        private int screenHeight { get { return Raylib.GetScreenHeight(); } }

        Camera3D camera3D = new Camera3D();
        public Vector2 winPos;
        public Vector2 winSize;

        public void Initilize()
        {
            camera3D.Position = new System.Numerics.Vector3(0, 15, 3);
            camera3D.Target = Vector3.Zero;
            camera3D.Up = new Vector3(0, 0, 1);
            camera3D.FovY = 45f;
            camera3D.Projection = CameraProjection.Perspective;
        }

        public void RenderUI()
        {
            // Render UI
            var io = ImGui.GetIO();
            var vp = ImGui.GetMainViewport();
            var vpPos = vp.Pos;
            var vpSize = vp.Size;
            float menuBarHeight = ImGui.GetFrameHeight();

            winPos = ImGui.GetWindowPos();
            winSize = ImGui.GetWindowSize();

            // Dimensions
            menuBarHeight += 0; // Note: this includes both top bars.
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

            // Sidebar dimensions
            int sidebarWidth = 300;

            // Position and size sidebar *below* the main menu bar
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(sidebarWidth, screenHeight - menuBarHeight), ImGuiCond.Always);

            // Optional: remove decorations
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Side Panel", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);

            HierarchyRender("Effects", "Effects", TrickyDataManager.trickyEffectHeaders);

            HierarchyRender("Functions", "Functions", TrickyDataManager.trickyFunctionHeaders);

            HierarchyRender("Effect Slots", "Effect Slots", TrickyDataManager.trickyEffectSlotObjects);

            HierarchyRender("Physics", "Physics", TrickyDataManager.trickyPhysicsObjects);

            ImGui.End();
            ImGui.PopStyleVar(2);


            float centerX = vpPos.X + outlinerWidth;
            ImGui.SetNextWindowPos(new Vector2(centerX, vpPos.Y + menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new Vector2(viewportWidth, vpSize.Y), ImGuiCond.Always);

            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0f, 0f, 0f, 0f));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Viewport", flags);

            var drawList = ImGui.GetWindowDrawList();
            winPos = ImGui.GetWindowPos();
            winSize = ImGui.GetWindowSize();

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
            ImGui.EndChild();

            ImGui.End();
            ImGui.PopStyleColor();
            ImGui.PopStyleVar(2);

        }

        public override void RenderUpdate()
        {
            Raylib.UpdateCamera(ref camera3D, CameraMode.Orbital);

            Raylib.BeginMode3D(camera3D);

            RaylibCustomGrid.DrawBasic3DGrid(10, 1, Color.Black);

            Raylib.EndMode3D();
        }

        public void HierarchyRender(string ObjectName, string ID, List<TrickyEffectHeader> baseObjects)
        {
                var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

                if (baseObjects.Count == 0)
                    flags |= ImGuiTreeNodeFlags.Leaf;

                bool nodeOpen = ImGui.TreeNodeEx(ObjectName + "###" + ID, flags);

                // Handle selection or context menu if needed
                if (ImGui.IsItemClicked())
                {
                    Console.WriteLine($"Selected: " + ObjectName + "###" + ID);
                }

                if (nodeOpen)
                {
                    for (global::System.Int32 i = 0; i < baseObjects.Count; i++)
                    {
                        baseObjects[i].HierarchyRender();
                    }
                    ImGui.TreePop();
                }
        }

        public void HierarchyRender(string ObjectName, string ID, List<TrickyFunctionHeader> baseObjects)
        {
            var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

            if (baseObjects.Count == 0)
                flags |= ImGuiTreeNodeFlags.Leaf;

            bool nodeOpen = ImGui.TreeNodeEx(ObjectName + "###" + ID, flags);

            // Handle selection or context menu if needed
            if (ImGui.IsItemClicked())
            {
                Console.WriteLine($"Selected: " + ObjectName + "###" + ID);
            }

            if (nodeOpen)
            {
                for (global::System.Int32 i = 0; i < baseObjects.Count; i++)
                {
                    baseObjects[i].HierarchyRender();
                }
                ImGui.TreePop();
            }
        }

        public void HierarchyRender(string ObjectName, string ID, List<TrickyEffectSlotObject> baseObjects)
        {
            var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

            if (baseObjects.Count == 0)
                flags |= ImGuiTreeNodeFlags.Leaf;

            bool nodeOpen = ImGui.TreeNodeEx(ObjectName + "###" + ID, flags);

            // Handle selection or context menu if needed
            if (ImGui.IsItemClicked())
            {
                Console.WriteLine($"Selected: " + ObjectName + "###" + ID);
            }

            if (nodeOpen)
            {
                for (global::System.Int32 i = 0; i < baseObjects.Count; i++)
                {
                    baseObjects[i].HierarchyRender();
                }
                ImGui.TreePop();
            }
        }

        public void HierarchyRender(string ObjectName, string ID, List<TrickyPhysicsObject> baseObjects)
        {
            var flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.SpanAvailWidth;

            if (baseObjects.Count == 0)
                flags |= ImGuiTreeNodeFlags.Leaf;

            bool nodeOpen = ImGui.TreeNodeEx(ObjectName + "###" + ID, flags);

            // Handle selection or context menu if needed
            if (ImGui.IsItemClicked())
            {
                Console.WriteLine($"Selected: " + ObjectName + "###" + ID);
            }

            if (nodeOpen)
            {
                for (global::System.Int32 i = 0; i < baseObjects.Count; i++)
                {
                    baseObjects[i].HierarchyRender();
                }
                ImGui.TreePop();
            }
        }
    }
}
