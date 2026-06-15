using AsciiOrbit.Rendering;

namespace AsciiOrbit.UI;

/// <summary>The big ASCII banners and a helper to draw them centred while preserving their shape.</summary>
internal static class Art
{
    public static readonly string[] Title =
    {
        @"   __    ___   ___  ____  ____    _____  ____  ____  ____  ____ ",
        @"  /__\  / __) / __)(_  _)(_  _)  (  _  )(  _ \(  _ \(_  _)(_  _)",
        @" /(__)\ \__ \( (__  _)(_  _)(_    )(_)(  )   / ) _ < _)(_   )(  ",
        @"(__)(__)(___/ \___)(____)(____)  (_____)(_)\_)(____/(____) (__) ",
    };

    public static readonly string[] GameOver =
    {
        @"  ____    _    __  __ _____    _____     _______ ____  ",
        @" / ___|  / \  |  \/  | ____|  / _ \ \   / / ____|  _ \ ",
        @"| |  _  / _ \ | |\/| |  _|   | | | \ \ / /|  _| | |_) |",
        @"| |_| |/ ___ \| |  | | |___  | |_| |\ V / | |___|  _ < ",
        @" \____/_/   \_\_|  |_|_____|  \___/  \_/  |_____|_| \_\",
    };

    public static readonly string[] HighScore =
    {
        @"  _   _ ___ ____ _   _   ____   ____ ___  ____  _____ ",
        @" | | | |_ _/ ___| | | | / ___| / ___/ _ \|  _ \| ____|",
        @" | |_| || | |  _| |_| | \___ \| |  | | | | |_) |  _|  ",
        @" |  _  || | |_| |  _  |  ___) | |__| |_| |  _ <| |___ ",
        @" |_| |_|___\____|_| |_| |____/ \____\___/|_| \_\_____|",
    };

    /// <summary>Draws a multi-line banner centred horizontally, keeping every line aligned to the same left edge.</summary>
    public static void DrawBanner(IRenderer r, string[] lines, int topY, ConsoleColor color)
    {
        int width = 0;
        foreach (string line in lines)
            width = Math.Max(width, line.Length);

        int left = (r.Width - width) / 2;
        for (int i = 0; i < lines.Length; i++)
            r.Write(left, topY + i, lines[i], color);
    }
}
