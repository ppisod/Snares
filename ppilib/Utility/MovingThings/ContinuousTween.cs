using System;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings;
public class ContinuousTween<T>
{
    private readonly Func<T> _get;
    private readonly Action<T> _set;
    private readonly Func<T, T, float, T> _lerp;


    public bool Active => true;

    private T _start;
    private float _progress;
    
    private T _lastTarget;
    public T Target { get; set; }

    public float Rate { get; }

    public Func<float, float> Ease { get; }

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
    
    public void Update (GameTime gameTime)
    {
        if (!Active) return;
        var dT = (float) gameTime.ElapsedGameTime.TotalSeconds;
        if (!_lastTarget.Equals(Target))
        {
            // changed target
            _start = _get();
            _lastTarget = Target;
            _progress = 0;
        }
        _progress += Math.Min(Rate * dT, 1 - _progress);
        var eased = Ease(_progress);
        var toSet = _lerp(_start, _lastTarget, eased);/*_start + _ease(_progress) * (_lastTarget - _start);*/
        _set(toSet);
    }

}