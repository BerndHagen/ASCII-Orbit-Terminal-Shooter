using AsciiOrbit.Audio;
using AsciiOrbit.Gameplay.Behaviors;
using AsciiOrbit.Input;
using AsciiOrbit.Rendering;

namespace AsciiOrbit.Gameplay;

/// <summary>
/// Owns one player's round of play: the player, invaders, projectiles and power-ups, plus the score,
/// lives and level. Everything runs on the single game-loop thread and advances by an explicit time
/// step, so there are no background timers mutating shared state and behaviour is identical
/// regardless of frame rate. When the player is hit, the ship explodes, the screen clears, and after
/// a short delay it respawns (or the round ends) — the classic Jetpac-style death.
/// </summary>
internal sealed class PlaySession
{
    private readonly ISoundEngine _sound;
    private readonly Random _rng;
    private readonly GameDifficulty _mode;

    private readonly Player _player = new();
    private readonly List<Invader> _invaders = new();
    private readonly List<Projectile> _projectiles = new();
    private readonly List<PowerUp> _powerUps = new();

    private readonly int[] _collected = new int[6];

    private double _spawnTimer;
    private int _killsThisLevel;
    private int _nextExtraLifeScore;
    private double _levelBannerTimer;
    private double _deathTimer;
    private int _deathX;

    private const double LevelBannerDuration = 1.6;
    private const double DeathDuration = 1.4;
    private const double RespawnGrace = 1.5;

    public PlaySession(ISoundEngine sound, Random rng, GameDifficulty mode)
    {
        _sound = sound;
        _rng = rng;
        _mode = mode;
        Reset();
    }

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int Level { get; private set; }
    public double Elapsed { get; private set; }
    public bool IsDying => _deathTimer > 0;

    /// <summary>True once the explosion has finished and the controller must respawn this player or hand off to the next.</summary>
    public bool AwaitingRespawn { get; private set; }

    /// <summary>True when this player has no ships left (and the explosion has finished).</summary>
    public bool IsOut => AwaitingRespawn && Lives <= 0;

    public Player Player => _player;
    public InvaderArchetype CurrentArchetype { get; private set; } = InvaderCatalog.ForLevel(1);
    public int KillsThisLevel => _killsThisLevel;
    public int KillsNeeded => Difficulty.InvadersToClear(Level);
    public bool ShowLevelBanner => _levelBannerTimer > 0;
    public int CollectedCount(PowerUpKind kind) => _collected[(int)kind];

    public IReadOnlyList<Invader> Invaders => _invaders;
    public IReadOnlyList<Projectile> Projectiles => _projectiles;
    public IReadOnlyList<PowerUp> PowerUps => _powerUps;

    public void Reset()
    {
        _invaders.Clear();
        _projectiles.Clear();
        _powerUps.Clear();
        _player.Reset(Layout.ArenaWidth / 2);

        Score = 0;
        Lives = _mode.StartingLives();
        Level = 1;
        Elapsed = 0;
        AwaitingRespawn = false;
        _killsThisLevel = 0;
        _nextExtraLifeScore = Difficulty.FirstExtraLifeScore;
        CurrentArchetype = InvaderCatalog.ForLevel(Level);
        _spawnTimer = NextSpawn();
        _levelBannerTimer = LevelBannerDuration;
        _deathTimer = 0;
        Array.Clear(_collected);
    }

    public void Update(double dt, IReadOnlyList<InputCommand> commands)
    {
        // Paused waiting for the controller to respawn this player or switch turns.
        if (AwaitingRespawn)
            return;

        Elapsed += dt;

        // While exploding, freeze the round: no input, spawning or movement until the death plays out.
        if (IsDying)
        {
            UpdateDeath(dt);
            return;
        }

        _player.Tick(dt);
        if (_levelBannerTimer > 0)
            _levelBannerTimer = Math.Max(0, _levelBannerTimer - dt);

        HandleInput(commands);
        UpdateSpawning(dt);
        UpdateProjectiles(dt);
        UpdateInvaders(dt);
        UpdatePowerUps(dt);
    }

    private void UpdateDeath(double dt)
    {
        _deathTimer = Math.Max(0, _deathTimer - dt);
        if (_deathTimer <= 0)
            AwaitingRespawn = true; // hand control back to the match controller
    }

    private void HandleInput(IReadOnlyList<InputCommand> commands)
    {
        foreach (InputCommand command in commands)
        {
            switch (command)
            {
                case InputCommand.MoveLeft:
                    _player.X = Math.Max(0, _player.X - 1);
                    break;
                case InputCommand.MoveRight:
                    _player.X = Math.Min(Layout.ArenaWidth - 1, _player.X + 1);
                    break;
                case InputCommand.Fire:
                    TryFire();
                    break;
            }
        }
    }

    private void TryFire()
    {
        if (!_player.CanFire)
            return;

        _player.StartFireCooldown(_player.RapidFireActive ? Difficulty.RapidFireCooldown : Difficulty.BaseFireCooldown);

        int y = Layout.PlayerRow - 1;
        SpawnProjectile(_player.X, y);
        if (_player.SpreadActive)
        {
            SpawnProjectile(_player.X - 1, y);
            SpawnProjectile(_player.X + 1, y);
        }
        _sound.Play(Sfx.Shoot);
    }

    private void SpawnProjectile(int x, int y)
    {
        if (x >= 0 && x < Layout.ArenaWidth)
            _projectiles.Add(new Projectile(x, y));
    }

    private double NextSpawn() => Difficulty.NextSpawnInterval(Level, _rng) * _mode.SpawnMultiplier();

    private void UpdateSpawning(double dt)
    {
        _spawnTimer -= dt;
        if (_spawnTimer > 0)
            return;

        _spawnTimer = NextSpawn();
        if (_invaders.Count >= Difficulty.MaxInvadersOnScreen)
            return;

        InvaderArchetype archetype = CurrentArchetype;
        double x = _rng.Next(0, Layout.ArenaWidth);
        var invader = new Invader
        {
            X = x,
            Y = 0,
            SpawnX = x,
            Letter = archetype.Letter,
            Color = archetype.Color,
            Hp = archetype.Hp,
            ScoreValue = archetype.ScoreValue,
            Behavior = archetype.Behavior,
            SpeedScale = Difficulty.SpeedScale(Level) * _mode.SpeedMultiplier(),
            Phase = _rng.NextDouble() * (2 * Math.PI),
            Direction = _rng.Next(2) == 0 ? -1 : 1,
        };
        _invaders.Add(invader);
        _sound.Play(Sfx.InvaderSpawn);
    }

    private void UpdateProjectiles(double dt)
    {
        for (int i = _projectiles.Count - 1; i >= 0; i--)
        {
            Projectile projectile = _projectiles[i];
            double previousY = projectile.Y;
            projectile.Y -= Difficulty.ProjectileSpeed * dt;

            if (projectile.Y < 0)
            {
                _projectiles.RemoveAt(i);
                continue;
            }

            // Test every cell the shot swept through this frame so fast shots never tunnel past an invader.
            int top = (int)Math.Floor(projectile.Y);
            int bottom = (int)Math.Ceiling(previousY);
            if (TryHitInvader(projectile.X, top, bottom))
                _projectiles.RemoveAt(i);
        }
    }

    private bool TryHitInvader(int column, int topRow, int bottomRow)
    {
        for (int j = 0; j < _invaders.Count; j++)
        {
            Invader invader = _invaders[j];
            if (invader.CellX == column && invader.CellY >= topRow && invader.CellY <= bottomRow)
            {
                invader.Hp--;
                if (invader.Hp <= 0)
                    KillInvader(j);
                else
                    _sound.Play(Sfx.InvaderHit);
                return true;
            }
        }
        return false;
    }

    private void KillInvader(int index)
    {
        Invader invader = _invaders[index];
        _invaders.RemoveAt(index);

        AddScore(invader.ScoreValue);
        _sound.Play(Sfx.InvaderHit);

        _killsThisLevel++;
        if (_killsThisLevel >= Difficulty.InvadersToClear(Level))
            AdvanceLevel();

        MaybeDropPowerUp(invader.CellX, invader.CellY);
    }

    private void AdvanceLevel()
    {
        Level++;
        _killsThisLevel = 0;
        CurrentArchetype = InvaderCatalog.ForLevel(Level);
        _levelBannerTimer = LevelBannerDuration;
        _sound.Play(Sfx.LevelUp);
    }

    private void AddScore(int amount)
    {
        Score += amount;
        while (Score >= _nextExtraLifeScore)
        {
            Lives++;
            _nextExtraLifeScore *= 2;
            _sound.Play(Sfx.ExtraLife);
        }
    }

    private void MaybeDropPowerUp(int x, int y)
    {
        if (_rng.NextDouble() >= PowerUpTable.DropChance)
            return;

        PowerUpKind kind = PowerUpTable.Roll(_rng);
        _powerUps.Add(new PowerUp(Math.Clamp(x, 0, Layout.ArenaWidth - 1), Math.Max(0, y), kind));
    }

    private void UpdateInvaders(double dt)
    {
        var ctx = new BehaviorContext(Layout.ArenaWidth, Layout.ArenaHeight, _player.X, _rng);
        for (int i = _invaders.Count - 1; i >= 0; i--)
        {
            Invader invader = _invaders[i];
            invader.Advance(in ctx, dt);

            if (invader.CellY >= Layout.PlayerRow)
            {
                _invaders.RemoveAt(i);
                Breach();
                if (IsDying)
                    return; // the death sequence cleared the list; stop iterating
            }
        }
    }

    private void Breach()
    {
        if (_player.HasShield)
        {
            _player.HasShield = false;
            _sound.Play(Sfx.InvaderHit);
            return;
        }

        Lives--;
        StartDeath();
    }

    private void StartDeath()
    {
        _deathTimer = DeathDuration;
        _deathX = _player.X;
        _levelBannerTimer = 0;
        _invaders.Clear();
        _projectiles.Clear();
        _powerUps.Clear();
        _sound.Play(Sfx.PlayerExplode);
    }

    /// <summary>Brings the ship back after an explosion (called by the match controller). Keeps score, level and lives.</summary>
    public void Respawn()
    {
        _player.Reset(Layout.ArenaWidth / 2);
        _spawnTimer = Math.Max(_spawnTimer, RespawnGrace); // a moment of calm before enemies return
        AwaitingRespawn = false;
        _deathTimer = 0;
    }

    private void UpdatePowerUps(double dt)
    {
        for (int i = _powerUps.Count - 1; i >= 0; i--)
        {
            PowerUp powerUp = _powerUps[i];
            powerUp.Y += Difficulty.PowerUpFallSpeed * dt;

            if (powerUp.CellY < Layout.PlayerRow)
                continue;

            if (Math.Abs(powerUp.X - _player.X) <= 1)
            {
                Collect(powerUp.Kind);
                _powerUps.RemoveAt(i);
            }
            else if (powerUp.CellY > Layout.PlayerRow)
            {
                _powerUps.RemoveAt(i); // fell past the player
            }
        }
    }

    private void Collect(PowerUpKind kind)
    {
        _collected[(int)kind]++;

        switch (kind)
        {
            case PowerUpKind.RapidFire:
                _player.RapidFireRemaining = Difficulty.PowerUpDuration;
                _sound.Play(Sfx.PowerUp);
                break;
            case PowerUpKind.SpreadShot:
                _player.SpreadRemaining = Difficulty.PowerUpDuration;
                _sound.Play(Sfx.PowerUp);
                break;
            case PowerUpKind.Shield:
                _player.HasShield = true;
                _sound.Play(Sfx.PowerUp);
                break;
            case PowerUpKind.ExtraLife:
                Lives++;
                _sound.Play(Sfx.ExtraLife);
                break;
            case PowerUpKind.BonusPoints:
                AddScore(Difficulty.BonusPickupScore(Level));
                _sound.Play(Sfx.BonusPickup);
                break;
            case PowerUpKind.Blast:
                DetonateBlast();
                _sound.Play(Sfx.Blast);
                break;
        }
    }

    /// <summary>Screen-clearing "smart bomb": destroys every invader on screen, scoring each (no chain drops).</summary>
    private void DetonateBlast()
    {
        foreach (Invader invader in _invaders)
        {
            AddScore(invader.ScoreValue);
            _killsThisLevel++;
            if (_killsThisLevel >= Difficulty.InvadersToClear(Level))
                AdvanceLevel();
        }
        _invaders.Clear();
    }

    /// <summary>Draws the play field contents: invaders, shots, power-ups, the ship (or its explosion) and the level flash. The cabinet chrome is drawn by the HUD.</summary>
    public void Draw(IRenderer r)
    {
        foreach (Invader invader in _invaders)
            r.Set(Layout.ToCanvasX(invader.CellX), Layout.ToCanvasY(invader.CellY), invader.Letter, invader.Color);

        foreach (Projectile projectile in _projectiles)
            r.Set(Layout.ToCanvasX(projectile.X), Layout.ToCanvasY(projectile.CellY), Symbols.Projectile, ConsoleColor.White);

        foreach (PowerUp powerUp in _powerUps)
        {
            PowerUpInfo info = PowerUpInfo.For(powerUp.Kind);
            r.Set(Layout.ToCanvasX(powerUp.X), Layout.ToCanvasY(powerUp.CellY), info.Symbol, info.Color);
        }

        if (IsDying)
            DrawExplosion(r);
        else
            r.Set(Layout.ToCanvasX(_player.X), Layout.ToCanvasY(Layout.PlayerRow), Symbols.Player,
                _player.HasShield ? ConsoleColor.Cyan : ConsoleColor.White);

        if (_levelBannerTimer > 0)
            DrawLevelBanner(r);
    }

    private void DrawExplosion(IRenderer r)
    {
        double phase = 1.0 - _deathTimer / DeathDuration; // 0 -> 1 over the death
        int frame = Math.Min(3, (int)(phase * 4));
        int x = _deathX;
        int y = Layout.PlayerRow;

        switch (frame)
        {
            case 0:
                Plot(r, x, y, '*', ConsoleColor.White);
                break;
            case 1:
                Plot(r, x, y, 'X', ConsoleColor.Yellow);
                Plot(r, x - 1, y, '+', ConsoleColor.Yellow);
                Plot(r, x + 1, y, '+', ConsoleColor.Yellow);
                Plot(r, x, y - 1, '+', ConsoleColor.Yellow);
                break;
            case 2:
                Plot(r, x, y, 'o', ConsoleColor.Red);
                Plot(r, x - 2, y, '*', ConsoleColor.Red);
                Plot(r, x + 2, y, '*', ConsoleColor.Red);
                Plot(r, x - 1, y - 1, '*', ConsoleColor.Red);
                Plot(r, x + 1, y - 1, '*', ConsoleColor.Red);
                Plot(r, x, y - 2, '*', ConsoleColor.Red);
                break;
            default:
                Plot(r, x - 3, y, '.', ConsoleColor.DarkRed);
                Plot(r, x + 3, y, '.', ConsoleColor.DarkRed);
                Plot(r, x - 2, y - 1, '.', ConsoleColor.DarkRed);
                Plot(r, x + 2, y - 1, '.', ConsoleColor.DarkRed);
                Plot(r, x, y - 3, '.', ConsoleColor.DarkRed);
                break;
        }
    }

    private static void Plot(IRenderer r, int arenaX, int arenaY, char ch, ConsoleColor color)
    {
        if ((uint)arenaX < (uint)Layout.ArenaWidth && (uint)arenaY < (uint)Layout.ArenaHeight)
            r.Set(Layout.ToCanvasX(arenaX), Layout.ToCanvasY(arenaY), ch, color);
    }

    private void DrawLevelBanner(IRenderer r)
    {
        int row = Layout.ArenaTop + Layout.ArenaHeight / 2;
        ConsoleColor flash = (int)(Elapsed * 6) % 2 == 0 ? ConsoleColor.White : ConsoleColor.Yellow;
        r.WriteCentered(row, $"- LEVEL {Level} -", flash);
    }
}
