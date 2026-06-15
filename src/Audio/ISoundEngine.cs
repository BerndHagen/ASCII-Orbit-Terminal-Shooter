namespace AsciiOrbit.Audio;

/// <summary>Plays sound effects. Implementations must be safe to call from the game loop and must never block it.</summary>
public interface ISoundEngine : IDisposable
{
    void Play(Sfx sound);
}
