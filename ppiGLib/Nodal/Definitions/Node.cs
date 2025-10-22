using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Utility;

namespace ppiGLib.Nodal.Definitions;

public abstract class Node
{
    #nullable enable
    private void w (string q)
    {
        Console.WriteLine(q);
    }

    public Node? Parent { get; set; }
    protected List<Node> Children { get; init; } = [];
    public string Name { get; init; }

    public bool IsTransformable
    {
        get;
        init;
    }
    public Transform2? Transform { get; protected set; }
    protected Vector2? LocalPos;
    protected Vector2? LocalScale;
    protected float LocalRot;
    
    private GraphicsDevice _graphicsDevice;

    public bool UpdateActive { get; set; } = true;
    public bool DrawActive { get; set; } = true;
    
    public UniqueId Identifier { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphDev">the graphics device</param>
    /// <param name="name">string, node name</param>
    /// <param name="transformable">, is it sized</param>
    /// <param name="parent">parent node</param>
    /// <param name="size">Size parameter. Size: 0 to 1 is 0% to 100% of parent size.</param>
    /// <param name="pos">Position parameter. Pos: 0 to 1 means 0% to 100% offset from top-left corner of parent size</param>
    /// <param name="rotation">Rotation parameter. This is added to the parent rotation.</param>
    protected Node (GraphicsDevice graphDev, string name, 
        bool transformable = false, Node? parent = null,
        Vector2? pos = null,
        Vector2? size = null,
        float? rotation = null)
    {
        w("----------------------------------");
        Name = name;
        Identifier = new UniqueId();
        IsTransformable = transformable;
        _graphicsDevice = graphDev;
        
        w($"{name} instantiated");
        w($"is transformable: {transformable}");
        w($"parent: {parent?.Name}");
        
        Parent = parent;
        if (parent == null)
        {
            RecalculateTransform();
            w("nilparent transform..");
            w($"t.pos {Transform?.Position}");
            w($"t.scl {Transform?.Size}");
        }

        if (!transformable)
        {
            w("-----------------------------------");
            return;
        }
        
        LocalPos = pos ?? Vector2.Zero;
        LocalScale = size ?? Vector2.One;
        LocalRot = rotation ?? 0f;
        
        w($"defined localpos: {LocalPos}");
        w($"defined localscale: {LocalScale}");
        w($"defined localrot: {LocalRot}");
        w($"argument pos: {pos}");
        w($"argument size: {size}");
        w($"argument rotation: {rotation}");
        w("-----------------------------------");

        /*
        var nullsafeSize = size ?? Vector2.One;
        var nullsafePosition = pos ?? Vector2.Zero;
        var nullsafeRotation = rotation ?? 0f;

        if (Parent == null)
        {
            var viewportWidth = graphDev.Viewport.Width;
            var viewportHeight = graphDev.Viewport.Height;
            var viewportSize = new Vector2(viewportWidth, viewportHeight);
            Transform = new Transform2(
                new Stretch2(viewportSize, Vector2.Zero, Vector2.Zero),
                new Stretch2(viewportSize, Vector2.One, Vector2.Zero),
                0
            ); // this is the node family's default initialization transform
            return;
        }


        Transform2? mamaTransform = GetMamaTransform();
        if (mamaTransform == null) throw new Exception("Can't find any mama transform...");
        var newSize = new Stretch2(
            mamaTransform.Size.Result,
            nullsafeSize,
            Vector2.Zero
        );
        var newPos = new Stretch2(
            newSize.Result,
            nullsafePosition,
            mamaTransform.Position.Result
        );
        var newRot = mamaTransform.Rotation + nullsafeRotation;
        Transform = new Transform2(
            newPos,
            newSize,
            newRot
        );
        */

    }
    
    public void AddNodeAsChild (Node child)
    {
        Children.Add(child);

        if (child.IsTransformable) child.RecalculateTransform();

    }

    public void RecalculateTransform ()
    {
        if (!IsTransformable || LocalPos == null || LocalScale == null) return;
        var mamaTransform = GetMamaTransform();

        if (mamaTransform == null)
        {
            var vW = _graphicsDevice.Viewport.Width;
            var vH = _graphicsDevice.Viewport.Height;
            var size = new Vector2(vW, vH);
            
            // Root Node Transform here
            var rootSize = new Stretch2(size, Vector2.One, Vector2.Zero);
            var rootPos = new Stretch2(rootSize.Result, Vector2.Zero, Vector2.Zero);
            const float rootRot = 0f;

            Transform = new Transform2(rootPos, rootSize, rootRot);
            w("Transform done for ROOT!");
        }
        else
        {
            var newSize = new Stretch2(mamaTransform.Size.Result, LocalScale.Value, Vector2.Zero);
            var newPos = new Stretch2(newSize.Result, LocalPos.Value, mamaTransform.Position.Result);
            var newRot = mamaTransform.Rotation + LocalRot;
            Transform = new Transform2(newPos, newSize, newRot);
            w("Transform done for CHILD!");
        }

        foreach (var child in Children.Where(child => child.IsTransformable))
        {
            child.RecalculateTransform();
            w("Transforming for children...");
        }
    }

    public void RemoveNodeFromChildren (Node child)
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

    public void Draw (SpriteBatch spriteBatch)
    {
        if (!DrawActive) return;
        if (IsTransformable) CustomDrawLogic(spriteBatch);
        foreach (var t in Children)
        {
            t.Draw(spriteBatch);
        }
    }

    public Node? GetChild (string name)
    {
        foreach (var c in Children.Where(c => c.Name == name))
        {
            return c;
        }
        return null;
    }
    
    public Transform2? GetMamaTransform ()
    {
        if (!IsTransformable)
        {
            return null;
        }
        
        var mama = Parent;
        if (mama == null) return null;
        if (mama.IsTransformable) return mama.Transform; // under all cases mama.Transform should not be null
        var transform = mama.GetMamaTransform(); // recursive
        return transform;
    }

    protected abstract void CustomUpdateLogic (GameTime gameTime);
    protected abstract void CustomDrawLogic (SpriteBatch spriteBatch);
    
    
}