using System;
using Microsoft.Xna.Framework;

namespace ppilib.Utility.MovingThings.Interfaces;

/// <summary>
/// Interface for a continuous tween that smoothly approaches a target value over time.
/// Unlike fixed-duration tweens, a continuous tween does not complete; it re-bases when the target changes.
/// </summary>
/// <typeparam name="T">The value type being interpolated (e.g., float, Vector2, Color).</typeparam>
public interface IContinuousTween<T>
{
    /// <summary>
    /// Whether this tween is currently active. Continuous tweens are usually always active by design.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Current target value being approached. Setting this will re-base the tween from the current value.
    /// </summary>
    T Target { get; set; }

    /// <summary>
    /// Normalized progress rate in units per second (higher is faster).
    /// </summary>
    float Rate { get; }

    /// <summary>
    /// Easing function mapping normalized progress [0..1] to eased progress [0..1].
    /// </summary>
    Func<float, float> Ease { get; }

    /// <summary>
    /// Advance the tween and apply the interpolated value.
    /// </summary>
    void Update(GameTime gameTime);
}