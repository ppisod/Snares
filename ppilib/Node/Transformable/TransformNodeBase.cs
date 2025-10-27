using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Base;
using ppilib.Types;
using ppilib.Types.Class;
using ppilib.Types.Struct;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Transformable;
#nullable enable

public class TransformNodeBase : NodeBase, ITransformNode
{
    private DirtyFlags _dirties = DirtyFlags.All;

    public TransformNodeBase (string name, INode? parent, LocalTransform wantedTransform) : base(name, parent)
    {
        Local = wantedTransform;
        World = Transform.Identity;
    }

    public LocalTransform Local { get; set; }
    public Transform World { get; private set; }

    public event Action<ITransformNode, Types.Class.Transform>? WorldTransformChanged;

    public void SetWorldAsRoot(Transform world)
    {
        if (Parent != null)
        {
            Console.WriteLine("[WARNING] Node is not root, cannot set world as root.");
            return;
        }
        Local = LocalTransform.Root;
        World = world;
        _dirties = DirtyFlags.None;
        // propagate to descendants and notify listeners
        MarkDescendantsDirty();
        WorldTransformChanged?.Invoke(this, World);
    }
    
    public void MarkDirty(DirtyFlags flags = DirtyFlags.All)
    {
        // Accumulate dirty flags (even though we currently have only None/All, this is future-proof)
        _dirties |= flags;
    }

    public Transform FindAncestralWorld()
    {
        // root node has to have a transform, so FindAncestralWorld will never return null. (and shouldn't return Identity!)
        var ancestralWorld = Transform.Identity;
        var ancestor = Parent;
        while (ancestor != null)
        {
            if (ancestor is ITransformNode parent)
            {
                // Ensure the parent's world is up-to-date before using it
                parent.EnsureWorldUpToDate();
                ancestralWorld = parent.World;
                break;
            }
            ancestor = ancestor.Parent;
        }
        if (ancestralWorld == Transform.Identity) Console.WriteLine("[WARNING] AncestralWorld is Identity! this means that the node is not attached to a root-node which has a transform.");
        return ancestralWorld;
    }

    public void MarkDescendantsDirty ()
    {
        // TODO: change this into a recursive tree traversal thing, this is inefficient!!!
        var descendants = GetDescendants();
        foreach (var descendant in descendants)
        {
            if (descendant is ITransformNode transformableDescendant)
            {
                transformableDescendant.MarkDirty();
            }
        }
    }

    public void EnsureWorldUpToDate()
    {
        if (_dirties != DirtyFlags.None)
        {
            RecalculateWorld();
        }
    }

    public void SetLocalTransform(LocalTransform t)
    {
        Local = t;
        RecalculateWorld();
    }

    public void RecalculateWorld()
    {
        var ancestralWorld = FindAncestralWorld();
        // we don't know if ancestralWorld is root or not, but it shouldn't matter anyways
        // so now we calculate world with Local
        var scale = new Stretch(ancestralWorld.Scale.Result, Local.Scale, Vector2.Zero);
        var position = new Stretch(ancestralWorld.Scale.Result, Local.Pos, ancestralWorld.Position.Result);
        var rotation = ancestralWorld.Rotation + Local.Rotation;
        World = new Transform(position, scale, rotation);
        // clear own dirties now that world is valid
        _dirties = DirtyFlags.None;
        // we need to mark all (transformable) descendants as dirty, so they can be recalculated.
        MarkDescendantsDirty();
        // fire the event
        WorldTransformChanged?.Invoke(this, World);
    }

    protected override void OnUpdate (GameTime gameTime)
    {
        EnsureWorldUpToDate();
    }
    protected override void OnDraw (SpriteBatch spriteBatch)
    {
        EnsureWorldUpToDate();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // release managed resources specific to TransformNodeBase
            WorldTransformChanged = null;
        }
        base.Dispose(disposing);
    }

    public Rectangle GetRect ()
    {
        return new Rectangle((int) World.Position.Result.X, (int) World.Position.Result.Y, (int) World.Scale.Result.X, (int) World.Scale.Result.Y);
    }
}