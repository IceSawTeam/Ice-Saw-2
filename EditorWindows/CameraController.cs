using IceSaw2.Renderer;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace IceSaw2.EditorWindows
{
    public class CameraController
    {
        private int screenWidth { get { return Raylib.GetScreenWidth(); } }
        private int screenHeight { get { return Raylib.GetScreenHeight(); } }
        private const float BOOST_MULTIPLIER = 2.0f;

        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float mouseSensitivity = 0.003f;
        public float moveSpeed = 0.1f;
        private const float moveSpeedStep = 0.08f;
        public float moveSpeedChangedTimer = 0.0f;

        public float nearClip = 1f;
        public float farClip = 100000f;

        public Camera3D viewCamera3D = new Camera3D();

        public CameraController()
        {
            viewCamera3D.Position = new Vector3(0f, 0f, 0.1f);
            viewCamera3D.Target = viewCamera3D.Position + new Vector3(0, 1, 0);
            viewCamera3D.Up = new Vector3(0, 0, 1);
            viewCamera3D.FovY = 65f;
            viewCamera3D.Projection = CameraProjection.Perspective;
        }

        public void Begin3D()
        {
            Rlgl.SetClipPlanes(nearClip, farClip);
            //Render 3D
            Raylib.BeginMode3D(viewCamera3D);
            FrustumCulling.UpdateFrustum(viewCamera3D);
        }

        public void ControllerLogic()
        {
            if (Input.IsActionDown("CameraActivate"))
            {
                Raylib.HideCursor();

                // Look
                Vector2 mouseDelta = Raylib.GetMouseDelta();
                yaw += mouseDelta.X * mouseSensitivity;
                pitch -= mouseDelta.Y * mouseSensitivity;
                pitch = Math.Clamp(pitch, -1.5f, 1.5f);

                Raylib.SetMousePosition(screenWidth / 2, screenHeight / 2);

                Vector3 forward = new Vector3(MathF.Cos(pitch) * MathF.Sin(yaw), MathF.Cos(pitch) * MathF.Cos(yaw), MathF.Sin(pitch));
                Vector3 right = new Vector3(MathF.Sin(yaw - MathF.PI / 2f), MathF.Cos(yaw - MathF.PI / 2f), 0f);
                Vector3 up = Raymath.Vector3CrossProduct(forward, right);

                viewCamera3D.Up = up;

                // Movement
                Vector3 newPosition = new Vector3(0, 0, 0);
                // Vector3 newPosition = Raymath.Vector3Zero();
                float currentSpeed = moveSpeed;
                if (Input.IsActionDown("CameraBoost")) currentSpeed *= BOOST_MULTIPLIER;
                if (Input.IsActionDown("CameraMoveForward")) newPosition += forward;
                if (Input.IsActionDown("CameraMoveBack")) newPosition -= forward;
                if (Input.IsActionDown("CameraMoveRight")) newPosition -= right;
                if (Input.IsActionDown("CameraMoveLeft")) newPosition += right;
                if (Input.IsActionDown("CameraMoveUp")) newPosition += up;
                if (Input.IsActionDown("CameraMoveDown")) newPosition -= up;
                viewCamera3D.Position += Raymath.Vector3Normalize(newPosition) * currentSpeed;

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    moveSpeed += wheel * moveSpeedStep;
                    moveSpeed = Math.Clamp(moveSpeed, 0.008f, 200f);
                    //Debug.WriteLine(moveSpeed, currentSpeed.ToString());
                    moveSpeedChangedTimer = 4.0f;
                }


                viewCamera3D.Target = viewCamera3D.Position + forward;
            }
            if (Input.IsActionReleased("CameraActivate"))
            {
                Raylib.ShowCursor();
            }
        }
    }
}
