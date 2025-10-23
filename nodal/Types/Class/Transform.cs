using System.Numerics;

namespace ppilib.Types.Class;

public class Transform (Stretch pos, Stretch scale, float rotation)
{
    public Stretch Position { get; set; } = pos;
    public Stretch Scale { get; set; } = scale;
    public float Rotation { get; set; } = rotation;

    public static readonly Transform Identity = new Transform(
        new Stretch(Vector2.Zero, Vector2.One, Vector2.Zero),
        new Stretch(Vector2.Zero, Vector2.One, Vector2.Zero), 
        0f);
};