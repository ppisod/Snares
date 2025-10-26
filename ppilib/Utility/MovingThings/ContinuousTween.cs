using System;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings;
public class ContinuousTween
{
    private readonly Func<float> _get;
    private readonly Action<float> _set;

    private float _start;
    private float _progress;
    
    private float _lastTarget;
    public float Target;
    
    private float _rate;
    
    private Func<float, float> _ease;

    public ContinuousTween (Func<float> get, Action<float> set, Func<float, float> ease, float rate)
    {
        _get = get;
        _set = set;
        _ease = ease;
        _rate = Math.Abs(rate);
        _start = _get();
        _lastTarget = _get();
        Target = _start;
    }

    public void Update (GameTime gameTime)
    {
        var dT = (float) gameTime.ElapsedGameTime.TotalSeconds;
        if (Math.Abs(_lastTarget - Target) > 0f)
        {
            // changed target
            _start = _get();
            _lastTarget = Target;
            _progress = 0;
        }

        _progress += Math.Min(_rate * dT, 1 - _progress);
        _set(_start + (_ease(_progress) * (_lastTarget - _start)));

    }

}