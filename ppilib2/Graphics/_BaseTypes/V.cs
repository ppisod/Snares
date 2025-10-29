using System;
using System.Numerics;

namespace ppilib2.Graphics._BaseTypes;

/// <summary>
/// A lightweight 2D vector using doubles for accuracy.
/// Provides arithmetic operators and common helpers (Lerp, Dot, Normalize, etc.).
/// </summary>
public class V (double x, double y) :
    IEquatable<V>,
    IUnaryNegationOperators<V, V>,
    IAdditionOperators<V, V, V>,
    ISubtractionOperators<V, V, V>,
    IMultiplyOperators<V, V, V>, IMultiplyOperators<V, double, V>,
    IDivisionOperators<V, V, V>, IDivisionOperators<V, double, V>
{
    public static V Zero => new(0, 0);
    public static V One => new(1, 1);
    public static V UnitX => new(1, 0);
    public static V UnitY => new(0, 1);

    public double X = x;
    public double Y = y;

    public V(double v) : this(v, v) { }
    public V(Vector2 v) : this(v.X, v.Y) { }

    public Vector2 ToVector2() => new((float)X, (float)Y);

    public static implicit operator V(Vector2 v) => new(v);
    public static explicit operator Vector2(V v) => v.ToVector2();

    // Basic operators
    public static V operator -(V v) => new(-v.X, -v.Y);
    public static V operator -(V a, V b) => new(a.X - b.X, a.Y - b.Y);
    public static V operator +(V a, V b) => new(a.X + b.X, a.Y + b.Y);
    public static V operator *(V a, V b) => new(a.X * b.X, a.Y * b.Y);
    public static V operator *(double f, V a) => new(a.X * f, a.Y * f);
    public static V operator *(V a, double f) => new(a.X * f, a.Y * f);
    public static V operator /(V a, double f)
    {
        var inv = 1.0 / f;
        return new V(a.X * inv, a.Y * inv);
    }
    public static V operator /(V a, V b) => new(a.X / b.X, a.Y / b.Y);

    // Equality
    public bool Equals(V other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return X.Equals(other.X) && Y.Equals(other.Y);
    }
    public override bool Equals(object obj) => obj is V v && Equals(v);
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public static bool operator ==(V a, V b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(V a, V b) => !(a == b);

    // Magnitudes
    public double Length => Math.Sqrt(X * X + Y * Y);
    public double LengthSquared => X * X + Y * Y;

    // Normalization
    public void Normalize()
    {
        var len = Length;
        if (len > 0)
        {
            var inv = 1.0 / len;
            X *= inv;
            Y *= inv;
        }
    }
    public V Normalized()
    {
        var len = Length;
        if (len > 0)
        {
            var inv = 1.0 / len;
            return new V(X * inv, Y * inv);
        }
        return new V(0, 0);
    }

    // Common helpers
    public static double Dot(V a, V b) => a.X * b.X + a.Y * b.Y;
    public static double Distance(V a, V b) => (a - b).Length;
    public static double DistanceSquared(V a, V b) => (a - b).LengthSquared;
    public static V Min(V a, V b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
    public static V Max(V a, V b) => new(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
    public static V Clamp(V v, V min, V max) => Max(min, Min(v, max));
    public static V Abs(V v) => new(Math.Abs(v.X), Math.Abs(v.Y));

    public static V Lerp(V a, V b, double t)
    {
        // Matches typical lerp semantics: a + (b - a) * t
        return new V(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
    }

    public void Deconstruct(out double x, out double y)
    {
        x = X; y = Y;
    }

    public override string ToString() => $"({X:0.###}, {Y:0.###})";
}