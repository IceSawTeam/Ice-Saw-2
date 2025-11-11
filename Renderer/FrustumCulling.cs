using Raylib_cs;
using System.Numerics;

namespace IceSaw2.Renderer
{
    public static class FrustumCulling
    {
        private static Plane[] frustumPlanes = new Plane[6];
        // Extract frustum planes from the camera's view and projection matrices
        public static void UpdateFrustum(Camera3D camera)
        {
            Matrix4x4 view = LookAtZUp(camera.Position, camera.Target, camera.Up);

            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(
            camera.FovY * Raylib.DEG2RAD,
            Raylib.GetScreenWidth() / (float)Raylib.GetScreenHeight(),
            (float)Rlgl.GetCullDistanceNear(),
            (float)Rlgl.GetCullDistanceFar()
            );

            Matrix4x4 m = Raymath.MatrixMultiply(proj, view);

            frustumPlanes[0] = MakePlane(m.M14 + m.M11, m.M24 + m.M21, m.M34 + m.M31, m.M44 + m.M41); // Left
            frustumPlanes[1] = MakePlane(m.M14 - m.M11, m.M24 - m.M21, m.M34 - m.M31, m.M44 - m.M41); // Right
            frustumPlanes[2] = MakePlane(m.M14 + m.M12, m.M24 + m.M22, m.M34 + m.M32, m.M44 + m.M42); // Bottom
            frustumPlanes[3] = MakePlane(m.M14 - m.M12, m.M24 - m.M22, m.M34 - m.M32, m.M44 - m.M42); // Top
            frustumPlanes[4] = MakePlane(m.M14 + m.M13, m.M24 + m.M23, m.M34 + m.M33, m.M44 + m.M43); // Near
            frustumPlanes[5] = MakePlane(m.M14 - m.M13, m.M24 - m.M23, m.M34 - m.M33, m.M44 - m.M43); // Far
        }

        private static Plane MakePlane(float a, float b, float c, float d)
        {
            Vector3 n = new Vector3(a, b, c);
            float len = n.Length();
            return new Plane(n / len, d / len);
        }

        private static Matrix4x4 LookAtZUp(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 f = Vector3.Normalize(target - eye);
            Vector3 r = Vector3.Normalize(Vector3.Cross(f, up));
            Vector3 u = Vector3.Cross(r, f);

            // Standard right-handed view matrix, with Z-up
            return new Matrix4x4(
                r.X, u.X, -f.X, 0,
                r.Y, u.Y, -f.Y, 0,
                r.Z, u.Z, -f.Z, 0,
                -Vector3.Dot(r, eye),
                -Vector3.Dot(u, eye),
                Vector3.Dot(f, eye),
                1
            );
        }

        public static bool IsSphereInFrustum(Vector3 center, float radius)
        {
            foreach (var plane in frustumPlanes)
            {
                float distance = Vector3.Dot(plane.Normal, center) + plane.D;
                if (distance < -radius)
                    return false;
            }
            return true;
        }

        private struct Plane
        {
            public Vector3 Normal;
            public float D;
            public Plane(Vector3 n, float d) { Normal = n; D = d; }
        }
    }
}
