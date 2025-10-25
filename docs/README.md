# Snares
in progress rhythm game in C#, with MonoGame (also good for learning C# and graphics in general)

## my vision with Snares
a straight line - the Track - where a moving vertical
bar (just like Taiko, except it's not the Track that's
moving, it's the bar that's moving) ping pongs between
two sides rhythmically; linearly (this can be changed);
where one from/back
can represent a quarter note, half note, or even a bar
... or just any rhythmic interval, depending on the
song.  
this is a one-key rhythm game, but i vision it to have the
whole keyboard available as inputs (on mobile its fullscreen), and there can be other
notes such as a hold, maybe a spam note as well.  
there can be **multiple** tracks, where simultaneous notes
require two inputs at the same time - but are not key bound.  
Similar to Rhythm Doctor, different tracks can have different
speeds or rhythm intervals to add the challenge of handling
polyrhythms.

In later difficulties, the easing between rhythmic intervals
can and should be mixed up, lerped up with different
functions...because the "beats" are stored in time values and not in
beat values / position values, the game is free to explore different easing
functions to mix it up.

The notes that fade in do it according to the rhythm, not according to a set delay (i want this to
be configurable)

## my vision with ppilib
ppilib is a library that I made for my own personal use
I made it to make it easier to make games in C# with MonoGame.
It (kinda) resembles Roblox's Gui node tree system, and I took large inspiration from it mainly due to its simplicity.
This is actually not the first version of ppilib, there was originally one earlier which was even more lackluster
I deleted it from the github
I'll probably put ppilib as a seperate repo for people to use it, but for now it's here.

---

## CHANGELOG

24/10/2025
- Added DummyBeatmap.json
- Added ppilib/Utility/MovingThings (a lerper system)
- Added a LerpableFrame into ppilib/Node/Custom
- ppiGLib is now obselete
- Made a demo of how the game will work.
- ISSUES: (for tomorrow)
    - [x] The slider spacing is inaccurate.
    - [x] The beat counter / section counter is not correct.
    - [x] it should be Current Beat, not Section
    - [x] We have to make a class for this track and slider
    - [x] we have to have a Metronome class to handle time and beat values.
    - [ ] Create the Button node, and detect mouse clicks, and handle them
        - [ ] figure out hierarchal issues so only the top button code will be executed when the region on screen is clicked
    - [ ] figure out how to store beatmaps and load them! (not doing snares yet)

25/10/2025
- Metronome class
- Track class (buggy)
- for tomorrow:
- [ ] TrackController class
- [ ] button class
- [ ] figure out how to use the scroll wheel?? that can help me make a ScrollableFrame where players can select beatmaps.
  - or make it similar to Osu but draw the frames outside the screen!
- [ ] FIRST BEATMAP....
- for the future:
- [ ] make a beatmap creator in Python with quantization built in.
    - Vision: for every different key pressed, it represents a new track. 
    - tracks are initialized a bar before the first beat, and every beat is a snare,
    - when tracks are not used they are deleted, time signatures are mixed up and configurable difficulty setting.
---