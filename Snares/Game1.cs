using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppilib;
using ppilib.Node.Base;
using ppilib.Node.Transformable;
using ppilib.Types.Class;
using ppilib.Types.Struct;


namespace Snares;

public class Game1() : Core("snares_development", 1400, 700, false)
{
    
    public TransformNodeBase RootNode { get; set; }
    
     protected override void Initialize()
    {
        // lets start noding!

        var root = new TransformNodeBase("Snares", null, LocalTransform.Root);
        var windowSize = new Vector2(GraphicsDevice.Viewport.X, GraphicsDevice.Viewport.Y);
        root.SetWorldAsRoot(
            new Transform(
                new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
                new Stretch(windowSize, Vector2.One, Vector2.Zero),
                0f
            )
        );

        var mainView = new NodeBase("MainView", root);
        root.AddChild(mainView);
        
        RootNode = root;
        
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
        

        base.Update(gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);
        
        CSpriteBatch.Begin();
        
        // draw
        
        CSpriteBatch.End();
        base.Draw(gameTime);
        
    }
}