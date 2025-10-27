using Microsoft.Xna.Framework;

namespace ppilib.Utility.MovingThings.Interfaces;

public interface ITween
{
    bool Update(GameTime gameTime);
    bool IsActive { get; }
}