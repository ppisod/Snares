using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Displayable;

public class Frame: Node
{
    public  Frame   (   GraphicsDevice gDev, 
                        string name, 
                        Vector2 position, 
                        Vector2 size, float rot, 
                        Texture2D display = null
                    )
        :   base    (
                        gDev, 
                        name, 
                        true, 
                        size, 
                        position, 
                        rot) 
    {
        
        // by the way this is the declaration body if i couldn't read my Nasty formatting
        
    }

    protected override void CustomUpdateLogic(GameTime gameTime)
    {
        throw new System.NotImplementedException();
    }

    protected override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        throw new System.NotImplementedException();
    }
}