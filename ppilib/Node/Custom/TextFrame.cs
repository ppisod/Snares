using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Custom;

public class TextFrame(
    string name,
    string text,
    INode parent,
    LocalTransform wantedTransform,
    SpriteFont font,
    Color color)
    : TransformNodeBase(name, parent, wantedTransform), ILerpableNode
{
    public Lerper Lerper { get; } = new();
    public Easing Easing { get; set; }
    public Lerper.Mode EasingMode { get; set; }
    public SpriteFont Font { get; set; } = font;
    public string Text { get; set; } = text;
    public Color Color { get; } = color;
    public float Opacity { get; set; }
    
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
        spriteBatch.DrawString(
            Font, Text, World.Position.Result, Color * Opacity, World.Rotation, Vector2.Zero, World.Scale.Result, SpriteEffects.None, 0f
            );
        base.OnDraw(spriteBatch);
    }
}