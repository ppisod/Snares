using System;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Interfaces;

public interface ILerpableNode : ITransformNode
{
    Lerper Lerper { get; }
    
    Easing Easing { get; set; }
    Lerper.Mode EasingMode { get; set; }
    
    void LerpAttribute <T> (Func<T> get, Action<T> set, Func<T, T, float, T> interpolate, T to, double time);
    
    void LerpLocalPos (Vector2 to, double time);
    void LerpLocalScale(Vector2 to, double time);
    void LerpLocalRotation (float to, double time);
    
    void LerpOpacity (float to, double time);
}