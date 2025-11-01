using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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

public class TitleScreen (
    Game1 game,
    INode parent,
    MouseController mouse,
    KeyboardController keyboard)
    : Screen<TitleScreenContext>(game, parent, mouse, keyboard)
{
    
    private readonly Game1 _gameInstance = game;
    private readonly INode _parent1 = parent;

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
        // Can this code be abstracted to a Transition class or FadeableNodes?
        foreach (var node in NodeGroups["Title"].Cast<ContinuousTextFrame>())
        {
            node.OpacityTween.Target = 1f;
        }
        
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            node.OpacityTween.Target = 1f;
        }
        
        // Check if all nodes have reached target opacity
        var allFinished = true;
        foreach (var nodeCollection in NodeGroups.Values)
        {
            foreach (var node in nodeCollection.Cast<ContinuousTextFrame>())
            {
                if (!node.OpacityTween.Finished) allFinished = false;
            } // [Finished] is known to fail
        }

        if (allFinished) State = ScreenState.On;
    }

    protected override void UnloadSequence(GameTime gT)
    {
        foreach (var node in NodeGroups["Title"].Cast<ContinuousTextFrame>())
        {
            node.OpacityTween.Target = 0f;
        }
        
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            node.OpacityTween.Target = 0f;
        }
        
        // Check if all nodes have reached target opacity
        var allFinished = true;
        foreach (var nodeCollection in NodeGroups.Values)
        {
            foreach (var node in nodeCollection.Cast<ContinuousTextFrame>())
            {
                Console.WriteLine($"{node.Name} state: {node.OpacityTween.Finished}");
                if (!node.OpacityTween.Finished) allFinished = false;
            } // [Finished] is known to fail
        }

        if (!allFinished) return;

        switch (Context) // this code is never reached? AllFinished is always false?
        {
            case TitleScreenContext.ActionQuit:
                Console.WriteLine("Quitting!");
                _gameInstance.Exit();
                break;
            case TitleScreenContext.ActionGame:
                Console.WriteLine("-> BeatmapSelectionScreen");
                break;
        }

        // EVERY screen implementation should call Unload() in UnloadSequence.
        Console.WriteLine("Unloading!");
        Unload();
        
        State = ScreenState.Off;
    }

    protected override void MouseMove(MouseState state)
    {
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        
        // can effects like these be abstracted into some Effects class
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            // SCALE UP SLIGHTLY
            node.Scale.Target = node.GetRect().Contains(state.Position)
                ? new Vector2(1f, 0.055f)
                : new Vector2(1f, 0.05f);
        }

        foreach (var node in NodeGroups["Title"].Cast<ContinuousTextFrame>())
        {
            node.Scale.Target  = node.GetRect().Contains(state.Position)
                ? new Vector2(1f, 0.105f) 
                : new Vector2(1f, 0.1f); 
        }
    }

    protected override void MouseDown(MouseState state)
    {
        // we don't actually do anything here, it's just for looks
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            // SCALE DOWN SLIGHTLY
            node.Scale.Target = node.GetRect().Contains(state.Position)
                ? new Vector2(1f, 0.045f)
                : new Vector2(1f, 0.05f);
        }
        base.MouseDown(state);
    }

    protected override void MouseUp(MouseState state)
    {
        if (State is not (ScreenState.On or ScreenState.Loading)) return;
        
        // we actually have to reference specific nodes here
        foreach (var node in NodeGroups["Body"].Cast<ContinuousTextFrame>())
        {
            if (!node.GetRect().Contains(state.Position))
            {
                return;
            }

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
        
        base.MouseUp(state);
    }
}