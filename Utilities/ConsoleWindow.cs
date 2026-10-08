using System.Runtime.InteropServices;

namespace IceSaw2.Utilities
{
    public class ConsoleWindow
    {
        static bool Initialised = false;

        public static void GenerateConsole()
        {
            if (OperatingSystem.IsWindows())
            {
                if (!Initialised)
                {
                    if (!AttachConsole(-1))
                        AllocConsole();

                    Initialised = true;
                }
                else
                {
                    var handle = GetConsoleWindow();
                    ShowWindow(handle, SW_SHOW);
                }
            }
        }

        public static void CloseConsole()
        {
            if (OperatingSystem.IsWindows())
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SW_HIDE);
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
    }
}
