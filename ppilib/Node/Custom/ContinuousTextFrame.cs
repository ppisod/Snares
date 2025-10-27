using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;

namespace ppilib.Node.Custom;

/// <summary>
/// Text rendering node built on ContinuousNodeBase so that position/scale/rotation and opacity can smoothly approach targets.
/// World.Scale.Y controls the rendered text height; the width is derived from the font's aspect ratio.
/// </summary>
public class ContinuousTextFrame: ContinuousNodeBase, ITextNode
{
    public ContinuousTextFrame(string name, INode parent, LocalTransform  wantedTransform, Func<float, float> easeF, 
        string text, Color color, SpriteFont font, float opacity) : base(name, parent, wantedTransform, easeF)
    {
        Text = text;
        Color = color;
        Opacity = opacity;
        Font = font;
        // smooth opacity like other fields
        OpacityTween = new ContinuousTween<float>(() => Opacity, v => Opacity = v, (a, b, t) => a + (b - a) * t, easeF, 5f);
        ColorTween = new ContinuousTween<Color>(() => Color, v => Color = v, Color.Lerp, easeF, 5f);
    }

    /// <summary>Displayed text.</summary>
    public string Text { get; set; }
    /// <summary>Base color multiplied by Opacity when drawing.</summary>
    public Color Color { get; set; }
    
    /// <summary>Draw opacity in 0..1.</summary>
    public float Opacity { get; set; }
    /// <summary>Controller for smoothly approaching Opacity.</summary>
    public readonly ContinuousTween<float> OpacityTween;
    /// <summary>Controller for smoothly approaching Color.</summary>
    public readonly ContinuousTween<Color> ColorTween;
    /// <summary>Font used for rendering.</summary>
    public SpriteFont Font { get; set; }

    protected override void OnUpdate(GameTime gameTime)
    {
        OpacityTween.Update(gameTime);
        ColorTween.Update(gameTime);
        base.OnUpdate(gameTime);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        var desiredHeight = World.Scale.Result.Y;
        float fontPixelHeight = Font.LineSpacing;
        var scale = desiredHeight > 0 && fontPixelHeight > 0
            ? desiredHeight / fontPixelHeight
            : 1f;
        
        var textSize = Font.MeasureString(Text) * scale;
    
        // Calculate the origin (pivot point) for proper centering
        var origin = textSize / 2f;
    
        spriteBatch.DrawString(
            Font, 
            Text, 
            World.Position.Result + (World.Scale.Result / 2), 
            Color * MathHelper.Clamp(Opacity, 0f, 1f), 
            World.Rotation, 
            origin,
            scale, 
            SpriteEffects.None, 
            0f
        );
        base.OnDraw(spriteBatch);
    }
}