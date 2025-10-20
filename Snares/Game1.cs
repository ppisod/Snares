using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppiGLib;


namespace Snares;

public class Game1() : Core("snares_development", 1400, 700, false)
{

    private Texture2D TextureSnare;
    // private string ContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "Vector");
    
    /*
     protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }
    */

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        TextureSnare = Content.Load<Texture2D>("Images/RhythmObjects/Snares/SnareL");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw (GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.AntiqueWhite);

        // TODO: Add your drawing code here
        CSpriteBatch.Begin();
        
        TextureUtility.DrawBarebones(CSpriteBatch, TextureSnare, Vector2.Zero, new Vector2(50, 50));
        CSpriteBatch.End();
        base.Draw(gameTime);
        
    }
}