using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ppilib;
/// <summary>
/// Singleton Core object for sir. Caltr (Four)
/// </summary>
public class Core : Game
{
    private static Core _Instance;
    
    /// <summary>
    /// Ref to core _Instance
    /// </summary>
    public static Core Instance => _Instance;
    
    public static GraphicsDeviceManager CGraphics { get; private set; }
    public static GraphicsDevice CGraphicsDevice { get; private set; }
    public static SpriteBatch CSpriteBatch { get; private set; }
    public new static ContentManager Content { get; private set; }

    /// <summary>
    /// Creates a Core
    /// </summary>
    /// <param name="title">Title of window _Instance?</param>
    /// <param name="width">Width?</param>
    /// <param name="height">Height?</param>
    /// <param name="fullscreen">Is fullscreen?</param>
    public Core(string title, int width, int height, bool fullscreen)
    {
        if (_Instance != null)
        {
            throw new InvalidOperationException("Only one core, please.");
        }
        
        _Instance = this;
        CGraphics = new GraphicsDeviceManager(this);
        CGraphics.PreferredBackBufferHeight = height;
        CGraphics.PreferredBackBufferWidth = width;
        CGraphics.IsFullScreen = fullscreen;
        
        CGraphics.ApplyChanges();

        Window.Title = title;

        Content = base.Content;
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        // set the Graphics device to the Game's graphics device
        CGraphicsDevice = base.GraphicsDevice;
        
        CSpriteBatch = new SpriteBatch(CGraphicsDevice);
        
    }
}