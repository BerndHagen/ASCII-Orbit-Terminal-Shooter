namespace AsciiOrbit.Gameplay;

internal enum PowerUpKind
{
    RapidFire,
    SpreadShot,
    Shield,
    ExtraLife,
    BonusPoints,
    Blast,
}

/// <summary>A power-up dropped by a destroyed invader, drifting down to be caught by the player.</summary>
internal sealed class PowerUp
{
    public int X;
    public double Y;
    public PowerUpKind Kind;

    public PowerUp(int x, double y, PowerUpKind kind)
    {
        X = x;
        Y = y;
        Kind = kind;
    }

    public int CellY => (int)Math.Round(Y);
}

/// <summary>How each power-up is drawn and labelled.</summary>
internal readonly struct PowerUpInfo
{
    public char Symbol { get; }
    public ConsoleColor Color { get; }
    public string Label { get; }

    private PowerUpInfo(char symbol, ConsoleColor color, string label)
    {
        Symbol = symbol;
        Color = color;
        Label = label;
    }

    public static PowerUpInfo For(PowerUpKind kind) => kind switch
    {
        PowerUpKind.RapidFire   => new PowerUpInfo(Symbols.RapidFire, ConsoleColor.Yellow,  "RAPID"),
        PowerUpKind.SpreadShot  => new PowerUpInfo(Symbols.Spread,    ConsoleColor.Cyan,    "SPREAD"),
        PowerUpKind.Shield      => new PowerUpInfo(Symbols.Shield,    ConsoleColor.Blue,    "SHIELD"),
        PowerUpKind.ExtraLife   => new PowerUpInfo(Symbols.Heart,     ConsoleColor.Red,     "1-UP"),
        PowerUpKind.BonusPoints => new PowerUpInfo(Symbols.Star,      ConsoleColor.Magenta, "BONUS"),
        PowerUpKind.Blast       => new PowerUpInfo(Symbols.Blast,     ConsoleColor.Red,     "BLAST"),
        _                       => new PowerUpInfo('?',               ConsoleColor.Gray,    "?"),
    };
}

/// <summary>The drop table: how often invaders drop a power-up and which kind is chosen.</summary>
internal static class PowerUpTable
{
    public const double DropChance = 0.12;

    private static readonly (PowerUpKind Kind, int Weight)[] Weights =
    {
        (PowerUpKind.BonusPoints, 38),
        (PowerUpKind.RapidFire,   18),
        (PowerUpKind.SpreadShot,  18),
        (PowerUpKind.Shield,      14),
        (PowerUpKind.ExtraLife,   10),
        (PowerUpKind.Blast,        6),
    };

    public static PowerUpKind Roll(Random rng)
    {
        int total = 0;
        foreach ((PowerUpKind _, int weight) in Weights)
            total += weight;

        int roll = rng.Next(total);
        foreach ((PowerUpKind kind, int weight) in Weights)
        {
            if (roll < weight)
                return kind;
            roll -= weight;
        }
        return PowerUpKind.BonusPoints;
    }
}
