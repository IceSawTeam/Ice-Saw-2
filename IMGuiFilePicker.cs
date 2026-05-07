using ImGuiNET;
using Microsoft.VisualBasic.FileIO;
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
        private string? selectedFile;
        private bool isOpen;
        private Action<string>? onFileSelected;
        private string fileType="";
        private string title = "File Picker";

        public bool IsVisible => isOpen;

        public IMGuiFilePicker(string? startPath = "")
        {
            if(startPath=="")
            {
                startPath = null;
            }

            currentPath = startPath ?? Directory.GetCurrentDirectory();
            selectedFile = null;
            isOpen = false;
        }

        public void Show(string Title, string FileType, Action<string> onSelected)
        {
            title = Title;
            fileType = FileType;
            onFileSelected = onSelected;
            isOpen = true;
        }

        public void Render()
        {
            if (!isOpen)
                return;

            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700, 500), ImGuiCond.FirstUseEver);
            if (ImGui.Begin(title, ref isOpen))
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
                        if (fileType != "")
                        {
                            if (file.EndsWith(fileType))
                            {
                                if (ImGui.Selectable(Path.GetFileName(file), selected))
                                {
                                    selectedFile = file;
                                }
                            }
                        }
                        else
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
                        Close();
                        onFileSelected?.Invoke(selectedFile);
                        onFileSelected = null;
                        selectedFile = null;
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("Cancel"))
                    {
                        Close();
                        onFileSelected = null;
                        selectedFile = null;
                    }
                }
                else
                {
                    if (ImGui.Button("Cancel"))
                    {
                        Close();
                        onFileSelected = null;
                        selectedFile = null;
                    }
                }

                
            }
            ImGui.End();
        }

        private void Close()
        {
            isOpen = false;
        }
    }
}
