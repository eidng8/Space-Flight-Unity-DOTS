using eidng8.SpaceFlight.Components;
using Unity.Entities;

namespace eidng8.SpaceFlight.Systems
{
    public class PrefabSystem : ComponentSystem
    {
        protected override void OnUpdate() {
            this.Entities.ForEach(
                (Entity entity, ref MovableComponent data) => {
                    for (int i = 0; i < 10; i++) {
                        this.EntityManager.Instantiate(data.entity);
                    }

                    this.EntityManager.DestroyEntity(entity);
                }
            );
        }
    }
}
