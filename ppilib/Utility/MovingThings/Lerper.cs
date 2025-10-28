using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Ease;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.MovingThings.Enums;
using ppilib.Utility.MovingThings.Interfaces;

namespace ppilib.Utility.MovingThings;

/// <summary>
/// Small, generic tween/lerp manager that can animate multiple attributes (Vector2, float, Color, etc.)
/// using the existing Easing types. Designed to be minimal and non-invasive.
/// </summary>
public class Lerper
{

    private readonly List<ITween> _tweens = new();

    /// <summary>
    /// Adds a generic tween.
    /// </summary>
    public void AddTween<T>(Func<T> get, Action<T> set, T end, double durationSeconds, Func<float, float> easing, Func<T, T, float, T> interpolate)
    {
        _tweens.Add(new Tween<T>(get, set, end, durationSeconds, easing, interpolate));
    }

    /// <summary>
    /// Convenience for Vector2 using Vector2.Lerp.
    /// </summary>
    public void LerpVector2(Func<Vector2> get, Action<Vector2> set, Vector2 end, double durationSeconds, Func<float, float> easing)
    {
        AddTween(get, set, end, durationSeconds, easing, Vector2.Lerp);
    }

    /// <summary>
    /// Convenience for float values.
    /// </summary>
    public void LerpFloat(Func<float> get, Action<float> set, float end, double durationSeconds, Func<float, float> easing)
    {
        AddTween(get, set, end, durationSeconds, easing, (a, b, t) => a + (b - a) * t);
        // also Don't be scared by that cuz it's just a simple lerp.
    }

    /// <summary>
    /// Convenience for Color values using Color.Lerp.
    /// </summary>
    public void LerpColor(Func<Color> get, Action<Color> set, Color end, double durationSeconds, Func<float, float> easing)
    {
        AddTween(get, set, end, durationSeconds, easing, Color.Lerp);
    }

    /// <summary>
    /// Advance all tweens by delta time. Call once per frame.........!!!
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