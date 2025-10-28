using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ppilib.Erroring;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Interfaces;

namespace ppilib.Node.Transformable;

/// <summary>
/// Transform node that continuously eases its local transform properties toward target values.
/// Useful for smooth following/approach behaviors rather than fixed-duration tweens.
/// </summary>
public class ContinuousNodeBase : TransformNodeBase, IContinuousNode
{
    /// <summary>Continuous tween controller for the local position.</summary>
    public IContinuousTween<Vector2> Pos { get; }
    /// <summary>Continuous tween controller for the local scale.</summary>
    public IContinuousTween<Vector2> Scale { get; }
    /// <summary>Continuous tween controller for the local rotation (radians).</summary>
    public IContinuousTween<float> Rot { get; }

    /// <summary>
    /// Creates a ContinuousNodeBase with provided ease function applied to approach progress.
    /// </summary>
    /// <param name="n">Node config.</param>
    protected ContinuousNodeBase (NodeConfig n) : base(n)
    {
        // Default rates chosen empirically; you can expose them if needed.
        Pos = new ContinuousTween<Vector2>(() => Local.Pos, v => Local.Pos = v, Vector2.Lerp, n.LerpMethod, 5f);
        Scale = new ContinuousTween<Vector2>(() => Local.Scale, v => Local.Scale = v, Vector2.Lerp, n.LerpMethod, 5f);
        Rot = new ContinuousTween<float>(() => Local.Rotation, v => Local.Rotation = v, (f, f1, t) => f + (f1 - f) * t, n.LerpMethod, 6f);
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