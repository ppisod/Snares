using Microsoft.Xna.Framework;
using ppilib.Input;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings.Ease.Definitions;

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
    

    protected override void Initialize()
    {
        Context = TitleScreenContext.None;
        // make nodes here and add them to nodegroups.
        
        var nodeConfig = new NodeConfig(null, Game.GraphicsDevice, true, true, true, true, true);

        nodeConfig
            .SetParent(Parent)
            .SetLerpMethod(EasingTypes.Quad.EaseOut)
            .SetColor(Color.Black)
            .SetFont(Game.Font)
            .SetOpacity(0f);
        
        // NODE :: TITLE
        nodeConfig
            .SetPos(new Vector2(0, 0.01f)).SetScale(new Vector2(1f, 0.1f))
            .SetName("Title")
            .SetText("game");

        var title = new ContinuousTextFrame(nodeConfig);
        Parent.AddChild(title);
        
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
        Parent.AddChild(game);
        
        // NODE :: QUIT
        nodeConfig
            .SetPos(new Vector2(0, 0.15f))
            .SetName("Quit")
            .SetText("quit");
        
        var quit = new ContinuousTextFrame(nodeConfig);
        Parent.AddChild(quit);
        
        // NODEGROUP :: BODY
        NodeGroups["Body"] = [game, quit];
    }
}