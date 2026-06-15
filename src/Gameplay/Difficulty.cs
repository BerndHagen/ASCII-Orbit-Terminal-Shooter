namespace AsciiOrbit.Gameplay;

/// <summary>Central place for the gameplay tuning values, so the difficulty curve is easy to read and adjust.</summary>
internal static class Difficulty
{
    public const int StartingLives = 3;
    public const int FirstExtraLifeScore = 8_000;
    public const int MaxInvadersOnScreen = 26;

    public const double BaseFireCooldown = 0.40;   // seconds between shots
    public const double RapidFireCooldown = 0.13;
    public const double PowerUpDuration = 8.0;      // seconds a timed power-up lasts
    public const double ProjectileSpeed = 30.0;     // cells per second, upward
    public const double PowerUpFallSpeed = 7.0;     // cells per second, downward

    /// <summary>Invaders to destroy to clear a level: 5, 7, 9, … (the original progression).</summary>
    public static int InvadersToClear(int level) => 5 + 2 * (level - 1);

    /// <summary>
    /// Per-level speed multiplier so the pace keeps rising even after the letters reach Z. The early
    /// levels stay near 1.0 to ease players in; deeper levels ramp up noticeably.
    /// </summary>
    public static double SpeedScale(int level) => 1.0 + 0.045 * (level - 1);

    /// <summary>Randomised delay until the next spawn; the window tightens as levels rise but starts roomy.</summary>
    public static double NextSpawnInterval(int level, Random rng)
    {
        double min = Math.Max(0.7, 2.8 - 0.10 * (level - 1));
        double max = Math.Max(min, 4.6 - 0.14 * (level - 1));
        return min + rng.NextDouble() * (max - min);
    }

    public static int KillScore(InvaderArchetype archetype, int level) => archetype.ScoreValue + (level - 1) * 25;

    public static int BonusPickupScore(int level) => 500 + level * 100;
}
