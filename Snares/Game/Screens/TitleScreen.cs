using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppilib.Input;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace Snares.Game.Screens;

public class TitleScreen
{
    public List<INode> Nodes { get; private set; } = [];
    private readonly INode _parent;
    private readonly MouseController _mouse;
    private readonly GraphicsDevice _g;
    public ScreenState State;

    private void AddNode (INode node, INode theirParent)
    {
        if (theirParent == null) throw new InvalidOperationException("parent node is currently null!");
        Nodes.Add(node);
        theirParent.AddChild(node);
    }

    public TitleScreen (GraphicsDevice gD, INode parent, SpriteFont defaultFont, MouseController mouse)
    {
        _g = gD;
        _mouse = mouse;
        _parent = parent;
        var nodeConfig = new NodeConfig(null, _g, true, true, true, true, true);
        nodeConfig
            .SetParent(parent)
            .SetLerpMethod(EasingTypes.Quad.EaseOut)
            .SetColor(Color.Black)
            .SetFont(defaultFont);
        var title = new ContinuousTextFrame(
            nodeConfig
                .SetPos(new Vector2(0, 0))
                .SetScale(new Vector2(1, 0.2f))
                .SetName("Title")
                .SetText("snares")
        ); // new LocalTransform(new Vector2(0, 0), new Vector2(0.2f, 1), 0f), "snares","Title"
        var game = new ContinuousTextFrame(
            nodeConfig
                .SetPos(new Vector2(0, 0.2f))
                .SetScale(new Vector2(1, 0.1f))
                .SetName("Game")
                .SetText("play")
        ); // new LocalTransform(new Vector2(0, 0.2f), new Vector2(0.1f, 1), 0f), "play","Game"
        var quit = new ContinuousTextFrame(
            nodeConfig
                .SetPos(new Vector2(0, 0.3f))
                .SetName("Quit")
                .SetText("quit")
        );
        // "Quit"

        State = ScreenState.Loading;
        
        AddNode(title, parent);
        AddNode(game, parent);
        AddNode(quit, parent);

        mouse.Hover += MouseHover;
    }

    public void Update (GameTime gT)
    {
        if (State == ScreenState.Off)
        {
            return;
        }
        // general code here, for example, update on the frames
        // TODO: why is the update(gT) function not already overriden in their respective classes??
        foreach (var node in Nodes)
        {
            switch (node)
            {
                case ContinuousNodeBase contNode:
                    contNode.Update(gT);
                    break;
                case Frame frame:
                    frame.Update(gT);
                    break;
            }
        }
        
        switch (State)
        {
            case ScreenState.Loading:
                LoadSequence(gT);
                break;
            case ScreenState.On:
                OnSequence(gT);
                break;
            case ScreenState.Unloading:
                UnloadSequence(gT);
                break;
        }
    }
    
    // we can make it in the BaseScreen class that these functions are overloadable

    public void LoadSequence (GameTime _)
    {
        foreach (var node in Nodes)
        {
            // this code is repeated
            if (node is not ContinuousTextFrame cNode) continue;
            cNode.OpacityTween.Target = 1f;
        }
    }

    public void OnSequence (GameTime gT)
    {
        
    }

    public void UnloadSequence (GameTime _)
    {
        foreach (var node in Nodes)
        {
            // this code is repeated
            if (node is not ContinuousTextFrame cNode) continue;
            cNode.OpacityTween.Target = 0f;
        }
    }
    
    private void MouseHover (MouseState state)
    {
        foreach (var node in Nodes)
        {
            if (node is not ContinuousTextFrame cNode) continue;
            cNode.Scale.Target = cNode.GetRect().Contains(state.Position) ? new Vector2(1f, 0.2f) : new Vector2(1f, 0.1f);
        }
    }
}