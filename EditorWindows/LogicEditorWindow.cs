using IceSaw2.Manager.Tricky;
using ImGuiNET;
using Raylib_cs;

namespace IceSaw2.EditorWindows
{
    public class LogicEditorWindow : BaseEditorWindow
    {
        private int screenWidth { get { return Raylib.GetScreenWidth(); } }
        private int screenHeight { get { return Raylib.GetScreenHeight(); } }

        public void Initilize()
        {

        }

        public override void RenderUpdate()
        {
            //UI render

            float menuBarHeight = ImGui.GetFrameHeight(); // Typically height of main menu bar

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

            // Add your sidebar content here

            ImGui.End();
            ImGui.PopStyleVar(2);

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
