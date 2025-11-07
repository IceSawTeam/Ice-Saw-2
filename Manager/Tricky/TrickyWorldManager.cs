using IceSaw2.EditorWindows;
using IceSaw2.Utilities;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System.Diagnostics;
using System.Numerics;

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
        bool showImGuiDemo = false;
        bool showProfiler = true;

        const int FRAME_TIME_HISTORY_SIZE = 100;
        static float[] frameTimes = new float[FRAME_TIME_HISTORY_SIZE];
        static int frameIndex = 0;

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
            LightIcon = Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.LightIcon.png"));
            CameraIcon = Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.CameraIcon.png"));
            ParticleIcon = Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.ParticleIcon.png"));

            ErrorTexture = Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.Error.png"));
        }

        public void UpdateLogic()
        {
            
        }

        public void UpdateRender()
        {
            Raylib.DrawRectangle(Raylib.GetScreenWidth() / 2 - 8, Raylib.GetScreenHeight() - 32, 95, 26, Raylib.GetColor(0x000000FF));
            Raylib.DrawFPS(Raylib.GetScreenWidth() / 2, Raylib.GetScreenHeight() - 30);
            filePicker.Render();
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
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
                    if (ImGui.MenuItem("Console"))
                    {
                        Settings.General.Instance.data.ConsoleWindow = !Settings.General.Instance.data.ConsoleWindow;

                        if(Settings.General.Instance.data.ConsoleWindow)
                        {
                            ConsoleWindow.GenerateConsole();
                        }
                        else
                        {
                            ConsoleWindow.CloseConsole();
                        }
                        Settings.General.Instance.Save();
                    }
                    if (ImGui.MenuItem("ImGui Demo"))
                    {
                        showImGuiDemo = !showImGuiDemo;
                    }

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("[Load last level]"))
                {
                    string selectedPath = Settings.General.Instance.data.LastLoad + "/ConfigTricky.ssx";
                    if (File.Exists(selectedPath))
                    {
                        TrickyDataManager.LoadProject(selectedPath);
                        Settings.General.Instance.data.LastLoad = Path.GetDirectoryName(selectedPath) ?? "";
                        Settings.General.Instance.Save();
                        Settings.KeyBinding.Instance.Save();
                        levelEditorWindow.GenerateRenderList();
                    }
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

                if (showImGuiDemo)
                {
                    ImGui.ShowDemoWindow();
                }

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

                
                
                if (showProfiler)
                {
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, Raylib.GetScreenHeight() - 300), ImGuiCond.FirstUseEver);
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(160, 300), ImGuiCond.FirstUseEver);
                    ImGui.Begin("Profiler (ms)", ref showProfiler);

                    ImGui.Text($"Input: {Program.InputTime:F3}");
                    ImGui.Text($"Logic: {Program.LogicTime:F3}");
                    ImGui.Text($"Render: {Program.RenderTime:F3}");
                    ImGui.Text($"Total: {Program.TotalTime:F3}");

                    ImGui.Separator();

                    float frameTime = Raylib.GetFrameTime() * 1000.0f;

                    ImGui.Text($"FrameTime (Delta): {frameTime:F3}");

                    frameTimes[frameIndex] = frameTime;
                    frameIndex = (frameIndex + 1) % FRAME_TIME_HISTORY_SIZE;

                    ImGui.PlotLines(
                        label: "##FrameTimeGraph",
                        values: ref frameTimes[0],
                        values_count: FRAME_TIME_HISTORY_SIZE,
                        values_offset: frameIndex,
                        overlay_text: "ms/frame",
                        scale_min: 0,
                        scale_max: 63.3f, // ~60 FPS
                        graph_size: new Vector2(0, 80)
                    );

                    ImGui.End();
                }

                }
            ImGui.PopStyleVar(2);
        }




        public enum WindowMode
        {
            World,
            Prefabs,
            Logic
        }

    }
}
