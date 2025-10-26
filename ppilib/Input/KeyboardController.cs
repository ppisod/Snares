using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Xna.Framework.Input;

namespace ppilib.Input;

public class KeyboardController
{
    private KeyboardState _previousState;
    public List<Keys> Alphabet; // 65 to 90
    public List<Keys> NumF; // numpad and F keys (96 to 135)
    public List<Keys> Special = [Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.LeftShift, Keys.RightShift, Keys.Tab, Keys.Enter, Keys.Back];

    public List<Keys> AllValidKeys;

    public event Action<Keys> KeyDown;
    public event Action<Keys> KeyUp;

    public KeyboardController()
    {
        Alphabet = [];
        NumF = [];
        Special = [];
        AllValidKeys = [];
        
        for (var i = 65; i <= 90; i++)
        {
            Alphabet.Add((Keys)i);
        }

        for (var i = 96; i <= 135; i++)
        {
            NumF.Add((Keys)i);
        }

        AllValidKeys.AddRange(Alphabet);
        AllValidKeys.AddRange(NumF);
        AllValidKeys.AddRange(Special);
    }
    
    public void GetPressed ()
    {
        Keyboard.GetState().GetPressedKeys();
    }
    
    public void Update ()
    {
        var currentState = Keyboard.GetState();
        _previousState = currentState;
        foreach (var key in AllValidKeys)
        {
            if (!_previousState.IsKeyDown(key) && currentState.IsKeyDown(key))
            {
                KeyDown?.Invoke(key);
                continue;
            }

            if (!_previousState.IsKeyUp(key) && currentState.IsKeyUp(key))
            {
                KeyUp?.Invoke(key);
            }
        }
    }
}