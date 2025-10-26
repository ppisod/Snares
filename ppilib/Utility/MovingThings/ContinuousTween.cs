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
    
    /// <summary>
    /// Update of the Continuous Tween, will return the new "set" value so you can build your own vector2 if you even
    /// need to
    /// </summary>
    /// <param name="gameTime"></param>
    /// <returns></returns>
    public float Update (GameTime gameTime)
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
        var toSet = _start + _ease(_progress) * (_lastTarget - _start);
        _set(toSet);
        return toSet;
    }

}