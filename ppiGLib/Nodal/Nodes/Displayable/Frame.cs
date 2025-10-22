using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Generators;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Displayable;

public class Frame: Node
{
    
    public bool DisplayDebug { get; set; }
    private Texture2D _debugTexure;
    
    public  Frame   (   GraphicsDevice gDev, 
                        string name, Node parent,
                        Vector2 position, 
                        Vector2 size, float rot,
                        Texture2D debugTexture = null
                    )
        :   base    (
                        gDev, 
                        name, 
                        true, parent,
                        position,
                        size, 
                        rot) 
    {
        if (debugTexture == null)
        {
            _debugTexure = ShapeGenerator.ColoredScalable(gDev, Color.Gray);
            return;
        }
        
        _debugTexure = debugTexture;
    }

    protected override void CustomUpdateLogic(GameTime gameTime)
    {
        
    }

    protected override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        if (!DisplayDebug) return;
        Debug.Assert(Transform != null, nameof(Transform) + " != null");
        spriteBatch.Draw(
            _debugTexure, 
            Transform.Position.Result, 
            null, 
            Color.White, 
            Transform.Rotation, 
            Vector2.Zero, 
            Transform.Size.Result, 
            SpriteEffects.None, 
            0f
        );
    }
}