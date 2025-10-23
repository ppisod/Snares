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
        
        _children = [];
        Parent = parent;
        if (parent == null)
        {
            // this node is root, it doesn't have a parent, logic here accordingly.
        }
    }
    
    public string Name { get; }
    public NodeId Id { get; }
    public INode? Parent { get; private set; }
    public IReadOnlyList<INode> Children => _children;
    private readonly List<INode> _children;
    public bool UpdateActive { get; set; }
    public bool DrawActive { get; set; }
    public void AddChild(INode child)
    {
        if (child == this) throw new InvalidOperationException("Cannot add myself as a child!");
        if (Children.Contains(child)) return;
        if (child.Parent is NodeBase anotherParent) anotherParent.RemoveChild(child);
        _children.Add(child);
        if (child is NodeBase n) n.Parent = this;
        else throw new InvalidOperationException("Child is not a NodeBase..");
    }

    public void RemoveChild(INode child)
    {
        if (!Children.Contains(child)) return;
        if (child.Parent != this) throw new InvalidOperationException("Not my child, not my business! make sure that the child's Parent is ME first. There may be a desync or something wrong with the parenting system.");
        _children.Remove(child);
        if (child is NodeBase n) n.Parent = null;
        else throw new InvalidOperationException("Child is not a NodeBase..");
    }

    public void Reparent(INode newParent, ReparentMode mode = ReparentMode.PreserveLocal)
    {
        Parent?.RemoveChild(this);
        newParent.AddChild(this);
    }

    public void Update(GameTime gameTime)
    {
        if (!UpdateActive) return;
        // Custom update logic here, in derived types.
        OnUpdate(gameTime);
        foreach (var t in Children) t.Update(gameTime);
        AfterUpdate(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!DrawActive) return;
        // Custom draw logic here, in derived types.
        OnDraw(spriteBatch);
        foreach (var t in Children) t.Draw(spriteBatch);
        AfterDraw(spriteBatch);
    }

    public INode GetChild(string name)
    {
        foreach (var c in _children.Where(c => c.Name == name)) return c;
        throw new KeyNotFoundException($"Child with name {name} not found.");
    }
    
    protected virtual void OnUpdate(GameTime gameTime) { }
    protected virtual void AfterUpdate(GameTime gameTime) { }
    protected virtual void OnDraw(SpriteBatch spriteBatch) { }
    protected virtual void AfterDraw(SpriteBatch spriteBatch) { }
}