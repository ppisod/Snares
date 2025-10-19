/*
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Svg;
namespace ppiGLib;

public class TextureVector
{
    /// <summary>
    /// Loads a SVG as a Texture2D via Bitmap
    /// </summary>
    /// <param name="graphicsDevice">your graphics device</param>
    /// <param name="filepath">the filepath of the SVG file</param>
    /// <param name="width">what you want the render width to be</param>
    /// <param name="height">what you want the render height to be</param>
    /// <param name="stretchy">if stretchy, then scale according to your preferences.
    /// if not stretchy, then scale to best fit the minimum of width/height to fit vector ratios.</param>
    /// <returns></returns>
    public static Texture2D LoadASvg (GraphicsDevice graphicsDevice, string filepath, int width, int height, bool stretchy)
    {
        var document = SvgDocument.Open(filepath);
        var bounds = document.Bounds;
        float scaleWidth = width / (float)bounds.Width;
        float scaleHeight = height / (float)bounds.Height;
        
        if (stretchy)
        {
            float scale = Math.Min(scaleWidth, scaleHeight);
            scaleWidth = scale;
            scaleHeight = scale;
        }
        
        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        
        graphics.Clear(System.Drawing.Color.Transparent);
        
        graphics.ScaleTransform(scaleWidth, scaleHeight);
        document.Draw(graphics);
        
        using var memoryStream = new MemoryStream();
        bitmap.Save(memoryStream, ImageFormat.Png);
        memoryStream.Position = 0;
        
        return Texture2D.FromStream(graphicsDevice, memoryStream);
    }
}
*/