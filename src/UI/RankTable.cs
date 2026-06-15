namespace AsciiOrbit.UI;

/// <summary>
/// Maps a final score to a letter grade from F up to S+. The thresholds are cumulative and grow by
/// 750 points per rank (3000, then 3750, then 4500, …), matching the progression documented in the
/// README.
/// </summary>
internal static class RankTable
{
    private static readonly string[] Ranks =
    {
        "F", "F+", "E-", "E", "E+", "D-", "D", "D+", "C-", "C",
        "C+", "B-", "B", "B+", "A-", "A", "A+", "S-", "S", "S+",
    };

    public static string ForScore(int score)
    {
        int rank = 0;
        int remaining = score;
        int threshold = 3000;

        while (remaining >= threshold && rank < Ranks.Length - 1)
        {
            remaining -= threshold;
            rank++;
            threshold = 3000 + 750 * rank;
        }
        return Ranks[rank];
    }
}
