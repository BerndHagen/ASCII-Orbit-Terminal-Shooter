namespace AsciiOrbit.Input;

/// <summary>
/// Produces commands from a caller-supplied generator instead of a keyboard. Used by the headless
/// self-test to drive the game with deterministic, randomised input.
/// </summary>
public sealed class ScriptedInputSource : IInputSource
{
    private readonly Func<IReadOnlyList<InputCommand>> _next;

    public ScriptedInputSource(Func<IReadOnlyList<InputCommand>> next) => _next = next;

    public IReadOnlyList<InputCommand> Poll() => _next();
}
