using ImGuiNET;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IceSaw2
{
    public class IMGuiFolderPicker
    {
        private string? currentPath;
        private bool isOpen;
        private Action<string>? onFileSelected;
        private string title = "Folder Picker";

        public bool IsVisible => isOpen;

        public IMGuiFolderPicker(string? startPath = "")
        {
            if(startPath=="")
            {
                startPath = null;
            }

            currentPath = startPath ?? Directory.GetCurrentDirectory();
            isOpen = false;
        }

        public void Show(string Title, Action<string> onSelected)
        {
            title = Title;
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
                }
                catch (Exception e)
                {
                    ImGui.TextColored(new System.Numerics.Vector4(1, 0, 0, 1), "Error reading directory.");
                    ImGui.TextWrapped(e.Message);
                }

                ImGui.Separator();

                ImGui.Text("Selected: " + Path.GetFileName(currentPath));
                if (ImGui.Button("Accept"))
                {
                    Close();
                    onFileSelected?.Invoke(currentPath);
                    onFileSelected = null;
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    Close();
                    onFileSelected = null;
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
