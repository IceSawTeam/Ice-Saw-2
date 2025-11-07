using Raylib_cs;

namespace IceSaw2
{
    internal class Program
    {

        public static void Main()
        {
            //Initalize
            Profiler.AddProfile("Input");
            Profiler.AddProfile("Logic");
            Profiler.AddProfile("Render");
            Profiler.AddProfile("-Patches");
            Profiler.AddProfile("-Instances");

            Core engineCore = new();
            while (engineCore.isRunning && !Raylib.WindowShouldClose())
            {
                Profiler.InitialStartTime = Raylib.GetTime();

                Profiler.SetStartTime("Input");
                engineCore.InputProccessing();
                Profiler.UpdateTime("Input");

                Profiler.SetStartTime("Logic");
                engineCore.LogicProccessing();
                Profiler.UpdateTime("Logic");

                Profiler.SetStartTime("Render");
                engineCore.RenderProcessing();
                Profiler.UpdateTime("Render");

                Profiler.TotalTime = (float)((Raylib.GetTime() - Profiler.InitialStartTime) * 1000);
            }
            engineCore.Exiting();
        }
    }
}
