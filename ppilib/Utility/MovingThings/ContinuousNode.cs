using System;
using Microsoft.Xna.Framework;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;

namespace ppilib.Utility.MovingThings;

public class ContinuousNode : TransformNodeBase
{
    public ContinuousTween<Vector2> Pos { get; }
    public ContinuousTween<Vector2> Scale { get; }
    public ContinuousTween<float> Rot { get; }

    public ContinuousNode (string name, INode parent, LocalTransform wantedTransform, Func<float, float> easeF) : base(name, parent, wantedTransform)
    {
        // why are the rates constant???????
        Pos = new ContinuousTween<Vector2>(() => Local.Pos, v => Local.Pos = v, Vector2.Lerp, easeF, 0.05f);
        Scale = new ContinuousTween<Vector2>(() => Local.Scale, v => Local.Scale = v, Vector2.Lerp, easeF, 0.05f);
        Rot = new ContinuousTween<float>(() => Local.Rotation, v => Local.Rotation = v, (f, f1, arg3) => f+(f1-f)*arg3, easeF, 2f);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Pos.Update(gameTime);
        Scale.Update(gameTime);
        Rot.Update(gameTime);
        base.OnUpdate(gameTime);
    }
}