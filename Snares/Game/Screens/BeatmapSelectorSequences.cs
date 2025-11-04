using System;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework;
using ppilib.Node.Custom;
using ppilib.Utility.MovingThings;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Snares.Game.Screens;

public partial class BeatmapSelectorScreen
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
            var buttons = NodeGroups["Buttons"].Cast<ContinuousTextFrame>().ToArray();
            foreach (var n in buttons)
            {
                // idkkkkk brooooo this is so buggyyyy
                _loadTimeline.TweenAt(0, n.Pos, new Vector2(-0.025f, 0f));
                _loadTimeline.TweenAt(0, n.OpacityTween, 1f);
            }
            
            _loadTimeline.Start();
            _loadStarted = true;
        }
        _loadTimeline.Update(gT);
        if (_loadTimeline.IsEmpty)
        {
            State = ScreenState.On;
            _loadStarted = false;
        }
        base.LoadSequence(gT);
    }

    protected override void UnloadSequence(GameTime gT)
    {
        if (!_unloadStarted)
        {
            _unloadTimeline.Reset();
            var buttons = NodeGroups["Buttons"].Cast<ContinuousTextFrame>().ToArray();
            foreach (var n in buttons)
            {
                _unloadTimeline.TweenAt(0, n.Pos, new Vector2(-3, 0.01f));
                _unloadTimeline.TweenAt(0, n.OpacityTween, 0f);
            }
            
            _unloadTimeline.Start();
            _unloadStarted = true;
        }
        
        _unloadTimeline.Update(gT);
        if (!_unloadTimeline.IsEmpty) return;

        switch (Context)
        {
            case BeatmapSelectorContext.BackToTitle:
                Game.TitleScreen.Load();
                break;
            case BeatmapSelectorContext.None:
                break;
            case BeatmapSelectorContext.Game:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Unload();
        State = ScreenState.Off;
        _unloadStarted = false;
        
        base.UnloadSequence(gT);
    }
}