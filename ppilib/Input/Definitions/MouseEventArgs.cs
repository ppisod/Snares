using System;
using Microsoft.Xna.Framework.Input;

namespace ppilib.Input.Definitions;

public class MouseEventArgs(MouseState state) : EventArgs
{
    public MouseState State { get; } = state;
    public bool Handled { get; set; }
}