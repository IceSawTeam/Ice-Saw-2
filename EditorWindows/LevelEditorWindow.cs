using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using ImGuiNET;
using Raylib_cs;
using rlImGui_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IceSaw2.WorldManager;

namespace IceSaw2.EditorWindows
{
    public class LevelEditorWindow
    {
        Camera3D worldCamera3D = new Camera3D();
        static FilePicker filePicker = new FilePicker();
        public bool Open = true;
        public void Initilize()
        {
            worldCamera3D.Position = new System.Numerics.Vector3(0, 100, 100);
            worldCamera3D.Target = Vector3.Zero;
            worldCamera3D.Up = new Vector3(0, 0, 1);
            worldCamera3D.FovY = 45f;
            worldCamera3D.Projection = CameraProjection.Perspective;
        }

        public void MainUpdate()
        {
            LogicUpdate();

            RenderUpdate();
        }

        public void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(worldCamera3D);

            //Render Skybox

            //Render Default
            Raylib.DrawGrid(100, 1);

            //Render Objects

            for (int i = 0; i < WorldManager.instance.trickyPatchObjects.Count; i++)
            {
                WorldManager.instance.trickyPatchObjects[i].Render();
            }

            for (int i = 0; i < WorldManager.instance.trickyInstanceObjects.Count; i++)
            {
                WorldManager.instance.trickyInstanceObjects[i].Render();
            }

            //Render Wires

            Raylib.EndMode3D();

            //Render UI
            //rlImGui.Begin();

            //ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            //ImGui.SetNextWindowSizeConstraints(new Vector2(400, 400), new Vector2((float)Raylib.GetScreenWidth(), (float)Raylib.GetScreenHeight()));

            //if (ImGui.Begin("3D View", ref Open, ImGuiWindowFlags.NoScrollbar))
            //{
            //    Focused = ImGui.IsWindowFocused(ImGuiFocusedFlags.ChildWindows);

            //    draw the view
            //    rlImGui.ImageRenderTextureFit(ViewTexture, true);

            //    ImGui.End();
            //}
            //ImGui.PopStyleVar();

            //rlImGui.End();

            filePicker.Draw();

            //Raylib.DrawTexture(skyboxTexture2Ds[0], 100, 100, Color.White);
        }

        public void LogicUpdate()
        {
            //Update Camera
            Raylib.UpdateCamera(ref worldCamera3D, CameraMode.Free);

            string picked = filePicker.GetSelectedFile();
            if (picked != null)
            {
                Console.WriteLine("Picked: " + picked);
                // You can now do something with the file
                WorldManager.instance.LoadProject(picked);
            }

            //Object Collision
            if (Raylib.IsKeyPressed(KeyboardKey.F))
            {
                filePicker.Open();
            }

            filePicker.Update();
        }


    }
}
