using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppiGLib.Nodal;

public class BaseNode: Node
{
    public BaseNode (string name) : base(name)
    {
        Name = name;
    }

    public override void CustomUpdateLogic(GameTime gameTime)
    {
        
    }

    public override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        
    }
}