using System;
using Microsoft.Xna.Framework;
using ppilib.Utility;

namespace ppilib.Types.Class;

public class Stretch : IEquatable<Stretch>
{
    private Vector2 _base;
    private Vector2 _scale;
    private Vector2 _offset;

    public Stretch(Vector2 baseSize, Vector2 scaleMultiplier, Vector2 pixelsOffset)
    {
        _base = baseSize;
        _scale = scaleMultiplier;
        _offset = pixelsOffset;
        UpdateResult();
    }

    public Vector2 Base
    {
        get => _base;
        set => SetBase(value);
    }

    public void SetBase (Vector2 a) {_base = a;UpdateResult();}

    public Vector2 Scale
    {
        get => _scale;
        set => SetScale(value);
    }
    public void SetScale (Vector2 a) {_scale = a;UpdateResult();}

    public Vector2 Offset
    {
        get => _offset;
        set => SetOffset(value);
    }
    public void SetOffset (Vector2 a) {_offset = a;UpdateResult();}

    public Vector2 Result
    {
        get;
        private set;
    }
    
    private void UpdateResult ()
    {
        Result = new Vector2(_base.X * _scale.X + _offset.X, _base.Y * _scale.Y + _offset.Y);
    }
    
    public bool Equals (Stretch other)
    {
        if (other == null) return false;
        return Vectors.AreVectorsEqual(Base, other.Base) && Vectors.AreVectorsEqual(Scale, other.Scale) && Vectors.AreVectorsEqual(Offset, other.Offset);
    }

    public override bool Equals(object obj) => Equals(obj as Stretch);

    public override int GetHashCode() => HashCode.Combine(Base, Scale, Offset);
    public override string ToString() => $"Stretch2(Base: {Base}, Scale: {Scale}, Offset: {Offset}, Result: {Result})";
    
    
}