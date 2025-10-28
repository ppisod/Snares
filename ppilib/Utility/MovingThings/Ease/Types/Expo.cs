using System;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Utility.MovingThings.Ease.Types;

public class Expo : Easing
{
    public override float EaseIn(float progress)
    {
        return progress == 0f ? 0f : MathF.Pow(2f, 10f * (progress - 1f));
    }

    public override float EaseOut(float progress)
    {
        return Math.Abs(progress - 1f) < 0.001f ? 1f : 1f - MathF.Pow(2f, -10f * progress);
    }

    public override float EaseInOut(float progress)
    {
        if (progress == 0f) return 0f;
        if (Math.Abs(progress - 1f) < 0.001f) return 1f;
        
        if (progress < 0.5f)
            return 0.5f * MathF.Pow(2f, 20f * progress - 10f);
        else
            return 1f - 0.5f * MathF.Pow(2f, -20f * progress + 10f);
    }
}