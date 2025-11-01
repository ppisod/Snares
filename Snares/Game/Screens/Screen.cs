using System.Collections.Generic;
using System.Linq;
using ppilib.Input;
using ppilib.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Snares.Game.Screens;

public class Screen<T>
{

    protected Dictionary<string, List<INode>> NodeGroups = new();

    private readonly MouseController _mouse;
    private readonly KeyboardController _keyboard;

    private readonly Game1 _game;

    private readonly INode _parent;

    protected T Context;
    protected ScreenState State;

    public Screen
        (Game1 game, 
            INode parent, 
            MouseController mouse, 
            KeyboardController keyboard)
    {
        _mouse = mouse;
        _keyboard = keyboard;
        _game = game;
        
        _parent = parent;
        
        _game = game;
        State = ScreenState.Off;

        _mouse.LeftMouseDown += MouseDown;
        _mouse.LeftMouseUp += MouseUp;
        _mouse.Hover += MouseMove;

        _keyboard.KeyDown += KeyDown;
        _keyboard.KeyUp += KeyUp;
    }

    public void Update(GameTime gT)
    {
        if (State == ScreenState.Off) return;
        switch (State)
        {
            case ScreenState.Loading: LoadSequence(gT); break;
            case ScreenState.On: OnSequence(gT); break;
            case ScreenState.Unloading: UnloadSequence(gT); break;
        }

        foreach (var node in NodeGroups.Values.SelectMany(groupNode => groupNode))
        {
            node.Update(gT); 
            // isin't the rootnode already going to update
            // this node (given that the parenting is done correctly?)
        }
    }

    public void Load ()
    {
        State = ScreenState.Loading;
        Initialize();
    }
    
    /// <summary>
    /// The initialize method is overrideable.
    /// Create your nodes and add them to NodeGroups.
    /// </summary>
    protected virtual void Initialize()
    {
        
    }

    protected virtual void LoadSequence(GameTime gT)
    {
        
    }

    protected virtual void OnSequence(GameTime gT)
    {
        
    }

    protected virtual void UnloadSequence(GameTime gT)
    {
        
    }

    protected virtual void MouseDown (MouseState state)
    {
        
    }

    protected virtual void MouseUp (MouseState state)
    {
        
    }

    protected virtual void MouseMove (MouseState state)
    {
        
    }

    protected virtual void KeyDown (Keys key)
    {
        
    }

    protected virtual void KeyUp (Keys key)
    {
        
    }

}