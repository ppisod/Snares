using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Input;
using ppilib.Input.Definitions;
using ppilib.Input.Interfaces;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;

namespace ppilib.Node.Custom;

public class ButtonRegion(NodeConfig n) : TransformNodeBase(n), IInputRegion
{
    public bool HitTest(Point point)
    {
        var pos = World.Position.Result;
        var size = World.Scale.Result;
        var rect = new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y);
        return rect.Contains(point);
    }

    public void OnLeftDown(MouseEventArgs a)
    {
        LeftDown(a);
        a.Handled = true;
    }

    public void OnLeftUp(MouseEventArgs a)
    {
        LeftUp(a);
        a.Handled = true;
    }

    protected virtual void LeftDown(MouseEventArgs a)
    {
        
    }

    protected virtual void LeftUp(MouseEventArgs a)
    {
        
    }
}