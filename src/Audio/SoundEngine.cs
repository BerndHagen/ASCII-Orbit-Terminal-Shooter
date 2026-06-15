namespace AsciiOrbit.Audio;

/// <summary>Chooses the right sound engine for the current platform.</summary>
public static class SoundEngine
{
    public static ISoundEngine Create(bool enabled = true)
    {
        if (!enabled)
            return new SilentSoundEngine();

        // Console.Beep only exists on Windows; everywhere else the game runs silently.
        if (OperatingSystem.IsWindows())
            return new WindowsSoundEngine();

        return new SilentSoundEngine();
    }
}

/// <summary>A sound engine that does nothing — used on non-Windows platforms and when audio is disabled.</summary>
public sealed class SilentSoundEngine : ISoundEngine
{
    public void Play(Sfx sound) { }
    public void Dispose() { }
}
