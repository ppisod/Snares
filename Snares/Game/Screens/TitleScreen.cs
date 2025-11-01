using Microsoft.Xna.Framework;
using ppilib.Input;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace Snares.Game.Screens;

public class TitleScreen(
    Game1 game,
    INode parent,
    MouseController mouse,
    KeyboardController keyboard)
    : Screen(game, parent, mouse, keyboard)
{
    
    private readonly Game1 _gameInstance = game;
    private readonly INode _parent1 = parent;

    protected override void Initialize()
    {
        // make nodes here and add them to nodegroups.
        
        var nodeConfig = new NodeConfig(null, _gameInstance.GraphicsDevice, true, true, true, true, true);

        nodeConfig
            .SetParent(_parent1)
            .SetLerpMethod(EasingTypes.Quad.EaseOut)
            .SetColor(Color.Black)
            .SetFont(_gameInstance.Helvetica)
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
}