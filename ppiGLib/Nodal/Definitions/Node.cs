using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Utility;

namespace ppiGLib.Nodal.Definitions;

public abstract class Node
{
    #nullable enable // you know maybe Dad DOES NOT HAVE A TRANSFORm m..... also good to just have it on
    public Node? Parent { get; set; }
    protected List<Node> Children { get; init; } = new List<Node>();
    public required string Name { get; set; }

    public required bool IsTransformable
    {
        get;
        init;
    }
    public Transform2? Transform { get; set; }

    public bool UpdateActive { get; set; } = true;
    public bool DrawActive { get; set; } = true;
    
    public UniqueId Identifier { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphDev">the graphics device</param>
    /// <param name="name">string, node name</param>
    /// <param name="transformable">, is it sized</param>
    /// <param name="size">Size parameter. Size: 0 to 1 is 0% to 100% of parent size.</param>
    /// <param name="pos">Position parameter. Pos: 0 to 1 means 0% to 100% offset from top-left corner of parent size</param>
    /// <param name="rotation">Rotation parameter. This is added to the parent rotation.</param>
    protected Node (GraphicsDevice graphDev, string name, 
        bool transformable = false, 
        Vector2? size = null, 
        Vector2? pos = null,
        float? rotation = null)
    {
        Name = name;
        Identifier = new UniqueId();
        
        if (!transformable) return;
        IsTransformable = true;
        
        var nullsafeSize = size ?? Vector2.One;
        var nullsavePosition = pos ?? Vector2.Zero;
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
            nullsavePosition, 
            Vector2.Zero
        );
        var newRot = mamaTransform.Rotation + nullsafeRotation;
        Transform = new Transform2(
            newPos, 
            newSize, 
            newRot
        );
        
    }

    public void AddNodeAsChild (Node child)
    {
        Children.Add(child);
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
        if (!IsTransformable) return;
        if (!DrawActive) return;
        CustomDrawLogic(spriteBatch);
        foreach (var t in Children)
        {
            t.Draw(spriteBatch);
        }
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