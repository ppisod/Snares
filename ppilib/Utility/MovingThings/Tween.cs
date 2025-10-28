using System;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.MovingThings.Enums;
using ppilib.Utility.MovingThings.Interfaces;

namespace ppilib.Utility.MovingThings;

public class Tween<T> : ITween
{
    private readonly Func<T> _get;
    private readonly Action<T> _set;
    private readonly T _start;
    private readonly T _end;
    private readonly Func<T, T, float, T> _interp;
    private readonly Func<float, float> _easingFunction;
    private readonly double _duration;

    private double _elapsed;
    public bool IsActive { get; private set; } = true;

    public Tween(Func<T> get, Action<T> set, T end, double durationSeconds, Func<float, float> easingFunction, Func<T, T, float, T> interpolate)
    {
        _get = get;
        _set = set;
        _start = get();
        _end = end;
        _duration = Math.Max(0.000001, durationSeconds);
        _easingFunction = easingFunction;
        _interp = interpolate;
    }

    public bool Update(GameTime gameTime)
    {
        if (!IsActive) return false;

        _elapsed += gameTime.ElapsedGameTime.TotalSeconds;
        var t = (float)Math.Clamp(_elapsed / _duration, 0.0, 1.0);

        float eased = _easingFunction(t);

        var value = _interp(_start, _end, eased);
        _set(value);

        if (t >= 1f)
        {
            IsActive = false;
        }
        return IsActive;
    }
}