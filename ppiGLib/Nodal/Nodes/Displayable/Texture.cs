using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppiGLib.Nodal.Definitions;

namespace ppiGLib.Nodal.Nodes.Displayable;

public class Texture(
    GraphicsDevice graphDev,
    string name, Node parent,
    Texture2D tex,
    bool startingEnabled,
    bool displayCentered,
    float displayOpaqueness,
    Vector2? pos,
    Vector2? size,
    float? rotation = 0)
    : Node(graphDev, name, true, parent, pos, size, rotation)
{
    public bool Enabled { get; set; } = startingEnabled;
    public Texture2D Tex { get; set; } = tex;

    public bool DisplayCentered { get; set; } = displayCentered;
    public float DisplayOpaqueness { get; set; } = displayOpaqueness;

    protected override void CustomUpdateLogic(GameTime gameTime)
    {
        
    }

    protected override void CustomDrawLogic(SpriteBatch spriteBatch)
    {
        if (!Enabled) return;
        
        Debug.Assert(Transform != null, nameof(Transform) + " != null");

        Vector2 origin = Vector2.Zero;
        var size = new Vector2(Tex.Width, Tex.Height);
        var wanted = Transform.Size.Result / size;
        var real = size * wanted;
        if (DisplayCentered)
        {
            origin = new Vector2(
                real.X / 2f, real.Y / 2f
            );
        }
        
        spriteBatch.Draw(
            Tex,
            Transform.Position.Result,
            null,
            Color.White * DisplayOpaqueness,
            Transform.Rotation,
            origin,
            wanted,
            SpriteEffects.None,
            0f
        );
    }
}