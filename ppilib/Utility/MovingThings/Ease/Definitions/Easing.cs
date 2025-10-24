namespace ppilib.Utility.MovingThings.Ease.Definitions;

public abstract class Easing
{
    public abstract float EaseIn (float progress);
    public abstract float EaseOut (float progress);
    public abstract float EaseInOut (float progress);
}