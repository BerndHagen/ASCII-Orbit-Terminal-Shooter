namespace AsciiOrbit.Gameplay;

/// <summary>
/// The classic arcade "Game A / Game B" difficulty choice. Game A is the normal pace; Game B starts
/// faster, spawns more aggressively and gives one fewer ship.
/// </summary>
public enum GameDifficulty
{
    GameA,
    GameB,
}

internal static class GameDifficultyExtensions
{
    public static double SpeedMultiplier(this GameDifficulty mode) => mode == GameDifficulty.GameB ? 1.30 : 1.0;
    public static double SpawnMultiplier(this GameDifficulty mode) => mode == GameDifficulty.GameB ? 0.75 : 1.0;
    public static int StartingLives(this GameDifficulty mode) => mode == GameDifficulty.GameB ? 2 : Difficulty.StartingLives;
    public static string Label(this GameDifficulty mode) => mode == GameDifficulty.GameB ? "GAME B" : "GAME A";
}
