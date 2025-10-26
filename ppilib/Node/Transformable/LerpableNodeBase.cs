using System;
using Microsoft.Xna.Framework;
using ppilib.Interfaces;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace ppilib.Node.Transformable;

public class LerpableNodeBase(string name, INode parent, LocalTransform wantedTransform)
    : TransformNodeBase(name, parent, wantedTransform), ILerpableNode
{
    public Lerper Lerper { get; } = new();
    public Easing Easing { get; set; }
    public LerpMode EasingMode { get; set; }
    
    public float Opacity { get; set; } = 1.0f;
    
    public void LerpAttribute<T>(Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time)
    {
        Lerper.AddTween(get, set, to, time, Easing, EasingMode, interpolate);
    }

    public void LerpLocalPos(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Pos, (v) => Local.Pos = v, to, time, Easing, EasingMode);
    }

    public void LerpLocalScale(Vector2 to, double time)
    {
        Lerper.LerpVector2(() => Local.Scale, (v) => Local.Scale = v, to, time, Easing, EasingMode);
    }

    public void LerpLocalRotation(float to, double time)
    {
        Lerper.LerpFloat(() => Local.Rotation, (v) => Local.Rotation = v, to, time, Easing, EasingMode);
    }

    public void LerpOpacity(float to, double time)
    {
        Lerper.LerpFloat(() => Opacity, f => Opacity = f, to, time, Easing, EasingMode);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Lerper.Update(gameTime);
        base.OnUpdate(gameTime);
    }
}