namespace AsciiOrbit.Gameplay;

/// <summary>
/// The player ship. Holds its column, the firing cooldown, and the remaining duration of any active
/// power-ups. All timers are in seconds and counted down by <see cref="Tick"/> so behaviour is
/// independent of frame rate.
/// </summary>
internal sealed class Player
{
    public int X { get; set; }

    public double FireCooldownRemaining { get; private set; }
    public double RapidFireRemaining { get; set; }
    public double SpreadRemaining { get; set; }
    public bool HasShield { get; set; }

    public bool RapidFireActive => RapidFireRemaining > 0;
    public bool SpreadActive => SpreadRemaining > 0;
    public bool CanFire => FireCooldownRemaining <= 0;

    public void Reset(int x)
    {
        X = x;
        FireCooldownRemaining = 0;
        RapidFireRemaining = 0;
        SpreadRemaining = 0;
        HasShield = false;
    }

    public void StartFireCooldown(double seconds) => FireCooldownRemaining = seconds;

    public void Tick(double dt)
    {
        FireCooldownRemaining = Math.Max(0, FireCooldownRemaining - dt);
        RapidFireRemaining = Math.Max(0, RapidFireRemaining - dt);
        SpreadRemaining = Math.Max(0, SpreadRemaining - dt);
    }
}
