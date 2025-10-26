using System;

namespace ppilib.Utility.MovingThings;

public class ContinuousLerper
{
    private readonly Tuple<Tuple<Func<float>, Action<float>>, Tuple<Func<float>, Action<float>>> _pos;
    private readonly Tuple<Tuple<Func<float>, Action<float>>, Tuple<Func<float>, Action<float>>> _scale;
    private readonly Tuple<Func<float>, Action<float>> _rot;
    private readonly Tuple<Func<float>, Action<float>> _opacity;

    public ContinuousLerper(float rate, Func<float, float> easingFunc, Tuple<Tuple<Func<float>, Action<float>>, Tuple<Func<float>>> posFunc,
        Tuple<Tuple<Func<float>, Action<float>>, Tuple<Func<float>>> scaleFunc,
        Tuple<Func<float>, Action<float>> rotFunc,
        Tuple<Func<float>, Action<float>> opacityFunc)
    {
        
    }
}