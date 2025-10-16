using IceSaw2.EditorWindows;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;

namespace IceSaw2.Manager.Tricky
{
    public class TrickyWorldManager
    {
        public static TrickyWorldManager instance = null;

        public LevelEditorWindow levelEditorWindow = new();
        public ModelsEditorWindow prefabEditorWindow = new();
        public LogicEditorWindow logicEditorWindow = new();

        public WindowMode windowMode = WindowMode.World;

        static IMGuiFilePicker filePicker = new();

        //Icon List
        public Texture2D LightIcon = new();
        public Texture2D CameraIcon = new();
        public Texture2D ParticleIcon = new();

        public Texture2D ErrorTexture = new();

        bool showAboutWindow = false;

        public TrickyWorldManager()
        {
            instance = this;

            filePicker = new IMGuiFilePicker(Settings.General.Instance.data.LastLoad);

            levelEditorWindow.Initilize();
            prefabEditorWindow.Initilize();
            //logicEditorWindow.Initilize();

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

            ErrorTexture = Raylib.LoadTextureFromImage(LoadEmbededImage.LoadImage("Error.png"));
        }

        public void UpdateLogic()
        {
            // if (Raylib.IsKeyPressed(Settings.KeyBinding.Instance.data.LogicWindow))
            if (Input.IsActionPressed("LogicSwitch"))
            {
                windowMode = WindowMode.Logic;
            }
            if (Input.IsActionPressed("ModelSwitch"))
            {
                windowMode = WindowMode.Prefabs;
            }
            if (Input.IsActionPressed("LevelSwitch"))
            {
                windowMode = WindowMode.World;
            }
        }

        public void UpdateRender()
        {
            Raylib.DrawFPS(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() - 30);
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
                            Settings.General.Instance.data.LastLoad = Path.GetDirectoryName(selectedPath) ?? "";
                            Settings.General.Instance.Save();
                            Settings.KeyBinding.Instance.Save();
                            levelEditorWindow.GenerateRenderList();
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

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("About"))
                    {
                        showAboutWindow = !showAboutWindow;
                    }
                    ImGui.EndMenu();
                }

                ImGui.SameLine(Raylib.GetScreenWidth()-170);

                if (ImGui.MenuItem("Level", "", windowMode==WindowMode.World))
                {
                    windowMode = WindowMode.World;
                }
                if (ImGui.MenuItem("Models", "" , windowMode == WindowMode.Prefabs))
                {
                    windowMode = WindowMode.Prefabs;
                }
                if (ImGui.MenuItem("Logic", "", windowMode == WindowMode.Logic))
                {
                    windowMode = WindowMode.Logic;
                }

                ImGui.EndMainMenuBar();

                if (showAboutWindow)
                {
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(300, 150), ImGuiCond.FirstUseEver);
                    ImGui.Begin("About", ref showAboutWindow, ImGuiWindowFlags.NoResize);

                    ImGui.Text("Ice Saw 2");
                    ImGui.Separator();
                    ImGui.Text("Created by: Archy, Platanito, Linkz");

                    ImGui.Text("Visit our project:");
                    ImGui.SameLine();

                    // Simulate a blue, underlined clickable link
                    ImGui.PushStyleColor(ImGuiCol.Text, new System.Numerics.Vector4(0.0f, 0.5f, 1.0f, 1.0f)); // Blue
                    ImGui.Text("Github");
                    ImGui.PopStyleColor();

                    // Make it look clickable
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                        ImGui.BeginTooltip();
                        ImGui.Text("Open in browser");
                        ImGui.EndTooltip();
                    }

                    if (ImGui.IsItemClicked())
                    {
                        try
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://github.com/GlitcherOG/Ice-Saw-2",
                                UseShellExecute = true
                            });
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed to open link: " + ex.Message);
                        }
                    }

                    ImGui.Text("© SSX Modding");
                    ImGui.Spacing();

                    if (ImGui.Button("OK"))
                    {
                        showAboutWindow = false;
                    }

                    ImGui.End();
                }
            }
        }




        public enum WindowMode
        {
            World,
            Prefabs,
            Logic
        }

    }
}
