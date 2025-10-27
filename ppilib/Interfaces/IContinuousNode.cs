using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Interfaces;

namespace ppilib.Interfaces;

/// <summary>
/// Interface for a transform node that uses continuous tweens to smoothly approach target
/// values for its local transform components (position, scale, rotation).
/// </summary>
public interface IContinuousNode : ITransformNode
{
    /// <summary>
    /// Continuous controller for the local position.
    /// </summary>
    IContinuousTween<Vector2> Pos { get; }

    /// <summary>
    /// Continuous controller for the local scale.
    /// </summary>
    IContinuousTween<Vector2> Scale { get; }

    /// <summary>
    /// Continuous controller for the local rotation (in radians).
    /// </summary>
    IContinuousTween<float> Rot { get; }
}