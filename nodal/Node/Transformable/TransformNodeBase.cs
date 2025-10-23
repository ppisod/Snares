using System;
using System.Numerics;
using ppilib.Interfaces;
using ppilib.Node.Base;
using ppilib.Types;
using ppilib.Types.Class;
using ppilib.Types.Struct;

namespace ppilib.Node.Transformable;
#nullable enable
public class TransformNodeBase : NodeBase, ITransformNode
{
    private DirtyFlags _dirties = DirtyFlags.All;
    private Transform _world;
    
    public TransformNodeBase (string name, INode? parent, LocalTransform wantedTransform) : base(name, parent)
    {
        Local = wantedTransform;
        _world = Transform.Identity;
    }

    public LocalTransform Local { get; set; }
    public Transform World { get; private set; }
    
    public event Action<ITransformNode, Types.Class.Transform>? WorldTransformChanged;
    
    public void MarkDirty(DirtyFlags flags = DirtyFlags.All)
    {
        _dirties = flags;
    }

    public Transform FindAncestralWorld()
    {
        // root node has to have a transform, so FindAncestralWorld will never return null. (and shouldn't return Identity!)
        var ancestralWorld = Transform.Identity;
        var ancestor = Parent;
        while (ancestor != null)
        {
            if (ancestor is ITransformNode parent) ancestralWorld = parent.World;
            ancestor = ancestor.Parent;
        }
        if (ancestralWorld == Transform.Identity) Console.WriteLine("[WARNING] AncestralWorld is Identity! this means that the node is not attached to a root-node which has a transform.");
        return ancestralWorld;
    }

    public void MarkDescendantsDirty ()
    {
        foreach (var child in Children)
        {
            if (child is ITransformNode transformableChild)
            {
                transformableChild.MarkDirty();
                transformableChild.MarkDescendantsDirty();
            }
            else
            {
                // what do I do here?
            }
        }
    }

    public void RecalculateWorld()
    {
        var ancestralWorld = FindAncestralWorld();
        // we don't know if ancestralWorld is root or not, but it shouldn't matter anyways
        // so now we calculate world with Local
        var scale = new Stretch(ancestralWorld.Scale.Result, Local.Scale, Vector2.Zero);
        var position = new Stretch(scale.Result, Local.Pos, ancestralWorld.Position.Result);
        var rotation = ancestralWorld.Rotation + Local.Rotation;
        World = new Transform(position, scale, rotation);
        // we need to mark all (transformable) descendants as dirty, so they can be recalculated.
        // there's an issue here,
        // if the parent only does a marking for their children,
        // and their children are not ITransform,
        // and their grandchildren are ITransform,
        // then their grandchildren will not be marked. This issue can be fixed with a recursive function.
        foreach (var child in Children)
        {
            if (child is ITransformNode transformableChild)
            {
                transformableChild.MarkDirty();
            }
            else
            {
                // 
            }
        }
    }
}