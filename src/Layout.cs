namespace AsciiOrbit;

/// <summary>
/// Fixed screen geometry, arcade-cabinet style: the whole canvas is wrapped in a bezel frame with a
/// score header bar across the top and a status bar along the bottom, and the play field fills
/// everything in between (like Space Invaders or Pac-Man). The canvas is a constant size and the
/// renderer centres it in whatever terminal it is given.
///
/// Entities live in "arena" coordinates (0,0 at the top-left of the play field);
/// <see cref="ToCanvasX"/>/<see cref="ToCanvasY"/> map those into canvas coordinates for drawing.
/// </summary>
internal static class Layout
{
    // Logical canvas size (matches the classic 80x30 console window).
    public const int CanvasWidth = 80;
    public const int CanvasHeight = 30;

    // Bezel frame occupies the outermost rows/columns. The score header sits just inside the top.
    public const int HeaderLabelRow = 1;
    public const int HeaderScoreRow = 2;
    public const int TopDividerRow = 3;
    public const int ArenaTop = TopDividerRow + 1;        // 4
    public const int BottomDividerRow = CanvasHeight - 3;  // 27
    public const int FooterRow = CanvasHeight - 2;         // 28 (status bar, between divider and frame)
    public const int ArenaLeft = 1;                        // just inside the left frame column

    public const int ArenaWidth = CanvasWidth - 2;         // 78 (between the frame columns)
    public const int ArenaHeight = BottomDividerRow - ArenaTop; // 27 - 4 = 23 rows (rows 4..26)

    // The player occupies the bottom interior row of the arena.
    public const int PlayerRow = ArenaHeight - 1;

    public static int ToCanvasX(int arenaX) => ArenaLeft + arenaX;
    public static int ToCanvasY(int arenaY) => ArenaTop + arenaY;

    public const int MinTerminalWidth = CanvasWidth;
    public const int MinTerminalHeight = CanvasHeight;
}
