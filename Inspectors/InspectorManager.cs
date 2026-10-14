using IceSaw2.LevelObject;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;

namespace IceSaw2.Inspectors
{
    public class InspectorManager
    {
        public static void DrawStandardDetails<T>(IReadOnlyList<BaseObject> objects)
        {
            Text("Name", objects, p => p.Name, (p, v) => p.Name = v);
            Vector3("Position", objects, p => p.Position, (p, v) => p.Position = v);
            Vector3("Scale", objects, p => p.Scale, (p, v) => p.Scale = v);
            Vector3("Rotation", objects, p => p.EulerAngles, (p, v) => p.EulerAngles = v);
        }
        public static void Text<T>(string label, IReadOnlyList<T> targets, Func<T, string> get, Action<T, string> set)
        {
            string value = get(targets[0]) ?? "";
            bool mixed = targets.Any(t => get(t) != value);

            if (mixed)
                value = "";

            if (ImGui.InputTextWithHint(label, mixed ? "--" : "", ref value, 512))
                foreach (var t in targets) set(t, value);
        }

        public static void Int<T>(string label, IReadOnlyList<T> targets, Func<T, int> get, Action<T, int> set)
        {
            int value = get(targets[0]);
            bool mixed = targets.Any(t => get(t) != value);
            if (ImGui.DragInt(label, ref value, 1f, 0, 0, mixed ? "--" : "%d"))
                foreach (var t in targets) set(t, value);
        }


        public static void Float<T>(string label, IReadOnlyList<T> targets, Func<T, float> get, Action<T, float> set)
        {
            float value = get(targets[0]);
            bool mixed = targets.Any(t => get(t) != value);
            if (ImGui.DragFloat(label, ref value, 1f, 0, 0, mixed ? "--" : "%d"))
                foreach (var t in targets) set(t, value);
        }

        public static void Bool<T>(string label, IReadOnlyList<T> targets, Func<T, bool> get, Action<T, bool> set)
        {
            bool value = get(targets[0]);
            bool mixed = targets.Any(t => get(t) != value);
            if (ImGui.Checkbox(label, ref value))
                foreach (var t in targets) set(t, value);
        }


        public static void Vector4<T>(string label, IReadOnlyList<T> targets, Func<T, Vector4> get, Action<T, Vector4> set)
        {
            Vector4 first = get(targets[0]);
            ImGui.TextUnformatted(label);
            ImGui.PushID(label);
            for (int c = 0; c < 4; c++)
            {
                float value = first[c];
                bool mixed = targets.Any(t => get(t)[c] != value);
                if (c > 0) ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / (4 - c));
                if (ImGui.DragFloat("##" + c, ref value, 0.01f, 0, 0, mixed ? "--" : "%.3f"))
                    foreach (var t in targets) { var v = get(t); v[c] = value; set(t, v); }
            }
            ImGui.PopID();
        }

        public static void Vector3<T>(string label, IReadOnlyList<T> targets, Func<T, Vector3> get, Action<T, Vector3> set)
        {
            Vector3 first = get(targets[0]);
            ImGui.TextUnformatted(label);
            ImGui.PushID(label);
            for (int c = 0; c < 3; c++)
            {
                float value = first[c];
                bool mixed = targets.Any(t => get(t)[c] != value);
                if (c > 0) ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / (3 - c));
                if (ImGui.DragFloat("##" + c, ref value, 0.01f, 0, 0, mixed ? "--" : "%.3f"))
                    foreach (var t in targets) { var v = get(t); v[c] = value; set(t, v); }
            }
            ImGui.PopID();
        }

        public static void Vector2<T>(string label, IReadOnlyList<T> targets, Func<T, Vector2> get, Action<T, Vector2> set)
        {
            Vector2 first = get(targets[0]);
            ImGui.TextUnformatted(label);
            ImGui.PushID(label);
            for (int c = 0; c < 2; c++)
            {
                float value = first[c];
                bool mixed = targets.Any(t => get(t)[c] != value);
                if (c > 0) ImGui.SameLine();
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / (2 - c));
                if (ImGui.DragFloat("##" + c, ref value, 0.01f, 0, 0, mixed ? "--" : "%.3f"))
                    foreach (var t in targets) { var v = get(t); v[c] = value; set(t, v); }
            }
            ImGui.PopID();
        }

    }
}
