namespace ppilib2.Graphics._BaseTypes;

public class Matrix
{
    
    /// <summary>
    /// Position of the Matrix.
    /// Normally, the UDim's scale is the position at which the child drawable is drawn at, itself,
    /// offset in pixels,
    /// and reference is the size of the parent drawable
    /// </summary>
    public UDimT<V> Pos = new(V.Zero, V.Zero, V.Zero); // NORMALIZED
    
    /// <summary>
    /// Scale or size of the Matrix.
    /// Normally, the UDim's scale is the size of the child drawable relative to the parent drawable,
    /// offset in pixels,
    /// and reference is the size of the parent drawable
    /// </summary>
    public UDimT<V> Scale = new(V.Zero, V.Zero, V.Zero); // NORMALIZED

    /// <summary>
    /// Rotation of the Matrix.
    /// </summary>
    public double Rotation = 0d;
    
    /// <summary>
    /// Origin of the Matrix.
    /// Where is the scale's origin and the rotation's origin applied to? (Normally it's the top left)
    /// </summary>
    public UDimT<V> Origin = new(V.Zero, V.Zero, V.Zero); // NORMALIZED - where is the scale and rotate dealt to?
    
    /// <summary>
    /// Anchor of the Matrix.
    /// Where in the parent node is it placed? this is processed before Pos.
    /// </summary>
    public UDimT<V> Anchor = new(V.Zero, V.Zero, V.Zero);

    
}