#nullable enable
using System;
using System.Collections.Generic;
using ppilib2.Graphics._Exceptions;

namespace ppilib2.Graphics;

public abstract class Node (string name)
{
    public Node? Parent { get; private set; }
    
    private List<Node> _children = new();
    public IReadOnlyList<Node> Children => _children;

    private string _name = name;
    public string Name => _name;

    public void AddChild (Node child)
    {

        ArgumentNullException.ThrowIfNull(child);
        if (child.Parent != null) throw new ChildAlreadyParented(child.Name);
        if (child == this) throw new InvalidOperationException("child to add cannot be this node");
        
        // deep-check if node's descendants have this node's parent to prevent cycling
        
        _children.Add(child);
        
    }
}