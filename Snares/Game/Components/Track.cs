using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ppilib.Interfaces;
using ppilib.Node.Custom;
using ppilib.Types.Struct;
using ppilib.Utility.MovingThings;
using ppilib.Utility.MovingThings.Ease.Definitions;
using Snares.Game.Rhythm;

namespace Snares.Game.Components;

public class Track
{
    private bool _direction;
    private readonly List<LerpableFrame> _tickers;
    private int _currentTicker; // 0 inclusive. for example: 0, 1, 2, 3, / 4, 3, 2, 1 for numerator 4.
                                // note that the number of tickers will always be 1 more.
    private bool _areTickersDirty;
    private readonly int _identifier;
    private readonly LerpableFrame _track;
    private readonly LerpableFrame _slider;
    public Metronome Metronome;
    private int _lastNumerator;

    private bool _isPlaying;

    private readonly Texture2D _tickTexture;

    private readonly Easing _easing;
    private readonly Lerper.Mode _mode;

    private bool _justBeated;
    
    
    public Track (int id, 
        INode parent,
        Metronome metronome, 
        Texture2D trackTexture, 
        Texture2D sliderTexture, 
        Texture2D tickerTexture,
        Easing easing, Lerper.Mode mode)
    {
        _easing = easing;
        _mode = mode;
        _currentTicker = 0;
        _lastNumerator = metronome.Numerator;
        _direction = true;
        _areTickersDirty = true;
        Metronome = metronome;
        _identifier = id;
        _tickTexture = tickerTexture;
        _tickers = [];
        _isPlaying = true;
        _track = new LerpableFrame(
            $"Track{id}",
            parent,
            new LocalTransform(new Vector2(0.1f, 0.25f), new Vector2(0.8f, 0.5f), 0f),
            trackTexture
        )
        {
            DrawDebugShape = true
        };
        parent.AddChild(_track);
        _track.Easing = EasingTypes.Quad;
        _track.EasingMode = Lerper.Mode.Out;

        _slider = new LerpableFrame(
            $"Slider{id}",
            _track,
            new LocalTransform(new Vector2(-0.0015f, -0.25f), new Vector2(0.01f, 1.5f), 0f),
            sliderTexture
        )
        {
            DrawDebugShape = true
        };
        _track.AddChild(_slider);
        Metronome.Beat += OnBeat;
        
        UpdateTickers();
    }

    private void OnBeat(long beats)
    {
        _justBeated = true;
    }

    public void Update (GameTime _)
    {
        if (!_isPlaying)
        {
            return;
        }
        
        if (Metronome.Numerator != _lastNumerator)
        {
            _areTickersDirty = true;
        }
        _lastNumerator = Metronome.Numerator;
        if (_areTickersDirty)
        {
            UpdateTickers();
        }
        // now, tickers are unsafe!
        if (!_justBeated) return;
        
        // we make it safe again
        CheckTickerStateIntegrity();
        
        _slider.LerpLocalPos(new Vector2(_tickers[_currentTicker].Local.Pos.X, -0.25f), Metronome.GetTimeToNextBeat());
        
        UpdateTickerState();
        
        _justBeated = false;
    }

    private void UpdateTickers()
    {
        foreach (var ticker in _tickers)
        {
            ticker.Destroy();
        }
        _tickers.Clear();
        var beats = Metronome.Numerator; // so, we create sections. Draw section lines.
        // get the length of each section.
        var lenOfSection = 1f / beats;
        for (var i = 0; i <= beats; i++)
        {
            var f = new LerpableFrame(
                $"TickerLine{_identifier}_{i}",
                _track,
                new LocalTransform(
                    new Vector2((i * lenOfSection) - 0.0015f, 0f),
                    new Vector2(0.01f, 1f), 0f
                ),
                _tickTexture
            )
            {
                DrawDebugShape = true
            };
            _track.AddChild(f);
            _tickers.Add(f);
        }
        
        _areTickersDirty = false;
    }

    private void CheckTickerStateIntegrity()
    {
        if (_currentTicker < 0 || _currentTicker >= _tickers.Count)
        {
            _currentTicker = Metronome.CurrentBeatInMeasure;
        }
    }

    private void UpdateTickerState()
    {
        if (_direction)
        {
            _currentTicker++;
            if (_currentTicker < _tickers.Count) return;
            _direction = false;
            _currentTicker -= 2;

        }
        else
        {
            _currentTicker--;
            if (_currentTicker >= 0) return;
            _direction = true;
            _currentTicker = 1;
        }   
    }

    public void SetIsRunning (bool v)
    {
        switch (_isPlaying)
        {
            case true when v:
            case false when !v:
                return;
        }
        _isPlaying = v;
        // tween transparency
        _slider.Easing = EasingTypes.Quad;
        _slider.EasingMode = Lerper.Mode.Out;
        if (!v)
        {
            _slider.LerpOpacity(0f, 0.5f);
            _slider.Easing = EasingTypes.Linear;
            _slider.EasingMode = Lerper.Mode.InOut;
            _track.LerpOpacity(0f, 0.5f);
            foreach (var ticker in _tickers)
            {
                ticker.LerpOpacity(0f, 0.5f);
            }
        }
        else
        {
            _track.LerpOpacity(1f, 0.1f);
            _slider.LerpOpacity(1f, 0.1f);
            _slider.Easing = EasingTypes.Linear;
            _slider.EasingMode = Lerper.Mode.InOut;
            UpdateTickers();
        }
    }
    
}