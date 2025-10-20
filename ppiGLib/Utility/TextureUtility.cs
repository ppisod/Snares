using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Utility;

namespace ppiGLib;

public class TextureUtility
{
    // docs for Sprite Batch draw
    // /// <summary>Submit a sprite for drawing in the current batch.</summary>
    // /// <param name="texture">A texture.</param>
    // /// <param name="position">The drawing location on screen.</param>
    // /// <param name="sourceRectangle">An optional region on the texture which will be rendered. If null - draws full texture.</param>
    // /// <param name="color">A color mask.</param>
    // /// <param name="rotation">A rotation of this sprite.</param>
    // /// <param name="origin">Center of the rotation. 0,0 by default.</param>
    // /// <param name="scale">A scaling of this sprite.</param>
    // /// <param name="effects">Modificators for drawing. Can be combined.</param>
    // /// <param name="layerDepth">A depth of the layer of this sprite.</param>

    public static void DrawBarebones (SpriteBatch spriteBatch, Texture2D texture, Vector2 pos, Vector2 wantedSize)
    {
        var textureW = texture.Width; var textureH = texture.Height;
        var scalingW = wantedSize.X / (float) textureW; var scalingH = wantedSize.Y / (float) textureH;
        
        spriteBatch.Draw(texture, 
            pos, 
            null, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            new Vector2(scalingW, scalingH), 
            SpriteEffects.None, 
            0f);
    }

    public static void Draw(
        SpriteBatch s, 
        Texture2D t, 
        Stretch2 pos, 
        Stretch2 size, 
        bool centered = false,
        float rot = 0f,
        float alpha = 1,
        SpriteEffects effects = SpriteEffects.None
        )
    {
        Vector2 origin = Vector2.Zero;
        if (centered)
        {
            origin = new Vector2(t.Width / 2f, t.Height / 2f);
        }
        s.Draw(t, 
            pos.Result, 
            null, 
            Color.White * alpha, 
            rot, 
            origin, 
            size.Scale, 
            effects, 
            0);
    }
}