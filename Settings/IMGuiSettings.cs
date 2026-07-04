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

            }

            ImGui.End();
        }
    }

}
