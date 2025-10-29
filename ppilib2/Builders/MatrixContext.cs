using ppilib2.Graphics._BaseTypes;

namespace ppilib2.Builders;

/// <summary>
/// The context used to build a [Drawable]Matrix.
/// </summary>
public class MatrixContext
{
    /// <summary>
    /// Position of the MatrixContext.
    /// Normally, the UDim's scale is the position at which the child drawable is drawn at, itself,
    /// offset in pixels,
    /// and reference is the size of the parent drawable
    /// </summary>
    public UDimT<V> Pos = new(V.Zero, V.Zero, V.Zero); // NORMALIZED
    
    /// <summary>
    /// Scale or size of the MatrixContext.
    /// Normally, the UDim's scale is the size of the child drawable relative to the parent drawable,
    /// offset in pixels,
    /// and reference is the size of the parent drawable
    /// </summary>
    public UDimT<V> Scale = new(V.Zero, V.Zero, V.Zero); // NORMALIZED

    /// <summary>
    /// Rotation of the MatrixContext.
    /// </summary>
    public double Rotation = 0d;
    
    /// <summary>
    /// Origin of the MatrixContext.
    /// Where is the scale's origin and the rotation's origin applied to? (Normally it's the top left)
    /// </summary>
    public UDimT<V> Origin = new(V.Zero, V.Zero, V.Zero); // NORMALIZED - where is the scale and rotate dealt to?
    
    /// <summary>
    /// Anchor of the MatrixContext.
    /// Where in the parent node is it placed? this is processed before Pos.
    /// </summary>
    public UDimT<V> Anchor = new(V.Zero, V.Zero, V.Zero); // NORMALIZED - where in the parent node is it placed?

    public MatrixContext SetPos (UDimT<V> to)
    {
        Pos = to;
        return this;
    }

    public MatrixContext SetScale (UDimT<V> to)
    {
        Scale = to;
        return this;
    }

    public MatrixContext SetRotation (double to)
    {
        Rotation = to;
        return this;
    }

    public MatrixContext SetOrigin (UDimT<V> to)
    {
        Origin = to;
        return this;
    }

    public MatrixContext SetAnchor (UDimT<V> to)
    {
        Anchor = to;
        return this;
    }
    
}