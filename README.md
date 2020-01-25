# Space-Flight-Unity-DOTS

A physics driven space flight simulation using Unity's new DOTS.


### Goal

I want to make this usable as a library. The `Assets/Scripts` directory contains
everything. Other stuff in the repo are just samples and documents.

It is not supposed to directly edit anything in this package. One could derive
sub-class if needed. Or use various delegates to tap into processes.

I also want to salvage my long forgotten maths & physics. So there are lots of grade
school comments littering all over.


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


### Beginner's Notes

I count myself as a beginner. So I have a dedicated section here.


##### Moment of Inertia is a Vector3 tensor

In Unity, moment of inertia is not a simple number. Instead, it is the inertia tensor
in `Vector3` for `GameObject`, and `float3` for ECS `Physics Shape`.
The `Physics Body` has a bit more parameters can be tweaked  than the `GameObject`'s
`Rigigbody.inertiaTensor` counterpart.



### General Notes

##### Entity Conversion

The `ConvertToEntity` component is a heavy duty task. The
 [Crosair](https://github.com/eidng8/Space-Flight-Unity/blob/278e03e11ebc2810bfe84ec449246ff671cb3796/Assets/Resources/Prefabs/Ships/Crosair.prefab)
game object from the Space-Flight-Unity takes around 3.5 seconds to convert
on-the-fly. Although the conversion is a one-time process happens at the first time
of instantiation. Working out a dozen game objects will freeze the game for quite a
while.

This is the reason to always stick to sub-scenes. Placing prefabs to sub-scene saves
a whole lot of runtime resources. Even prefabs not requiring entity conversion will
benefit greatly from the sub-scene cache.