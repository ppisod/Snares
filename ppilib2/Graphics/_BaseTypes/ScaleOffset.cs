using System.Numerics;

namespace ppilib2.Graphics._BaseTypes;

/// <summary>
/// Represents a Scale, Offset pair for a data type.
/// </summary>
/// <param name="scale">Wanted scale.</param>
/// <param name="offset">Wanted offset.</param>

public class ScaleOffset<T> (T scale, T offset) where T : IAdditionOperators<T, T, T>, IMultiplyOperators<T, T, T>
{
    public T Scale { get; set; } = scale;

    public T Offset { get; set; } = offset;

    public T FromReference (T reference) => Scale * reference + Offset;
}