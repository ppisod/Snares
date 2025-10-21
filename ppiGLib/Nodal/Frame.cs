using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppiGLib.Nodal;

public class Frame: Node
{
    public Frame (string name, Vector2 position, Vector2 size, Texture2D display = null) : base(name)
    {
        
    }

    public override void CustomUpdateLogic(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }

    public override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        throw new System.NotImplementedException();
    }
}