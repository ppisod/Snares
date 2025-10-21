using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Primal;

public class BaseNode: Node
{
    public BaseNode (GraphicsDevice gDev, string name) : base(gDev, name)
    {
        Name = name;
    }

    protected override void CustomUpdateLogic(GameTime gameTime)
    {
        
    }

    protected override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        
    }
}