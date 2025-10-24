using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppilib;
using ppilib.Node.Base;
using ppilib.Node.Custom;
using ppilib.Node.Transformable;
using ppilib.Types.Class;
using ppilib.Types.Struct;
using ppilib.Utility.Shapes;
using ppilib.Utility.Textures;


namespace Snares;

public class Game1() : Core("snares_development", 1600, 1000, true)
{
    
    public TransformNodeBase RootNode { get; set; }
    public TextureCache TextureCache { get; set; }
    
     protected override void Initialize()
    {
        var textureCache = new TextureCache(GraphicsDevice);
        textureCache.Add("gray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.Gray));
        textureCache.Add("lightgray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.LightGray));

        // Monkey patch for issues with scale. I don't know the issue here. And it doesn't work!!
        var root = new TransformNodeBase("Snares", null, LocalTransform.Root);
        var windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        root.SetWorldAsRoot(
            new Transform(
                new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
                new Stretch(windowSize, Vector2.One, Vector2.Zero),
                0f
            )
        );
        
        root.w($"window:{windowSize}");

        var mainView = new NodeBase("MainView", root);
        root.AddChild(mainView);

        var frame = new Frame("Frame", mainView, new LocalTransform(new Vector2(0, 0.45f), new Vector2(1, 0.1f), 0f),
            textureCache.Get("gray"));
        mainView.AddChild(frame);
        
        var track = new Frame("Track", frame, new LocalTransform(new Vector2(0.1f, 0.25f), new (0.8f, 0.5f), 0f), textureCache.Get("lightgray"))
            {
                DrawDebugShape = true
            };
        frame.AddChild(track);
        
        RootNode = root;
        TextureCache = textureCache;
        
        base.Initialize();
    }
    

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        RootNode.Update(gameTime);
        var windowSize = new Vector2(GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height);
        RootNode.SetWorldAsRoot(            new Transform(
            new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
            new Stretch(windowSize, Vector2.One, Vector2.Zero),
            0f
        ));

        base.Update(gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);
        
        CSpriteBatch.Begin();
        
        // draw
        RootNode.Draw(CSpriteBatch);
        
        CSpriteBatch.End();
        base.Draw(gameTime);
        
    }
}