using System;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;

namespace ppilib.Node.Custom;

public class ContinuousTextFrame: ContinuousNode
{
    public ContinuousTextFrame(string name, INode parent, LocalTransform  wantedTransform, Func<float, float> easeF, 
        string text, Color color, SpriteFont font, float opacity) : base(name, parent, wantedTransform, easeF)
    {
        Text = text;
        Color = color;
        Opacity = opacity;
        Font = font;
        // this is a BOOTY patch
        
        OpacityTween = new ContinuousTween<float>(() => Opacity, v => Opacity = v, (a, b, c) => a+(b-a)*c, easeF, 0.05f);
    }

    public string Text { get; set; }
    public Color Color { get; set; }
    
    public float Opacity { get; set; }
    public readonly ContinuousTween<float> OpacityTween;
    public SpriteFont Font { get; set; }

    // BOOTY PATCHHHHH
    protected override void OnUpdate(GameTime gameTime)
    {
        OpacityTween.Update(gameTime);
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