/*
This is the engine core. The main game loop runs here. All other modules
or managers will be linked to Core.
*/

using Raylib_cs;

namespace IceSaw2
{
    public class Core
    {
        public enum State
        {
            MAIN_WINDOW,
            OPENING_FILE,
            CONTROLLING_CAMERA,
            USING_XFORM_GIZMO,
            ADDING_ENTITIY,
            MOVING_HIERARCHY_ENTITY,
            PATCH_EDITING_SAC_MODE,
            OTHER,
        }
        public const int MAX_FPS = 60;

        public bool isRunning = true;
        // Raylib already has a GetFrameTime function, maybe set this to it every frame for ease of use?
        private float delta { get { return Raylib.GetFrameTime(); } }
        public State currentState = State.MAIN_WINDOW;


        public Core()
        {
            // Initialize managers
            // Initialize modules
            // Initialize raylib
            // Initialize cache
        }


        ~Core()
        {
            // Close raylib
            // Close managers
            // Clear cache
            // Close window
        }

        public void InputProccessing()
        {
            // Check for user input with ImGui and on the 3D viewport.
            // GUI should consume input so it doesnt pass to the 3D viewport.
            // Gizmo input handling.
            // Terrain Patch collision detection with mouse might require multi-threading.
            // Can change State from here.
        }


        public void LogicProccessing()
        {
            // Set the isRunning variable to exit loop.
            // Can change State from here.
            // Communicate with other modules and managers.
        }


        public void RenderProcessing()
        {
            // Render the 3D viewport including world gizmos and xform gizmo.
            // Render props, terrain, paths, wireframe, skybox, and bounding boxes.
            // Render Imgui. Lock or anchor the Imgui scale to 1080p, no need to big resolution flexibility.
            // Cap framerate
        }
    }
}
