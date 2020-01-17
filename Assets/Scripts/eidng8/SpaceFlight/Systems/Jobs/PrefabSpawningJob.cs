using System.Collections.Generic;
using eidng8.SpaceFlight.Components;
using eidng8.SpaceFlight.Components.Tags;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace eidng8.SpaceFlight.Systems.Jobs
{
    /// <summary>
    ///     The job scheduler for spawning prefab entities. This is for use
    ///     in
    ///     conjunction with <see cref="SpawnSystem" />.
    /// </summary>
    public class PrefabSpawningJob : JobComponentSystem
    {
        public const int QueueLength = 10;

        /// <summary>
        ///     The list of prefabs to be instantiated, a.k.a. the queue.
        ///     There's a mutex on this list.
        /// </summary>
        private static readonly List<PrefabComponent> Prefabs =
            new List<PrefabComponent>(PrefabSpawningJob.QueueLength);

        private EndSimulationEntityCommandBufferSystem _cmd;

        private NativeArray<PrefabComponent> _lastBatch;

        private JobHandle _lastJob;

        private bool _spawning;

        /// <summary>
        ///     Add a spawn request to queue. The queue is locked during this
        ///     call.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="count"></param>
        public static void Spawn(PrefabComponent prefab, int count = 1) {
            lock (PrefabSpawningJob.Prefabs) {
                for (int i = 0; i < count; i++) {
                    PrefabSpawningJob.Prefabs.Add(prefab);
                }
            }
        }

        /// <summary>
        ///     Clear the spawn queue. The queue is locked during this call.
        /// </summary>
        public static void ResetQueue() {
            lock (PrefabSpawningJob.Prefabs) {
                PrefabSpawningJob.Prefabs.Clear();
                PrefabSpawningJob.Prefabs.Capacity =
                    PrefabSpawningJob.QueueLength;
            }
        }

        protected override void OnCreate() {
            this._cmd = this.World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle dependsOn) {
            // The dispatching of jobs is single threaded.
            // It is not necessary to consider racing condition while accessing
            // the `_spawning` flag.
            if (this._spawning) {
                if (this._lastJob.IsCompleted) {
                    this._lastBatch.Dispose();
                    this._spawning = false;
                } else {
                    return default;
                }
            }

            int count = PrefabSpawningJob.Prefabs.Count;
            if (count < 1) {
                return default;
            }

            // Acquire a mutex on the job queue first
            lock (PrefabSpawningJob.Prefabs) {
                this._lastBatch = new NativeArray<PrefabComponent>(
                    PrefabSpawningJob.Prefabs.ToArray(),
                    Allocator.TempJob
                );
                // Everything has been moved to the native array.
                // We can clear the job queue now.
                PrefabSpawningJob.ResetQueue();
            }

            // Actually schedule a job
            Job job = new Job {
                cmd = this._cmd.CreateCommandBuffer().ToConcurrent(),
                prefabs = this._lastBatch
            };
            this._lastJob = job.Schedule(count, 1, dependsOn);
            this._cmd.AddJobHandleForProducer(this._lastJob);

            // Set the flag so we won't be running scheduling until the current
            // job is done.
            this._spawning = true;

            return this._lastJob;
        }

        [BurstCompile]
        private struct Job : IJobParallelFor
        {
            [ReadOnly] public EntityCommandBuffer.Concurrent cmd;
            public NativeArray<PrefabComponent> prefabs;

            public void Execute(int index) {
                PrefabComponent prefab = this.prefabs[index];
                Entity e = this.cmd.Instantiate(index, prefab.prefab);
                // Add the tag component so others can work specifically
                // on newly created entities.
                this.cmd.AddComponent<JustSpawned>(index, e);
            }
        }
    }
}
