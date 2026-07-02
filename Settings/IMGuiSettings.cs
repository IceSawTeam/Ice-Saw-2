using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Settings
{
    public class IMGuiSettings
    {
        private static bool isOpen;

        public bool IsVisible => isOpen;

        public static void Render()
        {
            if (!isOpen)
                return;
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(700, 500), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("Settings", ref isOpen))
            {

            }

        }
    }

}
