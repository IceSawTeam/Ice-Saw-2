using IceSaw2.EditorWindows.SSX3EditorWindows;
using IceSaw2.Manager.Tricky;
using IceSaw2.Renderer;
using IceSaw2.Settings;
using IceSaw2.Utilities;
using ImGuiNET;
using NativeFileDialogSharp;
using Raylib_cs;
using SSXLibrary;
using SSXLibrary.FileHandlers.LevelFiles.SSX3PS2;
using System.Diagnostics;

namespace IceSaw2.Manager.SSX3
{
    internal class SSX3WorldManager
    {
        public static SSX3WorldManager instance = null;

        public SSX3LevelEditorWindow levelEditorWindow = new();
        //public ModelsEditorWindow prefabEditorWindow = new();
        //public LogicEditorWindow logicEditorWindow = new();

        public WindowMode windowMode = WindowMode.World;

        //Icon List
        public List<Texture2D> LightIcons = new List<Texture2D>();
        public Texture2D CameraIcon = new();
        public Texture2D ParticleIcon = new();

        public Texture2D ErrorTexture = new();

        bool showAboutWindow = false;
        bool showImGuiDemo = false;
        bool showProfiler = false;
        bool showSettings = false;

        string[] LevelIDs = { "None" };
        public int SelectedLevel = 0;

        public SSX3WorldManager()
        {
            instance = this;

            levelEditorWindow.Initilize();
            //prefabEditorWindow.Initilize();
            //logicEditorWindow.Initilize();

            InitalizeAssets();
        }


        ~SSX3WorldManager()
        {
            // Cleanup before shutdown
            SSX3DataManager.UnloadProject();
            Raylib.CloseWindow();
        }

        public void InitalizeAssets()
        {
            LightIcons.Add(Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.LightDirectionalIcon.png")));
            LightIcons.Add(Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.LightSpotIcon.png")));
            LightIcons.Add(Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.LightPointIcon.png")));
            LightIcons.Add(Raylib.LoadTextureFromImage(LoadEmbeddedFile.LoadImage("Textures.LightAmbientIcon.png")));
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
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0);
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("New"))
                    {
                        ////Select Folder
                        //var FolderSelect = Dialog.FolderPicker(Settings.General.Instance.data.LastLoad);
                        //if (FolderSelect.IsOk)
                        //{
                        //    TrickyLevelInterface trickyLevelInterface = new TrickyLevelInterface();
                        //    trickyLevelInterface.GenerateTemplate(FolderSelect.Path);
                        //    TrickyDataManager.LoadProject(Path.Combine(FolderSelect.Path, "config.ssx"));
                        //    Settings.General.Instance.data.LastLoad = FolderSelect.Path;
                        //    Settings.General.Instance.Save();
                        //    Settings.KeyBinding.Instance.Save();
                        //    levelEditorWindow.GenerateRenderList();
                        //}
                    }

                    if (ImGui.MenuItem("Open..."))
                    {
                        var result = Dialog.FileOpen("ssx", Settings.General.Instance.data.LastLoad);
                        if (result.IsOk)
                        {
                            SSX3DataManager.LoadProject(result.Path);
                            LevelIDs = SSX3DataManager.GenerateLevelList();
                            Settings.General.Instance.data.LastLoad = Path.GetDirectoryName(result.Path) ?? "";
                            Settings.General.Instance.Save();
                            Settings.KeyBinding.Instance.Save();
                            levelEditorWindow.GenerateRenderList();
                        }
                    }

                    if (ImGui.MenuItem("Save"))
                    {
                        //TrickyDataManager.SaveProject(Path.Combine(Settings.General.Instance.data.LastLoad, "config.ssx"), false);
                    }
                    if (ImGui.MenuItem("Save As"))
                    {
                        //var result = Dialog.FileSave("ssx", Settings.General.Instance.data.LastLoad);
                        //if (result.IsOk)
                        //{
                        //    TrickyDataManager.SaveProject(result.Path, false);
                        //}
                    }

                    if (ImGui.MenuItem("Extract"))
                    {
                        //Handle save
                        var result = Dialog.FileOpen("ssb", Settings.General.Instance.data.LastLoad);
                        if (result.IsOk)
                        {
                            var FolderSelect = Dialog.FolderPicker(Settings.General.Instance.data.LastLoad);
                            if (FolderSelect.IsOk)
                            {
                                SSBExtractingHandler ssbExtractingHandler = new SSBExtractingHandler();

                                ssbExtractingHandler.LoadAndExtractSSBFromSBD(result.Path.Replace(".ssb", ""), FolderSelect.Path);
                                Settings.General.Instance.data.LastLoad = Path.GetDirectoryName(result.Path) ?? "";
                                Settings.General.Instance.Save();
                            }
                        }
                    }

                    if (ImGui.MenuItem("Export OBJ"))
                    {
                        //var FolderSelect = Dialog.FolderPicker(Settings.General.Instance.data.LastLoad);
                        //if (FolderSelect.IsOk)
                        //{
                        //    OBJLevelExtract.ExtractOBJ(FolderSelect.Path);
                        //}
                    }

                    //if (ImGui.MenuItem("Rebuild"))
                    //{
                    //    // Handle save
                    //}

                    if (ImGui.MenuItem("Settings"))
                    {
                        showSettings = !showSettings;
                    }

                    if (ImGui.MenuItem("Exit"))
                    {
                        // Handle exit
                        Environment.Exit(0);
                    }

                    ImGui.EndMenu();
                }

                //if (ImGui.BeginMenu("Edit"))
                //{
                //    if (ImGui.MenuItem("Undo")) { }
                //    if (ImGui.MenuItem("Redo")) { }
                //    ImGui.EndMenu();
                //}

                if (ImGui.BeginMenu("Help"))
                {
                    if (ImGui.MenuItem("About"))
                    {
                        showAboutWindow = !showAboutWindow;
                    }
                    if (OperatingSystem.IsWindows())
                    {
                        if (ImGui.MenuItem("Console"))
                        {
                            Settings.General.Instance.data.ConsoleWindow = !Settings.General.Instance.data.ConsoleWindow;

                            if (Settings.General.Instance.data.ConsoleWindow)
                            {
                                ConsoleWindow.GenerateConsole();
                            }
                            else
                            {
                                ConsoleWindow.CloseConsole();
                            }
                            Settings.General.Instance.Save();
                        }
                    }
                    if (ImGui.MenuItem("Profiler"))
                    {
                        showProfiler = !showProfiler;
                    }
                    //if (ImGui.MenuItem("ImGui Demo"))
                    //{
                    //    showImGuiDemo = !showImGuiDemo;
                    //}

                    ImGui.EndMenu();
                }

                if (ImGui.MenuItem("[Load last level]"))
                {
                    string selectedPath = Settings.General.Instance.data.LastLoad + "/ConfigSSX3.ssx";
                    if (File.Exists(selectedPath))
                    {
                        SSX3DataManager.LoadProject(selectedPath);
                        LevelIDs = SSX3DataManager.GenerateLevelList();
                        Settings.General.Instance.data.LastLoad = Path.GetDirectoryName(selectedPath) ?? "";
                        Settings.General.Instance.Save();
                        Settings.KeyBinding.Instance.Save();
                        levelEditorWindow.GenerateRenderList();
                    }
                }

                ImGui.SameLine(Raylib.GetScreenWidth() - 400);

                if (ImGui.MenuItem("Level", "", windowMode == WindowMode.World))
                {
                    windowMode = WindowMode.World;
                }

                if (ImGui.MenuItem("Models", "", windowMode == WindowMode.Prefabs))
                {
                    //windowMode = WindowMode.Prefabs;
                }
                if (ImGui.MenuItem("Logic", "", windowMode == WindowMode.Logic))
                {
                    //windowMode = WindowMode.Logic;
                }

                if (ImGui.Combo("Level Selection", ref SelectedLevel, LevelIDs, LevelIDs.Length))
                {
                    // The user selected a new item
                    string selectedValue = LevelIDs[SelectedLevel];
                    TessellatedPatch.Instance.ClearRenderLayers();
                    TessellatedPatch.Instance.AddRenderLayer(SelectedLevel);
                    levelEditorWindow.GenerateRenderList();
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

                if (showSettings)
                {
                    IMGuiSettings.Render();
                }

                if (showProfiler)
                {
                    Profiler.Render();
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
