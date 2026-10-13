using IceSaw2.LevelObject.TrickyObjects;
using ImGuiNET;
using SharpGLTF.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace IceSaw2.Inspectors.Tricky
{
    public static class PatchInspector
    {
        public static void Draw(IReadOnlyList<TrickyPatchObject> patches)
        {
            InspectorManager.Text("Name", patches, p => p.Name, (p, v) => p.Name = v);
            InspectorManager.Vector3("Position", patches, p => p.Position, (p, v) => p.Position = v);
            InspectorManager.Vector3("Scale", patches, p => p.Scale, (p, v) => p.Scale = v);
            InspectorManager.Vector3("Rotation", patches, p => p.EulerAngles, (p, v) => p.EulerAngles = v);
            InspectorManager.Vector4("Lightmap", patches, p => p.LightMapPoint, (p, v) => p.LightMapPoint = v);
            ImGui.Spacing();
            if (ImGui.CollapsingHeader($"UV Points"))
            {
                for (int i = 0; i < 4; i++)
                {
                    InspectorManager.Vector2("UV Point " + (i + 1), patches, p => p.UVPoints[i], (p, v) => p.UVPoints[i] = v);
                }
            }

            if (ImGui.CollapsingHeader($"Control Point"))
            {
                for (int i = 0; i < 16; i++)
                {
                    InspectorManager.Vector3("Control Point " + (i + 1), patches, p => p.controlPoints[i], (p, v) => p.controlPoints[i] = v);
                }
            }

            InspectorManager.Int("Surface Type", patches, p => p.SurfaceType, (p, v) => p.SurfaceType = v);
            InspectorManager.Bool("Trick Only Patch", patches, p => p.TrickOnlyPatch, (p, v) => p.TrickOnlyPatch = v);
            InspectorManager.Text("Texture Path", patches, p => p.TexturePath, (p, v) => p.TexturePath = v);
            InspectorManager.Int("Lightmap ID", patches, p => p.LightmapID, (p, v) => p.LightmapID = v);
        }

    }
}
