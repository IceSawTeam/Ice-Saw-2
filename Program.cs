using Raylib_cs;

namespace IceSaw2
{
    internal class Program
    {
        public static void Main()
        {
            Core engineCore = new();
            while (engineCore.isRunning && !Raylib.WindowShouldClose())
            {
                engineCore.InputProccessing();
                engineCore.LogicProccessing();
                engineCore.RenderProcessing();
            }
        }
    }
}
