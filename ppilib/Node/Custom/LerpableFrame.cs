using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Node.Custom;

public class LerpableFrame(string name, INode parent, LocalTransform wantedTransform, Texture2D tex)
    : TransformNodeBase(name, parent, wantedTransform), ILerpableNode
{
    public bool DrawDebugShape { get; set; } = false;

    public void SetLocalPos (Vector2 v)
    {
        SetLocalTransform(new LocalTransform(v, Local.Scale, Local.Rotation));
    }
    public Vector2 LocalPos
    {
        get => Local.Pos;
        set => SetLocalPos(value);
    }

    public void SetLocalScale(Vector2 v)
    {
        SetLocalTransform(new LocalTransform(Local.Pos, v, Local.Rotation));
    }
    public Vector2 LocalScale
    {
        get => Local.Scale;
        set => SetLocalScale(value);
    }

    public void SetLocalRotation(float f)
    {
        SetLocalTransform(new LocalTransform(Local.Pos, Local.Scale, f));
    }
    public float LocalRotation
    {
        get => Local.Rotation;
        set => SetLocalRotation(value);
    }

    public void SetOpacity(float f)
    {
        Opacity = f;
    }
    public float Opacity = 1;
    public Lerper Lerper { get; set; } = new Lerper();
    public Easing Easing { get; set; } = EasingTypes.Linear;
    public Lerper.Mode EasingMode { get; set; } = Lerper.Mode.InOut;
    public void LerpAttribute<T>(Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time) { Lerper.AddTween(get, set, to, time, Easing, EasingMode, interpolate); }
    public void LerpLocalPos(Vector2 to, double time) { Lerper.LerpVector2(() => Local.Pos, SetLocalPos, to, time, Easing, EasingMode); }
    public void LerpLocalScale(Vector2 to, double time) { Lerper.LerpVector2(() => Local.Scale, SetLocalScale, to, time, Easing, EasingMode); }
    public void LerpLocalRotation(float to, double time) { Lerper.LerpFloat(() => Local.Rotation, SetLocalRotation, to, time, Easing, EasingMode); }
    public void LerpOpacity(float to, double time) { Lerper.LerpFloat(() => Opacity, SetOpacity, to, time, Easing, EasingMode); }

    protected override void OnUpdate(GameTime gameTime)
    {
        Lerper.Update(gameTime);
        base.OnUpdate(gameTime);
    }

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        if (!DrawDebugShape) return;
        var texSize = new Vector2(tex.Width, tex.Height);
        var scale = World.Scale.Result / texSize;
        spriteBatch.Draw(
            tex, 
            World.Position.Result, 
            null,
            Color.White * Opacity, 
            World.Rotation, 
            Vector2.Zero,
            scale, 
            SpriteEffects.None, 
            0f
        );
    }
}