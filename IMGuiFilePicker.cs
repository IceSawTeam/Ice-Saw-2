using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2
{
    public class IMGuiFilePicker
    {
        private string currentPath;
        private string selectedFile;
        private bool isOpen;
        private Action<string> onFileSelected;

        public bool IsVisible => isOpen;

        public IMGuiFilePicker(string startPath = "")
        {
            if(startPath=="")
            {
                startPath = null;
            }

            currentPath = startPath ?? Directory.GetCurrentDirectory();
            selectedFile = null;
            isOpen = false;
        }

        public void Show(Action<string> onSelected)
        {
            onFileSelected = onSelected;
            isOpen = true;
        }

        public void Render()
        {
            if (!isOpen)
                return;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700, 500), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("File Picker", ref isOpen))
            {
                ImGui.Text($"Current Path: {currentPath}");

                if (ImGui.Button("^ Up"))
                {
                    try
                    {
                        currentPath = Directory.GetParent(currentPath)?.FullName ?? currentPath;
                    }
                    catch { }
                }

                ImGui.Separator();

                try
                {
                    // List directories
                    foreach (var dir in Directory.GetDirectories(currentPath))
                    {
                        if (ImGui.Selectable("  /" + Path.GetFileName(dir) + "/"))
                        {
                            currentPath = dir;
                        }
                    }

                    // List files
                    foreach (var file in Directory.GetFiles(currentPath))
                    {
                        bool selected = selectedFile == file;
                        if (file.EndsWith("ssx"))
                        {
                            if (ImGui.Selectable(Path.GetFileName(file), selected))
                            {
                                selectedFile = file;
                            }
                        }
                        
                    }
                }
                catch (Exception e)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), "Error reading directory.");
                    ImGui.TextWrapped(e.Message);
                }

                ImGui.Separator();

                if (selectedFile != null)
                {
                    ImGui.Text("Selected: " + Path.GetFileName(selectedFile));
                    if (ImGui.Button("Accept"))
                    {
                        onFileSelected?.Invoke(selectedFile);
                        Close();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel"))
                    {
                        Close();
                    }
                }
                else
                {
                    if (ImGui.Button("Cancel"))
                    {
                        Close();
                    }
                }

                ImGui.End();
            }
        }

        private void Close()
        {
            selectedFile = null;
            isOpen = false;
            onFileSelected = null;
        }
    }
}
