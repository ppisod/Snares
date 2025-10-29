using ppilib2.Graphics._Attr;
using ppilib2.Graphics._BaseTypes;

namespace ppilib2.Graphics;

public class Drawable
{
    public Drawable (Matrix local)
    {
        Local = local;
    }
    
    [Dependent]
    public Matrix World { get; private set; }
    
    [Independent]
    public Matrix Local { get; private set; }
}