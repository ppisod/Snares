using System;
using System.Linq;
using Microsoft.Xna.Framework;
using ppilib.Node.Custom;
using ppilib.Utility.MovingThings;

namespace Snares.Game.Screens;

public partial class TitleScreen
{
    private readonly TweenTimeline _loadTimeline = new();
    private readonly TweenTimeline _unloadTimeline = new();
    private bool _loadStarted;
    private bool _unloadStarted;
    
    protected override void LoadSequence(GameTime gT)
    {
        if (!_loadStarted)
        {
            _loadTimeline.Reset();
            // Stagger: Title at 0ms, then each body item every 150ms.
            var title = NodeGroups["Title"].Cast<ContinuousTextFrame>().ToArray();
            var body = NodeGroups["Body"].Cast<ContinuousTextFrame>().ToArray();
            foreach (var t in title)
                _loadTimeline.TweenAt(0, t.OpacityTween, 1f);
            for (int i = 0; i < body.Length; i++)
                _loadTimeline.TweenAt(150 * (i+1), body[i].OpacityTween, 1f);
            _loadTimeline.Start();
            _loadStarted = true;
        }
        _loadTimeline.Update(gT);
        // consider complete when timeline empty and all opacities near 1
        bool allVisible = NodeGroups.Values
            .SelectMany(x => x)
            .Cast<ContinuousTextFrame>()
            .All(n => Math.Abs(n.Opacity - 1f) < 0.02f);
        if (_loadTimeline.IsEmpty && allVisible)
        {
            State = ScreenState.On;
            _loadStarted = false; // ready for next time
        }
    }

    protected override void UnloadSequence(GameTime gT)
    {
        if (!_unloadStarted)
        {
            _unloadTimeline.Reset();
            var title = NodeGroups["Title"].Cast<ContinuousTextFrame>().ToArray();
            var body = NodeGroups["Body"].Cast<ContinuousTextFrame>().ToArray();
            foreach (var t in title)
                _unloadTimeline.TweenAt(0, t.OpacityTween, 0f);
            for (int i = 0; i < body.Length; i++)
                _unloadTimeline.TweenAt(100 * (i + 1), body[i].OpacityTween, 0f);
            _unloadTimeline.Start();
            _unloadStarted = true;
        }
        _unloadTimeline.Update(gT);
        bool allHidden = NodeGroups.Values
            .SelectMany(x => x)
            .Cast<ContinuousTextFrame>()
            .All(n => n.Opacity <= 0.02f);
        if (!(_unloadTimeline.IsEmpty && allHidden)) return;

        switch (Context)
        {
            case TitleScreenContext.ActionQuit:
                Game.Exit();
                break;
            case TitleScreenContext.ActionGame:
                // TODO: navigate to next screen
                Game.BeatmapSelectorScreen.Load();
                break;
        }
        Unload();
        State = ScreenState.Off;
        _unloadStarted = false;
    }
}