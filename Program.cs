using IceSaw2.Manager;

namespace IceSaw2
{
    internal class Program
    {
        public static WorldManager worldManager = new WorldManager();

        public static void Main()
        {
            worldManager.Initalise();
        }
    }
}
