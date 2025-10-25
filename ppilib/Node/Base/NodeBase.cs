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

public class NodeBase : INode, IDisposable
{
    private bool _disposed;
    public static void W (string q)
    {
        Console.WriteLine(q);
    }

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
    public List<INode> GetDescendants()
    {
        var result = new List<INode>();
        foreach (var c in Children)
        {
            result.Add(c);
            result.AddRange(c.GetDescendants());
        }

        return result;
    }

    public bool UpdateActive { get; set; } = true;
    public bool DrawActive { get; set; } = true;

    public bool IsDestroyed { get; private set; }

    public void Destroy()
    {
        if (IsDestroyed) return;
        // Prevent any further Update/Draw while destroying
        UpdateActive = false;
        DrawActive = false;
        
        // Copy children to avoid modification during iteration
        var childrenSnapshot = _children.ToArray();
        foreach (var child in childrenSnapshot)
        {
            child.Destroy();
        }
        
        // Detach from parent
        if (Parent is NodeBase parentNode)
        {
            parentNode.RemoveChild(this);
        }
        
        // Clear children list
        _children.Clear();
        
        // Allow subclasses to clean up
        OnDestroyed();
        
        // Dispose if supported
        if (this is IDisposable disposable)
        {
            try { disposable.Dispose(); } catch { /* swallow */ }
        }
        
        IsDestroyed = true;
    }

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
        if (!UpdateActive || IsDestroyed) return;
        // Custom update logic here, in derived types.
        OnUpdate(gameTime);
        foreach (var t in Children) t.Update(gameTime);
        AfterUpdate(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!DrawActive || IsDestroyed) return;
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
    protected virtual void OnDestroyed() { }

    // IDisposable pattern
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;
        // Note: managed/unmanaged cleanup would go here. Keep minimal for now.
    }
}