namespace AsciiOrbit.Rendering;

/// <summary>
/// One character cell of the screen: the visible glyph and the colour it is drawn in.
/// Cells are value types so the frame buffers can be compared cheaply for diffing.
/// </summary>
public readonly record struct Glyph(char Char, ConsoleColor Color)
{
    /// <summary>A blank cell. Colour is irrelevant for spaces but kept stable for equality.</summary>
    public static readonly Glyph Empty = new(' ', ConsoleColor.Gray);

    public bool IsBlank => Char == ' ';
}
