using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings.Ease.Types;

public class Quad : Easing
{
    public override float EaseIn(float f) => f * f;

    public override float EaseOut(float f) => f * (2 - f);

    public override float EaseInOut(float f)
    {
        if (f < 0.5f) return 2 * f * f;
        return 1 - 2 * (1 - f) * (1 - f);
    }
}