# Snares
my attempt at a rhythm game in C#, with MonoGame


---

## CHANGELOG

24/10/2025
- Added DummyBeatmap.json
- Added ppilib/Utility/MovingThings (a lerper system)
- Added a LerpableFrame into ppilib/Node/Custom
- ppiGLib is now obselete


---

## my vision
a straight line - the Track - where a moving vertical
bar (just like Taiko, except it's not the Track that's
moving, it's the bar that's moving) ping pongs between
two sides rhythmically; linearly (this can be changed);
where one from/back
can represent a quarter note, half note, or even a bar
... or just any rhythmic interval, depending on the
song.  
this is a one-key rhythm game, but i vision it to have the
whole keyboard available as inputs, and there can be other
notes such as a hold, maybe a spam note as well.  
there can be **multiple** tracks, where simultaneous notes
require two inputs at the same time - but are not key bound.  
Similar to Rhythm Doctor, different tracks can have different
speeds or rhythm intervals to add the challenge of handling
poly rhythms.

In later difficulties, the easing between rhythmic intervals
can and should be mixed up, lerped up with different
functions...because the "beats" are stored in time values and not in
beat values / position values, the game is free to explore different easing
functions to mix it up.

The notes that fade in do it according to the rhythm, not according to a set delay (i want this to
be configurable)

I want to take inspiration (especially visuals) from TETR.IO and osu!lazer with their sleek
interfaces and whatever

---

## my roadmap


-----------------------------------------


Info about the DummyBeatmap:  
The Snare event either takes the at_beats or the at_time, not both  
Any event, except for SongStart, can have a timestamp, so it uses at_beats or at_time  
Planned events:  
SongStart(starts the song. This event can be removed), SongEnd(Ends the song, at specified time/beat),  
TrackIntroduce(introduces a track, this track has to be not already introduced),  
TrackDelete(deletes a track from the screen, this track has to exist first),  
TrackModify(modifies the speed_multiplier or segments_per_bar or easing_style etc.)  
Snare(a key has to be pressed, has to be placed on a introduced track),  
Hold(a key has to be held, any key, it counts for score when pressed and released at the right time),   