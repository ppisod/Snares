using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ppilib.Utility.MovingThings.Interfaces;

namespace ppilib.Utility.MovingThings;

/// <summary>
/// Action queuer for tweens
/// </summary>
public class TweenTimeline
{
    private readonly List<TimelineEvent> _events = new();
    private int _cursor;
    private double _elapsedMs;
    private bool _started;

    private class TimelineEvent
    {
        public double DueMs;
        public Action Action = null!;
    }

    /// <summary>
    /// Clears existing events and resets time and state.
    /// </summary>
    public void Reset()
    {
        _events.Clear();
        _cursor = 0;
        _elapsedMs = 0;
        _started = false;
    }

    /// <summary>
    /// Begin accumulating time. Subsequent Update calls will process events.
    /// </summary>
    public void Start()
    {
        _started = true;
    }

    /// <summary>
    /// Returns true if there are no more pending events.
    /// </summary>
    public bool IsEmpty => _cursor >= _events.Count;

    /// <summary>
    /// Add an action to trigger at a given millisecond offset.
    /// </summary>
    public TweenTimeline ActionAt(double ms, Action action)
    {
        _events.Add(new TimelineEvent { DueMs = ms, Action = action });
        _events.Sort((a, b) => a.DueMs.CompareTo(b.DueMs));
        return this;
    }

    /// <summary>
    /// Add an event to set a tween's target at a given millisecond offset.
    /// </summary>
    public TweenTimeline TweenAt<T>(double ms, IContinuousTween<T> tween, T to)
    {
        return ActionAt(ms, () => tween.Target = to);
    }

    /// <summary>
    /// Advance time and invoke due events.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!_started || IsEmpty) return;
        _elapsedMs += gameTime.ElapsedGameTime.TotalMilliseconds;
        while (_cursor < _events.Count && _events[_cursor].DueMs <= _elapsedMs)
        {
            try { _events[_cursor].Action(); }
            catch { /* swallow to not break sequencing; consider logging */ }
            _cursor++;
        }
    }
}
