using System;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using eidng8.SpaceFlight.Configurable;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.SceneManagement;

namespace eidng8.SpaceFlight.Managers
{
    /// <todo>
    ///     Implement a caching mechanism for <see cref="GetPrefabs()" />.
    /// </todo>
    public static class PrefabCacheManager
    {
        public static Action<World> cleanup;

        private static bool _worldReady;

        private static World _world;

        static PrefabCacheManager() {
            PrefabCacheManager.cleanup += delegate { };
            GameManager.beforeSceneLoad += PrefabCacheManager.ClearWorld;
            SceneManager.sceneLoaded += PrefabCacheManager.SetupWorld;
            PrefabCacheManager.CreateWorld();
        }

        /// <summary>
        ///     Spawn the specified prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public static void Instantiate(PrefabTypes type, int count = 1) {
            PrefabCacheManager.Spawn(type, count);
        }

        /// <summary>
        ///     Spawn the specified prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cfg"></param>
        /// <param name="count"></param>
        public static void Instantiate(
            PrefabTypes type,
            IConfigurable cfg,
            int count = 1
        ) {
            PrefabCacheManager.Spawn(type, cfg, count);
        }

        private static void CreateWorld() {
            PlayerLoopManager.RegisterDomainUnload(
                PrefabCacheManager.DisposeWorld,
                10000
            );
            PrefabCacheManager._world = new World("Entity Creation");
            PrefabCacheManager._world.CreateSystem<PrefabSpawningJob>();
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(
                PrefabCacheManager._world
            );
            PrefabCacheManager._worldReady = true;
        }

        private static void ClearWorld() {
            if (!PrefabCacheManager._worldReady) { return; }

            PrefabCacheManager.cleanup.Invoke(PrefabCacheManager._world);
            EntityManager em = PrefabCacheManager._world.EntityManager;
            EntityQuery query = em.CreateEntityQuery(
                ComponentType.ReadOnly<PrefabComponent>()
            );
            em.DestroyEntity(query);
        }

        private static void SetupWorld(Scene scene, LoadSceneMode mode) {
            if (null == World.Active) { return; }

            EntityManager aem = World.Active.EntityManager;
            NativeArray<EntityRemapUtility.EntityRemapInfo> map =
                aem.CreateEntityRemapArray(Allocator.Temp);
        }

        private static void DisposeWorld() {
            if (PrefabCacheManager._worldReady) {
                PrefabCacheManager._world.Dispose();
            }
        }

        private static void Spawn(PrefabTypes type, int count = 1) {
            NativeArray<PrefabComponent> prefabs =
                PrefabCacheManager.GetPrefabs();
            PrefabCacheManager.SpawnPrefab(prefabs, (int)type, count);
            PrefabCacheManager.DisposeQuery(prefabs);
        }

        private static void Spawn(
            PrefabTypes type,
            IConfigurable cfg,
            int count = 1
        ) {
            NativeArray<PrefabComponent> prefabs =
                PrefabCacheManager.GetPrefabs();
            PrefabCacheManager.SpawnPrefab(prefabs, (int)type, cfg, count);
            PrefabCacheManager.DisposeQuery(prefabs);
        }

        private static NativeArray<PrefabComponent> GetPrefabs() {
            return World.Active.EntityManager
                .CreateEntityQuery(ComponentType.ReadOnly<PrefabComponent>())
                .ToComponentDataArray<PrefabComponent>(Allocator.TempJob);
        }

        private static void SpawnPrefab(
            NativeArray<PrefabComponent> prefabs,
            int type,
            int count
        ) {
            foreach (PrefabComponent c in prefabs) {
                if (c.type != type) {
                    continue;
                }

                PrefabSpawningJob.Request request =
                    new PrefabSpawningJob.Request {
                        prefab = c.prefab
                    };
                PrefabSpawningJob.Spawn(request, count);
                return;
            }
        }

        private static void SpawnPrefab(
            NativeArray<PrefabComponent> prefabs,
            int type,
            IConfigurable cfg,
            int count
        ) {
            foreach (PrefabComponent c in prefabs) {
                if (c.type != type) {
                    continue;
                }

                EntityManager em = World.Active.EntityManager;
                Entity e = em.Instantiate(c.prefab);
                em.AddComponent<JustSpawned>(e);
                // PrefabSpawningJob.Request request =
                //     new PrefabSpawningJob.Request {
                //         prefab = c.prefab,
                //         hasConfig = true,
                //         config = new ConfigurableComponent(cfg)
                //     };
                // PrefabSpawningJob.Spawn(request, count);
                return;
            }
        }

        private static void DisposeQuery(NativeArray<PrefabComponent> prefabs) {
            prefabs.Dispose();
        }
    }
}
