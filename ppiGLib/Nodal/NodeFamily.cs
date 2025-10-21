using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppiGLib.Nodal;

public class NodeFamily : Node
{
    
    public NodeFamily (string name, List<Node> nodes) : base(name)
    {
        Children = nodes;
        // disable everything at first.
        foreach (var child in Children)
        {
            child.DrawActive = false;
            child.UpdateActive = false;
        }
    }

    public override void CustomUpdateLogic(GameTime gameTime) {}

    public override void CustomDrawLogic(SpriteBatch spriteBatch) {}
}