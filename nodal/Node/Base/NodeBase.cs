#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Types;
using ppilib.Types.Struct;

namespace ppilib.Node.Base;

public class NodeBase : INode
{
    public NodeBase (string name, INode? parent)
    {
        Name = name;
        Id = new NodeId(Guid.CreateVersion7());
        Children = new List<INode>();
        Parent = parent;
        if (parent == null)
        {
            // this node is root, it doesn't have a parent, logic here accordingly.
        }
    }
    
    public string Name { get; }
    public NodeId Id { get; }
    public INode? Parent { get; }
    public IReadOnlyList<INode> Children { get; }
    public bool UpdateActive { get; set; }
    public bool DrawActive { get; set; }
    public void AddChild(INode child)
    {
        if (child == this) throw new InvalidOperationException("Cannot add myself as a child!");
        if (Children.Contains(child)) return;
        if (child.Parent is NodeBase anotherParent) anotherParent.RemoveChild(child);
        
    }

    public void RemoveChild(INode child)
    {
        throw new System.NotImplementedException();
    }

    public void Reparent(INode newParent, ReparentMode mode = ReparentMode.PreserveLocal)
    {
        throw new System.NotImplementedException();
    }

    public void Update(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        throw new System.NotImplementedException();
    }

    public INode GetChild(string name)
    {
        throw new System.NotImplementedException();
    }
}