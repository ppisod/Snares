using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using Color = Microsoft.Xna.Framework.Color;

namespace ppilib.Node.Custom;

public class Frame
                        (string name, INode parent, LocalTransform wantedTransform, Texture2D debugTexture)
    : TransformNodeBase (name, parent, wantedTransform)
{
    public bool DrawDebugShape { get; set; } = false;
    private readonly Texture2D _debugTexture = debugTexture;
    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        // draw the frame using World -> result
        if (!DrawDebugShape) return;
        // get scale multiplier cuz we want to draw the debug shape in the same scale as the frame
        var texSize = new Vector2(_debugTexture.Width, _debugTexture.Height);
        var scale = World.Scale.Result / texSize;
        spriteBatch.Draw(
            _debugTexture, 
            World.Position.Result, 
            null, 
            Color.White, 
            0f, 
            Vector2.Zero, 
            scale, 
            SpriteEffects.None, 
            0f
        );

    }
}