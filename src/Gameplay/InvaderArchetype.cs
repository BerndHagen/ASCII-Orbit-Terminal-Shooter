using AsciiOrbit.Gameplay.Behaviors;

namespace AsciiOrbit.Gameplay;

/// <summary>
/// The immutable definition of one letter: how it looks, how tough it is, how much it scores, and
/// how it moves. One archetype is shared by every invader of that letter.
/// </summary>
internal sealed class InvaderArchetype
{
    public InvaderArchetype(char letter, ConsoleColor color, int hp, int scoreValue, string bandName, IInvaderBehavior behavior)
    {
        Letter = letter;
        Color = color;
        Hp = hp;
        ScoreValue = scoreValue;
        BandName = bandName;
        Behavior = behavior;
    }

    public char Letter { get; }
    public ConsoleColor Color { get; }
    public int Hp { get; }
    public int ScoreValue { get; }

    /// <summary>Human-readable name of the behaviour family, shown in the HUD.</summary>
    public string BandName { get; }

    public IInvaderBehavior Behavior { get; }
}
