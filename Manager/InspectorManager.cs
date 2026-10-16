using IceSaw2.LevelObject;
using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace IceSaw2.Manager
{
    public sealed class HideInInspectorAttribute : Attribute { }

    public class InspectorManager
    {
        private sealed record Member(string Name, string Label, Type Type, Func<object, object?> Get, Action<object, object?> Set);

        private static readonly Dictionary<Type, List<Member>> Cache = new();

        public static void Draw<T>(IReadOnlyList<T> targets, params string[] skip) where T : class
        {
            foreach (var m in GetMembers(targets[0].GetType()))
            {
                if (!skip.Contains(m.Name))
                    DrawMember(m, targets);
            }
        }

        private static void DrawMember<T>(Member m, IReadOnlyList<T> targets) where T : class
        {
            void Write(T t, object? v)
            {
                m.Set(t, v);
                Notify(t, m.Name);
            }

            if (m.Type == typeof(int))
                Int(m.Label, targets, t => (int)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type == typeof(float))
                Float(m.Label, targets, t => (float)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type == typeof(bool))
                Bool(m.Label, targets, t => (bool)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type == typeof(string))
                Text(m.Label, targets, t => (string?)m.Get(t) ?? "", (t, v) => Write(t, v));
            else if (m.Type == typeof(Vector2))
                Vector2(m.Label, targets, t => (Vector2)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type == typeof(Vector3))
                Vector3(m.Label, targets, t => (Vector3)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type == typeof(Vector4))
                Vector4(m.Label, targets, t => (Vector4)m.Get(t)!, (t, v) => Write(t, v));
            else if (m.Type.IsEnum)
                DrawEnum(m, targets, Write);
            else if (ElementType(m.Type) is { } elementType)
                DrawList(m, elementType, targets);
            else if (IsStruct(m.Type))
                DrawStruct(m, targets);
            else
                ImGui.TextDisabled($"{m.Label}: {m.Type.Name}");
        }

        private static void DrawEnum<T>(Member m, IReadOnlyList<T> targets, Action<T, object?> write) where T : class
        {
            object value = m.Get(targets[0])!;
            bool mixed = targets.Any(t => !Equals(m.Get(t), value));

            if (ImGui.BeginCombo(m.Label, mixed ? "--" : value.ToString()))
            {
                foreach (var option in Enum.GetValues(m.Type))
                {
                    if (ImGui.Selectable(option.ToString()!, !mixed && option.Equals(value)))
                        foreach (var t in targets) write(t, option);
                }
                ImGui.EndCombo();
            }
        }

        private static void DrawList<T>(Member m, Type elementType, IReadOnlyList<T> targets) where T : class
        {
            var lists = targets.Select(t => m.Get(t) as IList).ToList();

            if (lists.Any(l => l == null))
            {
                ImGui.TextDisabled($"{m.Label}: null");
                return;
            }

            int count = lists[0]!.Count;
            if (lists.Any(l => l!.Count != count))
            {
                ImGui.TextDisabled($"{m.Label}: lengths differ across selection");
                return;
            }

            // ### keeps the node's ID stable when the count changes, so it doesn't collapse.
            if (!ImGui.TreeNode($"{m.Label} ({count})###{m.Label}"))
                return;

            // Arrays stay fixed-size (UVPoints must remain 4); only List<T> can grow or shrink.
            bool resizable = !m.Type.IsArray;
            int removeAt = -1;

            for (int i = 0; i < count; i++)
            {
                ImGui.PushID(i);
                if (resizable)
                {
                    if (ImGui.SmallButton("x")) removeAt = i;
                    ImGui.SameLine();
                }
                DrawMember(Element(m, elementType, i), targets);
                ImGui.PopID();
            }

            if (resizable && (elementType.IsValueType || elementType == typeof(string)) && ImGui.SmallButton("+ Add"))
                ModifyList(m, targets, list => list.Add(DefaultValue(elementType)));

            if (removeAt >= 0)
                ModifyList(m, targets, list => list.RemoveAt(removeAt));

            ImGui.TreePop();
        }

        private static void ModifyList<T>(Member m, IReadOnlyList<T> targets, Action<IList> change) where T : class
        {
            foreach (var t in targets)
            {
                var list = (IList)m.Get(t)!;
                change(list);
                m.Set(t, list);
                Notify(t, m.Name);
            }
        }

        // Structs are copies: read the parent, change the field on the copy, write the whole thing back.
        private static Member Nested(Member parent, Member child) => new(
            parent.Name + "." + child.Name,
            child.Label,
            child.Type,
            t => child.Get(parent.Get(t)!),
            (t, v) =>
            {
                object box = parent.Get(t)!;
                child.Set(box, v);
                parent.Set(t, box);
            });

        // Writing back through parent.Set means list items inside structs work too.
        private static Member Element(Member parent, Type elementType, int index) => new(
            $"{parent.Name}[{index}]",
            $"[{index}]",
            elementType,
            t => ((IList)parent.Get(t)!)[index],
            (t, v) =>
            {
                var list = (IList)parent.Get(t)!;
                list[index] = v;
                parent.Set(t, list);
            });

        private static Type? ElementType(Type type)
        {
            if (type.IsArray && type.GetArrayRank() == 1)
                return type.GetElementType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                return type.GetGenericArguments()[0];
            return null;
        }

        private static bool IsStruct(Type type) =>
            type.IsValueType && !type.IsPrimitive && !type.IsEnum && Nullable.GetUnderlyingType(type) == null;

        private static object? DefaultValue(Type type) =>
            type == typeof(string) ? "" : Activator.CreateInstance(type);

        private static void Notify(object target, string memberName)
        {
            if (target is BaseObject obj)
                obj.OnInspectorChanged(memberName);
        }

        private static List<Member> GetMembers(Type type)
        {
            if (Cache.TryGetValue(type, out var cached))
                return cached;

            var chain = new Stack<Type>();
            for (var t = type; t != null && t != typeof(object); t = t.BaseType)
                chain.Push(t);

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly;
            var members = new List<Member>();

            foreach (var t in chain)
            {
                foreach (var f in t.GetFields(flags).OrderBy(f => f.MetadataToken))
                    if (!f.IsInitOnly && !f.IsDefined(typeof(HideInInspectorAttribute)))
                        members.Add(new Member(f.Name, f.Name, f.FieldType, f.GetValue, f.SetValue));

                foreach (var p in t.GetProperties(flags).OrderBy(p => p.MetadataToken))
                    if (p.CanRead && p.SetMethod?.IsPublic == true && p.GetIndexParameters().Length == 0
                        && !p.IsDefined(typeof(HideInInspectorAttribute)))
                        members.Add(new Member(p.Name, p.Name, p.PropertyType, p.GetValue, p.SetValue));
            }

            Cache[type] = members;
            return members;
        }
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

        private static void DrawStruct<T>(Member parent, IReadOnlyList<T> targets) where T : class
        {
            var children = GetMembers(parent.Type);
            if (children.Count == 0)
            {
                ImGui.TextDisabled($"{parent.Name}: {parent.Type.Name}");
                return;
            }

            if (!ImGui.TreeNode(parent.Name))
                return;

            foreach (var child in children)
                DrawMember(Nested(parent, child), targets);

            ImGui.TreePop();
        }

    }
}
