namespace AsciiOrbit;

/// <summary>One row of the high-score table: a score and the three-letter initials of who set it.</summary>
internal readonly record struct HighScoreEntry(int Score, string Name);

/// <summary>
/// The top-five high scores, kept sorted high to low. Comes with a non-empty default table so the
/// game never shows blank scores.
/// </summary>
internal sealed class HighScoreTable
{
    public const int Capacity = 5;

    private readonly List<HighScoreEntry> _entries;

    public HighScoreTable(IEnumerable<HighScoreEntry> entries)
        => _entries = entries.OrderByDescending(e => e.Score).Take(Capacity).ToList();

    public IReadOnlyList<HighScoreEntry> Entries => _entries;

    public HighScoreEntry Top => _entries.Count > 0 ? _entries[0] : new HighScoreEntry(0, "---");

    /// <summary>True if a score is good enough to make the table.</summary>
    public bool Qualifies(int score) => score > 0 && (_entries.Count < Capacity || score > _entries[^1].Score);

    /// <summary>Inserts an entry (if it qualifies) and returns its rank (0-based), or -1 if it didn't make the table.</summary>
    public int Insert(int score, string name)
    {
        if (!Qualifies(score))
            return -1;

        var entry = new HighScoreEntry(score, name);
        _entries.Add(entry);
        _entries.Sort((a, b) => b.Score.CompareTo(a.Score));
        while (_entries.Count > Capacity)
            _entries.RemoveAt(_entries.Count - 1);
        return _entries.IndexOf(entry);
    }

    public static HighScoreTable Default() => new(new[]
    {
        new HighScoreEntry(10_000, "DEV"),
        new HighScoreEntry(8_000, "ACE"),
        new HighScoreEntry(6_000, "BOT"),
        new HighScoreEntry(4_000, "CPU"),
        new HighScoreEntry(2_000, "ZAP"),
    });
}

internal interface IHighScoreStore
{
    HighScoreTable Load();
    void Save(HighScoreTable table);
}

/// <summary>
/// Stores the high-score table (one "score name" line per entry) under the user's local application
/// data, so it survives between sessions. All I/O is best-effort: any failure falls back to the
/// default table, so a problem here can never crash or interrupt the game.
/// </summary>
internal sealed class FileHighScoreStore : IHighScoreStore
{
    private readonly string _path;

    public FileHighScoreStore()
        : this(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AsciiOrbit",
            "highscores.txt"))
    {
    }

    /// <summary>Creates a store backed by a specific file (used by tests).</summary>
    public FileHighScoreStore(string path) => _path = path;

    public HighScoreTable Load()
    {
        try
        {
            var entries = new List<HighScoreEntry>();
            foreach (string line in File.ReadAllLines(_path))
            {
                string[] parts = line.Trim().Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 1 && int.TryParse(parts[0], out int score))
                {
                    string name = parts.Length >= 2 ? parts[1] : "---";
                    entries.Add(new HighScoreEntry(score, NormalizeName(name)));
                }
            }
            if (entries.Count > 0)
                return new HighScoreTable(entries);
        }
        catch
        {
            // ignore — fall through to default
        }
        return HighScoreTable.Default();
    }

    public void Save(HighScoreTable table)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllLines(_path, table.Entries.Select(e => $"{e.Score} {NormalizeName(e.Name)}"));
        }
        catch
        {
            // best effort; ignore
        }
    }

    private static string NormalizeName(string name)
    {
        string cleaned = new string((name ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
        if (cleaned.Length == 0)
            return "---";
        return cleaned.Length > 3 ? cleaned.Substring(0, 3) : cleaned;
    }
}
