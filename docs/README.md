# Snares
my attempt at a rhythm game in C#, with MonoGame

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
- Right now, we are just going to use basic blocked out shapes instead of textures because i want to get the gameplay down first.
- [ ] the TextureController class (store textures by simple names in a hash table or something for easy access?) optimizations [here](https://docs.monogame.net/articles/tutorials/building_2d_games/07_optimizing_texture_rendering/index.html)
- [ ] the ShapeGenerator class to generate shapes into textures
- [x] Node system (Node abstract class)
- [x] BaseNode node ~~-> alias: Folder node~~
- [x] Frame node
- [x] Texture node
- [ ] ~~GamePage~~ NodeFamily node
- [x] ~~Pager system (handling for all game pages)~~
- [ ] get json thingies working and make a template for beatmap in json.
- [ ] Button node -> event system???????? button.registerMouseRelease(...)
- [ ] figure out how to render text and make a Text class? maybe just add a placeholder title for now...
- [ ] get interface working
- [ ] get the bar working
- [ ] get the rhythm working
- [ ] get the moving thing working
- [ ] evenly space out the beat spaces and figure out how to dynamically instantiate snares
- [ ] get the events working