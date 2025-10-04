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

namespace IceSaw2.Manager.Tricky
{
    public class TrickyWorldManager
    {
        public static TrickyWorldManager instance = null;

        public LevelEditorWindow levelEditorWindow = new();
        public PrefabEditorWindow prefabEditorWindow = new();
        public MaterialEditorWindow materialEditorWindow = new();
        public TextureEditorWindow textureEditorWindow = new();

        public WindowMode windowMode = WindowMode.World;

        static IMGuiFilePicker filePicker = new();

        //Icon List
        public Texture2D LightIcon = new();
        public Texture2D CameraIcon = new();
        public Texture2D ParticleIcon = new();

        public TrickyWorldManager()
        {
            instance = this;

            filePicker = new IMGuiFilePicker(Settings.Manager.Instance.General.LastLoad);

            levelEditorWindow.Initilize();
            prefabEditorWindow.Initilize();
            materialEditorWindow.Initilize();

            InitalizeAssets();
        }


        ~TrickyWorldManager()
        {
            // Cleanup before shutdown
            TrickyDataManager.UnloadProject();
            Raylib.CloseWindow();
        }

        public void InitalizeAssets()
        {
            LightIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("LightIcon.png"));
            CameraIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("CameraIcon.png"));
            ParticleIcon = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("ParticleIcon.png"));
        }

        public void UpdateLogic()
        {
            if (Raylib.IsKeyPressed(Settings.Manager.Instance.Hotkey.MaterialWindow))
            {
                windowMode = WindowMode.Materials;
            }
            if (Raylib.IsKeyPressed(Settings.Manager.Instance.Hotkey.PrefabWindow))
            {
                windowMode = WindowMode.Prefabs;
            }
            if (Raylib.IsKeyPressed(Settings.Manager.Instance.Hotkey.LevelWindow))
            {
                windowMode = WindowMode.World;
            }
        }

        public void UpdateRender()
        {
            Raylib.DrawFPS(Raylib.GetScreenWidth() - 150, Raylib.GetScreenHeight() - 30);
            filePicker.Render();
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open..."))
                    {
                        filePicker.Show((selectedPath) =>
                        {
                            TrickyDataManager.LoadProject(selectedPath);
                            Settings.Manager.Instance.General.LastLoad = Path.GetDirectoryName(selectedPath) ?? "";
                            Settings.Manager.Instance.SaveSettings();
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
