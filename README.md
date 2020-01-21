# Space-Flight-Unity-DOTS

### Playing around

Open `Assets/Scenes/Space/Startup.unity` and play.


### Problems met

* `EntityManager.MoveEntitiesFrom()` doesn't move the whole hierarchy of prefabs.
Only the entities selected by `EntityQuery` will be moved, no child under them would
be moved. Because of this, off-active-world entity creation is almost (practically)
impossible. However, such creation process happening on main thread is not too bad
at the moment, unless creating many hundreds or more entities at once. Mostly the
rendering pipeline will crumble if a few dozens entities were spawned in sight.
* The `PhysicsStep` component is not destroyed after a scene were unloaded. Say two
scene (A & B) both have an entity with `PhysicsStep` component on them. When you
switch from scene A to scene B using `SceneManager.LoadScene("B")`, the `PhysicsStep`
from scene A will persist into scene B. Resulting in broken physics simulation.