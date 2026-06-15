using System.Diagnostics;

namespace AsciiOrbit;

/// <summary>
/// Paces the game loop to a target frame rate and reports the time elapsed since the previous frame.
/// The delta is clamped so a long stall (a debugger break, dragging the window) can never make
/// everything jump across the screen in a single step.
/// </summary>
internal sealed class GameClock
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    private readonly double _targetFrameSeconds;
    private double _lastTime;

    public GameClock(int targetFps = 60) => _targetFrameSeconds = 1.0 / targetFps;

    public double NextFrame()
    {
        double now = _stopwatch.Elapsed.TotalSeconds;
        double sleep = _targetFrameSeconds - (now - _lastTime);
        if (sleep > 0)
        {
            Thread.Sleep((int)(sleep * 1000));
            now = _stopwatch.Elapsed.TotalSeconds;
        }

        double delta = now - _lastTime;
        _lastTime = now;
        return Math.Min(delta, 0.1);
    }
}
