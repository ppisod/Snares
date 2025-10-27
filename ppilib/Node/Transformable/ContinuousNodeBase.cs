using System;
using Microsoft.Xna.Framework;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;

namespace ppilib.Node.Transformable;

/// <summary>
/// Transform node that continuously eases its local transform properties toward target values.
/// Useful for smooth following/approach behaviors rather than fixed-duration tweens.
/// </summary>
public class ContinuousNodeBase : TransformNodeBase
{
    /// <summary>Continuous tween controller for the local position.</summary>
    public ContinuousTween<Vector2> Pos { get; }
    /// <summary>Continuous tween controller for the local scale.</summary>
    public ContinuousTween<Vector2> Scale { get; }
    /// <summary>Continuous tween controller for the local rotation (radians).</summary>
    public ContinuousTween<float> Rot { get; }

    /// <summary>
    /// Creates a ContinuousNodeBase with provided ease function applied to approach progress.
    /// </summary>
    /// <param name="name">Node name.</param>
    /// <param name="parent">Parent node.</param>
    /// <param name="wantedTransform">Initial local transform.</param>
    /// <param name="easeF">Easing function mapping progress [0..1] to [0..1].</param>
    public ContinuousNodeBase (string name, INode parent, LocalTransform wantedTransform, Func<float, float> easeF) : base(name, parent, wantedTransform)
    {
        // Default rates chosen empirically; you can expose them if needed.
        Pos = new ContinuousTween<Vector2>(() => Local.Pos, v => Local.Pos = v, Vector2.Lerp, easeF, 5f);
        Scale = new ContinuousTween<Vector2>(() => Local.Scale, v => Local.Scale = v, Vector2.Lerp, easeF, 5f);
        Rot = new ContinuousTween<float>(() => Local.Rotation, v => Local.Rotation = v, (f, f1, t) => f + (f1 - f) * t, easeF, 6f);
    }

    /// <inheritdoc />
    protected override void OnUpdate(GameTime gameTime)
    {
        Pos.Update(gameTime);
        Scale.Update(gameTime);
        Rot.Update(gameTime);
        base.OnUpdate(gameTime);
    }
}