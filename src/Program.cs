using AsciiOrbit.Audio;
using AsciiOrbit.Input;
using AsciiOrbit.Rendering;

namespace AsciiOrbit;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (HasFlag(args, "--selftest"))
            return SelfTest.Run();

        if (HasFlag(args, "--dump"))
            return DebugDump.Run();

        // The game draws with absolute cursor positioning and reads keys live, so it needs a real
        // interactive terminal. Bail out cleanly (instead of throwing) when piped or redirected.
        if (Console.IsInputRedirected || Console.IsOutputRedirected)
        {
            Console.Error.WriteLine("ASCII Orbit needs an interactive terminal; run it directly in a console window.");
            Console.Error.WriteLine("Use \"ASCII Orbit --selftest\" to run the headless simulation check.");
            return 1;
        }

        try { Console.Title = "ASCII Orbit"; } catch { }

        bool audioEnabled = !HasFlag(args, "--mute");
        using ISoundEngine sound = SoundEngine.Create(audioEnabled);

        var renderer = new ConsoleRenderer(Layout.CanvasWidth, Layout.CanvasHeight);
        var input = new ConsoleInputSource();
        var game = new Game(renderer, input, sound, new Random(), new FileHighScoreStore());
        game.Run();
        return 0;
    }

    private static bool HasFlag(string[] args, string flag)
    {
        foreach (string arg in args)
            if (string.Equals(arg, flag, StringComparison.OrdinalIgnoreCase))
                return true;
        return false;
    }
}
