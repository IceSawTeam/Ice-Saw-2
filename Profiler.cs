using ImGuiNET;
using Raylib_cs;
using System.Numerics;

namespace IceSaw2
{
    public static class Profiler
    {
        public static double TotalTime;
        public static double InitialStartTime;

        const int FRAME_TIME_HISTORY_SIZE = 100;
        static float[] frameTimes = new float[FRAME_TIME_HISTORY_SIZE];
        static int frameIndex = 0;

        private static Dictionary<string, int> ProfileDictionary = new Dictionary<string, int>();
        private static List<Profile> profiles = new List<Profile>();

        public static void AddProfile(string Item)
        {
            Profile profile = new Profile();
            profile.Name = Item;
            profiles.Add(profile);
            ProfileDictionary.Add(Item, profiles.Count - 1);
        }

        public static void SetStartTime(string Item)
        {
            var ProfileID = ProfileDictionary[Item];
            var Profile = profiles[ProfileID];
            Profile.Hide = false;
            Profile.StartTime = Raylib.GetTime();
            profiles[ProfileID] = Profile;
        }

        public static void UpdateTime(string Item)
        {
            var ProfileID = ProfileDictionary[Item];
            var profile = profiles[ProfileID];
            profile.Time = (float)((Raylib.GetTime() - profile.StartTime) * 1000);
            profiles[ProfileID] = profile;
        }

        public static void Render()
        {
            ImGui.SetNextWindowPos(new Vector2(0, Raylib.GetScreenHeight() - 300), ImGuiCond.FirstUseEver);
            ImGui.SetNextWindowSize(new Vector2(160, 300), ImGuiCond.FirstUseEver);
            ImGui.Begin("Profiler (ms)");

            float spacing = 80;

            for (int i = 0; i < profiles.Count; i++)
            {
                if (!profiles[i].Hide)
                {
                    var Profile = profiles[i];
                    ImGui.Text(Profile.Name + ":"); ImGui.SameLine(spacing); ImGui.Text($"{Profile.Time:F3}");
                    Profile.Hide = true;
                    profiles[i] = Profile;
                }
            }

            ImGui.Separator();

            float frameTime = Raylib.GetFrameTime() * 1000.0f;

            ImGui.Text($"FrameTime (Delta): {frameTime:F3}");

            frameTimes[frameIndex] = frameTime;
            frameIndex = (frameIndex + 1) % FRAME_TIME_HISTORY_SIZE;

            ImGui.PlotLines(
                label: "##FrameTimeGraph",
                values: ref frameTimes[0],
                values_count: FRAME_TIME_HISTORY_SIZE,
                values_offset: frameIndex,
                overlay_text: "ms/frame",
                scale_min: 0,
                scale_max: 63.3f, // ~60 FPS
                graph_size: new Vector2(0, 80)
            );

            ImGui.End();
        }

        struct Profile
        {
            public bool Hide;

            public string Name;
            public double StartTime;
            public double Time;
        }
    }
}
