using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Erroring;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using Color = Microsoft.Xna.Framework.Color;

namespace ppilib.Node.Custom;

/// <summary>
/// Simple rectangular frame node. Can optionally draw a debug texture scaled to the world size.
/// Inherit and override OnDraw for custom visuals; the built-in debug drawing is opt-in.
/// </summary>
public class Frame
                        (NodeConfig c)
    : LerpableNodeBase(c)
{
    /// <summary>When true, draws the provided debug texture scaled to the frame's world size.</summary>
    public bool DrawDebugShape { get; set; } = false;
    private readonly Texture2D _debugTexture = c.DebugTexture;
    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        // draw the frame using World -> result
        if (!DrawDebugShape) return;
        // get scale multiplier cuz we want to draw the debug shape in the same scale as the frame
        var texSize = new Vector2(_debugTexture.Width, _debugTexture.Height);
        var scale = texSize.X > 0 && texSize.Y > 0 ? World.Scale.Result / texSize : Vector2.One;
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