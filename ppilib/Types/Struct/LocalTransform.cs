
using Microsoft.Xna.Framework;

namespace ppilib.Types.Struct;

public struct LocalTransform(Vector2 pos, Vector2 scale, float rot)
{
    public Vector2 Scale { get; set; } = scale;
    public Vector2 Pos { get; set; } = pos;
    public float Rotation { get; set; } = rot;
    public static LocalTransform Root = new LocalTransform(Vector2.Zero, Vector2.One, 0f);
};