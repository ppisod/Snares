using System;
using Microsoft.Xna.Framework;

namespace ppiGLib.Utility;

public class Stretch2 : IEquatable<Stretch2>
{
    private Vector2 _offset;
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

    public Vector2 Offset
    {
        get => _offset;
        set => SetOffset(value);
    }
    
    public Vector2 Result { get; private set; }
    
    /// <summary>
    /// Stretch2 instead of Vector2 for scaling things in the nodal system
    /// </summary>
    /// <param name="baseSize">parent frame size</param>
    /// <param name="scale">child size</param>
    /// <param name="offset">child offset</param>
    public Stretch2 (Vector2 baseSize, Vector2 scale, Vector2 offset)
    {
        _baseSize = baseSize;
        _scale = scale;
        _offset = offset;
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

    public void SetOffset(Vector2 newOffset)
    {
        _offset = newOffset;
        UpdateResult();
    }

    private void UpdateResult()
    {
        Result = new Vector2(_baseSize.X * _scale.X + _offset.X, _baseSize.Y * _scale.Y + _offset.Y);
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