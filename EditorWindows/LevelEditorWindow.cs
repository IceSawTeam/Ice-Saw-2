using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using IceSaw2.Settings;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.Manager.WorldManager;

namespace IceSaw2.EditorWindows
{
    public class LevelEditorWindow : BaseEditorWindow
    {
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private float mouseSensitivity = 0.003f;
        private float moveSpeed = 0.1f;
        private const float moveSpeedStep = 0.008f;
        private int screenWidth;
        private int screenHeight;

        Camera3D camera3D = new Camera3D();
        public bool Open = true;


        public void Initilize()
        {
            camera3D.Position = new System.Numerics.Vector3(0, 100, 100);
            camera3D.Target = Vector3.Zero;
            camera3D.Up = new Vector3(0, 0, 1);
            camera3D.FovY = 65f;
            camera3D.Projection = CameraProjection.Perspective;

            screenWidth = GeneralSettings.ScreenWidth;
            screenHeight = GeneralSettings.ScreenHeight;
        }

        public override void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(camera3D);
            Rlgl.DisableBackfaceCulling();


            Rlgl.DisableDepthMask();
            //Render Skybox
            for (int i = 0; i < DataManager.trickySkyboxPrefabObjects.Count; i++)
            {
                var TempLocation = DataManager.trickySkyboxPrefabObjects[i].Position;

                DataManager.trickySkyboxPrefabObjects[i].Position = camera3D.Position / BaseObject.WorldScale;
                DataManager.trickySkyboxPrefabObjects[i].Render();
                DataManager.trickySkyboxPrefabObjects[i].Position = TempLocation;
            }
            Rlgl.EnableDepthMask();


            // Viewport Camera
            if (Raylib.IsMouseButtonDown(MouseButton.Right))
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

                // Movement
                float currentSpeed = moveSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.LeftShift)) currentSpeed *= 2.0f;
                if (Raylib.IsKeyDown(KeyboardKey.W)) camera3D.Position += forward * currentSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.S)) camera3D.Position -= forward * currentSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.D)) camera3D.Position -= right * currentSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.A)) camera3D.Position += right * currentSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.E)) camera3D.Position.Z += currentSpeed;
                if (Raylib.IsKeyDown(KeyboardKey.Q)) camera3D.Position.Z -= currentSpeed;

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    moveSpeed += wheel * moveSpeedStep;
                    moveSpeed = Math.Clamp(moveSpeed, 0.008f, 200f);
                    //Debug.WriteLine(moveSpeed, currentSpeed.ToString());
                }

                camera3D.Target = camera3D.Position + forward;
            }
            if (Raylib.IsMouseButtonReleased(MouseButton.Right))
            {
                Raylib.ShowCursor();
            }

            

            //Render Default
            Raylib.DrawLine3D(new Vector3(-1000, 0, 0), new Vector3(1000, 0, 0), new Color(212, 28, 4));
            Raylib.DrawLine3D(new Vector3(0, -1000, 0), new Vector3(0, 1000, 0), new Color(17, 212, 4));
            Raylib.DrawLine3D(new Vector3(0, 0, -1000), new Vector3(0, 0, 1000), new Color(2, 99, 224));

            //Render Objects

            for (int i = 0; i < DataManager.trickyPatchObjects.Count; i++)
            {
                DataManager.trickyPatchObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickyInstanceObjects.Count; i++)
            {
                DataManager.trickyInstanceObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickySplineObjects.Count; i++)
            {
                DataManager.trickySplineObjects[i].Render();
            }

            //Render Wires

            Raylib.EndMode3D();

            float menuBarHeight = ImGui.GetFrameHeight(); // Typically height of main menu bar

            // Sidebar dimensions
            int sidebarWidth = 300;

            // Position and size sidebar *below* the main menu bar
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(sidebarWidth, WorldManager.instance.heightScreen - menuBarHeight), ImGuiCond.Always);

            // Optional: remove decorations
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Side Panel", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);

            ImGui.Text("Sidebar content below main menu bar.");
            //ImGui.Separator();
            //ImGui.Button("Click Me");

            // Add your sidebar content here

            ImGui.End();
            ImGui.PopStyleVar(2);

            //Raylib.DrawTexture(skyboxTexture2Ds[0], 100, 100, Color.White);
        }

        //public override void LogicUpdate()
        //{
        //    //Update Camera
        //    Raylib.UpdateCamera(ref camera3D, CameraMode.Free);
        //}


    }
}
