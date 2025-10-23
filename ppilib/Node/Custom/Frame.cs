using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;

namespace ppilib.Node.Custom;

public class Frame
                        (string name, INode parent, LocalTransform wantedTransform)
    : TransformNodeBase (name, parent, wantedTransform) 
{
    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        
    }
}