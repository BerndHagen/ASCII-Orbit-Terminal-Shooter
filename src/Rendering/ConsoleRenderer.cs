using System.Text;

namespace AsciiOrbit.Rendering;

/// <summary>
/// Renders the canvas to a real terminal using double buffering. Each frame is drawn into a back
/// buffer, then only the cells that differ from what is already on screen are repainted, grouping
/// runs of same-coloured changes into single writes; this keeps the display flicker-free.
///
/// The canvas has a fixed logical size and is centred in whatever terminal it is given. The renderer
/// only reads the terminal size — it never resizes it — and repaints in full when that size changes.
/// </summary>
public sealed class ConsoleRenderer : IRenderer
{
    // A glyph that can never appear in normal output; used to mark the front buffer dirty so the
    // next frame repaints in full (after a clear or a terminal resize).
    private static readonly Glyph Dirty = new((char)1, ConsoleColor.Gray);

    private readonly FrameBuffer _back;
    private readonly FrameBuffer _front;
    private readonly StringBuilder _run = new();

    private int _consoleWidth;
    private int _consoleHeight;
    private int _offsetX;
    private int _offsetY;

    public int Width => _back.Width;
    public int Height => _back.Height;

    public ConsoleRenderer(int width, int height)
    {
        _back = new FrameBuffer(width, height);
        _front = new FrameBuffer(width, height);
        InitConsole();
        SyncMetrics(force: true);
    }

    private static void InitConsole()
    {
        // Each of these can throw when output is redirected or the platform disagrees; none of them
        // is essential, so failures are swallowed individually.
        try { Console.OutputEncoding = Encoding.UTF8; } catch { }
        try { Console.CursorVisible = false; } catch { }
        try { Console.Clear(); } catch { }
    }

    /// <summary>Re-reads the terminal size and, if it changed, recentres and forces a full repaint.</summary>
    private void SyncMetrics(bool force)
    {
        int w, h;
        try { w = Console.WindowWidth; h = Console.WindowHeight; }
        catch { w = Width; h = Height; }

        if (!force && w == _consoleWidth && h == _consoleHeight)
            return;

        _consoleWidth = Math.Max(1, w);
        _consoleHeight = Math.Max(1, h);
        _offsetX = Math.Max(0, (_consoleWidth - Width) / 2);
        _offsetY = Math.Max(0, (_consoleHeight - Height) / 2);
        try { Console.Clear(); } catch { }
        _front.Fill(Dirty);
    }

    public void BeginFrame()
    {
        SyncMetrics(force: false);
        _back.Clear();
    }

    public void Set(int x, int y, char ch, ConsoleColor color) => _back.Set(x, y, ch, color);

    public void Write(int x, int y, string text, ConsoleColor color) => _back.Write(x, y, text, color);

    public void WriteCentered(int y, string text, ConsoleColor color)
    {
        if (!string.IsNullOrEmpty(text))
            _back.Write((Width - text.Length) / 2, y, text, color);
    }

    public void EndFrame()
    {
        if (Console.IsOutputRedirected)
        {
            _front.CopyFrom(_back);
            return;
        }

        for (int y = 0; y < Height; y++)
        {
            int x = 0;
            while (x < Width)
            {
                if (_back[x, y] == _front[x, y])
                {
                    x++;
                    continue;
                }

                // Accumulate a run of changed cells that share a foreground colour.
                ConsoleColor color = _back[x, y].Color;
                int start = x;
                _run.Clear();
                while (x < Width && _back[x, y] != _front[x, y] && _back[x, y].Color == color)
                {
                    _run.Append(_back[x, y].Char);
                    x++;
                }
                FlushRun(start, y, color);
            }
        }

        _front.CopyFrom(_back);
        try { Console.SetCursorPosition(0, 0); } catch { }
    }

    private void FlushRun(int x, int y, ConsoleColor color)
    {
        int screenX = _offsetX + x;
        int screenY = _offsetY + y;
        if ((uint)screenY >= (uint)_consoleHeight || screenX < 0)
            return;

        int maxLen = _consoleWidth - screenX;
        if (maxLen <= 0)
            return;

        string text = _run.Length > maxLen ? _run.ToString(0, maxLen) : _run.ToString();
        try
        {
            Console.SetCursorPosition(screenX, screenY);
            Console.ForegroundColor = color;
            Console.Write(text);
        }
        catch
        {
            // A resize between the size check and the write can briefly invalidate the position.
            // The next frame detects the new size and repaints in full, so dropping this write is safe.
        }
    }
}
