using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Entities;

namespace eidng8.SpaceFlight.Systems
{
    public class SpawnSystem : ComponentSystem
    {
        protected override void OnUpdate() {
            List<Entity> processed = new List<Entity>();
            this.Entities.WithAllReadOnly<SpawnPrefab>()
                .ForEach(
                    (entity) => {
                        processed.Add(entity);
                        SpawnPrefab s = this.EntityManager
                            .GetComponentData<SpawnPrefab>(entity);
                        if (s.count <= 0) {
                            return;
                        }

                        this.Entities.WithAllReadOnly<PrefabComponent>()
                            .ForEach(
                                (ref PrefabComponent c) => {
                                    if (c.type == s.type) {
                                        PrefabSpawningJob.Spawn(c, s.count);
                                    }
                                }
                            );
                    }
                );

            foreach (Entity entity in processed) {
                this.EntityManager.DestroyEntity(entity);
            }
        }
    }
}
