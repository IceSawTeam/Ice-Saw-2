/*
    This is the engine core. The main game loop runs in main, but the callbacks are all here.
    All other modules or managers will be initliazed in Core. Core keeps a general state of the program,
    subsystems should have a state of their own for more specialized states.
    There should be no implementation methods defined here for any of the systems.
*/
#pragma warning disable IDE0079
#pragma warning disable CA1822

using IceSaw2.Manager.Tricky;
using Raylib_cs;
using rlImGui_cs;

namespace IceSaw2
{
    public class Core
    {
        public const int MaxFps = 120;

        public bool isRunning = true;

        //--------------------- Manager/Module/System declarations here -------------------------
        public static TrickyWorldManager? worldManager = null;


        public Core()
        {
            Raylib.InitWindow(1280, 720, "Ice Saw 2");
            Raylib.SetTargetFPS(MaxFps);

            // Load settings
            Settings.General.Instance.Load();
            Settings.KeyBinding.Instance.Load();
            ConfigFlags windowFlags = ConfigFlags.ResizableWindow;
            if (Settings.General.Instance.data.isMaximized) windowFlags |= ConfigFlags.MaximizedWindow;
            Raylib.SetWindowPosition((int)Settings.General.Instance.data.windowPositionX,
                                 (int)Settings.General.Instance.data.windowPositionY);
            Raylib.SetWindowSize((int)Settings.General.Instance.data.windowWidth,
                                 (int)Settings.General.Instance.data.windowHeight);
            Raylib.SetWindowState(windowFlags);

            rlImGui.Setup(true);

            //--------------------- Manager/Module/System initializations here -------------------------
            worldManager = new TrickyWorldManager();
        }


        public void Exiting()
        {
            Settings.General.Instance.Save();
            Settings.KeyBinding.Instance.Save();
            rlImGui.Shutdown();
            Raylib.CloseWindow();
        }
        

        public void InputProccessing()
        {
            // Check for user input with ImGui and on the 3D viewport.
            // GUI should consume input so it doesnt pass to the 3D viewport.
            // Gizmo input handling.
            // Terrain Patch collision detection with mouse might require multi-threading.
            // Can change State from here.
        }


        public void LogicProccessing()
        {
            // Set the isRunning variable to exit loop.
            // Can change State from here.
            // Communicate with other modules and managers.

            if (worldManager != null)
            {
                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Logic: worldManager.logicEditorWindow.LogicUpdate(); break;
                }
                worldManager.UpdateLogic();
            }
        }


        public void RenderProcessing()
        {
            // Render the 3D viewport including world gizmos and xform gizmo.
            // Render props, terrain, paths, wireframe, skybox, and bounding boxes.
            // Render Imgui. Lock or anchor the Imgui scale to 1080p, no need to big resolution flexibility.

            Raylib.BeginDrawing();
            rlImGui.Begin();
            Raylib.ClearBackground(new Color(120, 120, 120));
            Rlgl.DisableBackfaceCulling();

            if (worldManager != null)
            {
                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Logic: worldManager.logicEditorWindow.RenderUpdate(); break;
                }
                worldManager.UpdateRender();
             }

            Raylib.DrawText("Beta Test", 12, Raylib.GetScreenWidth() - 20, 20, Color.Black);
            rlImGui.End();
            Raylib.EndDrawing();
        }
    }
}
