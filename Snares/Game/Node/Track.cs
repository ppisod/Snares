using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Types.Struct;

namespace Snares.Game.Node;

public class    Track(string name, INode parent, LocalTransform wantedTransform, Texture2D tex) : 
                LerpableFrame(name, parent, wantedTransform, tex)
{
    protected override void OnUpdate(GameTime gameTime)
    {
        
        base.OnUpdate(gameTime);
    }
}