namespace AsciiOrbit.Gameplay.Behaviors;

// A small, reusable set of movement strategies. The invader catalogue assigns these to letters with
// escalating parameters, so A drifts down gently while Z dives and hunts. Each behaviour multiplies
// its base speed by the invader's per-level SpeedScale, which keeps difficulty climbing even after
// the letters cap out at Z.

/// <summary>Falls straight down at a constant speed.</summary>
internal sealed class StraightBehavior : IInvaderBehavior
{
    private readonly double _fallSpeed;
    public StraightBehavior(double fallSpeed) => _fallSpeed = fallSpeed;

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
        => invader.Y += _fallSpeed * invader.SpeedScale * dt;
}

/// <summary>Falls while swaying left and right in a smooth sine wave around its spawn column.</summary>
internal sealed class SwayBehavior : IInvaderBehavior
{
    private readonly double _fallSpeed;
    private readonly double _amplitude;
    private readonly double _frequency;

    public SwayBehavior(double fallSpeed, double amplitude, double frequency)
    {
        _fallSpeed = fallSpeed;
        _amplitude = amplitude;
        _frequency = frequency;
    }

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
    {
        invader.Y += _fallSpeed * invader.SpeedScale * dt;
        invader.X = invader.SpawnX + _amplitude * Math.Sin(_frequency * (invader.Age + invader.Phase));
    }
}

/// <summary>Falls while gliding sideways, bouncing off the arena walls.</summary>
internal sealed class DriftBehavior : IInvaderBehavior
{
    private readonly double _fallSpeed;
    private readonly double _sideSpeed;

    public DriftBehavior(double fallSpeed, double sideSpeed)
    {
        _fallSpeed = fallSpeed;
        _sideSpeed = sideSpeed;
    }

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
    {
        invader.Y += _fallSpeed * invader.SpeedScale * dt;
        invader.X += invader.Direction * _sideSpeed * invader.SpeedScale * dt;

        if (invader.X <= 0) { invader.X = 0; invader.Direction = 1; }
        else if (invader.X >= ctx.ArenaWidth - 1) { invader.X = ctx.ArenaWidth - 1; invader.Direction = -1; }
    }
}

/// <summary>Falls while creeping horizontally toward the player's current column.</summary>
internal sealed class TrackerBehavior : IInvaderBehavior
{
    private readonly double _fallSpeed;
    private readonly double _sideSpeed;

    public TrackerBehavior(double fallSpeed, double sideSpeed)
    {
        _fallSpeed = fallSpeed;
        _sideSpeed = sideSpeed;
    }

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
    {
        invader.Y += _fallSpeed * invader.SpeedScale * dt;

        double step = _sideSpeed * invader.SpeedScale * dt;
        if (invader.X < ctx.PlayerX - 0.5) invader.X += step;
        else if (invader.X > ctx.PlayerX + 0.5) invader.X -= step;
    }
}

/// <summary>Cruises slowly, then accelerates into a fast dive once it passes a trigger depth.</summary>
internal sealed class DiverBehavior : IInvaderBehavior
{
    private readonly double _cruiseSpeed;
    private readonly double _diveSpeed;
    private readonly double _triggerFraction;

    public DiverBehavior(double cruiseSpeed, double diveSpeed, double triggerFraction)
    {
        _cruiseSpeed = cruiseSpeed;
        _diveSpeed = diveSpeed;
        _triggerFraction = triggerFraction;
    }

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
    {
        double triggerY = ctx.ArenaHeight * _triggerFraction;
        double speed = invader.Y < triggerY ? _cruiseSpeed : _diveSpeed;
        invader.Y += speed * invader.SpeedScale * dt;
    }
}

/// <summary>Descends in stop-and-go bursts: it holds position, then lurches downward, then holds again.</summary>
internal sealed class StutterBehavior : IInvaderBehavior
{
    private readonly double _burstSpeed;
    private readonly double _moveDuration;
    private readonly double _cycleDuration;

    public StutterBehavior(double averageSpeed, double moveDuration, double pauseDuration)
    {
        _moveDuration = moveDuration;
        _cycleDuration = moveDuration + pauseDuration;
        // Move faster during the active window so the average descent still matches averageSpeed.
        _burstSpeed = averageSpeed * _cycleDuration / moveDuration;
    }

    public void Update(Invader invader, in BehaviorContext ctx, double dt)
    {
        double t = (invader.Age + invader.Phase) % _cycleDuration;
        if (t < _moveDuration)
            invader.Y += _burstSpeed * invader.SpeedScale * dt;
    }
}
