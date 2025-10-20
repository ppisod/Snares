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
- [ ] the Textures class (store textures by simple names in a hash table or something for easy access?)
- [ ] the Shape class
  - [ ] be able to convert a Shape into a Texture2D (with stretchy compatibility)
- [ ] GameObject class (using Texture2D OR text OR nothing) (SetTexture, SetPosition ...etc)
- [ ] a GameObjectController class (SetHierarchy, Add w/ name, Remove) -> this can actually be very useful if I somehow make it into a node tree system similar to the Roblox Workspace
  - [ ] use the GameObjectController in Game1 to control game objects
- [ ] a GamePage class
  - [ ] preconfigured GameObjects in each GamePage, transitioning between GamePages......


  - [ ] figure out how to render text and make a Text class? maybe just add a placeholder title for now...
  - [ ] the SpriteShape class - dynamic some thimg
    - [ ] make it extendable (? idk how to do - i put this here so that like if i wanted to make a class "Track" for example, i would just extend the SpriteShape class and add some extra functionality accordingly)
    - once i have everything mapped out systematically, i need to optimize my sprite rendering
      - [ ] create a sprite map with all the sprites i use. see [here](https://docs.monogame.net/articles/tutorials/building_2d_games/07_optimizing_texture_rendering/index.html)