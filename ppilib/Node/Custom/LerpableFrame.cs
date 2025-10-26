using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;

namespace ppilib.Node.Custom;

public class LerpableFrame(string name, INode parent, LocalTransform wantedTransform, Texture2D tex)
    : LerpableNodeBase(name, parent, wantedTransform)
{
    public bool DrawDebugShape { get; set; } = false;

    protected override void OnDraw(SpriteBatch spriteBatch)
    {
        if (!DrawDebugShape) return;
        var texSize = new Vector2(tex.Width, tex.Height);
        var scale = World.Scale.Result / texSize;
        spriteBatch.Draw(
            tex, 
            World.Position.Result, 
            null,
            Color.White * Opacity, 
            World.Rotation, 
            Vector2.Zero,
            scale, 
            SpriteEffects.None, 
            0f
        );
    }
}