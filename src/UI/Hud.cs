using AsciiOrbit.Gameplay;
using AsciiOrbit.Rendering;

namespace AsciiOrbit.UI;

/// <summary>The score header state for one frame: both players' scores, who is active, and the high score.</summary>
internal readonly record struct Scoreboard(int Score1, int Score2, int ActivePlayer, bool TwoPlayer, int HighScore);

/// <summary>
/// Draws the in-game cabinet furniture: the bezel and dividers, a 1UP / HIGH SCORE / 2UP score
/// header across the top (labels in red, the active player's blinking), and a bottom status bar with
/// the remaining lives, any active power-ups and the level.
/// </summary>
internal static class Hud
{
    private const int Left = 2;
    private static int Right => Layout.CanvasWidth - 3; // 77

    public static void Draw(IRenderer r, PlaySession session, Scoreboard board)
    {
        Frame.DrawGameChrome(r, ConsoleColor.DarkCyan);
        DrawScoreHeader(r, session, board);
        DrawStatusBar(r, session);
    }

    private static void DrawScoreHeader(IRenderer r, PlaySession session, Scoreboard board)
    {
        // The active player's label blinks (red -> dark red), arcade-style.
        bool flashOn = (int)(session.Elapsed * 2) % 2 == 0;
        ConsoleColor label1 = board.ActivePlayer == 1 && !flashOn ? ConsoleColor.DarkRed : ConsoleColor.Red;
        ConsoleColor label2 = board.ActivePlayer == 2 && !flashOn ? ConsoleColor.DarkRed : ConsoleColor.Red;

        r.Write(Left, Layout.HeaderLabelRow, "1UP", label1);
        r.WriteCentered(Layout.HeaderLabelRow, "HIGH SCORE", ConsoleColor.Red);
        RightAlign(r, Layout.HeaderLabelRow, "2UP", label2);

        r.Write(Left, Layout.HeaderScoreRow, $"{board.Score1:D6}", ConsoleColor.White);
        r.WriteCentered(Layout.HeaderScoreRow, $"{board.HighScore:D6}", ConsoleColor.White);
        RightAlign(r, Layout.HeaderScoreRow, $"{board.Score2:D6}", board.TwoPlayer ? ConsoleColor.White : ConsoleColor.DarkGray);
    }

    private static void DrawStatusBar(IRenderer r, PlaySession session)
    {
        int row = Layout.FooterRow;

        // Remaining lives as a count plus a single ship icon, e.g. "3 ▲".
        string lives = $"{session.Lives} ";
        r.Write(Left, row, lives, ConsoleColor.White);
        r.Set(Left + lives.Length, row, Symbols.Player, ConsoleColor.White);

        // Active power-ups just right of the lives, only while they last.
        DrawEffects(r, session.Player, row, Left + lives.Length + 4);

        RightAlign(r, row, $"LEVEL {session.Level:D2}", ConsoleColor.White);
    }

    private static void DrawEffects(IRenderer r, Player player, int row, int x)
    {
        if (player.RapidFireActive)
            x = PutEffect(r, x, row, Symbols.RapidFire, player.RapidFireRemaining, ConsoleColor.Yellow);
        if (player.SpreadActive)
            x = PutEffect(r, x, row, Symbols.Spread, player.SpreadRemaining, ConsoleColor.Cyan);
        if (player.HasShield)
            r.Set(x, row, Symbols.Shield, ConsoleColor.Blue);
    }

    private static int PutEffect(IRenderer r, int x, int row, char symbol, double remaining, ConsoleColor color)
    {
        r.Set(x, row, symbol, color);
        string seconds = $"{Math.Ceiling(remaining):0}";
        r.Write(x + 1, row, seconds, color);
        return x + 1 + seconds.Length + 2;
    }

    private static void RightAlign(IRenderer r, int row, string text, ConsoleColor color)
        => r.Write(Right - text.Length + 1, row, text, color);
}
