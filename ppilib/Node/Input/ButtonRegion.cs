using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ppilib.Input;
using ppilib.Input.Definitions;
using ppilib.Input.Interfaces;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;

namespace ppilib.Node.Custom;

public class ButtonRegion : TransformNodeBase, IInputRegion
{
    public ButtonRegion(string name, INode parent, LocalTransform region) : base(name,
        parent, region)
    {
        
    }

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