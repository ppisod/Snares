using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;

namespace ppilib.Node.Custom;

public class ContinuousTextFrame(string name, INode parent, LocalTransform  wantedTransform, Func<float, float> easeF, 
                                 string text, Color color, SpriteFont font, float opacity)
                : ContinuousNode(       name,       parent,                 wantedTransform,                    easeF)
{
    
    public string Text { get; set; } = text;
    public Color Color { get; set; } = color;
    public float Opacity { get; set; } = opacity;
    public SpriteFont Font { get; set; } = font;

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
            Color * Opacity, 
            World.Rotation, 
            origin,  // Use the calculated origin for centering
            scale, 
            SpriteEffects.None, 
            0f
        );
        base.OnDraw(spriteBatch);
    }
}