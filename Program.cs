using Raylib_cs;

namespace IceSaw2
{
    internal class Program
    {
        public static float InputTime;
        public static float LogicTime;
        public static float RenderTime;
        public static float TotalTime;

        private static double InitialStartTime;
        private static double StartTime;

        public static void Main()
        {
            Core engineCore = new();
            while (engineCore.isRunning && !Raylib.WindowShouldClose())
            {
                InitialStartTime = Raylib.GetTime();
                engineCore.InputProccessing();
                InputTime = (float)((Raylib.GetTime() - InitialStartTime) * 1000);

                StartTime = Raylib.GetTime();
                engineCore.LogicProccessing();
                LogicTime = (float)((Raylib.GetTime() - StartTime) * 1000);

                StartTime = Raylib.GetTime();
                engineCore.RenderProcessing();
                RenderTime = (float)((Raylib.GetTime() - StartTime) * 1000);

                TotalTime = (float)((Raylib.GetTime() - InitialStartTime) * 1000);
            }
            engineCore.Exiting();
        }
    }
}
