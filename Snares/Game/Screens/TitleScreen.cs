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

    private readonly ContinuousTextFrame _title;
    private readonly ContinuousTextFrame _game;
    private readonly ContinuousTextFrame _quit;

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
            .SetLerpMethod(EasingTypes.Expo.EaseOut)
            .SetColor(Color.Black * 0.9f)
            .SetFont(defaultFont)
            .SetOpacity(0f);

        nodeConfig
            .SetPos(new Vector2(0, 0.01f))
            .SetScale(new Vector2(1f, 0.1f))
            .SetName("Title")
            .SetText("snares");
        var title = new ContinuousTextFrame(
            nodeConfig
        );
        _title = title;

        nodeConfig
            .SetPos(new Vector2(0, 0.1f))
            .SetScale(new Vector2(1, 0.05f))
            .SetColor(Color.Black * 0.5f)
            .SetName("Game")
            .SetText("play");
        var game = new ContinuousTextFrame(
            nodeConfig
        );
        _game = game;

        nodeConfig
            .SetPos(new Vector2(0, 0.15f))
            .SetColor(Color.Black * 0.5f)
            .SetName("Quit")
            .SetText("quit");
        var quit = new ContinuousTextFrame(
            nodeConfig
        );
        _quit = quit;
        // "Quit"

        State = ScreenState.Loading;
        
        AddNode(title, parent);
        AddNode(game, parent);
        AddNode(quit, parent);

        mouse.Hover += MouseHover;
        mouse.LeftMouseDown += MouseDown;
        mouse.LeftMouseUp += MouseUp;
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
        var allFadedIn = true;
        const float epsilon = 0.001f;
        foreach (var node in Nodes)
        {
            if (node is not ContinuousTextFrame cNode) continue;
            cNode.OpacityTween.Target = 1f;
            if (cNode.OpacityTween.Finished) allFadedIn = false;
        }

        if (allFadedIn)
        {
            State = ScreenState.On;
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
        if (State != ScreenState.On) return;
        _title.Scale.Target = _title.GetRect().Contains(state.Position) ? new Vector2(1f, 0.105f) : new Vector2(1f, 0.1f);
        _game.Scale.Target = _game.GetRect().Contains(state.Position) ? new Vector2(1f, 0.055f) : new Vector2(1f, 0.05f);
        _quit.Scale.Target = _quit.GetRect().Contains(state.Position) ? new Vector2(1f, 0.055f) : new Vector2(1f, 0.05f);
    }

    private void MouseDown (MouseState state)
    {
        if (State != ScreenState.On) return;
        _title.Scale.Target = _title.GetRect().Contains(state.Position) ? new Vector2(1f, 0.095f) : new Vector2(1f, 0.1f);
        _game.Scale.Target = _game.GetRect().Contains(state.Position) ? new Vector2(1f, 0.045f) : new Vector2(1f, 0.05f);
        _quit.Scale.Target = _quit.GetRect().Contains(state.Position) ? new Vector2(1f, 0.045f) : new Vector2(1f, 0.05f);
    }
    
    private void MouseUp (MouseState state)
    {
        if (State != ScreenState.On) return;
        State = ScreenState.Unloading;
        
        
    }
}