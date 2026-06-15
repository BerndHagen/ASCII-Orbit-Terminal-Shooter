namespace AsciiOrbit.Input;

/// <summary>A single discrete intent for one frame, decoded from a key press.</summary>
public enum InputCommand
{
    None,
    MoveLeft,
    MoveRight,
    MoveUp,
    MoveDown,
    Fire,
    Confirm,
    Pause,
    Quit,
}
