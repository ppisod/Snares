using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Node.Custom;
using Snares.Game.Screens.Effects;

namespace Snares.Game.Screens;

public partial class BeatmapSelectorScreen
{
    private class ScaleSelector : IContinuousTweenSelector<ContinuousTextFrame, Vector2> // move these to other files?
    {
        public ppilib.Utility.MovingThings.Interfaces.IContinuousTween<Vector2>? Select(ContinuousTextFrame item) => item.Scale;
    }

    private void UpdatePointer(MouseState state)
    {
        var buttons = NodeGroups.TryGetValue("Buttons", out var buttonNodes)
            ? buttonNodes.Cast<ContinuousTextFrame>().ToArray()
            : [];
        
        PointerContinuousTweener.Apply(
            buttons,
            n => n.GetRect().Contains(state.Position),
            new ScaleSelector(),
            rest: new Vector2(0.2f, 0.1f),
            hover: new Vector2(0.22f, 0.12f),
            pressed: new Vector2(0.19f, 0.09f),
            mouse: state
        );
    }
}