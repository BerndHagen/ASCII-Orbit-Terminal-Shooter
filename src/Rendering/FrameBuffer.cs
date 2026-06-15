namespace AsciiOrbit.Rendering;

/// <summary>
/// A row-major grid of <see cref="Glyph"/> cells. All writes are clipped to the grid, so callers
/// can draw freely without bounds-checking. Two of these (a back and a front buffer) are what the
/// console renderer diffs to repaint only the cells that actually changed.
/// </summary>
public sealed class FrameBuffer
{
    private readonly Glyph[] _cells;

    public int Width { get; }
    public int Height { get; }

    public FrameBuffer(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new Glyph[width * height];
        Clear();
    }

    public Glyph this[int x, int y]
    {
        get => _cells[y * Width + x];
        set => _cells[y * Width + x] = value;
    }

    public bool InBounds(int x, int y) => (uint)x < (uint)Width && (uint)y < (uint)Height;

    public void Clear()
    {
        for (int i = 0; i < _cells.Length; i++)
            _cells[i] = Glyph.Empty;
    }

    public void Set(int x, int y, char ch, ConsoleColor color)
    {
        if (InBounds(x, y))
            _cells[y * Width + x] = new Glyph(ch, color);
    }

    public void Write(int x, int y, string text, ConsoleColor color)
    {
        if ((uint)y >= (uint)Height || string.IsNullOrEmpty(text))
            return;
        for (int i = 0; i < text.Length; i++)
            Set(x + i, y, text[i], color);
    }

    public void Fill(Glyph glyph)
    {
        for (int i = 0; i < _cells.Length; i++)
            _cells[i] = glyph;
    }

    public void CopyFrom(FrameBuffer other) => Array.Copy(other._cells, _cells, _cells.Length);
}
