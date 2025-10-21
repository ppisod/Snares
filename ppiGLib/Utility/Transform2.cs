namespace ppiGLib.Utility;

public class Transform2 (Stretch2 pos, Stretch2 size, float rot)
{
    public Stretch2 Position { get; set; } = pos;
    public Stretch2 Size { get; set; } = size;
    public float Rotation { get; set; } = rot;
}