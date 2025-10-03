/*
This is the engine core. The main game loop runs here. All other modules
or managers will be linked to Core.
*/

using IceSaw2.Manager.Tricky;
using IceSaw2.Settings;
using Raylib_cs;
using rlImGui_cs;

namespace IceSaw2
{
    public class Core
    {
        //public enum State
        //{
        //    MAIN_WINDOW,
        //    OPENING_FILE,
        //    CONTROLLING_CAMERA,
        //    USING_XFORM_GIZMO,
        //    ADDING_ENTITIY,
        //    MOVING_HIERARCHY_ENTITY,
        //    PATCH_EDITING_SAC_MODE,
        //    OTHER,
        //}
        //public const int MAX_FPS = 60;

        //public bool isRunning = true;
        //// Raylib already has a GetFrameTime function, maybe set this to it every frame for ease of use?
        //private float delta { get { return Raylib.GetFrameTime(); } }
        //public State currentState = State.MAIN_WINDOW;
        public static Core instance = null;
        public static TrickyWorldManager worldManager = null;
        public GeneralSettings generalSettings = new();
        public HotkeySettings hotkeySettings = new();

        public int heightScreen { get { return Raylib.GetScreenHeight(); } }
        public int widthScreen { get { return Raylib.GetScreenWidth(); } }

        public Core()
        {
            instance = this;
            LoadSettings();

            Raylib.InitWindow(generalSettings.ScreenWidth, generalSettings.ScreenHeight, "Ice Saw 2");
            rlImGui.Setup(true);
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            worldManager = new TrickyWorldManager();
            Update();
        }


        ~Core()
        {
            // Close raylib
            // Close managers
            // Clear cache
            // Close window
        }

        public void LoadSettings()
        {
            string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            if (!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }


            generalSettings = GeneralSettings.Load(Path.Combine(SaveFolder, "Settings.json"));
            hotkeySettings = HotkeySettings.Load(Path.Combine(SaveFolder, "Hotkeys.json"));

            //Incase of version Change save back to latest version
            SaveSettings();
        }

        public void SaveSettings()
        {
            string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            generalSettings.CreateJson(Path.Combine(SaveFolder, "Settings.json"));
            hotkeySettings.CreateJson(Path.Combine(SaveFolder, "Hotkeys.json"));
        }

        public void Update()
        {
            while (!Raylib.WindowShouldClose())
            {
                //Logic Update
                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Materials: worldManager.materialEditorWindow.LogicUpdate(); break;
                    case TrickyWorldManager.WindowMode.Textures: worldManager.textureEditorWindow.LogicUpdate(); break;
                }
                worldManager.UpdateLogic();

                Raylib.BeginDrawing();
                rlImGui.Begin();
                Raylib.ClearBackground(new Color(120, 120, 120));
                Rlgl.DisableBackfaceCulling();

                switch (worldManager.windowMode)
                {
                    case TrickyWorldManager.WindowMode.World: worldManager.levelEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Prefabs: worldManager.prefabEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Materials: worldManager.materialEditorWindow.RenderUpdate(); break;
                    case TrickyWorldManager.WindowMode.Textures: worldManager.textureEditorWindow.RenderUpdate(); break;
                }
                worldManager.UpdateRender();

                Raylib.DrawText("Beta Test", 12, widthScreen - 20, 20, Color.Black);
                rlImGui.End();
                Raylib.EndDrawing();
            }
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
        }


        public void RenderProcessing()
        {
            // Render the 3D viewport including world gizmos and xform gizmo.
            // Render props, terrain, paths, wireframe, skybox, and bounding boxes.
            // Render Imgui. Lock or anchor the Imgui scale to 1080p, no need to big resolution flexibility.
            // Cap framerate
        }
    }
}
