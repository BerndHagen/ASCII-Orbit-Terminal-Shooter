using AsciiOrbit.Gameplay.Behaviors;

namespace AsciiOrbit.Gameplay;

/// <summary>
/// A descending letter. Position is stored as doubles for smooth sub-cell motion and rounded only
/// for drawing and collision. The fields below hold the per-invader state that the shared behaviour
/// strategies read and write (age, spawn column, a random phase so they don't move in lockstep, and
/// a direction flag for bouncing patterns).
/// </summary>
internal sealed class Invader
{
    public double X;
    public double Y;
    public double SpawnX;
    public double Age;
    public double Phase;
    public int Direction = 1;

    public char Letter;
    public ConsoleColor Color;
    public int Hp;
    public int ScoreValue;
    public double SpeedScale = 1.0;
    public IInvaderBehavior Behavior = null!;

    public int CellX => (int)Math.Round(X);
    public int CellY => (int)Math.Round(Y);

    public void Advance(in BehaviorContext ctx, double dt)
    {
        Age += dt;
        Behavior.Update(this, in ctx, dt);

        // Keep invaders inside the arena horizontally no matter what a behaviour attempts.
        if (X < 0) X = 0;
        else if (X > ctx.ArenaWidth - 1) X = ctx.ArenaWidth - 1;
    }
}
