#nullable enable
using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.MovingThings.Ease.Types;
using ppilib.Utility.MovingThings.Enums;

namespace ppilib.Utility.Configs;

public class NodeConfig
{

    public NodeConfig (NodeConfig? past)
    {
        if (past == null) return;
        T = past.T;
        Parent = past.Parent;
        Name = past.Name;
        Opacity = past.Opacity;
        Font = past.Font;
        LerpMethod = past.LerpMethod;
        
    }
    
    // wantedTransform -> pos, scale, rot, modify accordingly
    
    // all methods reutrn another NodeConfig with modified attributes

    public LocalTransform T { get; } = LocalTransform.Root;
    public INode? Parent;
    public string? Name;
    
    // for display nodes
    public float Opacity = 1f;
    
    // for text nodes
    public SpriteFont? Font; // there is no fallback for this one, raise error if font is null
    
    // use Lerp Method if exists, if not, use easing + mode if exists, if not, use Linear + InOut
    public Func<float, float>? LerpMethod = new Linear().EaseInOut;

    
    public NodeConfig SetPos (Vector2 v)
    {
        T.Pos = v;
        return this;
    }

    public NodeConfig SetScale(Vector2 v)
    {
        T.Scale = v;
        return this;
    }

    public NodeConfig SetRotate(float v)
    {
        T.Rotation = v;
        return this;
    }
    
}