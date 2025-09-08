using IceSaw2.EditorWindows;
using IceSaw2.LevelObject;
using IceSaw2.LevelObject.Materials;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Settings;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using SSXMultiTool.JsonFiles.Tricky;
using System.Numerics;

namespace IceSaw2.Manager
{
    public class WorldManager
    {
        public static WorldManager instance = new WorldManager();

        public LevelEditorWindow levelEditorWindow = new LevelEditorWindow();
        public PrefabEditorWindow prefabEditorWindow = new PrefabEditorWindow();
        public MaterialEditorWindow materialEditorWindow = new MaterialEditorWindow();
        public TextureEditorWindow textureEditorWindow = new TextureEditorWindow();

        public int heightScreen { get { return Raylib.GetScreenHeight(); } }
        public int widthScreen { get { return Raylib.GetScreenWidth(); } }

        public WindowMode windowMode = WindowMode.World;

        static IMGuiFilePicker filePicker = new IMGuiFilePicker();

        public void Initalise()
        {
            instance = this;

            levelEditorWindow.Initilize();
            prefabEditorWindow.Initilize();
            materialEditorWindow.Initilize();

            Raylib.InitWindow(GeneralSettings.ScreenWidth, GeneralSettings.ScreenHeight, "Ice Saw 2");
            rlImGui.Setup(true);

            Raylib.SetWindowState(ConfigFlags.ResizableWindow);

            Update();

            Raylib.CloseWindow();
        }

        public void Update()
        {
            while (!Raylib.WindowShouldClose())
            {
                //Logic Update

                if (windowMode == WindowMode.World)
                {
                    levelEditorWindow.LogicUpdate();
                }
                if(windowMode == WindowMode.Prefabs)
                {
                    prefabEditorWindow.LogicUpdate();
                }
                if (windowMode == WindowMode.Materials)
                {
                    materialEditorWindow.LogicUpdate();
                }
                if (windowMode == WindowMode.Textures)
                {
                    textureEditorWindow.LogicUpdate();
                }

                UpdateLogic();

                Raylib.BeginDrawing();
                rlImGui.Begin();
                Raylib.ClearBackground(Color.White);
                Rlgl.DisableBackfaceCulling();
                //Render();

                if (windowMode == WindowMode.World)
                {
                    levelEditorWindow.RenderUpdate();
                }
                if (windowMode == WindowMode.Prefabs)
                {
                    prefabEditorWindow.RenderUpdate();
                }
                if (windowMode == WindowMode.Materials)
                {
                    materialEditorWindow.RenderUpdate();
                }
                if(windowMode == WindowMode.Textures)
                {
                    textureEditorWindow.RenderUpdate();
                }

                Render();

                Raylib.DrawText("Beta Test", 12, widthScreen-20, 20, Color.Black);
                rlImGui.End();
                Raylib.EndDrawing();
            }
        }

        public void UpdateLogic()
        {
            if (Raylib.IsKeyPressed(HotkeySettings.MaterialWindow))
            {
                windowMode = WindowMode.Materials;
            }
            if (Raylib.IsKeyPressed(HotkeySettings.PrefabWindow))
            {
                windowMode = WindowMode.Prefabs;
            }
            if (Raylib.IsKeyPressed(HotkeySettings.LevelWindow))
            {
                windowMode = WindowMode.World;
            }
        }

        public void Render()
        {
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

                if(ImGui.BeginMenu("Window"))
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
