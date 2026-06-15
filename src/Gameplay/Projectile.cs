namespace AsciiOrbit.Gameplay;

/// <summary>A player shot travelling straight up the arena. Y is a double so movement is smooth and frame-rate independent.</summary>
internal sealed class Projectile
{
    public int X;
    public double Y;

    public Projectile(int x, double y)
    {
        X = x;
        Y = y;
    }

    public int CellY => (int)Math.Round(Y);
}
