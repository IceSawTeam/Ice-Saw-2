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

            //Raylib.DrawTexture(skyboxTexture2Ds[0], 100, 100, Color.White);
        }

        public void LogicUpdate()
        {
            //Update Camera
            Raylib.UpdateCamera(ref worldCamera3D, CameraMode.Free);
        }


    }
}
