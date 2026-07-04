using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Settings
{
    public class IMGuiSettings
    {

        public static void Render()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700, 500), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Settings"))
            {
                ImGui.SliderInt("Patch Export Resolution", ref Settings.General.Instance.data.PatchResolution, 0, 12);
            }
            if(ImGui.Button("Save"))
            {
                Settings.General.Instance.Save();
            }
            ImGui.End();
        }
    }

}
