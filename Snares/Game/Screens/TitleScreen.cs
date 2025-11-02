using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Input;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings.Ease.Definitions;
using Snares.Game.Screens.Effects;
using ppilib.Utility.MovingThings;

namespace Snares.Game.Screens;

public enum TitleScreenContext
{
    None, ActionGame, ActionQuit
}

public partial class TitleScreen (
    Game1 game,
    INode parent,
    MouseController mouse,
    KeyboardController keyboard)
    : Screen<TitleScreenContext>(game, parent, mouse, keyboard)
{
    private readonly TweenTimeline _loadTimeline = new();
    private readonly TweenTimeline _unloadTimeline = new();
    private bool _loadStarted;
    private bool _unloadStarted;

    private class ScaleSelector : IContinuousTweenSelector<ContinuousTextFrame, Vector2> // move these to other files?
    {
        public ppilib.Utility.MovingThings.Interfaces.IContinuousTween<Vector2>? Select(ContinuousTextFrame item) => item.Scale;
    }
    
    private readonly Game1 _gameInstance = game;
    private readonly INode _parent1 = parent;

    private void UpdatePointerScale(MouseState state)
    {
        var body = NodeGroups.TryGetValue("Body", out var group) ? group.Cast<ContinuousTextFrame>().ToArray() : [];
        var title = NodeGroups.TryGetValue("Title", out var nodeGroup) ? nodeGroup.Cast<ContinuousTextFrame>().ToArray() : [];
        // body scales
        PointerContinuousTweener.Apply(
            body,
            n => n.GetRect().Contains(state.Position),
            new ScaleSelector(),
            rest: new Vector2(1f, 0.05f),
            hover: new Vector2(1f, 0.055f),
            pressed: new Vector2(1f, 0.049f),
            mouse: state);
        // title scales (slightly larger base)
        PointerContinuousTweener.Apply(
            title,
            n => n.GetRect().Contains(state.Position),
            new ScaleSelector(),
            rest: new Vector2(1f, 0.10f),
            hover: new Vector2(1f, 0.105f),
            pressed: new Vector2(1f, 0.095f),
            mouse: state);
    }

    protected override void Initialize()
    {
        Context = TitleScreenContext.None;
        // make nodes here and add them to nodegroups.
        
        var nodeConfig = new NodeConfig(null, _gameInstance.GraphicsDevice, true, true, true, true, true);

        nodeConfig
            .SetParent(_parent1)
            .SetLerpMethod(EasingTypes.Quad.EaseOut)
            .SetColor(Color.Black)
            .SetFont(_gameInstance.Font)
            .SetOpacity(0f);
        
        // NODE :: TITLE
        nodeConfig
            .SetPos(new Vector2(0, 0.01f)).SetScale(new Vector2(1f, 0.1f))
            .SetName("Title")
            .SetText("game");

        var title = new ContinuousTextFrame(nodeConfig);
        _parent1.AddChild(title);
        
        // NODEGROUP :: TITLE
        NodeGroups["Title"] = [title];

        nodeConfig
            .SetColor(Color.Black * 0.5f)
            .SetScale(new Vector2(1f, 0.05f));
        
        // NODE :: GAME
        nodeConfig
            .SetPos(new Vector2(0, 0.1f))
            .SetName("Game")
            .SetText("play");
        
        var game = new ContinuousTextFrame(nodeConfig);
        _parent1.AddChild(game);
        
        // NODE :: QUIT
        nodeConfig
            .SetPos(new Vector2(0, 0.15f))
            .SetName("Quit")
            .SetText("quit");
        
        var quit = new ContinuousTextFrame(nodeConfig);
        _parent1.AddChild(quit);
        
        // NODEGROUP :: BODY
        NodeGroups["Body"] = [game, quit];
    }

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
                _gameInstance.Exit();
                break;
            case TitleScreenContext.ActionGame:
                // TODO: navigate to next screen
                break;
        }
        Unload();
        State = ScreenState.Off;
        _unloadStarted = false;
    }

    protected override void MouseMove(MouseState state)
    {
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        UpdatePointerScale(state);
    }

    protected override void MouseDown(MouseState state)
    {
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        UpdatePointerScale(state);
        base.MouseDown(state);
    }

    protected override void MouseUp(MouseState state)
    {
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        // determine actions based on which body item was clicked
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            if (!node.GetRect().Contains(state.Position)) continue;
            switch (node.Name)
            {
                case "Game":
                    Context = TitleScreenContext.ActionGame;
                    State = ScreenState.Unloading; // some buttons don't cause unloading, so we have to specify here.
                    break;
                case "Quit":
                    Context = TitleScreenContext.ActionQuit;
                    State = ScreenState.Unloading;
                    break;
            }
        }
        // refresh scale targets after state change / release
        UpdatePointerScale(state);
        base.MouseUp(state);
    }
}