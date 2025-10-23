using Microsoft.Xna.Framework;
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

public class Game1() : Core("snares_development", 1400, 700, false)
{
    
    public TransformNodeBase RootNode { get; set; }
    public TextureCache TextureCache { get; set; }
    
     protected override void Initialize()
    {
        // lets start noding!
        
        // texturecache
        var textureCache = new TextureCache(GraphicsDevice);
        textureCache.Add("gray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.Gray));

        var root = new TransformNodeBase("Snares", null, LocalTransform.Root);
        var windowSize = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
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

        var frame = new Frame("Frame", mainView, new LocalTransform(new Vector2(0, 0.3f), new Vector2(1, 0.3f), 0f),
            textureCache.Get("gray"));
        frame.DrawDebugShape = true;
        mainView.AddChild(frame);
        
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