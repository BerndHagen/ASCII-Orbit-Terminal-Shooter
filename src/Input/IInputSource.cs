namespace AsciiOrbit.Input;

/// <summary>
/// Supplies the commands that occurred since the previous poll. Abstracting input this way lets the
/// game run from a real keyboard or from a scripted sequence (used by the self-test).
/// </summary>
public interface IInputSource
{
    /// <summary>Returns every command queued since the last call. The list is owned by the source and is only valid until the next poll.</summary>
    IReadOnlyList<InputCommand> Poll();
}
