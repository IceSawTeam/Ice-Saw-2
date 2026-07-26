using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace IceSaw2.Renderer
{
    /*
        Screen-space aware picking helpers for the level editor viewport.

        The viewport is rendered into an off-screen RenderTexture2D sized to its ImGui
        panel (see Core.cs), so raylib's screen-size-assuming Raylib.GetMouseRay/GetWorldToScreen
        can't be used directly - the "Ex" variants that take an explicit width/height are used
        instead so the ray/projection lines up with the panel's own aspect ratio, not the
        full window's.

        Mesh objects are hit-tested against their real triangles (broad-phase bounding box,
        then GetRayCollisionMesh). Objects with no mesh (lights, cameras, particle instances -
        all billboard icons) and line objects (splines/paths, which are just line lists) are
        picked by projecting their geometry to screen space and checking proximity to the
        mouse in pixels, since a 3D raycast has nothing solid to hit for them. Any world point
        used for a screen-space test is first checked to be in front of the camera - raylib's
        GetWorldToScreenEx doesn't clip against the camera plane, so a point behind the camera
        can otherwise still project onto a screen coordinate near the cursor and register a
        false hit.

        Patches (terrain) and instances (placed prefabs) aren't drawn per-object - patches go
        through a single shared tessellation batch and instances through a batched render cache
        keyed by prefab - so there's no single Mesh sitting on the object to hand to
        GetRayCollisionMesh directly. Both still get a real, accurate hit test: a bounding-box
        broad phase first (built from the NURBS control points for patches, since the surface is
        guaranteed to lie within their convex hull; from the prefab's mesh bounds for instances),
        then an exact test against the real shape for anything that passes - a small re-tessellated
        triangle mesh evaluated with the same Bezier math as the patch shader for patches, and the
        prefab's actual mesh triangles (via GetRayCollisionMesh, resolved through the instance's
        prefab) for instances. The broad phase alone isn't enough on its own for patches - adjacent
        terrain tiles' convex-hull boxes commonly overlap, so a click could land on a neighboring
        tile's box instead of the one actually under the cursor.
    */
    public static class Picking
    {
        public const float IconPickRadiusPx = 12f;
        public const float LinePickRadiusPx = 8f;

        public static BaseObject? Pick(Vector2 mouseScreenPos, Vector2 viewportPos, Vector2 viewportSize, Camera3D camera, IEnumerable<BaseObject> candidates)
        {
            if (!TryGetLocalMouse(mouseScreenPos, viewportPos, viewportSize, out Vector2 local))
                return null;

            Ray ray = Raylib.GetScreenToWorldRayEx(local, camera, (int)viewportSize.X, (int)viewportSize.Y);

            BaseObject? best = null;
            float bestDist = float.MaxValue;

            foreach (var obj in candidates)
            {
                if (!obj.Visible || !obj.Enabled) continue;

                float? dist = TryHit(obj, ray, local, camera, viewportSize);
                if (dist.HasValue && dist.Value < bestDist)
                {
                    bestDist = dist.Value;
                    best = obj;
                }
            }

            return best;
        }

        public static List<BaseObject> PickBox(Vector2 screenA, Vector2 screenB, Vector2 viewportPos, Vector2 viewportSize, Camera3D camera, IEnumerable<BaseObject> candidates)
        {
            List<BaseObject> result = new List<BaseObject>();

            Vector2 min = Vector2.Min(screenA, screenB) - viewportPos;
            Vector2 max = Vector2.Max(screenA, screenB) - viewportPos;

            foreach (var obj in candidates)
            {
                if (!obj.Visible || !obj.Enabled) continue;

                Vector3 worldPos = GetRepresentativeWorldPoint(obj);

                if (!IsInFrontOfCamera(worldPos, camera)) continue;

                Vector2 screen = Raylib.GetWorldToScreenEx(worldPos, camera, (int)viewportSize.X, (int)viewportSize.Y);

                if (screen.X >= min.X && screen.X <= max.X && screen.Y >= min.Y && screen.Y <= max.Y)
                {
                    result.Add(obj);
                }
            }

            return result;
        }

        private static bool TryGetLocalMouse(Vector2 mouseScreenPos, Vector2 viewportPos, Vector2 viewportSize, out Vector2 local)
        {
            local = mouseScreenPos - viewportPos;

            if (viewportSize.X <= 0 || viewportSize.Y <= 0) return false;
            if (local.X < 0 || local.Y < 0 || local.X > viewportSize.X || local.Y > viewportSize.Y) return false;

            return true;
        }

        private static bool IsInFrontOfCamera(Vector3 worldPos, Camera3D camera)
        {
            Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);
            return Vector3.Dot(forward, worldPos - camera.Position) > 0f;
        }

        private static Vector3 GetRepresentativeWorldPoint(BaseObject obj)
        {
            if (obj is TrickyPatchObject patch)
            {
                BoundingBox box = GetPatchWorldBoundingBox(patch);
                return Vector3.Lerp(box.Min, box.Max, 0.5f);
            }

            if (obj is TrickyInstanceObject instance)
            {
                BoundingBox box = GetInstanceWorldBoundingBox(instance);
                return Vector3.Lerp(box.Min, box.Max, 0.5f);
            }

            if (obj is MeshBaseObject meshObj && meshObj.meshRef.Mesh.VertexCount > 0)
            {
                return Vector3.Lerp(obj.worldBoundingBox.Min, obj.worldBoundingBox.Max, 0.5f);
            }

            if (obj is LineBaseObject line && line.WorldLinePoints.Count > 0)
            {
                return line.WorldLinePoints[line.WorldLinePoints.Count / 2];
            }

            return new Vector3(obj.worldMatrix4x4.M14, obj.worldMatrix4x4.M24, obj.worldMatrix4x4.M34);
        }

        private static float? TryHit(BaseObject obj, Ray ray, Vector2 mouseLocal, Camera3D camera, Vector2 viewportSize)
        {
            // Patches (terrain): broad-phase box first (cheap reject for the vast majority of
            // patches), then an exact test against the real tessellated surface. Adjacent terrain
            // tiles' convex-hull boxes commonly overlap each other significantly, so stopping at
            // the box test alone meant a click could land on a neighboring patch's box instead of
            // (or as well as) the one actually under the cursor.
            if (obj is TrickyPatchObject patch)
            {
                RayCollision boxHit = Raylib.GetRayCollisionBox(ray, GetPatchWorldBoundingBox(patch));
                if (!boxHit.Hit) return null;

                return RaycastPatchSurface(patch, ray, out float patchDist) ? patchDist : (float?)null;
            }

            // Instances (placed prefabs): broad-phase box, then the real mesh triangles - same
            // two-step test as ordinary mesh objects below, just resolving the mesh(es) through the
            // prefab/render-cache indirection instead of reading them straight off the object.
            if (obj is TrickyInstanceObject instance)
            {
                RayCollision boxHit = Raylib.GetRayCollisionBox(ray, GetInstanceWorldBoundingBox(instance));
                if (!boxHit.Hit) return null;

                return RaycastInstanceMeshes(instance, ray, out float instanceDist) ? instanceDist : (float?)null;
            }

            // Real mesh objects: broad-phase bounding box, then exact triangle test.
            if (obj is MeshBaseObject meshObj && meshObj.meshRef.Mesh.VertexCount > 0)
            {
                RayCollision boxHit = Raylib.GetRayCollisionBox(ray, obj.worldBoundingBox);
                if (!boxHit.Hit) return null;

                RayCollision meshHit = Raylib.GetRayCollisionMesh(ray, meshObj.meshRef.Mesh, obj.worldMatrix4x4);
                return meshHit.Hit ? meshHit.Distance : (float?)null;
            }

            // Line objects (splines/AI paths/race lines): pick by proximity to the nearest segment in screen space.
            if (obj is LineBaseObject line && line.WorldLinePoints.Count >= 2)
            {
                float bestScreenDist = float.MaxValue;
                float bestT = 0f;
                int bestSegment = -1;

                for (int i = 0; i < line.WorldLinePoints.Count - 1; i++)
                {
                    Vector3 p0 = line.WorldLinePoints[i];
                    Vector3 p1 = line.WorldLinePoints[i + 1];

                    if (!IsInFrontOfCamera(p0, camera) || !IsInFrontOfCamera(p1, camera))
                        continue;

                    Vector2 a = Raylib.GetWorldToScreenEx(p0, camera, (int)viewportSize.X, (int)viewportSize.Y);
                    Vector2 b = Raylib.GetWorldToScreenEx(p1, camera, (int)viewportSize.X, (int)viewportSize.Y);

                    float d = DistancePointToSegment(mouseLocal, a, b, out float t);
                    if (d < bestScreenDist)
                    {
                        bestScreenDist = d;
                        bestT = t;
                        bestSegment = i;
                    }
                }

                if (bestSegment >= 0 && bestScreenDist <= LinePickRadiusPx)
                {
                    Vector3 worldPoint = Vector3.Lerp(line.WorldLinePoints[bestSegment], line.WorldLinePoints[bestSegment + 1], bestT);
                    return Vector3.Distance(camera.Position, worldPoint);
                }

                return null;
            }

            // Icon-only objects (lights, cameras, particle instances): pick by proximity to the billboard's screen position.
            Vector3 iconWorldPos = new Vector3(obj.worldMatrix4x4.M14, obj.worldMatrix4x4.M24, obj.worldMatrix4x4.M34);

            if (!IsInFrontOfCamera(iconWorldPos, camera)) return null;

            Vector2 iconScreen = Raylib.GetWorldToScreenEx(iconWorldPos, camera, (int)viewportSize.X, (int)viewportSize.Y);
            float iconDist = Vector2.Distance(mouseLocal, iconScreen);
            float pickRadius = GetIconScreenPickRadius(obj, iconWorldPos, iconScreen, camera, viewportSize);

            if (iconDist <= pickRadius)
            {
                return Vector3.Distance(camera.Position, iconWorldPos);
            }

            return null;
        }

        // The billboards are drawn at a fixed WORLD size (see TrickyLightObject/TrickyCameraObject/
        // TrickyPaticleInstanceObject.Render()), so their on-screen size grows the closer the camera
        // gets. A fixed pixel radius made close-up icons only clickable dead-center - most of the
        // visibly large icon would miss. This projects the icon's actual world half-size to screen
        // space so the clickable area tracks what's actually drawn, with IconPickRadiusPx kept as a
        // floor so small/distant icons stay comfortably clickable.
        private static float GetIconScreenPickRadius(BaseObject obj, Vector3 iconWorldPos, Vector2 iconScreen, Camera3D camera, Vector2 viewportSize)
        {
            Vector3 forward = Vector3.Normalize(camera.Target - camera.Position);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, camera.Up));

            float halfSize = obj is TrickyLightObject ? 0.275f : 0.5f; // Camera / particle icons are drawn wider than lights.
            Vector3 edgeWorld = iconWorldPos + right * halfSize;
            Vector2 edgeScreen = Raylib.GetWorldToScreenEx(edgeWorld, camera, (int)viewportSize.X, (int)viewportSize.Y);

            float apparentRadius = Vector2.Distance(iconScreen, edgeScreen);
            return System.MathF.Max(apparentRadius, IconPickRadiusPx);
        }

        private static float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b, out float t)
        {
            Vector2 ab = b - a;
            float lenSq = ab.LengthSquared();

            t = lenSq > 0.0001f ? System.Math.Clamp(Vector2.Dot(p - a, ab) / lenSq, 0f, 1f) : 0f;

            Vector2 proj = a + ab * t;
            return Vector2.Distance(p, proj);
        }

        // The surface of a cubic NURBS/Bezier patch always lies within the convex hull of its
        // control points, so this box is a guaranteed (if loose) superset of the real surface.
        // Patches are often nearly flat along one axis (a thin height range relative to their
        // footprint), which leaves very little margin for a grazing ray to register - Inflate()
        // pads that out a little so an edge-of-surface click doesn't fall just outside the box.
        public static BoundingBox GetPatchWorldBoundingBox(TrickyPatchObject patch)
        {
            Vector3[] points = patch.controlPoints.ReturnControlPoints(false); // already WorldScale-applied

            Vector3 min = points[0];
            Vector3 max = points[0];

            for (int i = 1; i < points.Length; i++)
            {
                min = Vector3.Min(min, points[i]);
                max = Vector3.Max(max, points[i]);
            }

            return Inflate(new BoundingBox(min, max));
        }

        // Pads a box out by a small fraction of its own size (plus a flat minimum), so a ray that
        // just grazes the true surface/mesh edge still registers instead of missing by a hair.
        private static BoundingBox Inflate(BoundingBox box)
        {
            const float MinPadding = 0.02f;
            const float RelativePadding = 0.02f;

            Vector3 size = box.Max - box.Min;
            Vector3 pad = new Vector3(
                System.MathF.Max(MinPadding, size.X * RelativePadding),
                System.MathF.Max(MinPadding, size.Y * RelativePadding),
                System.MathF.Max(MinPadding, size.Z * RelativePadding));

            return new BoundingBox(box.Min - pad, box.Max + pad);
        }

        // Mirrors the exact matrix combination the renderer uses (TrickyModelMeshObject.AddToRenderCache /
        // TrickyModelObject.GenerateModel: instance.worldMatrix4x4 * meshChild.localMatrix4X4) so the box
        // lines up with where the instance is actually drawn.
        public static BoundingBox GetInstanceWorldBoundingBox(TrickyInstanceObject instance)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            bool any = false;

            var prefab = instance.TrickyPrefab;
            if (prefab != null)
            {
                for (int i = 0; i < prefab.trickyModelMeshObjects.Count; i++)
                {
                    var meshObj = prefab.trickyModelMeshObjects[i];
                    Matrix4x4 combined = instance.worldMatrix4x4 * meshObj.localMatrix4X4;

                    for (int m = 0; m < meshObj.meshes.Count; m++)
                    {
                        Mesh mesh = meshObj.meshes[m].meshRef.Mesh;
                        if (mesh.VertexCount == 0) continue;

                        BoundingBox localBox = Raylib.GetMeshBoundingBox(mesh);
                        ExpandByTransformedBox(ref min, ref max, localBox, combined);
                        any = true;
                    }
                }
            }

            if (!any)
            {
                // No prefab/mesh data resolved - fall back to a small box around the instance origin
                // so it's still clickable rather than silently unselectable.
                Vector3 p = new Vector3(instance.worldMatrix4x4.M14, instance.worldMatrix4x4.M24, instance.worldMatrix4x4.M34);
                return new BoundingBox(p - new Vector3(0.15f), p + new Vector3(0.15f));
            }

            return Inflate(new BoundingBox(min, max));
        }

        private static void ExpandByTransformedBox(ref Vector3 min, ref Vector3 max, BoundingBox box, Matrix4x4 transform)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = new Vector3(
                    (i & 1) == 0 ? box.Min.X : box.Max.X,
                    (i & 2) == 0 ? box.Min.Y : box.Max.Y,
                    (i & 4) == 0 ? box.Min.Z : box.Max.Z);

                Vector3 world = Raymath.Vector3Transform(corner, transform);
                min = Vector3.Min(min, world);
                max = Vector3.Max(max, world);
            }
        }

        // Matches TessellatedPatchBatch.vs's EvaluateBezierSurface/BezierInterpolate exactly, and
        // tessellates at the same 8x8 resolution as TessellatedPatch.GenerateMesh(), so this tests
        // (an approximation of) the actual rendered surface rather than its loose bounding box.
        private const int PatchTessellationResolution = 8;

        private static bool RaycastPatchSurface(TrickyPatchObject patch, Ray ray, out float distance)
        {
            Vector3[] controlPoints = patch.controlPoints.ReturnControlPoints(false);

            Vector3[,] grid = new Vector3[PatchTessellationResolution, PatchTessellationResolution];
            for (int y = 0; y < PatchTessellationResolution; y++)
            {
                float v = y / (float)(PatchTessellationResolution - 1);
                for (int x = 0; x < PatchTessellationResolution; x++)
                {
                    float u = x / (float)(PatchTessellationResolution - 1);
                    grid[y, x] = EvaluateBezierSurface(controlPoints, u, v);
                }
            }

            distance = float.MaxValue;
            bool hit = false;

            for (int y = 0; y < PatchTessellationResolution - 1; y++)
            {
                for (int x = 0; x < PatchTessellationResolution - 1; x++)
                {
                    Vector3 p00 = grid[y, x];
                    Vector3 p10 = grid[y, x + 1];
                    Vector3 p01 = grid[y + 1, x];
                    Vector3 p11 = grid[y + 1, x + 1];

                    if (RayIntersectsTriangle(ray.Position, ray.Direction, p00, p01, p11, out float t1) && t1 < distance)
                    {
                        distance = t1;
                        hit = true;
                    }

                    if (RayIntersectsTriangle(ray.Position, ray.Direction, p00, p11, p10, out float t2) && t2 < distance)
                    {
                        distance = t2;
                        hit = true;
                    }
                }
            }

            return hit;
        }

        private static Vector3 EvaluateBezierSurface(Vector3[] controlPoints, float u, float v)
        {
            Vector3[] uPoints = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                int row = i * 4;
                uPoints[i] = BezierInterpolate(controlPoints[row], controlPoints[row + 1], controlPoints[row + 2], controlPoints[row + 3], u);
            }
            return BezierInterpolate(uPoints[0], uPoints[1], uPoints[2], uPoints[3], v);
        }

        private static Vector3 BezierInterpolate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 q0 = Vector3.Lerp(p0, p1, t);
            Vector3 q1 = Vector3.Lerp(p1, p2, t);
            Vector3 q2 = Vector3.Lerp(p2, p3, t);
            Vector3 a = Vector3.Lerp(q0, q1, t);
            Vector3 b = Vector3.Lerp(q1, q2, t);
            return Vector3.Lerp(a, b, t);
        }

        // Real per-triangle test against the prefab's actual meshes (same GetRayCollisionMesh used
        // for ordinary mesh objects), just resolved through the instance -> prefab -> mesh-child
        // indirection instead of reading a mesh straight off the object.
        private static bool RaycastInstanceMeshes(TrickyInstanceObject instance, Ray ray, out float distance)
        {
            distance = float.MaxValue;
            bool hit = false;

            var prefab = instance.TrickyPrefab;
            if (prefab == null) return false;

            for (int i = 0; i < prefab.trickyModelMeshObjects.Count; i++)
            {
                var meshObj = prefab.trickyModelMeshObjects[i];
                Matrix4x4 combined = instance.worldMatrix4x4 * meshObj.localMatrix4X4;

                for (int m = 0; m < meshObj.meshes.Count; m++)
                {
                    Mesh mesh = meshObj.meshes[m].meshRef.Mesh;
                    if (mesh.VertexCount == 0) continue;

                    RayCollision meshHit = Raylib.GetRayCollisionMesh(ray, mesh, combined);
                    if (meshHit.Hit && meshHit.Distance < distance)
                    {
                        distance = meshHit.Distance;
                        hit = true;
                    }
                }
            }

            return hit;
        }

        // Standard Moller-Trumbore ray-triangle intersection, double-sided (doesn't cull backfaces),
        // since a patch/prop can legitimately be viewed or selected from either side.
        private static bool RayIntersectsTriangle(Vector3 rayOrigin, Vector3 rayDir, Vector3 v0, Vector3 v1, Vector3 v2, out float t)
        {
            const float Epsilon = 1e-7f;
            t = 0f;

            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 h = Vector3.Cross(rayDir, edge2);
            float a = Vector3.Dot(edge1, h);

            if (a > -Epsilon && a < Epsilon) return false; // Ray parallel to triangle plane.

            float f = 1f / a;
            Vector3 s = rayOrigin - v0;
            float u = f * Vector3.Dot(s, h);
            if (u < 0f || u > 1f) return false;

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(rayDir, q);
            if (v < 0f || u + v > 1f) return false;

            t = f * Vector3.Dot(edge2, q);
            return t > Epsilon;
        }
    }
}
