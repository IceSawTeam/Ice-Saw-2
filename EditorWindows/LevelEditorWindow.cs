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

        private float axisLineSize = 1000f;

        public Camera3D viewCamera3D = new Camera3D();
        public bool Open = true;


        public void Initilize()
        {
            viewCamera3D.Position = new System.Numerics.Vector3(100, 100, 100);
            viewCamera3D.Target = viewCamera3D.Position + new Vector3(0, 1, 0);
            viewCamera3D.Up = new Vector3(0, 0, 1);
            viewCamera3D.FovY = 65f;
            viewCamera3D.Projection = CameraProjection.Perspective;

            screenWidth = WorldManager.instance.generalSettings.ScreenWidth;
            screenHeight = WorldManager.instance.generalSettings.ScreenHeight;
        }

        public override void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(viewCamera3D);
            Rlgl.DisableBackfaceCulling();


            Rlgl.DisableDepthMask();
            //Render Skybox
            for (int i = 0; i < DataManager.trickySkyboxPrefabObjects.Count; i++)
            {
                var TempLocation = DataManager.trickySkyboxPrefabObjects[i].Position;

                DataManager.trickySkyboxPrefabObjects[i].Position = viewCamera3D.Position / BaseObject.WorldScale;
                DataManager.trickySkyboxPrefabObjects[i].Render();
                DataManager.trickySkyboxPrefabObjects[i].Position = TempLocation;
            }
            Rlgl.EnableDepthMask();            

            //Render Default
            Raylib.DrawLine3D(new Vector3(-axisLineSize, 0, 0), new Vector3(axisLineSize, 0, 0), new Color(212, 28, 4));
            Raylib.DrawLine3D(new Vector3(0, -axisLineSize, 0), new Vector3(0, axisLineSize, 0), new Color(17, 212, 4));
            Raylib.DrawLine3D(new Vector3(0, 0, -axisLineSize), new Vector3(0, 0, axisLineSize), new Color(2, 99, 224));

            //Render Objects

            for (int i = 0; i < DataManager.trickyPatchObjects.Count; i++)
            {
                DataManager.trickyPatchObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickyInstanceObjects.Count; i++)
            {
                DataManager.trickyInstanceObjects[i].Render();
            }

            //Render Wires
            for (int i = 0; i < DataManager.trickySplineObjects.Count; i++)
            {
                DataManager.trickySplineObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickyAIPAIPath.Count; i++)
            {
                DataManager.trickyAIPAIPath[i].Render();
            }

            for (int i = 0; i < DataManager.trickyAIPRaceLine.Count; i++)
            {
                DataManager.trickyAIPRaceLine[i].Render();
            }

            for (int i = 0; i < DataManager.trickySOPAIPath.Count; i++)
            {
                DataManager.trickySOPAIPath[i].Render();
            }

            for (int i = 0; i < DataManager.trickySOPRaceLine.Count; i++)
            {
                DataManager.trickySOPRaceLine[i].Render();
            }

            //Render Sprites
            for (int i = 0; i < DataManager.trickyLightObjects.Count; i++)
            {
                DataManager.trickyLightObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickyCameraObjects.Count; i++)
            {
                DataManager.trickyCameraObjects[i].Render();
            }

            for (int i = 0; i < DataManager.trickyPaticleInstanceObjects.Count; i++)
            {
                DataManager.trickyPaticleInstanceObjects[i].Render();
            }

            Raylib.EndMode3D();


            //Render UI

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

        public override void LogicUpdate()
        {
            //Update Camera
            //Raylib.UpdateCamera(ref viewCamera3D, CameraMode.Free);
            // Viewport Camera
            if (Raylib.IsMouseButtonDown(WorldManager.instance.hotkeySettings.ActivateCamera))
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
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Boost)) currentSpeed *= 2.0f;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Forward)) viewCamera3D.Position += forward * currentSpeed;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Back)) viewCamera3D.Position -= forward * currentSpeed;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Right)) viewCamera3D.Position -= right * currentSpeed;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Left)) viewCamera3D.Position += right * currentSpeed;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Up)) viewCamera3D.Position.Z += currentSpeed;
                if (Raylib.IsKeyDown(WorldManager.instance.hotkeySettings.Down)) viewCamera3D.Position.Z -= currentSpeed;

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    moveSpeed += wheel * moveSpeedStep;
                    moveSpeed = Math.Clamp(moveSpeed, 0.008f, 200f);
                    //Debug.WriteLine(moveSpeed, currentSpeed.ToString());
                }

                viewCamera3D.Target = viewCamera3D.Position + forward;
            }
            if (Raylib.IsMouseButtonReleased(WorldManager.instance.hotkeySettings.ActivateCamera))
            {
                Raylib.ShowCursor();
            }
        }


    }
}
