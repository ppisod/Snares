using Microsoft.Xna.Framework;

namespace ppilib.Utility.MovingThings;

public interface ITween
{
    bool Update(GameTime gameTime);
    bool IsActive { get; }
}