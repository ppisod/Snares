#nullable enable
using System;
using System.Diagnostics;

namespace Snares.Game.Rhythm;

public class Metronome : IDisposable
{
    private int _beatsPerMinute;
    private int _numerator; // beats per measure (was _beatsPerMeasure)
    private int _denominator; // note value that gets one beat
    private readonly Stopwatch _stopwatch;
    private long _lastBeatTicks;
    private bool _isRunning;
    private bool _disposed;

    public int BeatsPerMinute
    {
        get => _beatsPerMinute;
        set => SetTempo(value);
    }

    // Updated properties for time signature
    public int Numerator
    {
        get => _numerator;
        set => SetTimeSignature(value, _denominator);
    }

    public int Denominator
    {
        get => _denominator;
        set => SetTimeSignature(_numerator, value);
    }

    // Backwards compatibility - treat as numerator
    public int BeatsPerMeasure
    {
        get => _numerator;
        set => SetTimeSignature(value, _denominator);
    }

    public double SecondsPerBeat { get; private set; }
    public long TotalBeats { get; private set; } = 0;
    public long TotalMeasures { get; private set; } = 0;
    public int CurrentBeatInMeasure { get; private set; } = 0;
    public bool IsRunning => _isRunning && _stopwatch.IsRunning;

    // New property to get the time signature as a string
    public string TimeSignature => $"{_numerator}/{_denominator}";

    public event Action<long>? Beat;
    public event Action<long, long>? Measure;
    public event Action? Started;
    public event Action? Stopped;
    public event Action<int>? TempoChanged;
    public event Action<int, int>? TimeSignatureChanged;

    public Metronome(int beatsPerMinute = 120, int numerator = 4, int denominator = 4)
    {
        _beatsPerMinute = beatsPerMinute;
        _numerator = numerator;
        _denominator = denominator;
        _stopwatch = new Stopwatch();
        CalculateSecondsPerBeat();
        
        if (Stopwatch.IsHighResolution)
        {
            Console.WriteLine("Using high-resolution timer for metronome");
        }
    }

    public void Start()
    {
        if (_isRunning) return;
        
        _isRunning = true;
        _stopwatch.Start();
        _lastBeatTicks = _stopwatch.ElapsedTicks;
        Started?.Invoke();
    }

    public void Stop()
    {
        if (!_isRunning) return;
        
        _isRunning = false;
        _stopwatch.Stop();
        Stopped?.Invoke();
    }

    public void Reset()
    {
        _stopwatch.Reset();
        _lastBeatTicks = 0;
        TotalBeats = 0;
        TotalMeasures = 0;
        CurrentBeatInMeasure = 0;
    }

    public void Update(TimeSpan elapsed)
    {
        if (!_isRunning) return;

        long currentTicks = _stopwatch.ElapsedTicks;
        double beatIntervalTicks = SecondsPerBeat * Stopwatch.Frequency;
        
        // freezes protection here
        long ticksSinceLastBeat = currentTicks - _lastBeatTicks;
        int beatsElapsed = (int)(ticksSinceLastBeat / beatIntervalTicks);

        if (beatsElapsed > 0)
        {
            // last beat time
            _lastBeatTicks += (long)(beatsElapsed * beatIntervalTicks);
            
            // Process all elapsed beats
            for (int i = 0; i < beatsElapsed; i++)
            {
                AdvanceBeat();
            }
        }
        
        // Handle micro-timing for very high BPM or frame rate issues
        // If we're very close to the next beat but haven't quite reached it,
        // we can optionally trigger it early for better responsiveness
        double nextBeatTicks = _lastBeatTicks + beatIntervalTicks;
        double ticksToNextBeat = nextBeatTicks - currentTicks;
        double msToNextBeat = (ticksToNextBeat / Stopwatch.Frequency) * 1000;
        
        // Display latency in rhythm games. Doggy patch
        if (msToNextBeat is <= 0 or > 1.0 || beatsElapsed != 0) return;
        _lastBeatTicks = (long)nextBeatTicks;
        AdvanceBeat();
    }

    public void SetTempo(int beatsPerMinute)
    {
        if (beatsPerMinute <= 0)
            throw new ArgumentException("Tempo must be greater than 0", nameof(beatsPerMinute));

        if (_beatsPerMinute != beatsPerMinute)
        {
            // Calculate the current position in beats for tempo transition !
            if (_isRunning && _stopwatch.IsRunning)
            {
                long currentTicks = _stopwatch.ElapsedTicks;
                double currentBeatPosition = (double)(currentTicks - _lastBeatTicks) / (SecondsPerBeat * Stopwatch.Frequency);
                
                _beatsPerMinute = beatsPerMinute;
                CalculateSecondsPerBeat();
                
                // phase accuracy
                double newBeatIntervalTicks = SecondsPerBeat * Stopwatch.Frequency;
                _lastBeatTicks = currentTicks - (long)(currentBeatPosition * newBeatIntervalTicks);
            }
            else
            {
                _beatsPerMinute = beatsPerMinute;
                CalculateSecondsPerBeat();
            }
            
            TempoChanged?.Invoke(beatsPerMinute);
        }
    }

    // Updated method to handle both numerator and denominator
    public void SetTimeSignature(int numerator, int denominator)
    {
        if (numerator <= 0)
            throw new ArgumentException("Numerator must be greater than 0", nameof(numerator));
        
        if (denominator <= 0 || (denominator & (denominator - 1)) != 0) // pow 2!
            throw new ArgumentException("Denominator must be a positive power of 2", nameof(denominator));

        bool changed = _numerator != numerator || _denominator != denominator;
        
        _numerator = numerator;
        _denominator = denominator;
        
        // Adjust current beat in measure if it's now out of bounds
        CurrentBeatInMeasure = CurrentBeatInMeasure % numerator;
        
        if (changed)
        {
            TimeSignatureChanged?.Invoke(numerator, denominator);
        }
    }

    // backwards compatibility!
    public void SetTimeSignature(int numerator)
    {
        SetTimeSignature(numerator, _denominator);
    }

    /// <summary>
    /// Gets the current phase within the current beat (0 to 1)
    /// Useful for visual effects that follow dat bass
    /// </summary>
    public double GetCurrentBeatPhase()
    {
        if (!_isRunning) return 0;
        
        long currentTicks = _stopwatch.ElapsedTicks;
        double beatIntervalTicks = SecondsPerBeat * Stopwatch.Frequency;
        double ticksInCurrentBeat = currentTicks - _lastBeatTicks;
        
        return Math.Clamp(ticksInCurrentBeat / beatIntervalTicks, 0, 1);
    }

    /// <summary>
    /// Gets the time until the next beat in seconds
    /// </summary>
    public double GetTimeToNextBeat()
    {
        if (!_isRunning) return 0;
        
        long currentTicks = _stopwatch.ElapsedTicks;
        double nextBeatTicks = _lastBeatTicks + (SecondsPerBeat * Stopwatch.Frequency);
        return (nextBeatTicks - currentTicks) / Stopwatch.Frequency;
    }

    /// <summary>
    /// Gets the duration of a specific note value in seconds
    /// </summary>
    /// <param name="noteValue">The note value (4 = quarter note, 8 = eighth note, etc.)</param>
    public double GetNoteDuration(int noteValue)
    {
        if (noteValue <= 0)
            throw new ArgumentException("Note value must be greater than 0", nameof(noteValue));
        
        // Calculate the ratio between the requested note value and the beat note value
        double ratio = (double)_denominator / noteValue;
        return SecondsPerBeat * ratio;
    }

    /// <summary>
    /// Gets the duration of a measure in seconds
    /// </summary>
    public double GetMeasureDuration()
    {
        return SecondsPerBeat * _numerator;
    }

    private void CalculateSecondsPerBeat()
    {
        SecondsPerBeat = 60.0 / _beatsPerMinute;
    }

    private void AdvanceBeat()
    {
        TotalBeats++;
        
        // MEW MEASURE?
        if (CurrentBeatInMeasure == 0)
        {
            TotalMeasures++;
            Measure?.Invoke(TotalMeasures, TotalBeats);
        }

        Beat?.Invoke(TotalBeats);
        
        // wrap
        CurrentBeatInMeasure = (CurrentBeatInMeasure + 1) % _numerator;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _stopwatch.Stop();
            _disposed = true;
        }
    }
}