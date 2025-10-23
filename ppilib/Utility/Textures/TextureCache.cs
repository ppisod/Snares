using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Utility.Shapes;
using Color = Microsoft.Xna.Framework.Color;

namespace ppilib.Utility.Textures;

public class TextureCache
{

    public TextureCache(GraphicsDevice g)
    {
        _graphicsDevice = g;
        NotFound = ShapeGenerator.ColoredScalable(g, Color.Magenta);
    }
    
    private readonly GraphicsDevice _graphicsDevice;
    public Dictionary<string, Texture2D> Tex = new ();
    public readonly Texture2D NotFound;

    public void Add (string name, Texture2D texture)
    {
        Tex.Add(name, texture);
    }

    public Texture2D Get (string name)
    {
        return Tex.GetValueOrDefault(name, NotFound);
        
    }
}