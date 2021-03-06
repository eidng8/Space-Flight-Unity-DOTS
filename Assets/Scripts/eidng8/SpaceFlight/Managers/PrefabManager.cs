﻿using System;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Configurable;
using eidng8.SpaceFlight.Configurable;
using eidng8.SpaceFlight.Laws;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace eidng8.SpaceFlight.Managers
{
    /// <summary>
    ///     Handles spawning prefab entities to scene. It keeps a cache of
    ///     loaded
    ///     prefabs in memory.
    /// </summary>
    /// <remarks>
    ///     I've tried to use a separate world for entity creation, but
    ///     failed.
    ///     The problem was that when moving entities from active world,
    ///     using
    ///     <see
    ///         cref="EntityManager.MoveEntitiesFrom(EntityManager, EntityQuery, NativeArray&lt;Unity.Entities.EntityRemapUtility.EntityRemapInfo&gt;)" />
    ///     ,
    ///     only the main entity is moved. All of its child entities remain
    ///     in the
    ///     active world.
    /// </remarks>
    public static class PrefabManager
    {
        /// <summary>
        ///     Delegates to be invoked when new entities were created.
        ///     Components
        ///     supported by <see cref="ConfigurableComponent" /> are
        ///     automatically
        ///     added to entities on creation.
        /// </summary>
        public static Action<NativeArray<Entity>> entityCreated;

        private static bool _cached;

        private static NativeArray<PrefabComponent> _cache;

        static PrefabManager() {
            SceneManager.sceneLoaded += delegate {
                PrefabManager.ClearCache();
            };

            PrefabManager.entityCreated = delegate { };

            // The following doesn't work since 2019.3
            // Make sure to clear up the cache when the app quit.
            // PlayerLoopManager.RegisterDomainUnload(
            //     PrefabManager.ClearCache,
            //     10000
            // );
        }

        /// <summary>
        ///     Spawn the specified prefab, and add configuration components to
        ///     newly created entities. Please note that if<see cref="cfg" />
        ///     is
        ///     null, then no configuration component will be added to
        ///     entities.
        /// </summary>
        /// <param name="type">type of prefab</param>
        /// <param name="count">number of entities to instantiate</param>
        /// <param name="cfg">configuration to those entities</param>
        public static bool Instantiate(
            PrefabTypes type,
            int count = 1,
            IConfigurable cfg = null
        ) {
            NativeArray<Entity> entities =
                new NativeArray<Entity>(count, Allocator.TempJob);
            if (!PrefabManager._cached) {
                PrefabManager.CachePrefabs();
            }

            bool spawned = PrefabManager
                .SpawnPrefab((int)type, entities, cfg);

            // Try one more time if failed to spawn.
            // Just in case new prefabs were loaded after last cache.
            if (!spawned) {
                Debug.Log("PrefabCacheManager: cache miss");
                PrefabManager.CachePrefabs(true);
                spawned = PrefabManager
                    .SpawnPrefab((int)type, entities, cfg);
            }

            entities.Dispose();
            return spawned;
        }

        public static void ClearCache() {
            if (!PrefabManager._cached) {
                Debug.Log("Prefab cache is empty");
                return;
            }

            PrefabManager._cache.Dispose();
            PrefabManager._cached = false;
            Debug.Log("Prefab cache cleared");
        }

        private static void CachePrefabs(bool flush = false) {
            if (!flush && PrefabManager._cached) {
                return;
            }

            // make sure the cache is clean and disposed
            PrefabManager.ClearCache();

            // build a new cache, and sort it so we can use binary search later
            PrefabManager._cache = World.DefaultGameObjectInjectionWorld
                .EntityManager
                .CreateEntityQuery(ComponentType.ReadOnly<PrefabComponent>())
                .ToComponentDataArray<PrefabComponent>(Allocator.Persistent);
            PrefabManager._cache.Sort();
            PrefabManager._cached = true;
        }

        /// <summary>
        ///     Actually spawning the prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="entities"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        private static bool SpawnPrefab(
            int type,
            NativeArray<Entity> entities,
            IConfigurable cfg
        ) {
            int idx = PrefabManager._cache.BinarySearch(type);
            if (idx < 0) { return false; }

            EntityManager em = World.DefaultGameObjectInjectionWorld
                .EntityManager;
            em.Instantiate(PrefabManager._cache[idx].prefab, entities);

            // Configures the entities if provided
            if (null != cfg) {
                PrefabManager.SetupPrefab(entities, cfg);
            }

            PrefabManager.entityCreated.Invoke(entities);

            return true;
        }

        private static void SetupPrefab(
            NativeArray<Entity> entities,
            IConfigurable cfg
        ) {
            EntityManager em = World.DefaultGameObjectInjectionWorld
                .EntityManager;
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

            if (component.hasEnergy) {
                em.AddComponent<EnergyComponent>(entities);
                foreach (Entity entity in entities) {
                    em.SetComponentData(entity, component.energy);
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
