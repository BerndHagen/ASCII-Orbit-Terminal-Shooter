using System.Collections.Concurrent;
using System.Runtime.Versioning;

namespace AsciiOrbit.Audio;

/// <summary>
/// Plays retro bleeps with <see cref="Console.Beep(int,int)"/>. Because that call is blocking, all
/// playback happens on a dedicated background thread fed by a bounded queue; the game loop only ever
/// enqueues, and when the queue is full the sound is dropped rather than stalling gameplay.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsSoundEngine : ISoundEngine
{
    private static readonly IReadOnlyDictionary<Sfx, (int Freq, int Duration)[]> Patterns =
        new Dictionary<Sfx, (int, int)[]>
        {
            [Sfx.Shoot]         = new[] { (220, 35) },
            [Sfx.InvaderSpawn]  = new[] { (300, 55) },
            [Sfx.InvaderHit]    = new[] { (820, 45) },
            [Sfx.InvaderBreach] = new[] { (500, 110), (300, 130) },
            [Sfx.LevelUp]       = new[] { (520, 70), (620, 70), (720, 70), (840, 110) },
            [Sfx.ExtraLife]     = new[] { (1200, 140), (820, 160) },
            [Sfx.PowerUp]       = new[] { (700, 55), (1000, 85) },
            [Sfx.BonusPickup]   = new[] { (1000, 40), (1320, 60) },
            [Sfx.Blast]         = new[] { (1000, 50), (700, 70), (450, 90), (220, 220) },
            [Sfx.PlayerExplode] = new[] { (300, 90), (220, 110), (160, 140), (110, 220) },
            [Sfx.GameOver]      = new[] { (420, 200), (320, 200), (200, 320) },
            [Sfx.MenuMove]      = new[] { (500, 28) },
            [Sfx.MenuSelect]    = new[] { (720, 55), (920, 80) },
        };

    private readonly BlockingCollection<Sfx> _queue = new(boundedCapacity: 16);
    private readonly Thread _worker;

    public WindowsSoundEngine()
    {
        _worker = new Thread(PlayLoop) { IsBackground = true, Name = "AsciiOrbit.Audio" };
        _worker.Start();
    }

    public void Play(Sfx sound) => _queue.TryAdd(sound); // non-blocking; drops when saturated

    private void PlayLoop()
    {
        foreach (Sfx sound in _queue.GetConsumingEnumerable())
        {
            if (!Patterns.TryGetValue(sound, out (int Freq, int Duration)[]? pattern))
                continue;

            foreach ((int freq, int duration) in pattern)
            {
                try { Console.Beep(freq, duration); }
                catch { /* best effort: ignore audio device hiccups */ }
            }
        }
    }

    public void Dispose()
    {
        _queue.CompleteAdding();
        if (_worker.IsAlive)
            _worker.Join(500);
        _queue.Dispose();
    }
}
