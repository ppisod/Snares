using System;
using Microsoft.Xna.Framework;

namespace ppiGLib.Utility;

public class Stretch2 : IEquatable<Stretch2>
{
    private Vector2 _baseSize;
    private Vector2 _scale;
    
    public Vector2 BaseSize
    {
        get => _baseSize;
        set => SetBaseSize(value);
    }

    public Vector2 Scale
    {
        get => _scale;
        set => SetScale(value);
    }
    
    public Vector2 Result { get; private set; }

    /// <summary>
    /// a scale-based alternative to vector2
    /// </summary>
    /// <param name="baseSize">the base size of something, a frame, or maybe the window size</param>
    /// <param name="scale">the scale of something in terms of baseSize</param>
    public Stretch2 (Vector2 baseSize, Vector2 scale)
    {
        _baseSize = baseSize;
        _scale = scale;
        UpdateResult();
    }

    public void SetBaseSize (Vector2 newBaseSize)
    {
        _baseSize = newBaseSize;
        UpdateResult();
    }

    public void SetScale (Vector2 newScale)
    {
        _scale = newScale;
        UpdateResult();
    }

    private void UpdateResult()
    {
        Result = new Vector2(_baseSize.X * _scale.X, _baseSize.Y * _scale.Y);
    }

    public static Stretch2 operator + (Stretch2 a, Stretch2 b)
    {
        if (VectorUtility.AreVectorsEqual(a.BaseSize, b.BaseSize))
        {
            return new Stretch2(a.BaseSize, a.Scale + b.Scale);
        }

        throw new InvalidOperationException("base sizes don't match, they have to!");
    }

    public static Stretch2 operator - (Stretch2 a, Stretch2 b)
    {
        if (VectorUtility.AreVectorsEqual(a.BaseSize, b.BaseSize))
        {
            return new Stretch2(a.BaseSize, a.Scale - b.Scale);
        }
        
        throw new InvalidOperationException("base sizes don't match, they have to!");
    }

    public bool Equals(Stretch2 other)
    {
        if (other == null) return false;
        return VectorUtility.AreVectorsEqual(BaseSize, other.BaseSize) && VectorUtility.AreVectorsEqual(Scale, other.Scale);
    }

    public override bool Equals(object obj) => Equals(obj as Stretch2);

    public override int GetHashCode() => HashCode.Combine(BaseSize, Scale);

    public override string ToString() => $"Stretch2(BaseSize: {BaseSize}, Scale: {Scale}, Result: {Result})";

    public static bool operator == (Stretch2 a, Stretch2 b) => Equals(a, b);
    public static bool operator != (Stretch2 a, Stretch2 b) => !Equals(a, b);
}