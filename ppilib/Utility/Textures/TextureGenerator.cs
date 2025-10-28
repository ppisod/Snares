using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ppilib.Utility.Textures;

public static class TextureGenerator
{
    public static Texture2D CreateVerticalGradient(GraphicsDevice graphicsDevice, int width, int height, Color topColor, Color bottomColor)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
        
        for (int y = 0; y < height; y++)
        {
            float t = (float)y / (height - 1);
            
            Color gradientColor = Color.Lerp(topColor, bottomColor, t);
            
            for (int x = 0; x < width; x++)
            {
                data[y * width + x] = gradientColor;
            }
        }
        
        texture.SetData(data);
        return texture;
    }
    public static Texture2D CreateMultiStopGradient(GraphicsDevice graphicsDevice, int width, int height, params (float position, Color color)[] colorStops)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];
    
        // Sort color stops by position
        var sortedStops = colorStops.OrderBy(stop => stop.position).ToArray();
    
        for (int y = 0; y < height; y++)
        {
            float position = (float)y / (height - 1);
            Color gradientColor = GetColorAtPosition(position, sortedStops);
        
            for (int x = 0; x < width; x++)
            {
                data[y * width + x] = gradientColor;
            }
        }
    
        texture.SetData(data);
        return texture;
    }

    private static Color GetColorAtPosition(float position, (float position, Color color)[] stops)
    {
        if (position <= stops[0].position)
            return stops[0].color;
    
        if (position >= stops[^1].position)
            return stops[^1].color;
        
        for (int i = 0; i < stops.Length - 1; i++)
        {
            if (position >= stops[i].position && position <= stops[i + 1].position)
            {
                float localT = (position - stops[i].position) / (stops[i + 1].position - stops[i].position);
                return Color.Lerp(stops[i].color, stops[i + 1].color, localT);
            }
        }
    
        return stops[0].color;
    }
}