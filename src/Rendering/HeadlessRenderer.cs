using System.Text;

namespace AsciiOrbit.Rendering;

/// <summary>
/// An <see cref="IRenderer"/> that draws into memory and touches no console APIs. The headless
/// self-test uses it to exercise the full game loop on machines (or CI) with no real terminal.
/// </summary>
public sealed class HeadlessRenderer : IRenderer
{
    private readonly FrameBuffer _buffer;

    public int Width => _buffer.Width;
    public int Height => _buffer.Height;
    public long FramesRendered { get; private set; }

    public HeadlessRenderer(int width, int height) => _buffer = new FrameBuffer(width, height);

    public void BeginFrame() => _buffer.Clear();

    public void Set(int x, int y, char ch, ConsoleColor color) => _buffer.Set(x, y, ch, color);

    public void Write(int x, int y, string text, ConsoleColor color) => _buffer.Write(x, y, text, color);

    public void WriteCentered(int y, string text, ConsoleColor color)
    {
        if (!string.IsNullOrEmpty(text))
            _buffer.Write((Width - text.Length) / 2, y, text, color);
    }

    public void EndFrame() => FramesRendered++;

    /// <summary>Reads back a cell — handy for assertions in tests.</summary>
    public char CharAt(int x, int y) => _buffer.InBounds(x, y) ? _buffer[x, y].Char : ' ';

    /// <summary>Renders the whole canvas to plain text (one row per line) for visual inspection.</summary>
    public string Dump()
    {
        var sb = new StringBuilder((Width + 1) * Height);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
                sb.Append(_buffer[x, y].Char);
            sb.Append('\n');
        }
        return sb.ToString();
    }
}
