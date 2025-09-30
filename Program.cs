using IceSaw2.Manager;

namespace IceSaw2
{
    internal class Program
    {
        public static void Main()
        {
            WorldManager worldManager = new();

            Core engineCore = new();
            while (engineCore.isRunning)
            {
                engineCore.InputProccessing();
                engineCore.LogicProccessing();
                engineCore.RenderProcessing();
            }
        }
    }
}
