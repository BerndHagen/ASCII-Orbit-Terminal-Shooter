using System.Text;
using AsciiOrbit.Audio;
using AsciiOrbit.Gameplay;
using AsciiOrbit.Input;
using AsciiOrbit.Rendering;
using AsciiOrbit.UI;

namespace AsciiOrbit;

/// <summary>
/// Renders representative frames to plain text via the headless renderer. Run with <c>--dump</c>, it
/// lets you eyeball the screen layout (HUD, play field, menus) without an interactive terminal.
/// </summary>
internal static class DebugDump
{
    public static int Run()
    {
        try { Console.OutputEncoding = Encoding.UTF8; } catch { }

        var renderer = new HeadlessRenderer(Layout.CanvasWidth, Layout.CanvasHeight);
        using var sound = new SilentSoundEngine();
        var session = new PlaySession(sound, new Random(2024), GameDifficulty.GameA);
        var table = HighScoreTable.Default();

        // Let a handful of invaders accumulate so the gameplay frame is representative.
        IReadOnlyList<InputCommand> none = Array.Empty<InputCommand>();
        for (int i = 0; i < 600; i++)
            session.Update(1.0 / 60, none);

        var board = new Scoreboard(session.Score, 0, 1, false, table.Top.Score);
        var summary = new MatchSummary(new[] { session.Score }, session.Level, session.Elapsed);

        DumpFrame("GAMEPLAY", renderer, () => { session.Draw(renderer); Hud.Draw(renderer, session, board); });
        DumpFrame("TITLE", renderer, () => Screens.DrawTitle(renderer, time: 0.1, menuIndex: 0));
        DumpFrame("NAME ENTRY", renderer, () => Screens.DrawNameEntry(renderer, time: 0.1, score: 31200, new[] { 'B', 'H', 'A' }, slot: 2, table));
        DumpFrame("GAME OVER", renderer, () => Screens.DrawGameOver(renderer, time: 0.1, summary, table, newRecordRank: 0));
        return 0;
    }

    private static void DumpFrame(string label, HeadlessRenderer renderer, Action draw)
    {
        renderer.BeginFrame();
        draw();
        renderer.EndFrame();

        Console.WriteLine(new string('=', 20) + " " + label + " " + new string('=', 20));
        Console.WriteLine(renderer.Dump());
    }
}
