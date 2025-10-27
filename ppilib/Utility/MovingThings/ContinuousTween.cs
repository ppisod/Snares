using System;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings;

/// <summary>
/// Continuously approaches a target value each frame at a given rate using an easing curve.
/// Unlike fixed-duration tweens, this never "completes"; it re-bases when the target changes.
/// </summary>
public class ContinuousTween<T>
{
    private readonly Func<T> _get;
    private readonly Action<T> _set;
    private readonly Func<T, T, float, T> _lerp;

    /// <summary>
    /// Always active by design; you can wrap usage with your own enable flags.
    /// </summary>
    public bool Active => true;

    private T _start;
    private float _progress;
    
    private T _lastTarget;
    /// <summary>Current target being approached.</summary>
    public T Target { get; set; }

    /// <summary>
    /// Units per second for the normalized approach progress (0..1). Higher is faster.
    /// </summary>
    public float Rate { get; }

    /// <summary>
    /// Easing function mapping normalized progress to eased progress.
    /// </summary>
    public Func<float, float> Ease { get; }

    /// <summary>
    /// Create a new continuous tween.
    /// </summary>
    /// <param name="get">Accessor for the current value.</param>
    /// <param name="set">Setter to apply interpolated values.</param>
    /// <param name="lerp">Interpolation function (from, to, t).</param>
    /// <param name="ease">Easing function mapping [0..1] to [0..1].</param>
    /// <param name="rate">Progress speed in normalized units per second.</param>
    public ContinuousTween (Func<T> get, Action<T> set, Func<T, T, float, T> lerp, Func<float, float> ease, float rate)
    {
        _get = get;
        _set = set;
        _lerp = lerp;
        Ease = ease;
        Rate = Math.Abs(rate);
        _start = _get();
        _lastTarget = _get();
        Target = _start;
    }
    
    /// <summary>
    /// Advance the tween by elapsed time and apply the new interpolated value.
    /// </summary>
    public void Update (GameTime gameTime)
    {
        if (!Active) return;
        var dT = (float) gameTime.ElapsedGameTime.TotalSeconds;
        if (!_lastTarget!.Equals(Target))
        {
            // changed target: re-base from current value
            _start = _get();
            _lastTarget = Target;
            _progress = 0;
        }
        _progress += Math.Min(Rate * dT, 1 - _progress);
        var eased = Ease(_progress);
        var toSet = _lerp(_start, _lastTarget, eased);
        _set(toSet);
    }
}