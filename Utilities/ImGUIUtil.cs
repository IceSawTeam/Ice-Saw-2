using ImGuiNET;

namespace IceSaw2.Utilities
{
    public static class ImGUIUtil
    {
        public static string TextInput(string Input, uint Size = 99999)
        {
            ImGui.InputText("Texture Path", ref Input, Size);

            return Input;
        }
    }
}
