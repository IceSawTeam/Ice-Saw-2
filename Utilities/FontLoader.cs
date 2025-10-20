using ImGuiNET;
using System.Runtime.InteropServices;

namespace IceSaw2.Utilities
{
    public class FontLoader
    {
        private GCHandle? _fontHandle = null; // Keep it alive until font atlas is built

        public void LoadFont(string path, float pixelSize)
        {
            // Step 1: Load the .ttf file into a byte array
            byte[] fontBytes = LoadEmbeddedFile.LoadByte(path);

            // Step 2: Pin the byte array so it doesn't move in memory
            _fontHandle = GCHandle.Alloc(fontBytes, GCHandleType.Pinned);
            nint fontPtr = (nint)_fontHandle.Value.AddrOfPinnedObject();

            // Step 3: Use the pointer in AddFontFromMemoryTTF
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.AddFontFromMemoryTTF(fontPtr, fontBytes.Length, pixelSize);

            // ImGui will take ownership of the font memory when building the atlas,
            // but you can keep the handle until fonts are built to be safe.

            if (_fontHandle.HasValue && _fontHandle.Value.IsAllocated)
            {
                _fontHandle.Value.Free();
                _fontHandle = null;
            }
        }

        public void ReleaseFont()
        {
            if (_fontHandle.HasValue && _fontHandle.Value.IsAllocated)
            {
                _fontHandle.Value.Free();
                _fontHandle = null;
            }
        }
    }
}
