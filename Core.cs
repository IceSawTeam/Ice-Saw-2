/*
    This is the engine core. The main game loop runs in main, but the callbacks are all here.
    All other modules or managers will be initliazed in Core. Core keeps a general state of the program,
    subsystems should have a state of their own for more specialized states.
    There should be no implementation methods defined here for any of the systems.
*/

using IceSaw2.Manager.Tricky;
using Raylib_cs;
using rlImGui_cs;

namespace IceSaw2
{
    public class Core
    {
        // public enum State
        // {
        //    MAIN_WINDOW,
        //    OPENING_FILE,
        //    CONTROLLING_CAMERA,
        //    USING_XFORM_GIZMO,
        //    ADDING_ENTITIY,
        //    MOVING_HIERARCHY_ENTITY,
        //    PATCH_EDITING_SAC_MODE,
        //    OTHER,
        // }
        public const int MAX_FPS = 60;

        public bool isRunning = true;
        // public State currentState = State.MAIN_WINDOW;

        //--------------------- Manager/Module/System declarations here -------------------------
        public static TrickyWorldManager? worldManager = null;


        public Core()
        {
            Settings.General.Instance.Load();
            Settings.Hotkey.Instance.Load();
            Raylib.InitWindow(Settings.General.Instance.data.ScreenWidth,
                              Settings.General.Instance.data.ScreenHeight,
                              "Ice Saw 2");
            Raylib.SetTargetFPS(MAX_FPS);
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            rlImGui.Setup(true);

            //--------------------- Manager/Module/System initializations here -------------------------
            worldManager = new TrickyWorldManager();
        }

        ~Core()
        {
            // Close managers
            // Clear cache
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
                    case TrickyWorldManager.WindowMode.Materials: worldManager.materialEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Textures: worldManager.textureEditorWindow.LogicUpdate(); break;
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
                    case TrickyWorldManager.WindowMode.Materials: worldManager.materialEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Textures: worldManager.textureEditorWindow.RenderUpdate(); break;
                }
                worldManager.UpdateRender();
             }

            Raylib.DrawText("Beta Test", 12, Raylib.GetScreenWidth() - 20, 20, Color.Black);
            rlImGui.End();
            Raylib.EndDrawing();
        }
    }
}
