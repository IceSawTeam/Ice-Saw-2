using IceSaw2.EditorWindows;
// using IceSaw2.LevelObject;
// using IceSaw2.LevelObject.Materials;
// using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Settings;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
// using SSXMultiTool.JsonFiles.Tricky;
// using System.Numerics;

namespace IceSaw2.Manager
{
    public class WorldManager
    {
        public static WorldManager instance = new();

        public LevelEditorWindow levelEditorWindow = new();
        public PrefabEditorWindow prefabEditorWindow = new();
        public MaterialEditorWindow materialEditorWindow = new();
        public TextureEditorWindow textureEditorWindow = new();

        public int heightScreen { get { return Raylib.GetScreenHeight(); } }
        public int widthScreen { get { return Raylib.GetScreenWidth(); } }

        public WindowMode windowMode = WindowMode.World;

        static IMGuiFilePicker filePicker = new();

        public GeneralSettings generalSettings = new();
        public HotkeySettings hotkeySettings = new();

        //Icon List
        public Texture2D LightIcon = new();
        public Texture2D CameraIcon = new();
        public Texture2D ParticleIcon = new();

        public WorldManager()
        {
            instance = this;
            LoadSettings();

            filePicker = new IMGuiFilePicker(generalSettings.LastLoad);

            levelEditorWindow.Initilize();
            prefabEditorWindow.Initilize();
            materialEditorWindow.Initilize();

            Raylib.InitWindow(generalSettings.ScreenWidth, generalSettings.ScreenHeight, "Ice Saw 2");
            rlImGui.Setup(true);
            InitalizeAssets();
            Raylib.SetWindowState(ConfigFlags.ResizableWindow);
            Update();
        }

        public void LoadSettings()
        {
            string SaveFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IceSaw2");
            if(!Directory.Exists(SaveFolder))
            {
                Directory.CreateDirectory(SaveFolder);
            }


            generalSettings = GeneralSettings.Load(Path.Combine(SaveFolder,"Settings.json"));
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

        ~WorldManager()
        {
            // Cleanup before shutdown
            DataManager.UnloadProject();
            Raylib.CloseWindow();
        }

        public void InitalizeAssets()
        {
            LightIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("LightIcon.png"));
            CameraIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("CameraIcon.png"));
            ParticleIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("ParticleIcon.png"));
        }

        public void Update()
        {
            while (!Raylib.WindowShouldClose())
            {
                //Logic Update
                switch (windowMode)
                {
                    case WindowMode.World: levelEditorWindow.LogicUpdate(); break;
                    case WindowMode.Prefabs: prefabEditorWindow.LogicUpdate(); break;
                    case WindowMode.Materials: materialEditorWindow.LogicUpdate(); break;
                    case WindowMode.Textures: textureEditorWindow.LogicUpdate(); break;
                }
                UpdateLogic();

                Raylib.BeginDrawing();
                rlImGui.Begin();
                Raylib.ClearBackground(Color.White);
                Rlgl.DisableBackfaceCulling();

                switch (windowMode)
                {
                    case WindowMode.World: levelEditorWindow.RenderUpdate(); break;
                    case WindowMode.Prefabs: prefabEditorWindow.RenderUpdate(); break;
                    case WindowMode.Materials: materialEditorWindow.RenderUpdate(); break;
                    case WindowMode.Textures: textureEditorWindow.RenderUpdate(); break;
                }
                Render();

                Raylib.DrawText("Beta Test", 12, widthScreen - 20, 20, Color.Black);
                rlImGui.End();
                Raylib.EndDrawing();
            }
        }

        public void UpdateLogic()
        {
            if (Raylib.IsKeyPressed(hotkeySettings.MaterialWindow))
            {
                windowMode = WindowMode.Materials;
            }
            if (Raylib.IsKeyPressed(hotkeySettings.PrefabWindow))
            {
                windowMode = WindowMode.Prefabs;
            }
            if (Raylib.IsKeyPressed(hotkeySettings.LevelWindow))
            {
                windowMode = WindowMode.World;
            }
        }

        public void Render()
        {
            Raylib.DrawFPS(widthScreen - 150, heightScreen - 30);
            filePicker.Render();
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..."))
                    {
                        filePicker.Show((selectedPath) =>
                        {
                            DataManager.LoadProject(selectedPath);
                            WorldManager.instance.generalSettings.LastLoad = Path.GetDirectoryName(selectedPath);
                            WorldManager.instance.SaveSettings();
                            // Do something with selectedPath
                        });
                        // Handle file open
                        //filePicker.Open();
                    }

                    if (ImGui.MenuItem("Save"))
                    {
                        // Handle save
                    }

                    if (ImGui.MenuItem("Exit"))
                    {
                        // Handle exit
                        Environment.Exit(0);
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Edit"))
                {
                    if (ImGui.MenuItem("Undo")) { }
                    if (ImGui.MenuItem("Redo")) { }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Window"))
                {
                    if (ImGui.MenuItem("Level"))
                    {
                        windowMode = WindowMode.World;
                    }
                    if (ImGui.MenuItem("Prefab"))
                    {
                        windowMode = WindowMode.Prefabs;
                    }
                    if (ImGui.MenuItem("Material"))
                    {
                        windowMode = WindowMode.Materials;
                    }
                    if (ImGui.MenuItem("Texture"))
                    {
                        windowMode = WindowMode.Textures;
                    }
                    ImGui.EndMenu();
                }

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("About"))
                    {
                        // Show About modal
                    }
                    ImGui.EndMenu();
                }

                ImGui.EndMainMenuBar();
            }
        }




        public enum WindowMode
        {
            World,
            Materials,
            Prefabs,
            Textures
        }

    }
}
