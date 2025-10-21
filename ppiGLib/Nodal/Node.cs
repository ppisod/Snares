using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppiGLib.Nodal;

public abstract class Node
{
    public Node Parent { get; set; }
    public List<Node> Children { get; set; } = new List<Node>();
    public string Name { get; set; }
    
    public bool UpdateActive { get; set; } = true;
    public bool DrawActive { get; set; } = true;
    
    public UniqueId Identifier { get; private set; }

    protected Node (string name)
    {
        Name = name;
    }

    public void AddNodeAsChild(Node child)
    {
        Children.Add(child);
    }

    public void RemoveNodeFromChildren(Node child)
    {
        if (!Children.Contains(child)) return;
        
        Children.Remove(child);
        child.Parent = null;
    }

    public void Update (GameTime gameTime)
    {
        if (!UpdateActive) return;
        CustomUpdateLogic(gameTime);
        foreach (var t in Children)
        {
            t.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!DrawActive) return;
        CustomDrawLogic(spriteBatch);
        foreach (var t in Children)
        {
            t.Draw(spriteBatch);
        }
    }

    public abstract void CustomUpdateLogic (GameTime gameTime);
    public abstract void CustomDrawLogic (SpriteBatch spriteBatch);
    
    
}