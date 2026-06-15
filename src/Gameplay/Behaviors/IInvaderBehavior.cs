namespace AsciiOrbit.Gameplay.Behaviors;

/// <summary>Read-only world information a behaviour needs to steer an invader for one frame.</summary>
internal readonly struct BehaviorContext
{
    public BehaviorContext(int arenaWidth, int arenaHeight, int playerX, Random rng)
    {
        ArenaWidth = arenaWidth;
        ArenaHeight = arenaHeight;
        PlayerX = playerX;
        Rng = rng;
    }

    public int ArenaWidth { get; }
    public int ArenaHeight { get; }
    public int PlayerX { get; }
    public Random Rng { get; }
}

/// <summary>
/// The movement strategy for a letter. Behaviours are stateless and shared by every invader of a
/// given letter; all mutable per-invader state lives on the <see cref="Invader"/> itself. This is
/// what lets each letter "behave differently" while keeping the behaviour set small and reusable.
/// </summary>
internal interface IInvaderBehavior
{
    void Update(Invader invader, in BehaviorContext ctx, double dt);
}
