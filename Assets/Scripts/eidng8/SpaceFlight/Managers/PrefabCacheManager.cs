using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Configurable;
using eidng8.SpaceFlight.Systems.Jobs;
using Unity.Collections;
using Unity.Entities;

namespace eidng8.SpaceFlight.Managers
{
    /// <todo>
    ///     Implement a caching mechanism for <see cref="GetPrefabs()" />.
    /// </todo>
    public class PrefabCacheManager
    {
        private static readonly PrefabCacheManager Instance =
            new PrefabCacheManager();

        private PrefabCacheManager() { }

        protected static PrefabCacheManager M => PrefabCacheManager.Instance;

        /// <summary>
        ///     Spawn the specified prefab.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="count"></param>
        public static void Instantiate(PrefabTypes type, int count = 1) {
            PrefabCacheManager.M.Spawn(type, count);
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
            PrefabCacheManager.M.Spawn(type, cfg, count);
        }

        protected virtual void Spawn(PrefabTypes type, int count = 1) {
            NativeArray<PrefabComponent> prefabs = this.GetPrefabs();
            this.SpawnPrefab(prefabs, (int)type, count);
            this.DisposeQuery(prefabs);
        }

        protected virtual void Spawn(
            PrefabTypes type,
            IConfigurable cfg,
            int count = 1
        ) {
            NativeArray<PrefabComponent> prefabs = this.GetPrefabs();
            this.SpawnPrefab(prefabs, (int)type, cfg, count);
            this.DisposeQuery(prefabs);
        }

        protected virtual NativeArray<PrefabComponent> GetPrefabs() {
            return World.Active.EntityManager
                .CreateEntityQuery(ComponentType.ReadOnly<PrefabComponent>())
                .ToComponentDataArray<PrefabComponent>(Allocator.TempJob);
        }

        protected virtual void SpawnPrefab(
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

        protected virtual void SpawnPrefab(
            NativeArray<PrefabComponent> prefabs,
            int type,
            IConfigurable cfg,
            int count
        ) {
            foreach (PrefabComponent c in prefabs) {
                if (c.type != type) {
                    continue;
                }

                PrefabSpawningJob.Request request =
                    new PrefabSpawningJob.Request {
                        prefab = c.prefab,
                        hasConfig = true,
                        config = new ConfigurableComponent(cfg)
                    };
                PrefabSpawningJob.Spawn(request, count);
                return;
            }
        }

        protected virtual void DisposeQuery(
            NativeArray<PrefabComponent> prefabs
        ) {
            prefabs.Dispose();
        }
    }
}
