using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Primal;

public class BaseNode(GraphicsDevice gDev, string name, Node parent) : Node(gDev, name, false, parent)
{
    protected override void CustomUpdateLogic(GameTime gameTime)
    {
        
    }

    protected override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        
    }
}