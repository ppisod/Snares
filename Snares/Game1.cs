using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ppilib;
using ppilib.Node.Base;
using ppilib.Node.Custom;
using ppilib.Node.Transformable;
using ppilib.Types.Class;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using ppilib.Utility.Shapes;
using ppilib.Utility.Textures;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace Snares;

public class Game1() : Core("snares_development", 1600, 1000, true)
{
    private const int BeatsPerMinute = 180;
    private const int Sections = 3;
    private bool _forward = true;
    public SpriteFont Helvetica;
    private double _lastBeatTime;
    private int _sectionCounter;

    public LerpableFrame Slider;

    private double _timeSinceLastBeat;

    public float TotalGameTime;
    public TransformNodeBase RootNode { get; set; }
    public TextureCache TextureCache { get; set; }

    protected override void Initialize()
    {
        var textureCache = new TextureCache(GraphicsDevice);
        textureCache.Add("gray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.Gray));
        textureCache.Add("lightgray", ShapeGenerator.ColoredScalable(GraphicsDevice, Color.LightGray));
        
        // fonts
        Helvetica = Content.Load<SpriteFont>("Fonts/HelvNeue");

        // Monkey patch for issues with scale. I don't know the issue here.
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

        var track = new Frame("Track", frame, new LocalTransform(new Vector2(0.1f, 0.25f), new Vector2(0.8f, 0.5f), 0f),
            textureCache.Get("lightgray"))
        {
            DrawDebugShape = true
        };
        frame.AddChild(track);

        var slider = new LerpableFrame("Slider", track,
            new LocalTransform(new Vector2(-0.005f, -0.25f), new Vector2(0.01f, 1.5f), 0f),
            textureCache.Get("gray"))
        {
            DrawDebugShape = true
        };
        track.AddChild(slider);
        Slider = slider;

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
        RootNode.SetWorldAsRoot(new Transform(
            new Stretch(windowSize, Vector2.Zero, Vector2.Zero),
            new Stretch(windowSize, Vector2.One, Vector2.Zero),
            0f
        ));
        if (!(gameTime.TotalGameTime.TotalSeconds > 2)) return; // replace with some thing like a button
        // time per beat:
        const double secondsPerBeat = 60d / BeatsPerMinute;
        const float sectionLength = -0.005f + 1f / Sections;

        if (_timeSinceLastBeat > secondsPerBeat)
        {
            // Beat!
            _timeSinceLastBeat = 0;
            _lastBeatTime = gameTime.TotalGameTime.TotalSeconds;
            if (_sectionCounter >= Sections)
            {
                _sectionCounter = 0;
                _forward = !_forward;
            }

            if (_forward)
            {
                var resultPos = new Vector2(-0.005f + sectionLength * _sectionCounter, -0.25f);
                Slider.Lerper.LerpVector2(() => Slider.Local.Pos, Slider.SetLocalPos, resultPos, secondsPerBeat,
                    EasingTypes.Quad, Lerper.Mode.Out);
                _sectionCounter += 1;
            }
            else
            {
                var resultPos = new Vector2(0.995f - sectionLength * _sectionCounter, -0.25f);
                Slider.Lerper.LerpVector2(() => Slider.Local.Pos, Slider.SetLocalPos, resultPos, secondsPerBeat,
                    EasingTypes.Quad, Lerper.Mode.Out);
                _sectionCounter += 1;
            }
        }
        else
        {
            _timeSinceLastBeat = gameTime.TotalGameTime.TotalSeconds - _lastBeatTime;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);

        CSpriteBatch.Begin();

        // draw
        RootNode.Draw(CSpriteBatch);
        
        CSpriteBatch.DrawString(Helvetica, $"beat: {_sectionCounter}", new Vector2(10, 10), Color.Black);

        CSpriteBatch.End();
        base.Draw(gameTime);
    }
}