using AsciiOrbit.Audio;
using AsciiOrbit.Gameplay;
using AsciiOrbit.Input;
using AsciiOrbit.Rendering;
using AsciiOrbit.UI;

namespace AsciiOrbit;

internal enum GameScreen
{
    Title,
    PlayerReady,
    Playing,
    Paused,
    NameEntry,
    GameOver,
}

/// <summary>
/// The top-level state machine and game loop. It drives the title menu, runs a one- or two-player
/// match (in two-player the turn passes to the next player each time the active one is hit), handles
/// the explosion/respawn death cycle, lets a new record holder enter their initials, and shows the
/// results. Each player keeps their own <see cref="PlaySession"/> so their board and score persist
/// across turns.
/// </summary>
internal sealed class Game
{
    private const double ReadyDuration = 2.0;

    private readonly IRenderer _renderer;
    private readonly IInputSource _input;
    private readonly ISoundEngine _sound;
    private readonly IHighScoreStore _highScores;
    private readonly Random _rng;
    private readonly GameClock _clock = new(targetFps: 60);

    private GameScreen _screen = GameScreen.Title;
    private double _screenTime;
    private bool _running = true;

    private HighScoreTable _table;
    private int _newRecordRank = -1;

    // Menu / match state.
    private int _menuIndex;
    private int _playerCount = 1;
    private GameDifficulty _mode = GameDifficulty.GameA;
    private PlaySession[] _sessions;
    private int _active;
    private double _readyTimer;

    // Name entry state.
    private readonly char[] _initials = { 'A', 'A', 'A' };
    private int _initialSlot;
    private int _pendingScore;

    public Game(IRenderer renderer, IInputSource input, ISoundEngine sound, Random rng, IHighScoreStore highScores)
    {
        _renderer = renderer;
        _input = input;
        _sound = sound;
        _rng = rng;
        _highScores = highScores;
        _table = highScores.Load();
        _sessions = new[] { new PlaySession(sound, rng, _mode) };
    }

    private PlaySession Active => _sessions[_active];

    public void Run()
    {
        try
        {
            while (_running)
            {
                double dt = _clock.NextFrame();
                _screenTime += dt;
                IReadOnlyList<InputCommand> commands = _input.Poll();

                if (!TerminalLargeEnough())
                {
                    DrawResizePrompt();
                    if (Has(commands, InputCommand.Quit))
                        _running = false;
                    continue;
                }

                switch (_screen)
                {
                    case GameScreen.Title:       UpdateTitle(commands);          break;
                    case GameScreen.PlayerReady: UpdatePlayerReady(dt, commands); break;
                    case GameScreen.Playing:     UpdatePlaying(dt, commands);     break;
                    case GameScreen.Paused:      UpdatePaused(commands);          break;
                    case GameScreen.NameEntry:   UpdateNameEntry(commands);       break;
                    case GameScreen.GameOver:    UpdateGameOver(commands);        break;
                }
            }
        }
        finally
        {
            RestoreConsole();
        }
    }

    // ----- Title menu -----

    private void UpdateTitle(IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.Quit))
        {
            _running = false;
            return;
        }
        if (Has(commands, InputCommand.MoveUp))
        {
            _menuIndex = (_menuIndex + Screens.MenuOptions.Length - 1) % Screens.MenuOptions.Length;
            _sound.Play(Sfx.MenuMove);
        }
        if (Has(commands, InputCommand.MoveDown))
        {
            _menuIndex = (_menuIndex + 1) % Screens.MenuOptions.Length;
            _sound.Play(Sfx.MenuMove);
        }
        if (Has(commands, InputCommand.Confirm) || Has(commands, InputCommand.Fire))
        {
            StartMatch();
            return;
        }
        Render(() => Screens.DrawTitle(_renderer, _screenTime, _menuIndex));
    }

    private void StartMatch()
    {
        _playerCount = _menuIndex < 2 ? 1 : 2;
        _mode = _menuIndex % 2 == 0 ? GameDifficulty.GameA : GameDifficulty.GameB;

        _sessions = new PlaySession[_playerCount];
        for (int i = 0; i < _playerCount; i++)
            _sessions[i] = new PlaySession(_sound, _rng, _mode);

        _active = 0;
        _newRecordRank = -1;
        _sound.Play(Sfx.MenuSelect);
        BeginTurn();
    }

    private void BeginTurn()
    {
        _readyTimer = ReadyDuration;
        SwitchTo(GameScreen.PlayerReady);
    }

    // ----- Player ready splash -----

    private void UpdatePlayerReady(double dt, IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.Quit))
        {
            SwitchTo(GameScreen.Title);
            return;
        }

        _readyTimer -= dt;
        if (_readyTimer <= 0)
        {
            if (Active.AwaitingRespawn)
                Active.Respawn();
            SwitchTo(GameScreen.Playing);
            return;
        }

        Render(() =>
        {
            Hud.Draw(_renderer, Active, BuildScoreboard());
            Screens.DrawReadyOverlay(_renderer, _active + 1, _playerCount == 2);
        });
    }

    // ----- Playing -----

    private void UpdatePlaying(double dt, IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.Quit))
        {
            SwitchTo(GameScreen.Title);
            return;
        }
        if (Has(commands, InputCommand.Pause))
        {
            _sound.Play(Sfx.MenuSelect);
            SwitchTo(GameScreen.Paused);
            return;
        }

        Active.Update(dt, commands);
        if (Active.AwaitingRespawn)
        {
            HandleDeath();
            return;
        }

        Render(() =>
        {
            Active.Draw(_renderer);
            Hud.Draw(_renderer, Active, BuildScoreboard());
        });
    }

    private void HandleDeath()
    {
        int next = NextPlayerWithLives(_active);
        if (next < 0)
        {
            EndMatch();
            return;
        }

        if (next == _active)
        {
            // The same player carries on (single player, or the only one left with ships).
            Active.Respawn();
            SwitchTo(GameScreen.Playing);
        }
        else
        {
            // Pass the turn to the next player (their ship respawns when the ready splash ends).
            _active = next;
            BeginTurn();
        }
    }

    private int NextPlayerWithLives(int from)
    {
        for (int k = 1; k <= _playerCount; k++)
        {
            int index = (from + k) % _playerCount;
            if (_sessions[index].Lives > 0)
                return index;
        }
        return -1;
    }

    private void EndMatch()
    {
        int best = 0;
        foreach (PlaySession session in _sessions)
            best = Math.Max(best, session.Score);

        if (_table.Qualifies(best))
        {
            _pendingScore = best;
            _initials[0] = _initials[1] = _initials[2] = 'A';
            _initialSlot = 0;
            SwitchTo(GameScreen.NameEntry);
        }
        else
        {
            _newRecordRank = -1;
            SwitchTo(GameScreen.GameOver);
        }
    }

    // ----- Paused -----

    private void UpdatePaused(IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.Quit))
        {
            SwitchTo(GameScreen.Title);
            return;
        }
        if (Has(commands, InputCommand.Pause))
        {
            _sound.Play(Sfx.MenuSelect);
            SwitchTo(GameScreen.Playing);
            return;
        }

        Render(() =>
        {
            Active.Draw(_renderer);
            Hud.Draw(_renderer, Active, BuildScoreboard());
            Screens.DrawPauseOverlay(_renderer);
        });
    }

    // ----- Name entry -----

    private void UpdateNameEntry(IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.MoveUp))
        {
            CycleInitial(+1);
            _sound.Play(Sfx.MenuMove);
        }
        if (Has(commands, InputCommand.MoveDown))
        {
            CycleInitial(-1);
            _sound.Play(Sfx.MenuMove);
        }
        if (Has(commands, InputCommand.MoveLeft))
            _initialSlot = Math.Max(0, _initialSlot - 1);
        if (Has(commands, InputCommand.MoveRight))
            _initialSlot = Math.Min(_initials.Length - 1, _initialSlot + 1);

        if (Has(commands, InputCommand.Confirm) || Has(commands, InputCommand.Fire))
        {
            _sound.Play(Sfx.MenuSelect);
            if (_initialSlot + 1 < _initials.Length)
            {
                _initialSlot++;
            }
            else
            {
                CommitHighScore();
                SwitchTo(GameScreen.GameOver);
                return;
            }
        }

        if (Has(commands, InputCommand.Quit))
        {
            CommitHighScore();
            SwitchTo(GameScreen.GameOver);
            return;
        }

        Render(() => Screens.DrawNameEntry(_renderer, _screenTime, _pendingScore, _initials, _initialSlot, _table));
    }

    private void CycleInitial(int delta)
    {
        int letter = (_initials[_initialSlot] - 'A' + delta + 26) % 26;
        _initials[_initialSlot] = (char)('A' + letter);
    }

    private void CommitHighScore()
    {
        _newRecordRank = _table.Insert(_pendingScore, new string(_initials));
        _highScores.Save(_table);
    }

    // ----- Game over / results -----

    private void UpdateGameOver(IReadOnlyList<InputCommand> commands)
    {
        if (Has(commands, InputCommand.Quit))
        {
            _running = false;
            return;
        }
        if (Has(commands, InputCommand.Confirm) || Has(commands, InputCommand.Fire))
        {
            _sound.Play(Sfx.MenuSelect);
            SwitchTo(GameScreen.Title);
            return;
        }

        Render(() => Screens.DrawGameOver(_renderer, _screenTime, BuildSummary(), _table, _newRecordRank));
    }

    // ----- Helpers -----

    private Scoreboard BuildScoreboard()
    {
        int score1 = _sessions[0].Score;
        int score2 = _playerCount < 2 ? 0 : _sessions[1].Score;
        return new Scoreboard(score1, score2, _active + 1, _playerCount == 2, _table.Top.Score);
    }

    private MatchSummary BuildSummary()
    {
        var scores = new int[_sessions.Length];
        for (int i = 0; i < _sessions.Length; i++)
            scores[i] = _sessions[i].Score;
        return new MatchSummary(scores, _sessions[0].Level, _sessions[0].Elapsed);
    }

    private void SwitchTo(GameScreen screen)
    {
        _screen = screen;
        _screenTime = 0;
    }

    private void Render(Action draw)
    {
        _renderer.BeginFrame();
        draw();
        _renderer.EndFrame();
    }

    private void DrawResizePrompt()
    {
        int width = 0, height = 0;
        try { width = Console.WindowWidth; height = Console.WindowHeight; } catch { }
        Render(() => Screens.DrawResizePrompt(_renderer, width, height));
    }

    private static bool TerminalLargeEnough()
    {
        if (Console.IsOutputRedirected)
            return true;
        try
        {
            return Console.WindowWidth >= Layout.MinTerminalWidth && Console.WindowHeight >= Layout.MinTerminalHeight;
        }
        catch
        {
            return true;
        }
    }

    private static bool Has(IReadOnlyList<InputCommand> commands, InputCommand command)
    {
        for (int i = 0; i < commands.Count; i++)
            if (commands[i] == command)
                return true;
        return false;
    }

    private static void RestoreConsole()
    {
        try { Console.ResetColor(); } catch { }
        try { Console.CursorVisible = true; } catch { }
        try { Console.Clear(); } catch { }
        try { Console.SetCursorPosition(0, 0); } catch { }
    }
}
