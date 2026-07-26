using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager.Tricky;
using IceSaw2.Renderer;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.Manager
{
    /*
        Holds the level editor's current object selection and draws the highlight overlay
        for whatever is selected. Shared by both the viewport (click/box select) and the
        outliner (tree click select) so the two stay in sync automatically.

        Click semantics: plain click replaces the selection, Ctrl toggles an object in/out
        of the selection, Shift adds to it without toggling.
    */
    public static class SelectionManager
    {
        public static readonly Color HighlightColor = new Color(255, 170, 0, 255);
        private const float IconHighlightRadius = 0.35f;

        public static List<BaseObject> Selected { get; private set; } = new List<BaseObject>();

        public static BaseObject? Primary => Selected.Count > 0 ? Selected[Selected.Count - 1] : null;

        public static bool IsSelected(BaseObject obj) => Selected.Contains(obj);

        public static void Click(BaseObject obj, bool ctrl, bool shift)
        {
            if (ctrl)
            {
                if (!Selected.Remove(obj))
                {
                    Selected.Add(obj);
                }
            }
            else if (shift)
            {
                if (!Selected.Contains(obj))
                {
                    Selected.Add(obj);
                }
            }
            else
            {
                Selected.Clear();
                Selected.Add(obj);
            }
        }

        public static void Box(IEnumerable<BaseObject> objs, bool additive)
        {
            if (!additive)
            {
                Selected.Clear();
            }

            foreach (var obj in objs)
            {
                if (!Selected.Contains(obj))
                {
                    Selected.Add(obj);
                }
            }
        }

        public static void Clear()
        {
            Selected.Clear();
        }

        public static void RenderHighlights()
        {
            // Patches highlight through the tessellation shader itself (accurate to the actual
            // surface, unlike a bounding box), which is a persistent per-patch flag rather than
            // an immediate-mode draw call - so every patch needs to be explicitly synced each
            // frame, not just the selected ones, or a deselected patch would stay lit forever.
            HashSet<int> selectedPatchIds = new HashSet<int>();
            for (int i = 0; i < Selected.Count; i++)
            {
                if (Selected[i] is TrickyPatchObject selectedPatch)
                {
                    selectedPatchIds.Add(selectedPatch.TesPatchID);
                }
            }

            var allPatches = TrickyDataManager.trickyPatchObjects;
            for (int i = 0; i < allPatches.Count; i++)
            {
                TessellatedPatch.Instance.UpdatePatchHighlight(allPatches[i].TesPatchID, selectedPatchIds.Contains(allPatches[i].TesPatchID));
            }

            for (int i = 0; i < Selected.Count; i++)
            {
                var obj = Selected[i];

                if (obj is TrickyPatchObject)
                {
                    // Highlight already handled above via the shader.
                    continue;
                }
                else if (obj is TrickyInstanceObject instance)
                {
                    Raylib.DrawBoundingBox(Picking.GetInstanceWorldBoundingBox(instance), HighlightColor);
                }
                else if (obj is MeshBaseObject meshObj && meshObj.meshRef.Mesh.VertexCount > 0)
                {
                    Raylib.DrawBoundingBox(obj.worldBoundingBox, HighlightColor);
                }
                else if (obj is LineBaseObject line && line.WorldLinePoints.Count > 1)
                {
                    Rlgl.PushMatrix();
                    Rlgl.Begin(DrawMode.Lines);
                    Rlgl.Color3f(HighlightColor.R / 255f, HighlightColor.G / 255f, HighlightColor.B / 255f);

                    for (int p = 0; p < line.WorldLinePoints.Count - 1; p++)
                    {
                        Rlgl.Vertex3f(line.WorldLinePoints[p].X, line.WorldLinePoints[p].Y, line.WorldLinePoints[p].Z);
                        Rlgl.Vertex3f(line.WorldLinePoints[p + 1].X, line.WorldLinePoints[p + 1].Y, line.WorldLinePoints[p + 1].Z);
                    }

                    Rlgl.End();
                    Rlgl.PopMatrix();
                }
                else
                {
                    Vector3 worldPos = new Vector3(obj.worldMatrix4x4.M14, obj.worldMatrix4x4.M24, obj.worldMatrix4x4.M34);
                    Raylib.DrawSphereWires(worldPos, IconHighlightRadius, 8, 8, HighlightColor);
                }
            }
        }
    }
}
