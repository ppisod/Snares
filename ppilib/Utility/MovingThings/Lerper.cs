using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings;

/// <summary>
/// Small, generic tween/lerp manager that can animate multiple attributes (Vector2, float, etc.)
/// using the existing Easing types. Designed to be minimal and non-invasive.
/// </summary>
public class Lerper
{
    public enum Mode { In, Out, InOut }

    private interface ITween
    {
        bool Update(GameTime gameTime);
        bool IsActive { get; }
    }

    private class Tween<T> : ITween
    {
        private readonly Func<T> _get;
        private readonly Action<T> _set;
        private readonly T _start;
        private readonly T _end;
        private readonly Func<T, T, float, T> _interp;
        private readonly Easing _easing;
        private readonly Mode _mode;
        private readonly double _duration;

        private double _elapsed;
        public bool IsActive { get; private set; } = true;

        public Tween(Func<T> get, Action<T> set, T end, double durationSeconds, Easing easing, Mode mode, Func<T, T, float, T> interpolate)
        {
            _get = get;
            _set = set;
            _start = get();
            _end = end;
            _duration = Math.Max(0.000001, durationSeconds);
            _easing = easing;
            _mode = mode;
            _interp = interpolate;
        }

        public bool Update(GameTime gameTime)
        {
            if (!IsActive) return false;

            _elapsed += gameTime.ElapsedGameTime.TotalSeconds;
            var t = (float)Math.Clamp(_elapsed / _duration, 0.0, 1.0);

            float eased = _mode switch
            {
                Mode.In => _easing.EaseIn(t),
                Mode.Out => _easing.EaseOut(t),
                Mode.InOut => _easing.EaseInOut(t),
                _ => t
            };

            var value = _interp(_start, _end, eased);
            _set(value);

            if (t >= 1f)
            {
                IsActive = false;
            }
            return IsActive;
        }
    }

    private readonly List<ITween> _tweens = new();

    /// <summary>
    /// Adds a generic tween.
    /// </summary>
    public void AddTween<T>(Func<T> get, Action<T> set, T end, double durationSeconds, Easing easing, Mode mode, Func<T, T, float, T> interpolate)
    {
        _tweens.Add(new Tween<T>(get, set, end, durationSeconds, easing, mode, interpolate));
    }

    /// <summary>
    /// Convenience for Vector2 using Vector2.Lerp.
    /// </summary>
    public void LerpVector2(Func<Vector2> get, Action<Vector2> set, Vector2 end, double durationSeconds, Easing easing, Mode mode)
    {
        AddTween(get, set, end, durationSeconds, easing, mode, Vector2.Lerp);
    }

    /// <summary>
    /// Convenience for float values.
    /// </summary>
    public void LerpFloat(Func<float> get, Action<float> set, float end, double durationSeconds, Easing easing, Mode mode)
    {
        AddTween(get, set, end, durationSeconds, easing, mode, (a, b, t) => a + (b - a) * t);
    }

    /// <summary>
    /// Advance all tweens by delta time. Call once per frame.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        for (int i = _tweens.Count - 1; i >= 0; i--)
        {
            var tw = _tweens[i];
            var active = tw.Update(gameTime);
            if (!active) _tweens.RemoveAt(i);
        }
    }
}