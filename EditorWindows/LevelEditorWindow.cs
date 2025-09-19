using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.Manager.WorldManager;

namespace IceSaw2.EditorWindows
{
    public class LevelEditorWindow : BaseEditorWindow
    {
        Camera3D worldCamera3D = new Camera3D();
        public bool Open = true;
        public void Initilize()
        {
            worldCamera3D.Position = new System.Numerics.Vector3(0, 100, 100);
            worldCamera3D.Target = Vector3.Zero;
            worldCamera3D.Up = new Vector3(0, 0, 1);
            worldCamera3D.FovY = 65f;
            worldCamera3D.Projection = CameraProjection.Perspective;

            
        }

        public override void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(worldCamera3D);
            Rlgl.DisableBackfaceCulling();

            if (Raylib.IsMouseButtonDown(MouseButton.Right))
            {
                Raylib.DisableCursor();
            }
            if (Raylib.IsMouseButtonReleased(MouseButton.Right))
            {
                Raylib.EnableCursor();
            }


            Rlgl.DisableDepthMask();
            //Render Skybox
            for (int i = 0; i < DataManager.trickySkyboxPrefabObjects.Count; i++)
            {
                var TempLocation = DataManager.trickySkyboxPrefabObjects[i].Position;

                DataManager.trickySkyboxPrefabObjects[i].Position = worldCamera3D.Position / BaseObject.WorldScale;
                DataManager.trickySkyboxPrefabObjects[i].Render();
                DataManager.trickySkyboxPrefabObjects[i].Position = TempLocation;
            }
            Rlgl.EnableDepthMask();

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

        float cameraSpeed = 0.1f;
        float mouseSensitivity = 0.003f;
        float yaw = 0.0f;
        float pitch = 0.0f;

        public override void LogicUpdate()
        {
            //Update Camera
            Raylib.UpdateCamera(ref worldCamera3D, CameraMode.Free);
        }


    }
}
