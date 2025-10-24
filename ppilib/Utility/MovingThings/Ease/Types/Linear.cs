using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings.Ease.Types;

public class Linear : Easing
{
    // is this really needed
    public override float EaseIn(float progress) => progress;

    public override float EaseOut(float progress) => progress;

    public override float EaseInOut(float progress) => progress;
}