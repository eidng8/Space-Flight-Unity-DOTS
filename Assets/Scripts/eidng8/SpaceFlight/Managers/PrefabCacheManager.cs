using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using eidng8.SpaceFlight.Configurable;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eidng8.SpaceFlight.Managers
{
    public static class PrefabCacheManager
    {
        private static bool _cached;

        private static NativeArray<PrefabComponent> _cache;

        static PrefabCacheManager() {
            SceneManager.sceneUnloaded += delegate {
                if (!PrefabCacheManager._cached) { return; }

                PrefabCacheManager._cache.Dispose();
                PrefabCacheManager._cached = false;
                Debug.Log("PrefabCacheManager.sceneUnloaded");
            };
        }

        /// <summary>
        ///     Spawn the specified prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cfg"></param>
        /// <param name="count"></param>
        public static bool Instantiate(
            PrefabTypes type,
            int count = 1,
            IConfigurable cfg = null
        ) {
            NativeArray<PrefabComponent> prefabs =
                PrefabCacheManager.GetPrefabs();
            NativeArray<Entity> entities =
                new NativeArray<Entity>(count, Allocator.TempJob);
            bool spawned = PrefabCacheManager
                .SpawnPrefab(prefabs, (int)type, entities, cfg);

            if (!spawned) {
                prefabs = PrefabCacheManager.GetPrefabs(true);
                spawned = PrefabCacheManager
                    .SpawnPrefab(prefabs, (int)type, entities, cfg);
            }

            entities.Dispose();
            return spawned;
        }

        private static NativeArray<PrefabComponent> GetPrefabs(
            bool flush = false
        ) {
            if (!flush && PrefabCacheManager._cached) {
                return PrefabCacheManager._cache;
            }

            if (PrefabCacheManager._cached) {
                PrefabCacheManager._cache.Dispose();
            }

            PrefabCacheManager._cache = World.Active.EntityManager
                .CreateEntityQuery(ComponentType.ReadOnly<PrefabComponent>())
                .ToComponentDataArray<PrefabComponent>(Allocator.Persistent);
            PrefabCacheManager._cached = true;

            return PrefabCacheManager._cache;
        }

        private static bool SpawnPrefab(
            NativeArray<PrefabComponent> prefabs,
            int type,
            NativeArray<Entity> entities,
            IConfigurable cfg
        ) {
            foreach (PrefabComponent c in prefabs) {
                if (c.type != type) { continue; }

                EntityManager em = World.Active.EntityManager;
                em.Instantiate(c.prefab, entities);
                em.AddComponent<JustSpawned>(entities);
                if (null != cfg) {
                    PrefabCacheManager.SetupPrefab(entities, cfg);
                }

                return true;
            }

            return false;
        }

        private static void SetupPrefab(
            NativeArray<Entity> entities,
            IConfigurable cfg
        ) {
            EntityManager em = World.Active.EntityManager;
            ConfigurableComponent component = new ConfigurableComponent(cfg);

            foreach (Entity entity in entities) {
                PhysicsMass mass = em.GetComponentData<PhysicsMass>(entity);
                mass.InverseMass = component.mass;
                em.SetComponentData(entity, mass);
            }

            if (component.hasPropulsion) {
                em.AddComponent<PropulsionComponent>(entities);
                foreach (Entity entity in entities) {
                    em.SetComponentData(entity, component.propulsion);
                }
            }

            if (component.hasDefense) {
                em.AddComponent<DefenseComponent>(entities);
                foreach (Entity entity in entities) {
                    em.SetComponentData(entity, component.defense);
                }
            }

            if (component.hasOffense) {
                em.AddComponent<OffenseComponent>(entities);
                foreach (Entity entity in entities) {
                    em.SetComponentData(entity, component.offense);
                }
            }

            if (component.hasPower) {
                em.AddComponent<PowerComponent>(entities);
                foreach (Entity entity in entities) {
                    em.SetComponentData(entity, component.power);
                }
            }
        }
    }
}
