using IceSaw2.LevelObject;
using IceSaw2.LevelObject.TrickyObjects;
using IceSaw2.Manager.Tricky;
using ImGuiNET;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;

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
        private int screenWidth { get { return Raylib.GetScreenWidth(); } }
        private int screenHeight { get { return Raylib.GetScreenHeight(); } }

        private float axisLineSize = 1000f;

        public Camera3D viewCamera3D = new Camera3D();
        public bool Open = true;

        public List<BaseObject> RenderItems = new List<BaseObject>();

        public void Initilize()
        {
            viewCamera3D.Position = new Vector3(0f, 0f, 0.1f);
            viewCamera3D.Target = viewCamera3D.Position + new Vector3(0, 1, 0);
            viewCamera3D.Up = new Vector3(0, 0, 1);
            viewCamera3D.FovY = 65f;
            viewCamera3D.Projection = CameraProjection.Perspective;

        }

        bool BatchTestLoad = false;

        Mesh mesh;
        Texture2D texture;
        Material material;
        Matrix4x4 matrix4X4 = Matrix4x4.Identity;
        Shader shader;



        public override void RenderUpdate()
        {
            //Render 3D
            Raylib.BeginMode3D(viewCamera3D);
            Rlgl.DisableBackfaceCulling();

            Rlgl.DisableDepthMask();
            //Render Skybox

            //Nasty Check to see if loaded
            if (TrickyDataManager.trickySkyboxPrefabObjects.Count != 0)
            {
                if (!BatchTestLoad)
                {
                    BatchTestLoad = true;
                    var Loaded = Batch.Skybox.FromFolder(Path.Combine(TrickyDataManager.LoadPath, "Skybox"));
                    mesh = Loaded.Item1;
                    Raylib.UploadMesh(ref mesh, false);
                    var Image = Loaded.Item2;
                    texture = Raylib.LoadTextureFromImage(Image);
                    material = Raylib.LoadMaterialDefault();
                    // Raylib.SetMaterialTexture(ref material, MaterialMapIndex.Diffuse, texture);
                    matrix4X4 = Raymath.MatrixScale(BaseObject.WorldScale, BaseObject.WorldScale, BaseObject.WorldScale);
                    shader = Raylib.LoadShader("/home/eric/Documents/Github/Ice-Saw-2/Shaders/Instance.vs",
                                                "/home/eric/Documents/Github/Ice-Saw-2/Shaders/Instance.fs");

                    // unsafe
                    // {
                    //     int* locs = (int*)shader.Locs;
                    //     locs[(int)ShaderLocationIndex.MatrixMvp] = Raylib.GetShaderLocation(shader, "mvp");
                    //     locs[(int)ShaderLocationIndex.VectorView] = Raylib.GetShaderLocation(shader, "viewPos");
                    //     locs[(int)ShaderLocationIndex.MatrixModel] = Raylib.GetShaderLocationAttrib(
                    //         shader,
                    //         "instanceTransform"
                    //     );
                    // }
                    material.Shader = shader;
                }

                Rlgl.EnableDepthMask();

                List<Matrix4x4> instances = [];
                for (var y = 0; y < 10; y++)
                {
                    for (var x = 0; x < 10; x++)
                    {
                        var offset = 5000;
                        var mat = Raymath.MatrixScale(BaseObject.WorldScale, BaseObject.WorldScale, BaseObject.WorldScale);
                        mat = Raymath.MatrixMultiply(Raymath.MatrixTranslate(x * offset, y * offset, 0), mat);
                        instances.Add(mat);
                    }
                }


                // foreach (var instance in instances)
                // {
                //     Raylib.DrawMesh(mesh, material, instance);
                // }

                Matrix4x4[] arr = instances.ToArray();
                Raylib.DrawMeshInstanced(mesh, material, arr, instances.Count);



            }

            //for (int i = 0; i < TrickyDataManager.trickySkyboxPrefabObjects.Count; i++)
            //{
            //    var TempLocation = TrickyDataManager.trickySkyboxPrefabObjects[i].Position;

            //    TrickyDataManager.trickySkyboxPrefabObjects[i].Position = viewCamera3D.Position / BaseObject.WorldScale;
            //    TrickyDataManager.trickySkyboxPrefabObjects[i].Render();
            //    TrickyDataManager.trickySkyboxPrefabObjects[i].Position = TempLocation;
            //}
            Rlgl.EnableDepthMask();            

            //Render Default
            Raylib.DrawLine3D(new Vector3(-axisLineSize, 0, 0), new Vector3(axisLineSize, 0, 0), new Color(212, 28, 4));
            Raylib.DrawLine3D(new Vector3(0, -axisLineSize, 0), new Vector3(0, axisLineSize, 0), new Color(17, 212, 4));
            Raylib.DrawLine3D(new Vector3(0, 0, -axisLineSize), new Vector3(0, 0, axisLineSize), new Color(2, 99, 224));

            //GenerateRenderList();
            var RenderList = CollectionsMarshal.AsSpan(RenderItems);

            ////Render Objects
            //for (int i = 0; i < RenderList.Length; i++)
            //{
            //    RenderList[i].Render();
            //}

            Raylib.EndMode3D();


            //Render UI

            float menuBarHeight = ImGui.GetFrameHeight(); // Typically height of main menu bar

            // Sidebar dimensions
            int sidebarWidth = 300;

            // Position and size sidebar *below* the main menu bar
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, menuBarHeight), ImGuiCond.Always);
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(sidebarWidth, screenHeight - menuBarHeight), ImGuiCond.Always);

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

        public void GenerateRenderList()
        {
            RenderItems = new List<BaseObject>();

            RenderItems.AddRange(TrickyDataManager.trickyPatchObjects);
            RenderItems.AddRange(TrickyDataManager.trickyInstanceObjects);
            RenderItems.AddRange(TrickyDataManager.trickySplineObjects);
            RenderItems.AddRange(TrickyDataManager.trickyAIPAIPath);
            RenderItems.AddRange(TrickyDataManager.trickyAIPRaceLine);
            RenderItems.AddRange(TrickyDataManager.trickySOPAIPath);
            RenderItems.AddRange(TrickyDataManager.trickySOPRaceLine);
            RenderItems.AddRange(TrickyDataManager.trickyLightObjects);
            RenderItems.AddRange(TrickyDataManager.trickyCameraObjects);
            RenderItems.AddRange(TrickyDataManager.trickyPaticleInstanceObjects);
        }

        public override void LogicUpdate()
        {
            //Update Camera
            //Raylib.UpdateCamera(ref viewCamera3D, CameraMode.Free);
            // Viewport Camera
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
