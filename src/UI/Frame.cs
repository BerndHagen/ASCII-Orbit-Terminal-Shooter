using AsciiOrbit.Rendering;

namespace AsciiOrbit.UI;

/// <summary>Draws the cabinet-style chrome: the outer bezel and the horizontal dividers that separate the HUD bars from the play field.</summary>
internal static class Frame
{
    /// <summary>Draws the bezel around the whole screen.</summary>
    public static void DrawScreenBorder(IRenderer r, ConsoleColor color)
        => DrawBox(r, 0, 0, r.Width, r.Height, color);

    /// <summary>Draws the full in-game chrome: bezel plus the header and footer dividers.</summary>
    public static void DrawGameChrome(IRenderer r, ConsoleColor color)
    {
        DrawScreenBorder(r, color);
        DrawDivider(r, Layout.TopDividerRow, color);
        DrawDivider(r, Layout.BottomDividerRow, color);
    }

    /// <summary>A horizontal divider that ties into the left and right frame columns.</summary>
    public static void DrawDivider(IRenderer r, int row, ConsoleColor color)
    {
        r.Set(0, row, Symbols.TeeLeft, color);
        r.Set(r.Width - 1, row, Symbols.TeeRight, color);
        for (int x = 1; x < r.Width - 1; x++)
            r.Set(x, row, Symbols.BorderH, color);
    }

    public static void DrawBox(IRenderer r, int left, int top, int width, int height, ConsoleColor color)
    {
        int right = left + width - 1;
        int bottom = top + height - 1;

        r.Set(left, top, Symbols.CornerTL, color);
        r.Set(right, top, Symbols.CornerTR, color);
        r.Set(left, bottom, Symbols.CornerBL, color);
        r.Set(right, bottom, Symbols.CornerBR, color);

        for (int x = left + 1; x < right; x++)
        {
            r.Set(x, top, Symbols.BorderH, color);
            r.Set(x, bottom, Symbols.BorderH, color);
        }
        for (int y = top + 1; y < bottom; y++)
        {
            r.Set(left, y, Symbols.BorderV, color);
            r.Set(right, y, Symbols.BorderV, color);
        }
    }
}
