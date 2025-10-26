

using Microsoft.Xna.Framework;
using ppilib.Input.Definitions;

namespace ppilib.Input.Interfaces;

public interface IInputRegion
{
    bool HitTest(Point point);
    void OnLeftDown(MouseEventArgs arguments);
    void OnLeftUp(MouseEventArgs arguments);
}