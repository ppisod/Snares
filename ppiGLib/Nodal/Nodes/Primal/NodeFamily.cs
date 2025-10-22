using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Primal;

public class NodeFamily : Node
{
    
    public NodeFamily (GraphicsDevice gDev, string name, List<Node> nodes) : base(gDev, name, true)
    {
        Children = nodes;
        // disable everything at first.
        foreach (var child in Children)
        {
            child.DrawActive = false;
            child.UpdateActive = false;
        }
    }

    public void Enable (Node node)
    {
        if (!Children.Contains(node)) return;
        node.UpdateActive = true; node.DrawActive = true;
    }

    public void Enable (string name)
    {
        foreach (var child in Children.Where(child => child.Name == name))
        {
            child.UpdateActive = true;
            child.DrawActive = true;
        }
    }

    public void Disable (Node node)
    {
        if (!Children.Contains(node)) return;
        node.UpdateActive = false;
        node.DrawActive = false;
    }
    
    public void Disable (string name)
    {
        foreach (var child in Children.Where(child => child.Name == name))
        {
            child.UpdateActive = false;
            child.DrawActive = false;
        }
    }
    
    protected override void CustomUpdateLogic(GameTime gameTime) {}

    protected override void CustomDrawLogic(SpriteBatch spriteBatch) {}
}