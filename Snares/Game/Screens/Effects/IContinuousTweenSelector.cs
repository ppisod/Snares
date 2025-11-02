#nullable enable
using ppilib.Utility.MovingThings.Interfaces;

namespace Snares.Game.Screens.Effects;

/// <summary>
/// Provides a ContinuousTween for a given item. Abstracted so caller can pick which tween (Scale, Pos, Opacity, etc.).
/// </summary>
public interface IContinuousTweenSelector<in TItem, TValue>
{
    IContinuousTween<TValue>? Select(TItem item);
}