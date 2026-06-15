using AsciiOrbit.Gameplay;
using AsciiOrbit.Rendering;

namespace AsciiOrbit.UI;

/// <summary>The end-of-match results passed to the game-over screen.</summary>
internal readonly record struct MatchSummary(int[] Scores, int Level, double Elapsed)
{
    public bool TwoPlayer => Scores.Length > 1;
}

/// <summary>
/// Pure drawing routines for the non-gameplay screens, styled as an arcade attract screen: a bezel
/// frame, a flickering logo, a game-select menu, and the player-ready, name-entry and results
/// screens (the last two show the top-five high-score table). The game loop owns all timing and input.
/// </summary>
internal static class Screens
{
    public static readonly string[] MenuOptions =
    {
        "1 PLAYER  GAME A",
        "1 PLAYER  GAME B",
        "2 PLAYER  GAME A",
        "2 PLAYER  GAME B",
    };

    public static void DrawTitle(IRenderer r, double time, int menuIndex)
    {
        Frame.DrawScreenBorder(r, ConsoleColor.DarkCyan);

        // The only motion is the logo flickering quickly between two near-identical shades.
        ConsoleColor logo = FastFlash(time) ? ConsoleColor.Cyan : ConsoleColor.DarkCyan;
        Art.DrawBanner(r, Art.Title, topY: 3, logo);
        r.WriteCentered(7, "T E R M I N A L   S H O O T E R", ConsoleColor.DarkCyan);

        r.WriteCentered(10, "- SELECT GAME -", ConsoleColor.Yellow);
        int left = (Layout.CanvasWidth - MenuOptions[0].Length) / 2;
        for (int i = 0; i < MenuOptions.Length; i++)
        {
            bool selected = i == menuIndex;
            r.Set(left - 2, 12 + i, selected ? '>' : ' ', ConsoleColor.Yellow);
            r.Write(left, 12 + i, MenuOptions[i], selected ? ConsoleColor.White : ConsoleColor.Gray);
        }

        r.WriteCentered(17, "GAME A = NORMAL     GAME B = HARD", ConsoleColor.DarkGray);

        if (Blink(time))
            r.WriteCentered(20, "PRESS SPACE TO START", ConsoleColor.White);

        r.WriteCentered(24, "[<- ->] MOVE     [SPACE] FIRE     [P] PAUSE     [ESC] QUIT", ConsoleColor.DarkGray);
        r.WriteCentered(26, $"{Symbols.Copyright} 2024 BERND HAGEN", ConsoleColor.DarkGray);
    }

    /// <summary>Overlay shown before a turn begins; drawn on top of the cabinet chrome.</summary>
    public static void DrawReadyOverlay(IRenderer r, int player, bool twoPlayer)
    {
        int midY = Layout.ArenaTop + Layout.ArenaHeight / 2;
        if (twoPlayer)
            r.WriteCentered(midY, $"GET READY P{player}", player == 1 ? ConsoleColor.Cyan : ConsoleColor.Magenta);
        else
            r.WriteCentered(midY, "GET READY", ConsoleColor.White);
    }

    public static void DrawNameEntry(IRenderer r, double time, int score, char[] initials, int slot, HighScoreTable table)
    {
        Frame.DrawScreenBorder(r, ConsoleColor.DarkYellow);

        ConsoleColor banner = FastFlash(time) ? ConsoleColor.Yellow : ConsoleColor.DarkYellow;
        Art.DrawBanner(r, Art.HighScore, topY: 3, banner);

        r.WriteCentered(9, $"{score:D6}", ConsoleColor.White);
        r.WriteCentered(11, "ENTER YOUR INITIALS", ConsoleColor.Gray);

        int center = Layout.CanvasWidth / 2;
        for (int i = 0; i < initials.Length; i++)
        {
            int x = center - 4 + i * 4;
            bool active = i == slot;
            ConsoleColor color = active && !Blink(time) ? ConsoleColor.DarkGray : ConsoleColor.White;
            r.Set(x, 13, initials[i], color);
            if (active)
                r.Set(x, 14, '^', ConsoleColor.Yellow);
        }

        DrawHighScoreTable(r, 16, table, highlightRank: -1);

        r.WriteCentered(26, "[UP/DOWN] LETTER     [<- ->] SLOT     [SPACE] NEXT", ConsoleColor.DarkGray);
    }

    public static void DrawGameOver(IRenderer r, double time, MatchSummary summary, HighScoreTable table, int newRecordRank)
    {
        Frame.DrawScreenBorder(r, ConsoleColor.DarkRed);

        ConsoleColor banner = FastFlash(time) ? ConsoleColor.Red : ConsoleColor.DarkRed;
        Art.DrawBanner(r, Art.GameOver, topY: 3, banner);

        DrawHighScoreTable(r, 10, table, newRecordRank);

        if (summary.TwoPlayer)
        {
            r.WriteCentered(19, $"P1  {summary.Scores[0]:D6}       P2  {summary.Scores[1]:D6}", ConsoleColor.White);
            r.WriteCentered(20, WinnerText(summary.Scores[0], summary.Scores[1]), ConsoleColor.Yellow);
        }
        else
        {
            r.WriteCentered(19, $"YOUR SCORE  {summary.Scores[0]:D6}    RANK {RankTable.ForScore(summary.Scores[0])}", ConsoleColor.White);
        }

        if (Blink(time))
            r.WriteCentered(24, "PRESS SPACE TO CONTINUE", ConsoleColor.White);
        r.WriteCentered(26, "[ESC] QUIT", ConsoleColor.DarkGray);
    }

    public static void DrawPauseOverlay(IRenderer r)
    {
        int midY = Layout.ArenaTop + Layout.ArenaHeight / 2;
        r.WriteCentered(midY - 1, "[ PAUSED ]", ConsoleColor.Yellow);
        r.WriteCentered(midY + 1, "PRESS P TO RESUME", ConsoleColor.Gray);
    }

    public static void DrawResizePrompt(IRenderer r, int consoleWidth, int consoleHeight)
    {
        int midY = r.Height / 2;
        r.WriteCentered(midY - 2, "TERMINAL TOO SMALL", ConsoleColor.Yellow);
        r.WriteCentered(midY, $"Please resize to at least {Layout.MinTerminalWidth} x {Layout.MinTerminalHeight}.", ConsoleColor.Gray);
        r.WriteCentered(midY + 1, $"Current size: {consoleWidth} x {consoleHeight}", ConsoleColor.DarkGray);
        r.WriteCentered(midY + 3, "[ESC] QUIT", ConsoleColor.DarkGray);
    }

    private static void DrawHighScoreTable(IRenderer r, int topY, HighScoreTable table, int highlightRank)
    {
        r.WriteCentered(topY, "HIGH SCORES", ConsoleColor.Yellow);
        int left = (Layout.CanvasWidth - 15) / 2;
        IReadOnlyList<HighScoreEntry> entries = table.Entries;
        for (int i = 0; i < entries.Count; i++)
        {
            ConsoleColor color = i == highlightRank ? ConsoleColor.White : ConsoleColor.DarkGray;
            int row = topY + 2 + i;
            r.Write(left, row, $"{i + 1}", color);
            r.Write(left + 3, row, entries[i].Name.PadRight(3), color);
            r.Write(left + 8, row, $"{entries[i].Score:D6}", color);
        }
    }

    private static string WinnerText(int p1, int p2)
    {
        if (p1 == p2) return "DRAW";
        return p1 > p2 ? "PLAYER 1 WINS" : "PLAYER 2 WINS";
    }

    /// <summary>Toggles twice per second, for blinking "press a key" prompts.</summary>
    private static bool Blink(double time) => (int)(time * 2) % 2 == 0;

    /// <summary>Toggles ~8 times per second, for the rapid, subtle logo flicker.</summary>
    private static bool FastFlash(double time) => (int)(time * 16) % 2 == 0;
}
