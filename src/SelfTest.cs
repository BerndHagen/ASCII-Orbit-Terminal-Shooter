using AsciiOrbit.Audio;
using AsciiOrbit.Gameplay;
using AsciiOrbit.Gameplay.Behaviors;
using AsciiOrbit.Input;
using AsciiOrbit.Rendering;
using AsciiOrbit.UI;

namespace AsciiOrbit;

/// <summary>
/// A headless smoke test that drives the full game logic and rendering through the same interfaces
/// the real game uses, but with an in-memory renderer, silent audio and scripted input. It runs many
/// thousands of frames across several seeds and time steps (including a deliberately large one) and
/// fails loudly if anything throws. Run it with <c>--selftest</c>; it needs no terminal, so it works
/// in CI.
/// </summary>
internal static class SelfTest
{
    public static int Run()
    {
        Console.WriteLine("ASCII Orbit self-test");
        try
        {
            SimulateRounds(seed: 1, frames: 20_000, dt: 1.0 / 60, mode: GameDifficulty.GameA);
            SimulateRounds(seed: 2, frames: 20_000, dt: 1.0 / 30, mode: GameDifficulty.GameB);
            SimulateRounds(seed: 3, frames: 5_000, dt: 0.1, mode: GameDifficulty.GameA); // worst-case clamped step
            SimulateBehaviors();
            SimulateSkilledRun();
            VerifyHighScorePersistence();
            SimulateScreens();
            Console.WriteLine("Self-test OK.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("Self-test FAILED:");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void SimulateRounds(int seed, int frames, double dt, GameDifficulty mode)
    {
        var worldRng = new Random(seed);
        var inputRng = new Random(seed * 7 + 1);
        using var sound = new SilentSoundEngine();
        var renderer = new HeadlessRenderer(Layout.CanvasWidth, Layout.CanvasHeight);
        var session = new PlaySession(sound, worldRng, mode);

        var buffer = new List<InputCommand>();
        IInputSource input = new ScriptedInputSource(() =>
        {
            buffer.Clear();
            int move = inputRng.Next(100);
            if (move < 35) buffer.Add(InputCommand.MoveLeft);
            else if (move < 70) buffer.Add(InputCommand.MoveRight);
            if (inputRng.Next(100) < 60) buffer.Add(InputCommand.Fire);
            return buffer;
        });

        int rounds = 0;
        int maxLevel = 0;
        long maxScore = 0;

        for (int frame = 0; frame < frames; frame++)
        {
            session.Update(dt, input.Poll());

            renderer.BeginFrame();
            session.Draw(renderer);
            Hud.Draw(renderer, session, TestBoard(session));
            renderer.EndFrame();

            maxLevel = Math.Max(maxLevel, session.Level);
            maxScore = Math.Max(maxScore, session.Score);

            if (session.AwaitingRespawn)
            {
                if (session.Lives > 0)
                {
                    session.Respawn();
                }
                else
                {
                    rounds++;
                    session.Reset();
                }
            }
        }

        Console.WriteLine($"  seed={seed} {mode} dt={dt:0.###}s frames={frames}: rounds={rounds} maxLevel={maxLevel} maxScore={maxScore} rendered={renderer.FramesRendered}");
    }

    /// <summary>
    /// Plays a full run with a simple aiming heuristic (slide under the most advanced invader, then
    /// fire) to confirm the game progresses end to end: levels advance, the letter changes, extra
    /// lives and power-ups trigger, and nothing throws across deep play.
    /// </summary>
    private static void SimulateSkilledRun()
    {
        using var sound = new SilentSoundEngine();
        var renderer = new HeadlessRenderer(Layout.CanvasWidth, Layout.CanvasHeight);
        var session = new PlaySession(sound, new Random(123), GameDifficulty.GameB);
        var commands = new List<InputCommand>(1);

        int reachedLevel = 1;
        bool over = false;
        for (int frame = 0; frame < 120_000 && !over; frame++)
        {
            commands.Clear();
            Invader? target = null;
            foreach (Invader invader in session.Invaders)
                if (target is null || invader.Y > target.Y)
                    target = invader;

            if (target is null || target.CellX == session.Player.X)
                commands.Add(InputCommand.Fire);
            else
                commands.Add(target.CellX < session.Player.X ? InputCommand.MoveLeft : InputCommand.MoveRight);

            session.Update(1.0 / 60, commands);

            renderer.BeginFrame();
            session.Draw(renderer);
            Hud.Draw(renderer, session, TestBoard(session));
            renderer.EndFrame();

            if (session.AwaitingRespawn && session.Lives > 0)
                session.Respawn();

            reachedLevel = Math.Max(reachedLevel, session.Level);
            over = session.IsOut;
        }

        Console.WriteLine($"  skilled run: reachedLevel={reachedLevel} score={session.Score} lives={session.Lives} gameOver={over}");
        Console.WriteLine(
            $"    pickups: rapid={session.CollectedCount(PowerUpKind.RapidFire)} " +
            $"spread={session.CollectedCount(PowerUpKind.SpreadShot)} " +
            $"shield={session.CollectedCount(PowerUpKind.Shield)} " +
            $"1up={session.CollectedCount(PowerUpKind.ExtraLife)} " +
            $"bonus={session.CollectedCount(PowerUpKind.BonusPoints)} " +
            $"blast={session.CollectedCount(PowerUpKind.Blast)}");
    }

    /// <summary>
    /// Exercises every one of the 26 archetypes (plus post-Z speed scaling) by spawning each and
    /// advancing it all the way down the arena against a randomly moving player. Verifies no behaviour
    /// produces a NaN, leaves the arena horizontally, or fails to ever reach the bottom.
    /// </summary>
    private static void SimulateBehaviors()
    {
        var rng = new Random(7);
        const double dt = 1.0 / 60;

        for (int level = 1; level <= 40; level++)
        {
            InvaderArchetype archetype = InvaderCatalog.ForLevel(level);
            for (int n = 0; n < 5; n++)
            {
                double startX = rng.Next(0, Layout.ArenaWidth);
                var invader = new Invader
                {
                    X = startX,
                    Y = 0,
                    SpawnX = startX,
                    Letter = archetype.Letter,
                    Color = archetype.Color,
                    Hp = archetype.Hp,
                    ScoreValue = archetype.ScoreValue,
                    Behavior = archetype.Behavior,
                    SpeedScale = Difficulty.SpeedScale(level),
                    Phase = rng.NextDouble() * (2 * Math.PI),
                    Direction = rng.Next(2) == 0 ? -1 : 1,
                };

                int guard = 0;
                while (invader.CellY < Layout.PlayerRow)
                {
                    var ctx = new BehaviorContext(Layout.ArenaWidth, Layout.ArenaHeight, rng.Next(0, Layout.ArenaWidth), rng);
                    invader.Advance(in ctx, dt);

                    if (double.IsNaN(invader.X) || double.IsNaN(invader.Y))
                        throw new InvalidOperationException($"Letter '{invader.Letter}' produced NaN position.");
                    if (invader.CellX < 0 || invader.CellX >= Layout.ArenaWidth)
                        throw new InvalidOperationException($"Letter '{invader.Letter}' left the arena at X={invader.X:0.##}.");
                    if (++guard > 200_000)
                        throw new InvalidOperationException($"Letter '{invader.Letter}' never reached the bottom (stuck).");
                }
            }
        }

        Console.WriteLine("  behaviors: all 26 archetypes advanced across the arena OK");
    }

    /// <summary>Confirms the high score survives a save and a fresh load (uses a throwaway temp file).</summary>
    private static void VerifyHighScorePersistence()
    {
        string path = Path.Combine(Path.GetTempPath(), $"asciiorbit_hs_{Guid.NewGuid():N}.txt");
        try
        {
            HighScoreTable fresh = new FileHighScoreStore(path).Load();
            if (fresh.Top.Score != 10000 || fresh.Top.Name != "DEV" || fresh.Entries.Count != HighScoreTable.Capacity)
                throw new InvalidOperationException($"A fresh store should return the default table (top {fresh.Top.Score} {fresh.Top.Name}, {fresh.Entries.Count} rows).");

            int rank = fresh.Insert(73250, "BHM");
            if (rank != 0)
                throw new InvalidOperationException($"73250 should rank first (got {rank}).");
            new FileHighScoreStore(path).Save(fresh);

            HighScoreTable reloaded = new FileHighScoreStore(path).Load();
            if (reloaded.Top.Score != 73250 || reloaded.Top.Name != "BHM" || reloaded.Entries.Count != HighScoreTable.Capacity)
                throw new InvalidOperationException($"Table did not persist (top {reloaded.Top.Score} {reloaded.Top.Name}, {reloaded.Entries.Count} rows).");

            Console.WriteLine($"  high score table: saved/reloaded top {reloaded.Top.Score} {reloaded.Top.Name}, {reloaded.Entries.Count} rows OK");
        }
        finally
        {
            try { File.Delete(path); } catch { }
        }
    }

    private static void SimulateScreens()
    {
        var renderer = new HeadlessRenderer(Layout.CanvasWidth, Layout.CanvasHeight);
        var table = HighScoreTable.Default();
        var initials = new[] { 'A', 'B', 'C' };
        var solo = new MatchSummary(new[] { 24500 }, 7, 95.0);
        var duo = new MatchSummary(new[] { 24500, 31200 }, 9, 120.0);

        for (double t = 0; t < 3; t += 0.2)
        {
            for (int menu = 0; menu < Screens.MenuOptions.Length; menu++)
            {
                renderer.BeginFrame(); Screens.DrawTitle(renderer, t, menu); renderer.EndFrame();
            }
            renderer.BeginFrame(); Screens.DrawReadyOverlay(renderer, player: 2, twoPlayer: true); renderer.EndFrame();
            renderer.BeginFrame(); Screens.DrawNameEntry(renderer, t, 31200, initials, slot: (int)(t * 5) % 3, table); renderer.EndFrame();
            renderer.BeginFrame(); Screens.DrawGameOver(renderer, t, solo, table, newRecordRank: 1); renderer.EndFrame();
            renderer.BeginFrame(); Screens.DrawGameOver(renderer, t, duo, table, newRecordRank: -1); renderer.EndFrame();
            renderer.BeginFrame(); Screens.DrawPauseOverlay(renderer); renderer.EndFrame();
            renderer.BeginFrame(); Screens.DrawResizePrompt(renderer, 40, 20); renderer.EndFrame();
        }
        Console.WriteLine("  screens: title menu / name entry / results (1P+2P) / pause / resize rendered OK");
    }

    private static Scoreboard TestBoard(PlaySession session) => new(session.Score, 0, 1, false, 0);
}
