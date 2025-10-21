using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Primal;

public class NodeFamily : Node
{
    
    public NodeFamily (GraphicsDevice gDev, string name, List<Node> nodes) : base(gDev, name)
    {
        Children = nodes;
        // disable everything at first.
        foreach (var child in Children)
        {
            child.DrawActive = false;
            child.UpdateActive = false;
        }
    }

    protected override void CustomUpdateLogic(GameTime gameTime) {}

    protected override void CustomDrawLogic(SpriteBatch spriteBatch) {}
}