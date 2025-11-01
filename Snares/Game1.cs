using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppilib;
using ppilib.Input;
using ppilib.Node.Base;
using ppilib.Node.Custom;
using ppilib.Node.Transformable;
using ppilib.Types.Class;
using ppilib.Types.Struct;
using ppilib.Utility.Configs;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.Shapes;
using ppilib.Utility.Textures;
using Snares.Game.Components;
using Snares.Game.Rhythm;
using Snares.Game.Screens;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace Snares;

public class Game1() : Core("snares_development", 1920, 1080, true)
{

    public MouseController Mouse;
    public KeyboardController Keyboard;
    
    private OldTitleScreen _oldTitleScreen;

    private Vector2 _windowSize;
    
    public SpriteFont Helvetica;
    public TransformNodeBase RootNode { get; set; }
    public TextureCache TextureCache { get; set; }

    protected override void Initialize()
    {
        Mouse = new MouseController();
        Keyboard = new KeyboardController();
        
        var textureCache = new TextureCache(GraphicsDevice);
        textureCache.Add("gray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.Gray));
        textureCache.Add("lightgray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.LightGray));
        textureCache.Add("bg", TextureGenerator.CreateVerticalGradient(GraphicsDevice, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height, Color.White, Color.Gray));
        
        // fonts
        Helvetica = Content.Load<SpriteFont>("Fonts/HelvNeue");

        // Monkey patch for issues with scale. I don't know the issue here.
        var rootConfig = new NodeConfig(null, GraphicsDevice, true, false, false, false, false);
        rootConfig.SetName("Snares");
        
        var root = new TransformNodeBase(rootConfig);
        _windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        root.SetWorldAsRoot(
            new Transform(
                new Stretch(_windowSize, Vector2.Zero, Vector2.Zero),
                new Stretch(_windowSize, Vector2.One, Vector2.Zero),
                0f
            )
        );

        NodeBase.W($"window:{_windowSize}");
        
        var mainViewConfig = new NodeConfig(null, GraphicsDevice, false, false, false, false, false);
        mainViewConfig.SetName("MainView").SetParent(root);
        
        var mainView = new NodeBase(mainViewConfig);
        root.AddChild(mainView);

        _oldTitleScreen = new OldTitleScreen(this, mainView, Helvetica, Mouse);
        
        RootNode = root;
        TextureCache = textureCache;

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        
        Mouse.Update();
        Keyboard.Update();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Microsoft.Xna.Framework.Input.Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        RootNode.Update(gameTime);
        _windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        RootNode.SetWorldAsRoot(new Transform(
            new Stretch(_windowSize, Vector2.Zero, Vector2.Zero),
            new Stretch(_windowSize, Vector2.One, Vector2.Zero),
            0f
        ));
        
        _oldTitleScreen.Update(gameTime);
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        // draw BG first to hide black.

        CSpriteBatch.Begin();
        // calculate BG height/width, scale
        var bg = TextureCache.Get("bg"); 
        var bgSize = new Vector2(bg.Width, bg.Height);
        var scale = _windowSize / bgSize;
        CSpriteBatch.Draw(bg, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        // draw
        RootNode.Draw(CSpriteBatch);

        CSpriteBatch.End();
        base.Draw(gameTime);
    }
}