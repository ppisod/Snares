using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings.Ease.Types;

public class Cubic : Easing
{
    public override float EaseIn(float f) => f * f * f;

    public override float EaseOut(float f) { f -= 1; return f * f * f + 1; }

    public override float EaseInOut(float f) { if (f < 0.5f) { return 4 * f * f * f; } f = 2 * f - 2; return 0.5f * f * f * f + 1; }
}