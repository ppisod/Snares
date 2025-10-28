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
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace Snares;

public class Game1() : Core("snares_development", 1920, 1080, true)
{

    private MouseController _mouse;
    private KeyboardController _keyboard;
    
    public SpriteFont Helvetica;
    public TransformNodeBase RootNode { get; set; }
    public TextureCache TextureCache { get; set; }

    protected override void Initialize()
    {
        _mouse = new MouseController();
        _keyboard = new KeyboardController();
        
        var textureCache = new TextureCache(GraphicsDevice);
        textureCache.Add("gray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.Gray));
        textureCache.Add("lightgray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.LightGray));
        
        // fonts
        Helvetica = Content.Load<SpriteFont>("Fonts/HelvNeue");

        // Monkey patch for issues with scale. I don't know the issue here.
        var rootConfig = new NodeConfig(null, GraphicsDevice, true, false, false, false, false);
        rootConfig.SetName("Snares");
        
        var root = new TransformNodeBase(rootConfig);
        var windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        root.SetWorldAsRoot(
            new Transform(
                new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
                new Stretch(windowSize, Vector2.One, Vector2.Zero),
                0f
            )
        );

        NodeBase.W($"window:{windowSize}");
        
        var mainViewConfig = new NodeConfig(null, GraphicsDevice, false, false, false, false, false);
        mainViewConfig.SetName("MainView").SetParent(root);
        
        var mainView = new NodeBase(mainViewConfig);
        root.AddChild(mainView);
        
        
        RootNode = root;
        TextureCache = textureCache;

        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        
        _mouse.Update();
        _keyboard.Update();
        
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        RootNode.Update(gameTime);
        var windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        RootNode.SetWorldAsRoot(new Transform(
            new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
            new Stretch(windowSize, Vector2.One, Vector2.Zero),
            0f
        ));
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        CSpriteBatch.Begin();
        // draw
        RootNode.Draw(CSpriteBatch);

        CSpriteBatch.End();
        base.Draw(gameTime);
    }
}