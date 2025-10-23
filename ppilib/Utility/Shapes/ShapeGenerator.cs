using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppilib.Utility.Shapes;

public static class ShapeGenerator
{
    public static Texture2D ColoredScalable (GraphicsDevice graphicsDevice, Color color)
    {
        var tex = new Texture2D (graphicsDevice, 1, 1);
        tex.SetData ([color]);
        return tex;
    }
}