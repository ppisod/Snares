using System.Collections.Generic;
using ppilib.Input;
using ppilib.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Snares.Game.Screens;

public class Screen
{
    private enum Context
    {
        None
    }
    
    public Dictionary<string, List<INode>> NodeGroups = new();

    private readonly MouseController _mouse;
    private readonly KeyboardController _keyboard;

    private readonly Microsoft.Xna.Framework.Game _game;

    private Context _ctx;
    private ScreenState _state;

    public Screen
        (Microsoft.Xna.Framework.Game game, 
            INode parent, 
            MouseController mouse, 
            KeyboardController keyboard)
    {
        _mouse = mouse;
        _keyboard = keyboard;
        _game = game;
        
        _ctx = Context.None;
        _game = game;
        _state = ScreenState.Off;

        _mouse.LeftMouseDown += MouseDown;
        _mouse.LeftMouseUp += MouseUp;
        _mouse.Hover += MouseMove;
        
        
        
        Initialize();
    }

    protected virtual void Initialize()
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
    
    

}