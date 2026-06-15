namespace AsciiOrbit.Rendering;

/// <summary>
/// A fixed-size character canvas that the game draws into. Implementations decide where the
/// pixels actually go (a real console, or an in-memory buffer for the headless self-test).
///
/// Every drawing call is clipped to the canvas bounds, so callers never have to range-check
/// their coordinates. This is what makes the classic "buffer overflow" crash impossible.
/// </summary>
public interface IRenderer
{
    int Width { get; }
    int Height { get; }

    /// <summary>Starts a new frame by clearing the back buffer.</summary>
    void BeginFrame();

    /// <summary>Draws a single glyph. Out-of-bounds coordinates are ignored.</summary>
    void Set(int x, int y, char ch, ConsoleColor color);

    /// <summary>Draws a string starting at (x, y), left to right. The part that falls outside is clipped.</summary>
    void Write(int x, int y, string text, ConsoleColor color);

    /// <summary>Writes <paramref name="text"/> horizontally centred on the canvas at row <paramref name="y"/>.</summary>
    void WriteCentered(int y, string text, ConsoleColor color);

    /// <summary>Pushes the back buffer to the output device.</summary>
    void EndFrame();
}
