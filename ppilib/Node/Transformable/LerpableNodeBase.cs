using System;
using Microsoft.Xna.Framework;
using ppilib.Erroring;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.MovingThings.Ease.Types;
using ppilib.Utility.MovingThings.Enums;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Transformable;

/// <summary>
/// Base class for transformable nodes that exposes simple tween/lerp helpers.
/// Provides default easing settings and convenience methods to animate common attributes.
/// </summary>
public class LerpableNodeBase(NodeConfig n)
    : TransformNodeBase(n), ILerpableNode
{
    /// <inheritdoc />
    public Lerper Lerper { get; } = new();

    /// <inheritdoc />
    public Func<float, float> EasingFunction { get; set; }
    
    /// <summary>
    /// Optional opacity value that can also be animated via <see cref="LerpOpacity"/>. Consumers may use this in their draw logic.
    /// </summary>
    public float Opacity { get; set; } = 1.0f;
    
    /// <inheritdoc />
    public void LerpAttribute<T>(Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time)
    {
        Lerper.AddTween(get, set, to, time, EasingFunction, interpolate);
    }

    /// <summary>
    /// Starts a tween for local position with optional overriding easing parameters.
    /// </summary>
    public void LerpLocalPos(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Pos, v => Local.Pos = v, to, time, EasingFunction);
    }
    
    /// <summary>
    /// Starts a tween for local scale.
    /// </summary>
    public void LerpLocalScale(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Scale, v => Local.Scale = v, to, time, EasingFunction);
    }

    /// <summary>
    /// Starts a tween for local rotation (radians).
    /// </summary>
    public void LerpLocalRotation(float to, double time)
    {
        Lerper.LerpFloat(() => Local.Rotation, v => Local.Rotation = v, to, time, EasingFunction);
    }

    /// <summary>
    /// Starts a tween for the node's opacity field.
    /// </summary>
    public void LerpOpacity(float to, double time)
    {
        Lerper.LerpFloat(() => Opacity, f => Opacity = f, to, time, EasingFunction);
    }

    /// <summary>
    /// Updates active tweens and then normal update logic.
    /// </summary>
    protected override void OnUpdate(GameTime gameTime)
    {
        Lerper.Update(gameTime);
        base.OnUpdate(gameTime);
    }
}