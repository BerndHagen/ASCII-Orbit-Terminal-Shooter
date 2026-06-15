namespace AsciiOrbit;

/// <summary>
/// Central catalogue of the non-ASCII glyphs the game draws. They are built from explicit code
/// points so the source file stays plain ASCII and renders identically no matter how it is saved.
/// </summary>
internal static class Symbols
{
    public const char Player     = (char)0x25B2; // up-pointing triangle
    public const char Projectile = (char)0x2502; // light vertical line
    public const char BorderH    = (char)0x2550; // double horizontal
    public const char BorderV    = (char)0x2551; // double vertical
    public const char CornerTL   = (char)0x2554; // double down-and-right
    public const char CornerTR   = (char)0x2557; // double down-and-left
    public const char CornerBL   = (char)0x255A; // double up-and-right
    public const char CornerBR   = (char)0x255D; // double up-and-left
    public const char TeeLeft    = (char)0x2560; // double vertical-and-right (├ on a double frame)
    public const char TeeRight   = (char)0x2563; // double vertical-and-left  (┤ on a double frame)

    public const char Heart      = (char)0x2665; // filled heart (a life)
    public const char Star       = (char)0x2605; // filled star (bonus points)
    public const char Shield     = (char)0x25C6; // filled diamond (shield)
    public const char RapidFire  = (char)0x00BB; // right guillemet (rapid fire)
    public const char Spread     = (char)0x2261; // identical-to / triple bar (spread shot)
    public const char Blast      = (char)0x25CE; // bullseye (screen-clearing blast)
    public const char Copyright  = (char)0x00A9; // (c)
}
