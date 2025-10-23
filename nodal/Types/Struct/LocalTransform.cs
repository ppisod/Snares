using System.Numerics;

namespace ppilib.Types.Struct;

public struct LocalTransform(Vector2 pos, Vector2 scale, float rot)
{
    public Vector2 Scale { get; set; } = scale;
    public Vector2 Pos { get; set; } = pos;
    public float Rotation { get; set; } = rot;
};