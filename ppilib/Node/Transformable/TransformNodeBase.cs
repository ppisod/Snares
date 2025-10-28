using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Erroring;
using ppilib.Interfaces;
using ppilib.Node.Base;
using ppilib.Types;
using ppilib.Types.Class;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Transformable;
#nullable enable

/// <summary>
/// Base node that carries a local transform and computes a world transform from its ancestors.
/// Ensures world transform is lazily recalculated when marked dirty and exposes a change event.
/// </summary>
public class TransformNodeBase : NodeBase, ITransformNode
{
    private DirtyFlags _dirties = DirtyFlags.All;

    /// <summary>
    /// Create a new transform node with an initial local transform.
    /// </summary>
    public TransformNodeBase (NodeConfig n) : base(n)
    {
        Local = n.T ?? throw new NodeConfigMissing(nameof(LocalTransform), nameof(TransformNodeBase));
        World = Transform.Identity;
    }

    /// <summary>
    /// Local transform relative to the parent.
    /// </summary>
    public LocalTransform Local { get; set; }

    /// <summary>
    /// Cached world transform. Call <see cref="EnsureWorldUpToDate"/> before reading if you don't run inside update/draw.
    /// </summary>
    public Transform World { get; private set; }

    /// <summary>
    /// Fired whenever the world transform has been recalculated.
    /// </summary>
    public event Action<ITransformNode, Types.Class.Transform>? WorldTransformChanged;

    /// <summary>
    /// Use the provided world transform as the root transform of this node. Only valid for root nodes (no parent).
    /// </summary>
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
    
    /// <summary>
    /// Marks this node's world transform as dirty, so it will be recomputed on next access.
    /// </summary>
    public void MarkDirty(DirtyFlags flags = DirtyFlags.All)
    {
        // Accumulate dirty flags (even though we currently have only None/All, this is future-proof)
        _dirties |= flags;
    }

    /// <summary>
    /// Finds the nearest ancestor transform to use as a base for world computation. Warns if identity.
    /// </summary>
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

    /// <summary>
    /// Marks all transformable descendants as dirty.
    /// </summary>
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

    /// <summary>
    /// Ensures the world transform has been computed if it was marked dirty.
    /// </summary>
    public void EnsureWorldUpToDate()
    {
        if (_dirties != DirtyFlags.None)
        {
            RecalculateWorld();
        }
    }

    /// <summary>
    /// Sets a new local transform and triggers a world recalculation.
    /// </summary>
    public void SetLocalTransform(LocalTransform t)
    {
        Local = t;
        RecalculateWorld();
    }

    /// <summary>
    /// Recomputes the world transform from the ancestor world and the local transform.
    /// Also clears the dirty state, marks descendants dirty, and raises <see cref="WorldTransformChanged"/>.
    /// </summary>
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

    /// <summary>
    /// Convenience rectangle based on the current world transform (position as top-left, scale as size).
    /// </summary>
    public Rectangle GetRect ()
    {
        return new Rectangle((int) World.Position.Result.X, (int) World.Position.Result.Y, (int) World.Scale.Result.X, (int) World.Scale.Result.Y);
    }
}