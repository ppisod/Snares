using ppilib2.Graphics._Attr;
using ppilib2.Graphics._BaseTypes;

namespace ppilib2.Graphics;

public class Drawable
{
    [Dependent]
    public UDimT<V> WorldPosition { get; private set; }
    
    [Dependent]
    public UDimT<V> WorldSize { get; private set; }
    
    
}