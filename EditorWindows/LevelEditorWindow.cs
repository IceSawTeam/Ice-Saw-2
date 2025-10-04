using IceSaw2.LevelObject;
using IceSaw2.Manager.Tricky;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;

namespace IceSaw2.EditorWindows
{
    public class LevelEditorWindow : BaseEditorWindow
    {
        private const float BOOST_MULTIPLIER = 2.0f;

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
            viewCamera3D.Position = new Vector3(0f, 0f, 0.1f);
            viewCamera3D.Target = viewCamera3D.Position + new Vector3(0, 1, 0);
            viewCamera3D.Up = new Vector3(0, 0, 1);
            viewCamera3D.FovY = 65f;
            viewCamera3D.Projection = CameraProjection.Perspective;

            screenWidth = Settings.General.Instance.data.ScreenWidth;
            screenHeight = Settings.General.Instance.data.ScreenHeight;
        }

        public override void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(viewCamera3D);
            Rlgl.DisableBackfaceCulling();


            Rlgl.DisableDepthMask();
            //Render Skybox
            for (int i = 0; i < TrickyDataManager.trickySkyboxPrefabObjects.Count; i++)
            {
                var TempLocation = TrickyDataManager.trickySkyboxPrefabObjects[i].Position;

                TrickyDataManager.trickySkyboxPrefabObjects[i].Position = viewCamera3D.Position / BaseObject.WorldScale;
                TrickyDataManager.trickySkyboxPrefabObjects[i].Render();
                TrickyDataManager.trickySkyboxPrefabObjects[i].Position = TempLocation;
            }
            Rlgl.EnableDepthMask();            

            //Render Default
            Raylib.DrawLine3D(new Vector3(-axisLineSize, 0, 0), new Vector3(axisLineSize, 0, 0), new Color(212, 28, 4));
            Raylib.DrawLine3D(new Vector3(0, -axisLineSize, 0), new Vector3(0, axisLineSize, 0), new Color(17, 212, 4));
            Raylib.DrawLine3D(new Vector3(0, 0, -axisLineSize), new Vector3(0, 0, axisLineSize), new Color(2, 99, 224));

            //Render Objects
            for (int i = 0; i < TrickyDataManager.trickyPatchObjects.Count; i++)
            {
                TrickyDataManager.trickyPatchObjects[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickyInstanceObjects.Count; i++)
            {
                TrickyDataManager.trickyInstanceObjects[i].Render();
            }

            //Render Wires
            for (int i = 0; i < TrickyDataManager.trickySplineObjects.Count; i++)
            {
                TrickyDataManager.trickySplineObjects[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickyAIPAIPath.Count; i++)
            {
                TrickyDataManager.trickyAIPAIPath[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickyAIPRaceLine.Count; i++)
            {
                TrickyDataManager.trickyAIPRaceLine[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickySOPAIPath.Count; i++)
            {
                TrickyDataManager.trickySOPAIPath[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickySOPRaceLine.Count; i++)
            {
                TrickyDataManager.trickySOPRaceLine[i].Render();
            }

            //Render Sprites
            for (int i = 0; i < TrickyDataManager.trickyLightObjects.Count; i++)
            {
                TrickyDataManager.trickyLightObjects[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickyCameraObjects.Count; i++)
            {
                TrickyDataManager.trickyCameraObjects[i].Render();
            }

            for (int i = 0; i < TrickyDataManager.trickyPaticleInstanceObjects.Count; i++)
            {
                TrickyDataManager.trickyPaticleInstanceObjects[i].Render();
            }

            Raylib.EndMode3D();


            //Render UI

            float menuBarHeight = ImGui.GetFrameHeight(); // Typically height of main menu bar

            // Sidebar dimensions
            int sidebarWidth = 300;

            // Position and size sidebar *below* the main menu bar
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(sidebarWidth, Raylib.GetScreenHeight() - menuBarHeight), ImGuiCond.Always);

            // Optional: remove decorations
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
            ImGui.Begin("Side Panel", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse);

            for (int i = 0; i < TrickyDataManager.LevelNodeTree.Count; i++)
            {
                TrickyDataManager.LevelNodeTree[i].HierarchyRender();
            }

            // Add your sidebar content here

            ImGui.End();
            ImGui.PopStyleVar(2);
        }

        public override void LogicUpdate()
        {
            //Update Camera
            //Raylib.UpdateCamera(ref viewCamera3D, CameraMode.Free);
            // Viewport Camera
            if (Raylib.IsMouseButtonDown(Settings.Hotkey.Instance.data.ActivateCamera))
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

                // Movement
                Vector3 newPosition = new Vector3(0, 0, 0);
                // Vector3 newPosition = Raymath.Vector3Zero();
                float currentSpeed = moveSpeed;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Boost)) currentSpeed *= BOOST_MULTIPLIER;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Forward)) newPosition += forward;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Back)) newPosition -= forward;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Right)) newPosition -= right;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Left)) newPosition += right;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Up)) newPosition += up;
                if (Raylib.IsKeyDown(Settings.Hotkey.Instance.data.Down)) newPosition -= up;
                viewCamera3D.Position += Raymath.Vector3Normalize(newPosition) * currentSpeed;

                float wheel = Raylib.GetMouseWheelMove();
                if (wheel != 0)
                {
                    moveSpeed += wheel * moveSpeedStep;
                    moveSpeed = Math.Clamp(moveSpeed, 0.008f, 200f);
                    //Debug.WriteLine(moveSpeed, currentSpeed.ToString());
                }
                viewCamera3D.Target = viewCamera3D.Position + forward;
            }
            if (Raylib.IsMouseButtonReleased(Settings.Hotkey.Instance.data.ActivateCamera))
            {
                Raylib.ShowCursor();
            }
        }


    }
}
