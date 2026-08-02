using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using Raylib_cs;
using System.Numerics;

namespace IceSaw2.Renderer
{
    /*
        World-space move/rotate/scale gizmo for the level editor viewport, driven directly by
        SelectionManager.Selected. Axes are always drawn along world X/Y/Z (not the selected
        object's own rotated axes) for all three modes, including Scale - BaseObject.Scale is a
        single local Vector3 (no shear support), so "scale along world X" really just means "drag
        adjusts Scale.X"; this is exactly correct for unrotated objects and a well-understood
        simplification for rotated ones, same trade-off most lightweight editor gizmos make.

        Terrain patches (TrickyPatchObject) don't use BaseObject.Position/Rotation/Scale at all -
        their geometry lives entirely in 16 raw NURBS control points - so Translate/Rotate/uniform-
        Scale are special-cased to move/rotate/scale every control point around the gizmo pivot
        directly (see the ApplyPatch* helpers and GetPivotWorld). Per-axis Scale has no equivalent
        for a patch (no local axes to speak of) and stays hidden/disabled for them, same as it
        already is for any multi-selection.

        Hover/pick testing is done in screen space (project handle geometry, measure pixel distance
        to the mouse - mirrors Picking.cs's line/icon picking) since that's simple and robust.
        Dragging is done with 3D ray-plane intersection against a plane containing the axis and
        angled to face the camera, which is the standard technique for constraining a drag to a
        single 3D axis from a 2D mouse position.
    */
    public static class Gizmo
    {
        public enum Mode { Translate, Rotate, Scale }
        public static Mode CurrentMode = Mode.Translate;

        private enum Handle { None, AxisX, AxisY, AxisZ, Center }

        private const float ScreenSizePx = 90f;
        private const float PickToleranceScreenPx = 8f;
        private const float CenterPickRadiusScreenPx = 10f;
        private const int RingSegments = 32;

        private static readonly Color ColorX = new Color(230, 60, 70, 255);
        private static readonly Color ColorY = new Color(70, 210, 90, 255);
        private static readonly Color ColorZ = new Color(70, 130, 230, 255);
        private static readonly Color ColorHover = new Color(255, 225, 40, 255);
        private static readonly Color ColorCenter = new Color(230, 230, 230, 255);

        private static Handle _hovered = Handle.None;
        private static Handle _dragging = Handle.None;

        // Drag reference state, captured once when a drag begins.
        private static Vector3 _dragPivotWorld;
        private static float _dragStartT;      // translate / per-axis scale: signed distance along the axis at drag start
        private static float _dragLastAngle;   // rotate: previous frame's angle within the ring plane
        private static float _dragStartScreenDist; // uniform scale: initial pivot-to-mouse screen distance

        private struct StartState
        {
            public BaseObject Obj;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
            public Vector3[]? PatchControlPoints; // raw (unscaled), only populated when Obj is a TrickyPatchObject
        }
        private static readonly List<StartState> _dragStart = [];

        private static bool ShouldShow => SelectionManager.Selected.Count > 0;

        // Returns true if the gizmo consumed this frame's mouse input (hovering and/or dragging a
        // handle), so the caller should skip its own click/box-select handling for this frame.
        // allowNewInteraction should be false while the mouse is over an ImGui widget, so a click on
        // a UI element that happens to screen-overlap a handle doesn't start a drag - mirrors the
        // !ImGui.IsAnyItemHovered() guard LevelEditorWindow already uses for box-select.
        public static bool HandleInput(Vector2 mouseScreen, Vector2 viewportPos, Vector2 viewportSize, Camera3D camera, bool allowNewInteraction)
        {
            if (!ShouldShow)
            {
                if (_dragging != Handle.None) EndDrag();
                _hovered = Handle.None;
                return false;
            }

            Vector2 local = mouseScreen - viewportPos;
            bool overViewport = viewportSize.X > 0 && viewportSize.Y > 0
                && local.X >= 0 && local.Y >= 0 && local.X <= viewportSize.X && local.Y <= viewportSize.Y;

            // An active drag still needs to keep running (and be releasable) even if the mouse
            // slips outside the viewport rect mid-drag - only bail out early when idle.
            if (_dragging == Handle.None && !overViewport)
            {
                _hovered = Handle.None;
                return false;
            }

            Vector3 pivot = GetPivotWorld();
            float size = GetScreenConstantWorldSize(pivot, camera, viewportSize.Y);
            Ray ray = Raylib.GetScreenToWorldRayEx(local, camera, (int)viewportSize.X, (int)viewportSize.Y);

            if (_dragging == Handle.None)
            {
                _hovered = PickHandle(pivot, size, local, camera, viewportSize);

                if (_hovered != Handle.None && allowNewInteraction && Input.IsActionPressed("Click"))
                {
                    BeginDrag(_hovered, pivot, ray, local, camera, viewportSize);
                    return true;
                }

                return false;
            }

            UpdateDrag(_dragging, size, ray, local, camera, viewportSize);

            if (Input.IsActionReleased("Click"))
            {
                EndDrag();
            }

            return true;
        }

        public static void Render(Camera3D camera, Vector2 viewportSize)
        {
            if (!ShouldShow || viewportSize.Y <= 0) return;

            Vector3 pivot = GetPivotWorld();
            float size = GetScreenConstantWorldSize(pivot, camera, viewportSize.Y);

            // Gizmo always draws on top of the scene so it stays grabbable even inside geometry.
            // DrawLine3D/DrawCylinderEx/etc. don't draw immediately - they queue into rlgl's active
            // vertex batch, which only actually flushes to the GPU on batch-full/state-change/
            // EndMode3D. Without an explicit flush here, the scene's own pending batch (still queued
            // with depth test enabled) and the gizmo's batch (queued after disabling it) can end up
            // rasterized in the wrong order relative to these toggles, so both sides need to force a
            // flush against the depth-test state that's actually active at that moment.
            Rlgl.DrawRenderBatchActive();
            Rlgl.DisableDepthTest();

            switch (CurrentMode)
            {
                case Mode.Translate: RenderTranslate(pivot, size); break;
                case Mode.Rotate: RenderRotate(pivot, size); break;
                case Mode.Scale: RenderScale(pivot, size); break;
            }

            Rlgl.DrawRenderBatchActive();

            Rlgl.EnableDepthTest();
        }

        private static Vector3 GetPivotWorld()
        {
            var primary = SelectionManager.Primary;
            if (primary == null) return Vector3.Zero;

            // Patches never touch Position, so worldMatrix4x4 stays at the identity - the pivot has
            // to come from the control points themselves instead. Mirrors the same centroid calc
            // TessellatedPatch.PatchEntry.UpdateBoundingSphere() already uses for its bounding sphere.
            if (primary is TrickyPatchObject patch)
            {
                Vector3 sum = Vector3.Zero;
                for (int i = 0; i < 16; i++) sum += patch.controlPoints[i];
                return (sum / 16f) * BaseObject.WorldScale;
            }

            return new Vector3(primary.worldMatrix4x4.M14, primary.worldMatrix4x4.M24, primary.worldMatrix4x4.M34);
        }

        // Keeps the gizmo a constant apparent size on screen regardless of distance from the camera.
        private static float GetScreenConstantWorldSize(Vector3 pivot, Camera3D camera, float viewportHeightPx)
        {
            float distance = Vector3.Distance(camera.Position, pivot);
            float worldPerPixel = 2f * distance * MathF.Tan(camera.FovY * 0.5f * Raylib.DEG2RAD) / MathF.Max(viewportHeightPx, 1f);
            return ScreenSizePx * worldPerPixel;
        }

        private static Vector3 AxisDirFromHandle(Handle h) => h switch
        {
            Handle.AxisX => Vector3.UnitX,
            Handle.AxisY => Vector3.UnitY,
            Handle.AxisZ => Vector3.UnitZ,
            _ => Vector3.Zero
        };

        // The two axes spanning the plane perpendicular to `axis` - shared between ring drawing,
        // ring hover picking, and the rotate angle calculation so they all agree on the same 0deg
        // reference and winding direction.
        private static void GetRingBasis(Vector3 axis, out Vector3 u, out Vector3 v)
        {
            if (axis == Vector3.UnitX) { u = Vector3.UnitY; v = Vector3.UnitZ; }
            else if (axis == Vector3.UnitY) { u = Vector3.UnitZ; v = Vector3.UnitX; }
            else { u = Vector3.UnitX; v = Vector3.UnitY; }
        }

        private static float AngleInPlane(Vector3 offset, Vector3 axis)
        {
            GetRingBasis(axis, out Vector3 u, out Vector3 v);
            return MathF.Atan2(Vector3.Dot(offset, v), Vector3.Dot(offset, u));
        }

        private static bool RayPlaneIntersect(Ray ray, Vector3 planePoint, Vector3 planeNormal, out Vector3 hit)
        {
            float denom = Vector3.Dot(planeNormal, ray.Direction);
            if (MathF.Abs(denom) < 1e-6f)
            {
                hit = Vector3.Zero;
                return false;
            }
            float t = Vector3.Dot(planePoint - ray.Position, planeNormal) / denom;
            hit = ray.Position + ray.Direction * t;
            return true;
        }

        // A plane containing the drag axis, oriented to face the camera as much as possible - the
        // standard trick for turning a 2D mouse position into a 1D offset along a 3D axis. Falls
        // back to a plane built from camera.Up (and then world X) when the axis points straight at
        // the camera, where the primary construction degenerates.
        private static Vector3 GetAxisDragPlaneNormal(Vector3 axisDir, Vector3 pivot, Camera3D camera)
        {
            Vector3 toCamera = camera.Position - pivot;
            Vector3 normal = Vector3.Cross(axisDir, Vector3.Cross(toCamera, axisDir));
            if (normal.LengthSquared() < 1e-8f)
            {
                normal = Vector3.Cross(axisDir, camera.Up);
                if (normal.LengthSquared() < 1e-8f) normal = Vector3.Cross(axisDir, Vector3.UnitX);
            }
            return Vector3.Normalize(normal);
        }

        private static float DistancePointToSegment2D(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float lenSq = ab.LengthSquared();
            float t = lenSq > 0.0001f ? Math.Clamp(Vector2.Dot(p - a, ab) / lenSq, 0f, 1f) : 0f;
            Vector2 proj = a + ab * t;
            return Vector2.Distance(p, proj);
        }

        private static Handle PickHandle(Vector3 pivot, float size, Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            if (CurrentMode == Mode.Rotate)
            {
                return PickClosestRing(pivot, size, mouseLocal, camera, viewportSize);
            }

            Vector2 pivotScreen = Raylib.GetWorldToScreenEx(pivot, camera, (int)viewportSize.X, (int)viewportSize.Y);

            if (CurrentMode == Mode.Scale && Vector2.Distance(mouseLocal, pivotScreen) <= CenterPickRadiusScreenPx)
                return Handle.Center;

            // Per-axis scale is only meaningful (and only offered) for a single, non-patch selection -
            // see the class comment on BaseObject.Scale being a plain local vector (patches have no
            // local axes to scale along at all).
            if (CurrentMode == Mode.Scale && (SelectionManager.Selected.Count > 1 || SelectionManager.Primary is TrickyPatchObject))
                return Handle.None;

            Handle best = Handle.None;
            float bestDist = PickToleranceScreenPx;

            TestAxis(Handle.AxisX, Vector3.UnitX);
            TestAxis(Handle.AxisY, Vector3.UnitY);
            TestAxis(Handle.AxisZ, Vector3.UnitZ);

            return best;

            void TestAxis(Handle h, Vector3 dir)
            {
                Vector2 tipScreen = Raylib.GetWorldToScreenEx(pivot + dir * size, camera, (int)viewportSize.X, (int)viewportSize.Y);
                float d = DistancePointToSegment2D(mouseLocal, pivotScreen, tipScreen);
                if (d < bestDist) { bestDist = d; best = h; }
            }
        }

        private static Handle PickClosestRing(Vector3 pivot, float size, Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            Handle best = Handle.None;
            float bestDist = PickToleranceScreenPx;

            TestRing(Handle.AxisX, Vector3.UnitY, Vector3.UnitZ);
            TestRing(Handle.AxisY, Vector3.UnitZ, Vector3.UnitX);
            TestRing(Handle.AxisZ, Vector3.UnitX, Vector3.UnitY);

            return best;

            void TestRing(Handle h, Vector3 u, Vector3 v)
            {
                Vector2? prev = null;
                for (int i = 0; i <= RingSegments; i++)
                {
                    float t = i / (float)RingSegments * 2f * MathF.PI;
                    Vector3 worldPoint = pivot + (u * MathF.Cos(t) + v * MathF.Sin(t)) * size;
                    Vector2 screenPoint = Raylib.GetWorldToScreenEx(worldPoint, camera, (int)viewportSize.X, (int)viewportSize.Y);
                    if (prev.HasValue)
                    {
                        float d = DistancePointToSegment2D(mouseLocal, prev.Value, screenPoint);
                        if (d < bestDist) { bestDist = d; best = h; }
                    }
                    prev = screenPoint;
                }
            }
        }

        private static void BeginDrag(Handle handle, Vector3 pivot, Ray ray, Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            _dragging = handle;
            _dragPivotWorld = pivot;
            _dragStart.Clear();
            foreach (var obj in SelectionManager.Selected)
            {
                Vector3[]? patchPoints = null;
                if (obj is TrickyPatchObject patch)
                {
                    patchPoints = new Vector3[16];
                    for (int i = 0; i < 16; i++) patchPoints[i] = patch.controlPoints[i];
                }

                _dragStart.Add(new StartState { Obj = obj, Position = obj.Position, Rotation = obj.Rotation, Scale = obj.Scale, PatchControlPoints = patchPoints });
            }

            switch (CurrentMode)
            {
                case Mode.Translate:
                case Mode.Scale when handle != Handle.Center:
                    {
                        Vector3 axisDir = AxisDirFromHandle(handle);
                        Vector3 planeNormal = GetAxisDragPlaneNormal(axisDir, pivot, camera);
                        _dragStartT = RayPlaneIntersect(ray, pivot, planeNormal, out Vector3 hit) ? Vector3.Dot(hit - pivot, axisDir) : 0f;
                        break;
                    }
                case Mode.Scale:
                    {
                        Vector2 pivotScreen = Raylib.GetWorldToScreenEx(pivot, camera, (int)viewportSize.X, (int)viewportSize.Y);
                        _dragStartScreenDist = MathF.Max(1f, Vector2.Distance(mouseLocal, pivotScreen));
                        break;
                    }
                case Mode.Rotate:
                    {
                        Vector3 axisDir = AxisDirFromHandle(handle);
                        _dragLastAngle = RayPlaneIntersect(ray, pivot, axisDir, out Vector3 hit) ? AngleInPlane(hit - pivot, axisDir) : 0f;
                        break;
                    }
            }
        }

        private static void UpdateDrag(Handle handle, float size, Ray ray, Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            switch (CurrentMode)
            {
                case Mode.Translate:
                    ApplyTranslate(handle, ray, camera);
                    break;
                case Mode.Rotate:
                    ApplyRotate(handle, ray);
                    break;
                case Mode.Scale:
                    if (handle == Handle.Center) ApplyUniformScale(mouseLocal, camera, viewportSize);
                    else ApplyAxisScale(handle, size, ray, camera);
                    break;
            }
        }

        private static void ApplyTranslate(Handle handle, Ray ray, Camera3D camera)
        {
            Vector3 axisDir = AxisDirFromHandle(handle);
            Vector3 planeNormal = GetAxisDragPlaneNormal(axisDir, _dragPivotWorld, camera);
            if (!RayPlaneIntersect(ray, _dragPivotWorld, planeNormal, out Vector3 hit)) return;

            float t = Vector3.Dot(hit - _dragPivotWorld, axisDir);
            // worldMatrix4x4 applies BaseObject.WorldScale on top of Position for every selectable
            // object (none of them are parented - see BaseObject.parent usage across the codebase),
            // so a world-space delta has to be un-scaled before it's added to the local Position.
            Vector3 deltaPosition = axisDir * ((t - _dragStartT) / BaseObject.WorldScale);

            foreach (var start in _dragStart)
            {
                if (start.Obj is TrickyPatchObject patch && start.PatchControlPoints != null)
                {
                    for (int i = 0; i < 16; i++) patch.controlPoints[i] = start.PatchControlPoints[i] + deltaPosition;
                    continue;
                }

                start.Obj.Position = start.Position + deltaPosition;
                RefreshRenderCacheIfInstance(start.Obj);
            }
        }

        private static void ApplyAxisScale(Handle handle, float size, Ray ray, Camera3D camera)
        {
            // Single-selection only (PickHandle already excludes this handle for multi-select).
            if (_dragStart.Count == 0) return;

            Vector3 axisDir = AxisDirFromHandle(handle);
            Vector3 planeNormal = GetAxisDragPlaneNormal(axisDir, _dragPivotWorld, camera);
            if (!RayPlaneIntersect(ray, _dragPivotWorld, planeNormal, out Vector3 hit)) return;

            float t = Vector3.Dot(hit - _dragPivotWorld, axisDir);
            float factor = MathF.Max(0.01f, 1f + (t - _dragStartT) / size);

            var start = _dragStart[0];
            Vector3 newScale = start.Scale;
            if (handle == Handle.AxisX) newScale.X = start.Scale.X * factor;
            else if (handle == Handle.AxisY) newScale.Y = start.Scale.Y * factor;
            else if (handle == Handle.AxisZ) newScale.Z = start.Scale.Z * factor;

            start.Obj.Scale = newScale;
            RefreshRenderCacheIfInstance(start.Obj);
        }

        private static void ApplyUniformScale(Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            Vector2 pivotScreen = Raylib.GetWorldToScreenEx(_dragPivotWorld, camera, (int)viewportSize.X, (int)viewportSize.Y);
            float dist = MathF.Max(1f, Vector2.Distance(mouseLocal, pivotScreen));
            float factor = MathF.Max(0.01f, dist / _dragStartScreenDist);
            Vector3 pivotRaw = _dragPivotWorld / BaseObject.WorldScale;

            foreach (var start in _dragStart)
            {
                if (start.Obj is TrickyPatchObject patch && start.PatchControlPoints != null)
                {
                    for (int i = 0; i < 16; i++) patch.controlPoints[i] = pivotRaw + (start.PatchControlPoints[i] - pivotRaw) * factor;
                    continue;
                }

                start.Obj.Scale = start.Scale * factor;
                RefreshRenderCacheIfInstance(start.Obj);
            }
        }

        private static void ApplyRotate(Handle handle, Ray ray)
        {
            Vector3 axisDir = AxisDirFromHandle(handle);
            if (!RayPlaneIntersect(ray, _dragPivotWorld, axisDir, out Vector3 hit)) return;

            float angle = AngleInPlane(hit - _dragPivotWorld, axisDir);
            float frameDelta = ShortestAngleDelta(angle, _dragLastAngle);
            _dragLastAngle = angle;
            if (MathF.Abs(frameDelta) < 1e-6f) return;

            // Applied incrementally (this frame's delta from last frame, not drag-start), so a
            // continuous multi-turn drag doesn't run into the +-180deg wraparound that comparing
            // against a fixed start angle would hit.
            Quaternion deltaRotation = Quaternion.CreateFromAxisAngle(axisDir, frameDelta);
            Vector3 pivotRaw = _dragPivotWorld / BaseObject.WorldScale;

            foreach (var start in _dragStart)
            {
                if (start.Obj is TrickyPatchObject patch)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Vector3 offset = patch.controlPoints[i] - pivotRaw;
                        patch.controlPoints[i] = pivotRaw + Vector3.Transform(offset, deltaRotation);
                    }
                    continue;
                }

                var obj = start.Obj;
                obj.Rotation = Quaternion.Normalize(deltaRotation * obj.Rotation);

                // Orbit the object's position around the shared pivot too, so a multi-selection
                // rotates as a group instead of every object spinning in place. For the primary
                // object itself this is a no-op (its offset from the pivot is zero).
                Vector3 worldPos = new Vector3(obj.worldMatrix4x4.M14, obj.worldMatrix4x4.M24, obj.worldMatrix4x4.M34);
                Vector3 newOffset = Vector3.Transform(worldPos - _dragPivotWorld, deltaRotation);
                obj.Position = (_dragPivotWorld + newOffset) / BaseObject.WorldScale;

                RefreshRenderCacheIfInstance(obj);
            }
        }

        private static float ShortestAngleDelta(float current, float previous)
        {
            float delta = current - previous;
            while (delta > MathF.PI) delta -= 2f * MathF.PI;
            while (delta < -MathF.PI) delta += 2f * MathF.PI;
            return delta;
        }

        private static void RefreshRenderCacheIfInstance(BaseObject obj)
        {
            if (obj is TrickyInstanceObject instance) instance.RefreshRenderCache();
        }

        private static void EndDrag()
        {
            _dragging = Handle.None;
            _dragStart.Clear();
        }

        private static Handle ActiveHandle => _dragging != Handle.None ? _dragging : _hovered;

        private static Color HandleColor(Handle handle)
        {
            if (handle == ActiveHandle) return ColorHover;
            return handle switch
            {
                Handle.AxisX => ColorX,
                Handle.AxisY => ColorY,
                Handle.AxisZ => ColorZ,
                _ => ColorCenter
            };
        }

        private static void RenderTranslate(Vector3 pivot, float size)
        {
            DrawAxisArrow(pivot, Vector3.UnitX, size, Handle.AxisX);
            DrawAxisArrow(pivot, Vector3.UnitY, size, Handle.AxisY);
            DrawAxisArrow(pivot, Vector3.UnitZ, size, Handle.AxisZ);
        }

        private static void DrawAxisArrow(Vector3 pivot, Vector3 dir, float size, Handle handle)
        {
            Color color = HandleColor(handle);
            Vector3 shaftEnd = pivot + dir * (size * 0.8f);
            Vector3 tip = pivot + dir * size;
            Raylib.DrawLine3D(pivot, shaftEnd, color);
            Raylib.DrawCylinderEx(shaftEnd, tip, size * 0.05f, 0f, 8, color);
        }

        private static void RenderRotate(Vector3 pivot, float size)
        {
            DrawRing(pivot, size, Vector3.UnitY, Vector3.UnitZ, Handle.AxisX);
            DrawRing(pivot, size, Vector3.UnitZ, Vector3.UnitX, Handle.AxisY);
            DrawRing(pivot, size, Vector3.UnitX, Vector3.UnitY, Handle.AxisZ);
        }

        private static void DrawRing(Vector3 pivot, float size, Vector3 u, Vector3 v, Handle handle)
        {
            Color color = HandleColor(handle);
            Rlgl.Begin(DrawMode.Lines);
            Rlgl.Color4ub(color.R, color.G, color.B, color.A);

            Vector3 prev = pivot + u * size;
            for (int i = 1; i <= RingSegments; i++)
            {
                float t = i / (float)RingSegments * 2f * MathF.PI;
                Vector3 next = pivot + (u * MathF.Cos(t) + v * MathF.Sin(t)) * size;
                Rlgl.Vertex3f(prev.X, prev.Y, prev.Z);
                Rlgl.Vertex3f(next.X, next.Y, next.Z);
                prev = next;
            }
            Rlgl.End();
        }

        private static void RenderScale(Vector3 pivot, float size)
        {
            if (SelectionManager.Selected.Count == 1 && SelectionManager.Primary is not TrickyPatchObject)
            {
                DrawScaleAxis(pivot, Vector3.UnitX, size, Handle.AxisX);
                DrawScaleAxis(pivot, Vector3.UnitY, size, Handle.AxisY);
                DrawScaleAxis(pivot, Vector3.UnitZ, size, Handle.AxisZ);
            }

            Raylib.DrawSphere(pivot, size * 0.08f, HandleColor(Handle.Center));
        }

        private static void DrawScaleAxis(Vector3 pivot, Vector3 dir, float size, Handle handle)
        {
            Color color = HandleColor(handle);
            Vector3 tip = pivot + dir * size;
            Raylib.DrawLine3D(pivot, tip, color);
            Raylib.DrawCube(tip, size * 0.09f, size * 0.09f, size * 0.09f, color);
        }
    }
}
