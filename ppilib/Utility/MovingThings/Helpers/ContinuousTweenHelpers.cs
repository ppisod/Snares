using System;
using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace ppilib.Utility.MovingThings.Helpers;

/// <summary>
/// Helper factory methods for building continuous tweens of common types.
/// </summary>
public static class ContinuousTweenHelpers
{
    /// <summary>
    /// Creates a ContinuousTween for Microsoft.Xna.Framework.Color using Color.Lerp as the interpolator.
    /// </summary>
    public static ContinuousTween<Color> CreateColor(Func<Color> get, Action<Color> set, Func<float, float> ease, float rate)
        => new (get, set, Color.Lerp, ease, rate);
    public static ContinuousTween<Vector2> CreateVector2(Func<Vector2> get, Action<Vector2> set, Func<float, float> ease, float rate) 
        => new (get, set, Vector2.Lerp, ease, rate);
    public static ContinuousTween<float> CreateFloat(Func<float> get, Action<float> set, Func<float, float> ease, float rate) 
        => new (get, set, (a, b, t) => a + (b - a) * t, ease, rate);
}