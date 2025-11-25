/*
    This is the engine core. The main game loop runs in main, but the callbacks are all here.
    All other modules or managers will be initliazed in Core. Core keeps a general state of the program,
    subsystems should have a state of their own for more specialized states.
    There should be no implementation methods defined here for any of the systems.
*/
#pragma warning disable IDE0079
#pragma warning disable CA1822

using IceSaw2.Manager.Tricky;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;
using System.Numerics;

namespace IceSaw2
{
    public class Core
    {
        public const int MaxFps = 120;

        public bool isRunning = true;
        private RenderTexture2D _viewportTexture;
        private Vector2 _lastViewportPos = Vector2.Zero;
        private Vector2 _lastViewportSize = Vector2.Zero;

        //--------------------- Manager/Module/System declarations here -------------------------
        public static TrickyWorldManager? worldManager = null;

        public Core()
        {
            ConsoleWindow.GenerateConsole();

            Raylib.InitWindow(1280, 720, "Ice Saw 2");
            Raylib.SetWindowIcon(LoadEmbeddedFile.LoadImage("icon.png"));
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
            Raylib.SetWindowMinSize(1280, 720);
            Raylib.SetWindowState(windowFlags);

            FontLoader InterNewFont = new();

            rlImGui.SetupUserFonts += (ImGuiIOPtr imGuiIo) =>
            {
                var io = ImGui.GetIO();
                io.Fonts.Clear();
                InterNewFont.LoadFont("Fonts.Inter_New.ttf", 14f);
            };
            rlImGui.Setup(true);

            if (!Settings.General.Instance.data.ConsoleWindow)
            {
                ConsoleWindow.CloseConsole();
            }

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

            // Update render texture if screen size changed
            Vector2 winPos = Vector2.Zero;
            Vector2 winSize = Vector2.Zero;
            if (worldManager != null)
            {
                if (worldManager.windowMode == TrickyWorldManager.WindowMode.World)
                {
                    winPos = worldManager.levelEditorWindow.winPos;
                    winSize = worldManager.levelEditorWindow.winSize;
                } else if (worldManager.windowMode == TrickyWorldManager.WindowMode.Prefabs)
                {
                    winPos = worldManager.prefabEditorWindow.winPos;
                    winSize = worldManager.prefabEditorWindow.winSize;
                }
             }

            if (winPos != _lastViewportPos || winSize != _lastViewportSize)
            {
                if (Raylib.IsRenderTextureValid(_viewportTexture))
                {
                    Raylib.UnloadRenderTexture(_viewportTexture);
                }
                _viewportTexture = Raylib.LoadRenderTexture((int)winSize.X, (int)winSize.Y);
                _lastViewportPos = winPos;
                _lastViewportSize = winSize;
                // Note: Fov might not match since the camera is in the level editor class
                Rlgl.SetMatrixProjection(Raymath.MatrixPerspective(65, winSize.X/winSize.Y, Rlgl.GetCullDistanceNear(), Rlgl.GetCullDistanceFar()));
            }

            // Render viewport to texture
            Raylib.BeginTextureMode(_viewportTexture);
            Raylib.ClearBackground(new Color(120, 120, 120));
            if (worldManager != null)
            {
                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.RenderUpdate(); break;
                }
             }
            Raylib.EndTextureMode();

            // Render viewport texture and GUI
            Raylib.BeginDrawing();
            Raylib.DrawTexturePro(_viewportTexture.Texture,
                    new Rectangle(0, 0, _viewportTexture.Texture.Width, -_viewportTexture.Texture.Height),
                    new Rectangle(_lastViewportPos.X, _lastViewportPos.Y, _lastViewportSize.X, _lastViewportSize.Y),
                    Vector2.Zero, 0, Color.White);
            rlImGui.Begin();
            if (worldManager != null)
            {
                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.RenderUI(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.RenderUI(); break;
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
