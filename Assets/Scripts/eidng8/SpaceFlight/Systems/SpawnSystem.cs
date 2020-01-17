using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Entities;

namespace eidng8.SpaceFlight.Systems
{
    /// <summary>
    ///     Process requests of spawning prefab entities. This is for use
    ///     in
    ///     conjunction with <see cref="PrefabSpawningJob" />.
    /// </summary>
    public class SpawnSystem : ComponentSystem
    {
        /// <summary>
        ///     Spawn the specified prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public static void SpawnPrefab(PrefabTypes type, int count = 1) {
            EntityManager em = World.Active.EntityManager;
            SpawnPrefab s = new SpawnPrefab {
                type = (int)type,
                count = count
            };
            EntityArchetype t = em.CreateArchetype(
                ComponentType.ReadWrite<SpawnPrefab>()
            );
            Entity e = em.CreateEntity(t);
            em.SetComponentData(e, s);
        }

        protected override void OnUpdate() {
            List<Entity> processed = new List<Entity>();
            this.Entities.WithAllReadOnly<SpawnPrefab>()
                .ForEach(
                    entity => {
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
