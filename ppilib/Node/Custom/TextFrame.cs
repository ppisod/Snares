using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Erroring;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.MovingThings.Enums;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Custom;

public class TextFrame(NodeConfig c)
    : TransformNodeBase(c.Name, c.Parent, c.T ?? throw new NodeConfigMissing(nameof(LocalTransform), nameof(TextFrame))), ILerpableNode, ITextNode
{
    public Lerper Lerper { get; } = new();
    public Easing Easing { get; set; }
    public LerpMode EasingMode { get; set; }
    public SpriteFont Font { get; set; } = c.Font ?? throw new NodeConfigMissing(nameof(Font), nameof(TextFrame));
    public string Text { get; set; } = c.Text ?? throw new NodeConfigMissing(nameof(Text), nameof(TextFrame));
    public Color Color { get; set; } = c.Color ?? throw new NodeConfigMissing(nameof(Color), nameof(TextFrame));
    public float Opacity { get; set; } = 1f;
    
    public void LerpAttribute<T>(Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time)
    {
        Lerper.AddTween(get, set, to, time, Easing, EasingMode, interpolate);
    }

    public void LerpLocalPos(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Pos, v => SetLocalTransform(new LocalTransform(v, Local.Scale, Local.Rotation)), to, time, Easing, EasingMode);
    }

    public void LerpLocalScale(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Scale, v => SetLocalTransform(new LocalTransform(Local.Pos, v, Local.Rotation)), to, time, Easing, EasingMode);
    }

    public void LerpLocalRotation(float to, double time)
    {
        Lerper.LerpFloat(() => Local.Rotation, v => SetLocalTransform(new LocalTransform(Local.Pos, Local.Scale, v)), to, time, Easing, EasingMode);
    }

    public void LerpOpacity(float to, double time)
    {
        Lerper.LerpFloat(() => Opacity, v => Opacity = v, to, time, Easing, EasingMode);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        // Desired text height in pixels taken from world scale
        float desiredHeight = World.Scale.Result.Y;

        // Compute a uniform scale so text height ~= desiredHeight
        float fontPixelHeight = Font.LineSpacing; // base height of the font in pixels
        float scale = desiredHeight > 0 && fontPixelHeight > 0
            ? desiredHeight / fontPixelHeight
            : 1f;
        // set to display centered?
        spriteBatch.DrawString(
            Font, Text, World.Position.Result, Color * Opacity, World.Rotation, Vector2.Zero, scale, SpriteEffects.None, 0f
        );
        base.OnDraw(spriteBatch);
    }
}