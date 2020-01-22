# Space-Flight-Unity-DOTS

A physics driven space flight simulation using Unity's new DOTS.


### Goal

I want to make this usable as a library. The `Assets/Scripts` directory contains
everything. Other stuff in the repo are just samples and documents.

It is not supposed to directly edit anything in this package. One could derive
sub-class if needed. Or use various delegates to tap into processes.


### Playing around

Open `Assets/Scenes/Space/Startup.unity` and play.

Current thought is to use one empty scene repeatedly and spawn everything on the fly.


### Problems met

##### EntityManager.MoveEntitiesFrom()

`EntityManager.MoveEntitiesFrom()` doesn't move the whole hierarchy of prefabs.
Only the entities selected by `EntityQuery` will be moved, no child under them would
be moved. Because of this, off-active-world entity creation is almost (practically)
impossible.
 
However, such creation process happening on main thread is not too bad
at the moment, unless creating many hundreds or more entities at once. Mostly the
rendering pipeline will crumble if a few dozens entities were spawned in sight.
Long before one would have to worry about entity creation.


##### The PhysicsStep Component

The `PhysicsStep` component is not destroyed after a scene were unloaded. Say two
scene (A & B) both have an entity with `PhysicsStep` component on them. When you
switch from scene A to scene B using `SceneManager.LoadScene("B")`, the `PhysicsStep`
from scene A will persist into scene B. Resulting in broken physics simulation.