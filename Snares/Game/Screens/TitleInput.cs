using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Node.Custom;
using Snares.Game.Screens.Effects;

namespace Snares.Game.Screens;

public partial class TitleScreen
{
    private class ScaleSelector : IContinuousTweenSelector<ContinuousTextFrame, Vector2> // move these to other files?
    {
        public ppilib.Utility.MovingThings.Interfaces.IContinuousTween<Vector2>? Select(ContinuousTextFrame item) => item.Scale;
    }
    
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