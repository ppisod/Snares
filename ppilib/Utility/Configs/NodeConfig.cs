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
using ppilib.Utility.Shapes;

namespace ppilib.Utility.Configs;

public class NodeConfig
{

    public NodeConfig (NodeConfig? past, GraphicsDevice device, bool transformable, bool display, bool text, bool lerp, bool contLerp)
    {
        _isTransformSupported = transformable;
        _isDisplaySupported = display;
        _isTextSupported = text;
        _isLerp = lerp;
        _isContLerp = contLerp;
        if (!transformable)
        {
            // this node does not support transforming
            // code here accordingly
        }
        else
        {
            T = LocalTransform.Root;
        }

        if (display)
        {
            if (!transformable)
            {
                throw new InvalidOperationException("display node has to support transformation");
            }
            
            Opacity = 1f;
            DebugTexture = ShapeGenerator.ColoredScalable(device, Microsoft.Xna.Framework.Color.Black);
        }

        if (text)
        {
            if (!display)
            {
                throw new InvalidOperationException("text has to support display");
            }
            
            Text = "";
        }

        if (lerp)
        {
            if (!transformable)
            {
                throw new InvalidOperationException("lerp has to support transformation");
            }
            
            LerpMethod = new Linear().EaseInOut;
        }

        if (contLerp)
        {
            if (!lerp)
            {
                throw new InvalidOperationException("contLerp has to use lerp attributes");
            }
            
            LerpRate = 0.5f;
        }
        
        if (past == null) return;
        // clone last
        _isTransformSupported = past._isTransformSupported;
        _isDisplaySupported = past._isDisplaySupported;
        _isTextSupported = past._isTextSupported;
        _isLerp = past._isLerp;
        _isContLerp = past._isContLerp;
        T = past.T;
        Parent = past.Parent;
        Name = past.Name;
        Opacity = past.Opacity;
        Font = past.Font;
        LerpMethod = past.LerpMethod;
        LerpRate = past.LerpRate;
    }

    private readonly bool _isTransformSupported;
    private bool _isDisplaySupported;
    private bool _isTextSupported;
    private bool _isLerp;
    private bool _isContLerp;
    
    // wantedTransform -> pos, scale, rot, modify accordingly
    
    // all methods reutrn another NodeConfig with modified attributes

    public LocalTransform? T { get; private set; }
    public INode? Parent; // if parent is null, node is root
    public string Name = "Node";
    
    // for display nodes
    public float? Opacity;
    public Texture2D? DebugTexture;
    
    // for text nodes
    public SpriteFont? Font; // there is no fallback for this one, raise error if font is null (and node applied requires a font)
    public Color? Color;
    public string? Text;
    
    // for lerpable nodes
    public Func<float, float>? LerpMethod;

    // for contlerp nodes
    public float? LerpRate;
    public NodeConfig SetParent (INode parent)
    {
        Parent = parent;
        return this;
    }

    public NodeConfig SetName (string name)
    {
        Name = name;
        return this;
    }
    
    public NodeConfig SetPos (Vector2 v)
    {
        if (!_isTransformSupported) throw new InvalidOperationException("not transformable");
        if (T == null) throw new InvalidOperationException("Transform is null, cannot set any transform");
        T.Pos = v;
        return this;
    }

    public NodeConfig SetScale (Vector2 v)
    {
        if (!_isTransformSupported) throw new InvalidOperationException("not transformable");
        if (T == null) throw new InvalidOperationException("Transform is null, cannot set any transform");
        T.Scale = v;
        return this;
    }

    public NodeConfig SetRotate (float v)
    {
        if (!_isTransformSupported) throw new InvalidOperationException("not transformable");
        if (T == null) throw new InvalidOperationException("Transform is null, cannot set any transform ");
        T.Rotation = v;
        return this;
    }

    public NodeConfig SetOpacity(float v)
    {
        if (!_isDisplaySupported) throw new InvalidOperationException("not display");
        Opacity = v;
        return this;
    }

    public NodeConfig SetText(string v)
    {
        if (!_isTextSupported) throw new InvalidOperationException("not text");
        Text = v;
        return this;
    }

    public NodeConfig SetColor(Color v)
    {
        if (!_isTextSupported) throw new InvalidOperationException("not text");
        Color = v;
        return this;
    }

    public NodeConfig SetFont(SpriteFont v)
    {
        if (!_isTextSupported) throw new InvalidOperationException("not text");
        Font = v;
        return this;
    }

    public NodeConfig SetLerpMethod(Func<float, float> f)
    {
        if (!_isLerp) throw new InvalidOperationException("not lerp");
        LerpMethod = f;
        return this;
    }

    public NodeConfig SetLerpRate(float f)
    {
        if (!_isContLerp) throw new InvalidOperationException("not contlerp");
        LerpRate = f;
        return this;
    }

    public NodeConfig SetDebugTexture(Texture2D texture)
    {
        if (!_isDisplaySupported) throw new InvalidOperationException("not display");
        DebugTexture = texture;
        return this;

    }
}