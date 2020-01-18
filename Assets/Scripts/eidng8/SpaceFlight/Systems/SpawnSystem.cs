using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Managers;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Collections;
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
        public static bool UseCache = false;

        private bool _cached;

        private NativeArray<PrefabComponent> _cache;

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
            NativeArray<PrefabComponent> prefabs = this.GetPrefabs();
            // A temporary entity array is generated inside query builder's
            // `ForEach()` method. We are actually iterating over that temporary
            // array here. So it should be OK to destroy the entity inside
            // the `ForEach()` call.
            this.Entities.WithAllReadOnly<SpawnPrefab>()
                .ForEach(
                    entity => {
                        SpawnPrefab s = this.EntityManager
                            .GetComponentData<SpawnPrefab>(entity);
                        if (s.count <= 0) {
                            this.EntityManager.DestroyEntity(entity);
                            return;
                        }

                        foreach (PrefabComponent c in prefabs) {
                            if (c.type == s.type) {
                                PrefabSpawningJob.Spawn(c, s.count);
                                break;
                            }
                        }

                        this.EntityManager.DestroyEntity(entity);
                    }
                );

            if (!this._cached) {
                prefabs.Dispose();
            }
        }

        protected override void OnDestroy() {
            if (this._cached) {
                this._cache.Dispose();
            }

            base.OnDestroy();
        }

        private NativeArray<PrefabComponent> GetPrefabs() {
            if (this._cached) {
                if (this._cache.Length > 0) {
                    return this._cache;
                }

                this._cache.Dispose();
            }

            NativeArray<PrefabComponent> a = this.Entities
                .WithAllReadOnly<PrefabComponent>()
                .ToEntityQuery()
                .ToComponentDataArray<PrefabComponent>(Allocator.Persistent);
            if (SpawnSystem.UseCache) {
                this._cache = a;
                this._cached = true;
            }

            return a;
        }
    }
}
