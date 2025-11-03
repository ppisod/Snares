using System.Drawing;
using ppilib.Input;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings.Ease.Definitions;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Snares.Game.Screens;

public enum BeatmapSelectorContext
{
    None, Game, BackToTitle
}

public partial class BeatmapSelectorScreen(Game1 game, INode parent, MouseController mouse, KeyboardController keyboard)
    : Screen<BeatmapSelectorContext>(game, parent, mouse, keyboard)
{
    protected override void Initialize()
    {
        Context = BeatmapSelectorContext.None;
        var nodeConfig = new NodeConfig(null, Game.GraphicsDevice, true, true, true, true, true);

        nodeConfig
            .SetParent(Parent)
            .SetFont(Game.Font)
            .SetLerpMethod(EasingTypes.Quad.EaseOut)
            .SetColor(Color.Black * 0.6f)
            .SetOpacity(0f).SetOpacityLerpRate(0.3f);
        
        // NODE :: Back button
        nodeConfig
            .SetPos(new Vector2(-3, 0f)).SetScale(new Vector2(0.2f, 0.1f))
            .SetName("BackButton")
            .SetText("back");

        var back = new ContinuousTextFrame(nodeConfig);
        Parent.AddChild(back);
        
        // NODEGROUP :: buttons
        NodeGroups["Buttons"] = [back];
    }
    
}