namespace ppilib.Types;

public enum DirtyFlags
{
    None =  0b0001, 
    Pos =   0b0010,
    Scale = 0b0100,
    Rot =   0b1000,
    
}