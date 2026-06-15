namespace AsciiOrbit.Input;

/// <summary>
/// Reads key presses from the real console without blocking. All currently buffered keys are
/// drained each poll so input never lags behind the game loop. Reading is skipped entirely when
/// input is redirected, which keeps the game from throwing in a non-interactive shell.
/// </summary>
public sealed class ConsoleInputSource : IInputSource
{
    private readonly List<InputCommand> _commands = new();

    public IReadOnlyList<InputCommand> Poll()
    {
        _commands.Clear();
        if (Console.IsInputRedirected)
            return _commands;

        while (Console.KeyAvailable)
        {
            InputCommand command = Map(Console.ReadKey(intercept: true).Key);
            if (command != InputCommand.None)
                _commands.Add(command);
        }
        return _commands;
    }

    private static InputCommand Map(ConsoleKey key) => key switch
    {
        ConsoleKey.LeftArrow or ConsoleKey.A => InputCommand.MoveLeft,
        ConsoleKey.RightArrow or ConsoleKey.D => InputCommand.MoveRight,
        ConsoleKey.UpArrow or ConsoleKey.W => InputCommand.MoveUp,
        ConsoleKey.DownArrow or ConsoleKey.S => InputCommand.MoveDown,
        ConsoleKey.Spacebar => InputCommand.Fire,
        ConsoleKey.Enter => InputCommand.Confirm,
        ConsoleKey.P => InputCommand.Pause,
        ConsoleKey.Escape or ConsoleKey.Q => InputCommand.Quit,
        _ => InputCommand.None,
    };
}
