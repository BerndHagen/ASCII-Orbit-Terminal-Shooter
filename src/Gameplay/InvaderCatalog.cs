using AsciiOrbit.Gameplay.Behaviors;

namespace AsciiOrbit.Gameplay;

/// <summary>
/// Builds the archetype for every letter A-Z. Letters are grouped into behaviour bands that get
/// progressively nastier: gentle straight descents (A-C), swaying drifters (D-I), homing trackers
/// (J-L), divers (M-O), stop-and-go stutterers (P-R), and finally fast, armoured zig-zaggers,
/// hunters and divers (S-Z). Base fall speed also rises with the letter index, and the toughest
/// bands take two hits to destroy.
/// </summary>
internal static class InvaderCatalog
{
    public const int LetterCount = 26;

    private static readonly InvaderArchetype[] ByIndex = BuildAll();

    /// <summary>Returns the archetype to spawn at the given level (letters cap at Z for very deep runs).</summary>
    public static InvaderArchetype ForLevel(int level)
    {
        int index = Math.Clamp(level - 1, 0, LetterCount - 1);
        return ByIndex[index];
    }

    private static InvaderArchetype[] BuildAll()
    {
        var archetypes = new InvaderArchetype[LetterCount];
        for (int i = 0; i < LetterCount; i++)
            archetypes[i] = Build(i);
        return archetypes;
    }

    private static InvaderArchetype Build(int i)
    {
        char letter = (char)('A' + i);
        double fall = 1.2 + i * 0.07;           // base descent speed, rising with the letter
        int score = 50 + i * 12;                // tougher letters are worth more

        IInvaderBehavior behavior;
        ConsoleColor color;
        int hp = 1;
        string band;

        if (i <= 2)
        {
            // A-C: slow, straight descents — the gentle on-ramp.
            behavior = new StraightBehavior(fall);
            color = ConsoleColor.Green;
            band = "Drifter";
        }
        else if (i <= 5)
        {
            // D-F: a soft, readable sway so they can be led with a shot rather than darting about.
            behavior = new SwayBehavior(fall, amplitude: 3 + (i - 3), frequency: 1.1 + 0.12 * (i - 3));
            color = ConsoleColor.Cyan;
            band = "Weaver";
        }
        else if (i <= 8)
        {
            behavior = new DriftBehavior(fall, sideSpeed: 3.0 + 0.6 * (i - 6));
            color = ConsoleColor.Yellow;
            band = "Glider";
        }
        else if (i <= 11)
        {
            behavior = new TrackerBehavior(fall, sideSpeed: 2.0 + 0.4 * (i - 9));
            color = ConsoleColor.Magenta;
            band = "Hunter";
        }
        else if (i <= 14)
        {
            behavior = new DiverBehavior(cruiseSpeed: fall * 0.7, diveSpeed: fall * 3.0, triggerFraction: 0.5);
            color = ConsoleColor.Red;
            band = "Diver";
        }
        else if (i <= 17)
        {
            behavior = new StutterBehavior(averageSpeed: fall, moveDuration: 0.5, pauseDuration: 0.35);
            color = ConsoleColor.DarkYellow;
            band = "Stutterer";
        }
        else if (i <= 20)
        {
            behavior = new SwayBehavior(fall, amplitude: 6 + (i - 18), frequency: 2.4 + 0.2 * (i - 18));
            color = ConsoleColor.DarkCyan;
            band = "Zigzag";
            if (i == 20) hp = 2; // U
        }
        else if (i <= 23)
        {
            behavior = new TrackerBehavior(fall, sideSpeed: 3.6 + 0.6 * (i - 21));
            color = ConsoleColor.DarkMagenta;
            band = "Stalker";
            if (i >= 22) hp = 2; // W, X
        }
        else
        {
            behavior = new DiverBehavior(cruiseSpeed: fall * 0.9, diveSpeed: fall * 3.6, triggerFraction: 0.35);
            color = ConsoleColor.White;
            band = "Reaper";
            hp = 2; // Y, Z
        }

        if (hp > 1)
            score = (int)(score * 1.5); // armoured letters are worth more

        score = RoundToFifty(score); // arcade-style round point values (50, 100, 150, …)
        return new InvaderArchetype(letter, color, hp, score, band, behavior);
    }

    private static int RoundToFifty(int value) => (int)(Math.Round(value / 50.0, MidpointRounding.AwayFromZero) * 50);
}
