using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Utility.MovingThings.Interfaces;

namespace Snares.Game.Screens.Effects;

/// <summary>
/// Utility to reduce repeated code for changing ContinuousTween targets based on mouse states.
/// You provide a set of items and a predicate indicating whether the mouse is "over/active" for that item,
/// plus the targets for rest/hover/pressed states.
/// </summary>
public static class PointerContinuousTweener
{
    public static void Apply<TItem, TValue>(
        IEnumerable<TItem> items,
        Func<TItem, bool> isPointerOver,
        IContinuousTweenSelector<TItem, TValue> tweenSelector,
        TValue rest,
        TValue hover,
        TValue pressed,
        MouseState mouse)
    {
        var isDown = mouse.LeftButton == ButtonState.Pressed;
        foreach (var item in items)
        {
            var tween = tweenSelector.Select(item);
            if (tween == null) continue;
            if (isPointerOver(item))
            {
                tween.Target = isDown ? pressed : hover;
            }
            else
            {
                tween.Target = rest;
            }
        }
    }
}

/// <summary>
/// Provides a ContinuousTween for a given item. Abstracted so caller can pick which tween (Scale, Pos, Opacity, etc.).
/// </summary>
public interface IContinuousTweenSelector<in TItem, TValue>
{
    IContinuousTween<TValue>? Select(TItem item);
}
