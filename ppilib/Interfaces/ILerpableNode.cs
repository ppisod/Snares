using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Interfaces;

/// <summary>
/// Interface for a transform node that supports time-based interpolation (tweening) of its properties.
/// Implementations should provide convenience helpers for common attributes like position, scale, rotation and opacity.
/// </summary>
public interface ILerpableNode : ITransformNode
{
    /// <summary>
    /// The internal tween manager that runs scheduled tweens.
    /// </summary>
    Lerper Lerper { get; }
    
    /// <summary>
    /// The default easing function used for newly started tweens when an overload without explicit easing is used.
    /// </summary>
    Easing Easing { get; set; }
    
    /// <summary>
    /// The default easing mode (In, Out, InOut) used for overloads that don't pass a mode explicitly.
    /// </summary>
    LerpMode EasingMode { get; set; }
    
    /// <summary>
    /// Starts a tween for an arbitrary attribute.
    /// </summary>
    /// <param name="get">Accessor that returns the current value.</param>
    /// <param name="set">Setter used to apply interpolated values each frame.</param>
    /// <param name="interpolate">Interpolation function: (from, to, t) -&gt; value.</param>
    /// <param name="to">Target value to reach at the end of the tween.</param>
    /// <param name="time">Duration in seconds.</param>
    void LerpAttribute <T> (Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time);
    
    /// <summary>
    /// Starts a tween for the local position.
    /// </summary>
    void LerpLocalPos (Vector2 to, double time);
    /// <summary>
    /// Starts a tween for the local scale.
    /// </summary>
    void LerpLocalScale(Vector2 to, double time);
    /// <summary>
    /// Starts a tween for the local rotation (in radians).
    /// </summary>
    void LerpLocalRotation (float to, double time);
    
    /// <summary>
    /// Starts a tween for the node's opacity.
    /// </summary>
    void LerpOpacity (float to, double time);
}