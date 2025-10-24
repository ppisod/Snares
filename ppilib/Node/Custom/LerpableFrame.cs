using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Transformable;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;

namespace ppilib.Node.Custom;

public class LerpableFrame(string name, INode parent, LocalTransform wantedTransform, Texture2D tex)
    : TransformNodeBase(name, parent, wantedTransform)
{
    public bool DrawDebugShape { get; set; } = false;

    public void SetLocalPos (Vector2 v)
    {
        SetLocalTransform(new LocalTransform(v, Local.Scale, Local.Rotation));
    }
    public Vector2 LocalPos
    {
        get => Local.Pos;
        set => SetLocalPos(value);
    }

    public float Opacity = 1;
    public Lerper Lerper { get; set; } = new Lerper();
    
    protected override void OnUpdate(GameTime gameTime)
    {
        Lerper.Update(gameTime);
        base.OnUpdate(gameTime);
    }

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
            0f, 
            Vector2.Zero, 
            scale, 
            SpriteEffects.None, 
            0f
        );
    }
}