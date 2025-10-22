using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppiGLib;
using ppiGLib.Generators;
using ppiGLib.Nodal.Definitions;
using ppiGLib.Nodal.Nodes.Displayable;
using ppiGLib.Nodal.Nodes.Primal;


namespace Snares;

public class Game1() : Core("snares_development", 1400, 700, false)
{

    public NodeFamily NodeFamily { get; private set; }
    
    
     protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        // lets start noding!
        
        

        var main = new BaseNode(GraphicsDevice, "MainScreen", null);
        var frame = new Frame(GraphicsDevice, "frame", main, new Vector2(0, 0.3f), new Vector2(1, 0.3f), 0)
        {
            DisplayDebug = true
        };
        main.AddNodeAsChild(frame);

        var other = new BaseNode(GraphicsDevice, "OtherScreen", null);
        
        NodeFamily = new NodeFamily(GraphicsDevice, "Snares", [main, other]);

        main.Parent = NodeFamily;
        other.Parent = NodeFamily;
        
        NodeFamily.Enable("MainScreen");
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
        
        NodeFamily.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);
        
        CSpriteBatch.Begin();
        
        // draw
        NodeFamily.Draw(CSpriteBatch);
        
        CSpriteBatch.End();
        base.Draw(gameTime);
        
    }
}