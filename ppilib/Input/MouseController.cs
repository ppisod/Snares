using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ppilib.Input;

public class MouseController
{
    private MouseState _lastState = Mouse.GetState();

    public MouseState GetState()
    {
        return Mouse.GetState();
    }

    public event Action<MouseState> Hover;
    public event Action<MouseState> LeftMouseDown;
    public event Action<MouseState> LeftMouseUp;
    public event Action<MouseState> RightMouseDown;
    public event Action<MouseState> RightMouseUp;
    public event Action<MouseState, Point> LeftDrag;
    public event Action<MouseState, Point> RightDrag;
    public event Action<MouseState, int> Scroll;

    public void Update()
    {
        var left = _lastState.LeftButton;
        var right = _lastState.RightButton;
        var middle = _lastState.MiddleButton;
        var scroll = _lastState.ScrollWheelValue;
        var pos = _lastState.Position;
        
        var newLeft = Mouse.GetState().LeftButton;
        var newRight = Mouse.GetState().RightButton;
        var newMiddle = Mouse.GetState().MiddleButton;
        var newScroll = Mouse.GetState().ScrollWheelValue;
        var newPos = Mouse.GetState().Position;
        
        switch (left)
        {
            case ButtonState.Released when newLeft == ButtonState.Pressed:
                LeftMouseDown?.Invoke(Mouse.GetState());
                break;
            case ButtonState.Pressed when newLeft == ButtonState.Released:
                LeftMouseUp?.Invoke(Mouse.GetState());
                break;
            case ButtonState.Pressed when newLeft == ButtonState.Pressed:
                if (pos != newPos)
                {
                    var change = newPos - pos;
                    LeftDrag?.Invoke(Mouse.GetState(), change);
                }
                break;
        }

        switch (right)
        {
            case ButtonState.Released when newRight == ButtonState.Pressed:
                RightMouseDown?.Invoke(Mouse.GetState());
                break;
            case ButtonState.Pressed when newRight == ButtonState.Released:
                RightMouseUp?.Invoke(Mouse.GetState());
                break;
            case ButtonState.Pressed when newRight == ButtonState.Pressed:
                if (pos != newPos)
                {
                    var change = newPos - pos;
                    RightDrag?.Invoke(Mouse.GetState(), change);
                }
                break;
        }

        if (newLeft == ButtonState.Released && newRight == ButtonState.Released && pos != newPos)
        {
            Hover?.Invoke(Mouse.GetState());
        }

        if (newScroll != scroll)
        {
            Scroll?.Invoke(Mouse.GetState(), newScroll - scroll);
        }
        
        
        _lastState = Mouse.GetState();
    }
}